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
            Service.NativeController = new NativeController(pluginInterface);

            Service.Config = Service.Interface.GetPluginConfig() as Config ?? new Config();
            Service.ConfigAddon = GetConfigAddon();
            Service.CollectableAddon = GetCollectableAddon();
            Service.CollectableAddon.DebugOpen();

            Service.GatheringController = new GatheringController();
            Service.MasterpieceController = new MasterpieceController();
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
            Service.CollectableAddon.Dispose();
            Service.ConfigAddon.Dispose();
            Service.NativeController.Dispose();
        }

        private static void OpenConfig()
            => Service.ConfigAddon.Toggle();

        private static ConfigAddon GetConfigAddon() => new ConfigAddon
        {
            NativeController = Service.NativeController,
            InternalName = "LazyGathererConfig",
            Title = "LazyGatherer configuration",
        };

        private static CollectableAddon GetCollectableAddon() => new CollectableAddon
        {
            NativeController = Service.NativeController,
            InternalName = "LazyGathererCollectable",
            Title = "LazyGatherer collectable configuration",
        };
    }
}
