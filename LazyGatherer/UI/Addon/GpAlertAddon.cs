using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Text;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Node.Simple;

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

    private readonly Dictionary<uint, string> soundOptions = new()
    {
        { 1, "Sound 1" },
        { 2, "Sound 2" },
        { 3, "Sound 3" },
        { 4, "Sound 4" },
        { 5, "Sound 5" },
        { 6, "Sound 6" },
        { 7, "Sound 7" },
        { 8, "Sound 8" },
        { 9, "Sound 9" },
        { 10, "Sound 10" },
        { 11, "Sound 11" },
        { 12, "Sound 12" },
        { 13, "Sound 13" },
        { 14, "Sound 14" },
        { 15, "Sound 15" },
        { 16, "Sound 16" },
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
            SelectedOption = soundOptions[Service.Config.GpAlertSound],
            OnOptionSelected = selectedItem =>
            {
                Service.Config.GpAlertSound = soundOptions
                                              .First(kvp => kvp.Value == selectedItem).Key;
                UIGlobals.PlayChatSoundEffect(Service.Config.GpAlertSound);
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
