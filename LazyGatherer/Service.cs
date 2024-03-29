using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using LazyGatherer.Models;

namespace LazyGatherer
{
    internal class Service
    {
        [PluginService]
        internal static DalamudPluginInterface Interface { get; private set; } = null!;

        [PluginService]
        internal static IClientState ClientState { get; private set; } = null!;

        [PluginService]
        internal static IDataManager DataManager { get; private set; } = null!;

        [PluginService]
        internal static IFramework Framework { get; private set; } = null!;

        [PluginService]
        internal static IGameGui GameGui { get; private set; } = null!;

        [PluginService]
        internal static ICommandManager Commands { get; private set; } = null!;

        [PluginService]
        internal static IPluginLog Log { get; private set; } = null!;
    }
}
