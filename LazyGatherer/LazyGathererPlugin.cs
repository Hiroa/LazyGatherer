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

            Service.GatheringController = new GatheringController();
            Service.MasterpieceController = new MasterpieceController();
            Service.GpAlertController = new GpAlertController();
            Service.UIController = new UIController();
            Service.Hooks = new Hooks();

            Service.Interface.UiBuilder.OpenConfigUi += OpenConfig;
        }

        public void Dispose()
        {
            Service.Interface.UiBuilder.OpenConfigUi -= OpenConfig;
            Service.Hooks.Dispose();
            Service.UIController.Dispose();
            Service.MasterpieceController.Dispose();
            Service.GatheringController.Dispose();
            Service.ConfigAddon.Dispose();
            Service.GpAlertAddon.Dispose();
            KamiToolKitLibrary.Dispose();
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
