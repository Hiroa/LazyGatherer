using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using LazyGatherer.Controller;
using LazyGatherer.Models;
using LazyGatherer.UI.Addon;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace LazyGatherer
{
    internal class Service
    {
        // Dalamud service
        [PluginService]
        internal static IDalamudPluginInterface Interface { get; private set; } = null!;

        [PluginService]
        internal static IDataManager DataManager { get; private set; } = null!;

        [PluginService]
        internal static IGameGui GameGui { get; private set; } = null!;

        [PluginService]
        internal static IPluginLog Log { get; private set; } = null!;

        [PluginService]
        public static IGameInteropProvider GameInteropProvider { get; private set; } = null!;

        [PluginService]
        public static IObjectTable ObjectTable { get; private set; } = null!;

        [PluginService]
        public static IPlayerState PlayerState { get; private set; } = null!;

        [PluginService]
        public static IClientState ClientState { get; private set; } = null!;

        [PluginService]
        public static IFramework Framework { get; private set; } = null!;

        [PluginService]
        public static IChatGui ChatGui { get; private set; } = null!;

        [PluginService]
        public static IDutyState DutyState { get; private set; } = null!;

        // Plugin service
        internal static Config Config { get; set; }
        internal static UIController UIController { get; set; }
        internal static GatheringController GatheringController { get; set; }
        internal static MasterpieceController MasterpieceController { get; set; }
        internal static GpAlertController GpAlertController { get; set; }
        internal static Hooks Hooks { get; set; }

        // Addons
        internal static ConfigAddon ConfigAddon { get; set; }
        internal static GpAlertAddon GpAlertAddon { get; set; }
    }
}
