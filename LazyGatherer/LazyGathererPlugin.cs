using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using LazyGatherer.Controller;

namespace LazyGatherer
{
    public sealed class LazyGathererPlugin : IDalamudPlugin
    {
        private readonly ConfigController configController;
        private readonly GatheringController gatheringController;
        private readonly UIController uiController;

        public LazyGathererPlugin(DalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<Service>();

            configController = new ConfigController();
            gatheringController = new GatheringController();
            uiController = new UIController(configController.Config, gatheringController.GatheringOutcomes);

            Service.Framework.Update += OnFrameworkUpdate;
        }

        public void Dispose()
        {
            Service.Framework.Update -= OnFrameworkUpdate;
            uiController.Dispose();
            gatheringController.Dispose();
            configController.Dispose();
        }
        
        private void OnFrameworkUpdate(IFramework framework)
        {
            gatheringController.OnFrameworkUpdate();
            uiController.OnFrameworkUpdate();
        }
    }
}
