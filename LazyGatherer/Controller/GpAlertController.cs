using System;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using LazyGatherer.Models;

namespace LazyGatherer.Controller;

public class GpAlertController : IDisposable
{
    private bool triggered = true;

    public GpAlertController()
    {
        Service.ClientState.ClassJobChanged += ClientState_ClassJobChanged;
        if (Service.PlayerState.ClassJob.RowId is (uint)Job.Bot or (uint)Job.Min)
        {
            Service.Framework.Update += OnFrameworkUpdate;
        }
    }

    public void Dispose()
    {
        Service.Framework.Update -= OnFrameworkUpdate;
        Service.ClientState.ClassJobChanged -= ClientState_ClassJobChanged;
    }

    // 
    private void ClientState_ClassJobChanged(uint classJobId)
    {
        if (classJobId is (uint)Job.Bot or (uint)Job.Min)
        {
            // To not attach it multiple time - Maybe set a bool ? 
            Service.Framework.Update -= OnFrameworkUpdate;
            Service.Framework.Update += OnFrameworkUpdate;
        }
        // Only disable if GP are not regenerating or alert is disabled
        else if (!Service.Config.GpAlertEnabled || triggered)
        {
            Service.Framework.Update -= OnFrameworkUpdate;
        }
    }

    private void OnFrameworkUpdate(IFramework framework)
    {
        var config = Service.Config;
        if (!Service.Config.GpAlertEnabled)
        {
            return;
        }

        // If 0, player is probably teleporting or has changed job and GP is not available, ignored
        var playerGp = Service.ObjectTable.LocalPlayer?.CurrentGp ?? 0;
        if (playerGp == 0)
        {
            return;
        }

        switch (triggered)
        {
            case false when playerGp >= config.GpAlertThreshold:
                Service.Log.Verbose($"GP Alert triggered: Current GP = {playerGp}, " +
                                    $"Threshold = {config.GpAlertThreshold}, Triggered = {triggered}, InDuty = {Service.DutyState.IsDutyStarted}");
                // Do not trigger on duty
                if (Service.DutyState.IsDutyStarted)
                {
                    Service.Framework.Update -= OnFrameworkUpdate;
                    triggered = true;
                    return;
                }

                // Play sound and print message
                UIGlobals.PlayChatSoundEffect(Service.Config.GpAlertSound);
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
                 .Append("GP has reached the configured threshold!");

        Service.ChatGui.Print(new XivChatEntry { Type = Service.Config.GpAlertChatType, Message = sb.BuiltString });
    }
}
