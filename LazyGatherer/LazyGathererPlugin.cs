using System.Numerics;
using Dalamud.Plugin;
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
            Service.ConfigAddon = new ConfigAddon
            {
                Size = new Vector2(270.0f, 190.0f),
                Position = new Vector2(300.0f, 300.0f),
                InternalName = "LazyGathererConfig",
                NativeController = Service.NativeController,
                Title = "LazyGatherer configuration",
            };

            Service.GatheringController = new GatheringController();
            Service.UIController = new UIController();

            Service.Interface.UiBuilder.OpenConfigUi += OpenConfig;
        }

        public void Dispose()
        {
            Service.Interface.UiBuilder.OpenConfigUi -= OpenConfig;
            Service.UIController.Dispose();
            Service.GatheringController.Dispose();
            Service.ConfigAddon.Dispose();
            Service.NativeController.Dispose();
        }

        private static void OpenConfig()
            => Service.ConfigAddon.Toggle();
    }
}
