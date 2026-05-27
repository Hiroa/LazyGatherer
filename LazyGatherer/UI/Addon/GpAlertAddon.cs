using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Text;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Node.Simple;
using LazyGatherer.Controller;
using LazyGatherer.Models;

namespace LazyGatherer.UI.Addon;

public class GpAlertAddon : NativeAddon
{
    private readonly Dictionary<XivChatType, string> chatTypeOptions = new()
    {
        { XivChatType.Echo, "Echo" },
        { XivChatType.Say, "Say" },
        { XivChatType.Party, "Party" },
        { XivChatType.Alliance, "Alliance" },
        { XivChatType.Yell, "Yell" },
        { XivChatType.Shout, "Shout" },
    };

    private readonly Dictionary<AlertSoundConfig, string> soundOptions = new()
    {
        // Alarm sound
        { new AlertSoundConfig(AlarmSoundEffect.Bell), "Alarm - Bell" },
        { new AlertSoundConfig(AlarmSoundEffect.MusicBox), "Alarm - MusicBox" },
        { new AlertSoundConfig(AlarmSoundEffect.Prelude), "Alarm - Prelude" },
        { new AlertSoundConfig(AlarmSoundEffect.Chocobo), "Alarm - Chocobo" },
        { new AlertSoundConfig(AlarmSoundEffect.LaNoscea), "Alarm - LaNoscea" },
        { new AlertSoundConfig(AlarmSoundEffect.Festival), "Alarm - Festival" },

        // Sound effect
        { new AlertSoundConfig(1), "Sound effect 1" },
        { new AlertSoundConfig(2), "Sound effect 2" },
        { new AlertSoundConfig(3), "Sound effect 3" },
        { new AlertSoundConfig(4), "Sound effect 4" },
        { new AlertSoundConfig(5), "Sound effect 5" },
        { new AlertSoundConfig(6), "Sound effect 6" },
        { new AlertSoundConfig(7), "Sound effect 7" },
        { new AlertSoundConfig(8), "Sound effect 8" },
        { new AlertSoundConfig(9), "Sound effect 9" },
        { new AlertSoundConfig(10), "Sound effect 10" },
        { new AlertSoundConfig(11), "Sound effect 11" },
        { new AlertSoundConfig(12), "Sound effect 12" },
        { new AlertSoundConfig(13), "Sound effect 13" },
        { new AlertSoundConfig(14), "Sound effect 14" },
        { new AlertSoundConfig(15), "Sound effect 15" },
        { new AlertSoundConfig(16), "Sound effect 16" },
    };

    protected override unsafe void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValueSpan)
    {
        SetWindowSize(new Vector2(300.0f, 250.0f));
        // Enable CB
        var gpAlertEnableCb = new CheckboxNode
        {
            IsVisible = true,
            Size = ContentSize with { Y = 20 },
            Position = ContentStartPosition with { Y = 45 },
            String = "Enable alert",
            IsChecked = Service.Config.GpAlertEnabled,
            OnClick = isChecked =>
            {
                Service.Config.GpAlertEnabled = isChecked;
                Service.Interface.SavePluginConfig(Service.Config);
            }
        };
        gpAlertEnableCb.AttachNode(this);

        new SimpleImageNode
        {
            IsVisible = true,
            Size = ContentSize with { Y = 20 },
            Position = new Vector2(ContentSize.X - 25, gpAlertEnableCb.Y - 5),
            TexturePath = "ui/uld/CircleButtons.tex",
            TextureSize = new Vector2(28.0f, 28.0f),
            TextureCoordinates = new Vector2(112.0f, 84.0f),
            TextTooltip = "Trigger a sound alert when your GP goes under then reach again the configured threshold.\n" +
                          "You can also choose to send a chat message in a specific channel.",
        }.AttachNode(this);

        // Threshold TextNode
        var gpAlertThresholdTn = new TextNode
        {
            IsVisible = true,
            Size = new Vector2(ContentSize.X - 20.0f, 20f),
            Position = new Vector2(ContentStartPosition.X + 20, gpAlertEnableCb.Bounds.Bottom),
            String = "GP threshold:",
            FontSize = 14,
        };
        gpAlertThresholdTn.AttachNode(this);

        // Threshold NumericInputNode
        var gpAlertThresholdNin = new NumericInputNode
        {
            IsVisible = true,
            Size = new Vector2(ContentSize.X - 20.0f, 20f),
            Position = new Vector2(ContentStartPosition.X + 20, gpAlertThresholdTn.Bounds.Bottom - 5),
            Min = 1,
            Max = 9999, // Let's be reasonable
            Value = (int)Service.Config.GpAlertThreshold,
            OnValueUpdate = newValue =>
            {
                Service.Config.GpAlertThreshold = (uint)newValue;
                Service.Interface.SavePluginConfig(Service.Config);
            }
        };
        gpAlertThresholdNin.AttachNode(this);

        // Sound TextNode
        var gpAlertSoundTn = new TextNode
        {
            IsVisible = true,
            Size = new Vector2(ContentSize.X - 20.0f, 20f),
            Position = new Vector2(ContentStartPosition.X + 20, gpAlertThresholdNin.Bounds.Bottom + 10),
            String = "Sound to play:",
            FontSize = 14,
        };
        gpAlertSoundTn.AttachNode(this);

        // Sound Dropdown
        var gpAlertSoundDd = new TextDropDownNode
        {
            IsVisible = true,
            Size = new Vector2(ContentSize.X - 20.0f, 24f),
            Position = new Vector2(ContentStartPosition.X + 20, gpAlertSoundTn.Bounds.Bottom),
            Options = soundOptions.Values.ToList(),
            SelectedOption = soundOptions[AlertSoundConfig.FromConfig()],
            OnOptionSelected = selectedItem =>
            {
                var alertSound = soundOptions.First(kvp => kvp.Value == selectedItem).Key;
                Service.Config.GpAlertSoundIsAlarm = alertSound.IsAnAlarm;
                Service.Config.GpAlertSoundEffectId = alertSound.SoundEffectId;
                Service.Config.GpAlertAlarmSoundEffect = alertSound.AlarmSoundEffect;
                GpAlertController.PlayAlertSound();
                Service.Interface.SavePluginConfig(Service.Config);
                Service.GatheringController.ComputeRotations();
            },
        };
        gpAlertSoundDd.AttachNode(this);

        // Send chat message CB
        var gpAlertMessageCb = new CheckboxNode
        {
            IsVisible = true,
            Size = ContentSize with { Y = 20 },
            Position = ContentStartPosition with { Y = gpAlertSoundDd.Bounds.Bottom + 10 },
            String = "Send chat message",
            IsChecked = Service.Config.GpAlertSendChatMessage,
            OnClick = isChecked =>
            {
                Service.Config.GpAlertSendChatMessage = isChecked;
                Service.Interface.SavePluginConfig(Service.Config);
            }
        };
        gpAlertMessageCb.AttachNode(this);

        // Chat Type TextNode
        var gpAlertChatTypeTn = new TextNode
        {
            IsVisible = true,
            Size = new Vector2(ContentSize.X - 20.0f, 20f),
            Position = new Vector2(ContentStartPosition.X + 20, gpAlertMessageCb.Bounds.Bottom),
            String = "Chat channel:",
            FontSize = 14,
        };
        gpAlertChatTypeTn.AttachNode(this);

        // Chat Type Dropdown
        var gpAlertChatTypeDd = new TextDropDownNode
        {
            IsVisible = true,
            Size = new Vector2(ContentSize.X - 20.0f, 24f),
            Position = new Vector2(ContentStartPosition.X + 20, gpAlertChatTypeTn.Bounds.Bottom),
            Options = chatTypeOptions.Values.ToList(),
            SelectedOption = chatTypeOptions[Service.Config.GpAlertChatType],
            OnOptionSelected = selectedItem =>
            {
                Service.Config.GpAlertChatType = chatTypeOptions
                                                 .First(kvp => kvp.Value == selectedItem).Key;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.GatheringController.ComputeRotations();
            },
        };
        gpAlertChatTypeDd.AttachNode(this);
    }
}
