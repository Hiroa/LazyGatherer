using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using KamiToolKit;
using LazyGatherer.Controller;

namespace LazyGatherer
{
    public sealed class LazyGathererPlugin : IDalamudPlugin
    {
        private readonly ConfigController configController;
        private readonly GatheringController gatheringController;

        public LazyGathererPlugin(IDalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<Service>();
            Service.NativeController = new NativeController(pluginInterface);

            configController = new ConfigController();
            gatheringController = new GatheringController();
            Service.UIController = new UIController(gatheringController.GatheringOutcomes);

            Service.Framework.Update += OnFrameworkUpdate;
        }

        public void Dispose()
        {
            Service.Framework.Update -= OnFrameworkUpdate;
            Service.UIController.Dispose();
            gatheringController.Dispose();
            configController.Dispose();
        }

        private void OnFrameworkUpdate(IFramework framework)
        {
            gatheringController.OnFrameworkUpdate();
            Service.UIController.OnFrameworkUpdate();
        }
    }
}
