using System.Numerics;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using KamiToolKit;
using LazyGatherer.Controller;
using LazyGatherer.Models;
using LazyGatherer.UI;

namespace LazyGatherer
{
    public sealed class LazyGathererPlugin : IDalamudPlugin
    {
        public LazyGathererPlugin(IDalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<Service>();
            Service.NativeController = new NativeController(pluginInterface);

            Service.Config = Service.Interface.GetPluginConfig() as Config ?? new Config();
            Service.GatheringController = new GatheringController();
            Service.UIController = new UIController(Service.GatheringController.GatheringOutcomes);

            Service.ConfigAddon = new ConfigAddon
            {
                Size = new Vector2(270.0f, 200.0f),
                Position = new Vector2(300.0f, 300.0f),
                InternalName = "LazyGathererConfig",
                NativeController = Service.NativeController,
                Title = "LazyGatherer configuration",
            };

            Service.Interface.UiBuilder.OpenConfigUi += OpenConfig;
            Service.Framework.Update += OnFrameworkUpdate;
        }

        public void Dispose()
        {
            Service.Framework.Update -= OnFrameworkUpdate;
            Service.Interface.UiBuilder.OpenConfigUi -= OpenConfig;
            Service.ConfigAddon.Dispose();
            Service.UIController.Dispose();
            Service.GatheringController.Dispose();
            Service.NativeController.Dispose();
        }

        private void OnFrameworkUpdate(IFramework framework)
        {
            Service.GatheringController.OnFrameworkUpdate();
            Service.UIController.OnFrameworkUpdate();
        }

        private static void OpenConfig()
            => Service.ConfigAddon.Toggle();
    }
}
