using System;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using LazyGatherer.Models;
using AgentId = Dalamud.Game.Agent.AgentId;

namespace LazyGatherer.Controller;

public class GpAlertController : IDisposable
{
    private bool triggered = true;
    private int nextCheck;

    public GpAlertController()
    {
        Service.ClientState.ClassJobChanged += ClientState_ClassJobChanged;
        Service.ClientState.Logout += ClientState_OnLogout;
        Service.ClientState.Login += ClientState_OnLogin;
        if (Service.PlayerState.ClassJob.RowId is (uint)Job.Bot or (uint)Job.Min)
        {
            Service.Framework.Update += OnFrameworkUpdate;
        }

        Service.CommandManager.AddHandler("/gpalert", new CommandInfo(OnCommand)
        {
            HelpMessage = "Displays GP alert configuration.\n" +
                          "/gpalert <On/Off> -> enable or disable the alert.\n" +
                          "/gpalert <1-9999> -> set the GP threshold."
        });
    }

    public void Dispose()
    {
        Service.ClientState.ClassJobChanged -= ClientState_ClassJobChanged;
        Service.ClientState.Logout -= ClientState_OnLogout;
        Service.Framework.Update -= OnFrameworkUpdate;
    }

    private void ClientState_OnLogout(int type, int code)
    {
        Service.Framework.Update -= OnFrameworkUpdate;
        Service.ClientState.ClassJobChanged -= ClientState_ClassJobChanged;
        triggered = true;
    }

    private void ClientState_OnLogin()
    {
        if (Service.PlayerState.ClassJob.RowId is (uint)Job.Bot or (uint)Job.Min)
        {
            Service.Framework.Update += OnFrameworkUpdate;
        }
    }

    private void ClientState_ClassJobChanged(uint classJobId)
    {
        if (classJobId is (uint)Job.Bot or (uint)Job.Min)
        {
            // To not attach it multiple time - Maybe set a bool ? 
            Service.Framework.Update -= OnFrameworkUpdate;
            Service.Framework.Update += OnFrameworkUpdate;
        }
        // Only disable if GP are not regenerating or alert is disabled or option to not trigger on non gatherer is on
        else if (!Service.Config.GpAlertEnabled || triggered || Service.Config.GpAlertOnlyGatherer)
        {
            Service.Framework.Update -= OnFrameworkUpdate;
            triggered = true;
        }
    }

    private void OnFrameworkUpdate(IFramework framework)
    {
        // To reduce load, check every 3 seconds
        if (Environment.TickCount < nextCheck) return;
        nextCheck = Environment.TickCount + 3000;

        // Check if enable
        var config = Service.Config;
        if (!config.GpAlertEnabled) return;

        // If 0, player is probably teleporting or has changed job and GP is not available, ignored
        var playerGp = Service.ObjectTable.LocalPlayer?.CurrentGp ?? 0;
        if (playerGp == 0) return;

        switch (triggered)
        {
            case false when playerGp >= config.GpAlertThreshold:
                Service.Log.Verbose($"GP Alert triggered: Current GP = {playerGp}, " +
                                    $"Threshold = {config.GpAlertThreshold}, Triggered = {triggered}, InDuty = {Service.DutyState.IsDutyStarted}");
                // Do not trigger on duty or gathering (Revisit)
                if (Service.DutyState.IsDutyStarted ||
                    Service.Condition.Any(ConditionFlag.Gathering))
                {
                    Service.Framework.Update -= OnFrameworkUpdate;
                    triggered = true;
                    return;
                }

                // Play sound and print message
                PlayAlertSound();
                if (Service.Config.GpAlertSendChatMessage)
                {
                    PrintMessage();
                }

                // Disable update if player is not a gatherer anymore
                if (Service.PlayerState.ClassJob.RowId is not (uint)Job.Bot and not (uint)Job.Min)
                {
                    Service.Framework.Update -= OnFrameworkUpdate;
                }

                // reset trigger
                triggered = true;
                break;
            case true when playerGp < config.GpAlertThreshold:
                Service.Log.Verbose($"GP Alert on stand by: Current GP = {playerGp}, " +
                                    $"Threshold = {config.GpAlertThreshold}, Triggered = {triggered}, InDuty = {Service.DutyState.IsDutyStarted}");
                triggered = false;
                break;
        }
    }

    private void PrintMessage()
    {
        var sb = new SeStringBuilder()
                 .AddUiForeground("[LazyGatherer] ", 45)
                 .AddUiForeground("[GP Alert] ", 57)
                 .Append($"GP has reached {Service.Config.GpAlertThreshold}.");

        Service.ChatGui.Print(new XivChatEntry { Type = Service.Config.GpAlertChatType, Message = sb.BuiltString });
    }

    public static unsafe void PlayAlertSound()
    {
        if (Service.Config.GpAlertSoundIsAlarm)
        {
            var agentInterfacePtr = Service.GameGui.GetAgentById((int)AgentId.Alarm);
            if (agentInterfacePtr != IntPtr.Zero)
            {
                ((AgentAlarm*)agentInterfacePtr.Address)->PlayAlarmSoundEffect(Service.Config.GpAlertAlarmSoundEffect);
            }
            else
            {
                Service.Log.Warning("Could not find AgentAlarm, playing default sound effect.");
                UIGlobals.PlayChatSoundEffect(1);
            }
        }
        else
        {
            UIGlobals.PlayChatSoundEffect(Service.Config.GpAlertSoundEffectId);
        }
    }

    private void OnCommand(string command, string arguments)
    {
        if (arguments.IsNullOrEmpty())
        {
            Service.GpAlertAddon.Toggle();
        }

        var split = arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        switch (split[0].ToLower())
        {
            case "on":
                Service.Config.GpAlertEnabled = true;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.GpAlertAddon.Update();
                break;
            case "off":
                Service.Config.GpAlertEnabled = false;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.GpAlertAddon.Update();
                break;
            default:
                if (uint.TryParse(split[0], out var threshold))
                {
                    Service.Config.GpAlertThreshold = Math.Clamp(threshold, 1, 9999);
                    Service.Interface.SavePluginConfig(Service.Config);
                    Service.GpAlertAddon.Update();
                }

                break;
        }
    }
}
