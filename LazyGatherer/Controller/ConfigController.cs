using System;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using LazyGatherer.Models;
using LazyGatherer.Windows;

namespace LazyGatherer.Controller;

public class ConfigController : IDisposable
{
    private const String Command = "/lg";
    private readonly WindowSystem windowSystem = new();
    private readonly ConfigWindow configWindow;
    private readonly Action? openConfig;
    public readonly Config Config;

    public ConfigController()
    {
        Config = Service.Interface.GetPluginConfig() as Config ?? new Config();
        configWindow = new ConfigWindow(Config);
        windowSystem.AddWindow(configWindow);
        
        openConfig = () => { configWindow.Toggle(); };
        Service.Interface.UiBuilder.OpenConfigUi += openConfig;
        Service.Interface.UiBuilder.Draw += windowSystem.Draw;
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

}
