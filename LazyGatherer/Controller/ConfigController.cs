using System;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using LazyGatherer.Models;
using LazyGatherer.Windows;

namespace LazyGatherer.Controller;

public class ConfigController : IDisposable
{
    private const String Command = "/lg";
    private readonly WindowSystem windowSystem;
    private readonly ConfigWindow configWindow;
    private readonly Action? openConfig;

    public ConfigController()
    {
        Service.Config = Service.Interface.GetPluginConfig() as Config ?? new Config();
        windowSystem = new WindowSystem(Service.Interface.Manifest.InternalName);
        configWindow = new ConfigWindow();
        windowSystem.AddWindow(configWindow);

        openConfig = () => { configWindow.Toggle(); };
        Service.Interface.UiBuilder.Draw += windowSystem.Draw;
        Service.Interface.UiBuilder.OpenConfigUi += openConfig;
        Service.Commands.AddHandler(Command, new CommandInfo(CommandHandler)
        {
            HelpMessage = "Open configuration"
        });
    }

    public void Dispose()
    {
        Service.Commands.RemoveHandler(Command);
        Service.Interface.UiBuilder.Draw -= windowSystem.Draw;
        Service.Interface.UiBuilder.OpenConfigUi -= openConfig;
        windowSystem.RemoveAllWindows();
        configWindow.Dispose();
    }

    private void CommandHandler(string command, string arguments)
    {
        configWindow.Toggle();
    }

    public void ToggleConfigWindow()
    {
        configWindow.Toggle();
    }

    public void ToggleDisplay()
    {
        Service.Config.Display = !Service.Config.Display;
        Service.Interface.SavePluginConfig(Service.Config);
        Service.UIController.Update(false);
    }
}
