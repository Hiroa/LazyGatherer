using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using KamiToolKit;
using LazyGatherer.Controller;

namespace LazyGatherer
{
    public sealed class LazyGathererPlugin : IDalamudPlugin
    {
        private readonly ConfigController configController;

        public LazyGathererPlugin(IDalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<Service>();
            Service.NativeController = new NativeController(pluginInterface);

            configController = new ConfigController();
            Service.GatheringController = new GatheringController();
            Service.UIController = new UIController(Service.GatheringController.GatheringOutcomes);

            Service.Framework.Update += OnFrameworkUpdate;
        }

        public void Dispose()
        {
            Service.Framework.Update -= OnFrameworkUpdate;
            Service.UIController.Dispose();
            Service.GatheringController.Dispose();
            configController.Dispose();
        }

        private void OnFrameworkUpdate(IFramework framework)
        {
            Service.GatheringController.OnFrameworkUpdate();
            Service.UIController.OnFrameworkUpdate();
        }
    }
}
