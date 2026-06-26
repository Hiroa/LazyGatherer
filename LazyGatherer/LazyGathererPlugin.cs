using Dalamud.Game.Command;
using Dalamud.Plugin;
using KamiToolKit;
using LazyGatherer.Controller;
using LazyGatherer.Models;
using LazyGatherer.UI.Addon;

namespace LazyGatherer
{
    public sealed class LazyGathererPlugin : IDalamudPlugin
    {
        public LazyGathererPlugin(IDalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<Service>();
            KamiToolKitLibrary.Initialize(pluginInterface);

            Service.Config = Service.Interface.GetPluginConfig() as Config ?? new Config();
            Service.ConfigAddon = GetConfigAddon();
            Service.GpAlertAddon = GetGpAlertAddon();

            Service.UIController = new UIController();
            Service.GatheringController = new GatheringController();
            Service.MasterpieceController = new MasterpieceController();
            Service.GpAlertController = new GpAlertController();
            Service.Hooks = new Hooks();

            Service.Interface.UiBuilder.OpenConfigUi += OpenConfig;

            Service.CommandManager.AddHandler("/lazygatherer", new CommandInfo(OnCommand)
            {
                HelpMessage = "Open the configuration"
            });
            Service.CommandManager.AddHandler("/lg", new CommandInfo(OnCommand)
            {
                HelpMessage = "Open the configuration"
            });
        }

        public void Dispose()
        {
            Service.CommandManager.RemoveHandler("/lg");
            Service.CommandManager.RemoveHandler("/lazygatherer");
            Service.Interface.UiBuilder.OpenConfigUi -= OpenConfig;
            Service.Hooks.Dispose();
            Service.MasterpieceController.Dispose();
            Service.GatheringController.Dispose();
            Service.UIController.Dispose();
            Service.GpAlertController.Dispose();
            Service.ConfigAddon.Dispose();
            Service.GpAlertAddon.Dispose();
            KamiToolKitLibrary.Dispose();
        }

        private void OnCommand(string command, string arguments)
        {
            OpenConfig();
        }

        private static void OpenConfig()
            => Service.ConfigAddon.Toggle();

        private static ConfigAddon GetConfigAddon() => new ConfigAddon
        {
            InternalName = "LazyGathererConfig",
            Title = "LazyGatherer - Configuration",
        };

        private GpAlertAddon GetGpAlertAddon() => new GpAlertAddon
        {
            InternalName = "LazyGathererGpAlertConfig",
            Title = "LazyGatherer - GP alert configuration",
        };
    }
}
