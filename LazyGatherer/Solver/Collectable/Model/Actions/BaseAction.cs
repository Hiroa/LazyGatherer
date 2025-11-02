using System.Collections.Generic;
using System.Linq;
using Dalamud.Utility;
using LazyGatherer.Models;
using Action = Lumina.Excel.Sheets.Action;

namespace LazyGatherer.Solver.Collectable.Model.Actions
{
    public abstract class BaseAction
    {
        public static Dictionary<CollectableAction, BaseAction> Actions = new()
        {
            { CollectableAction.Attempt, new Attempt() },
            { CollectableAction.Brazen, new Brazen() },
            { CollectableAction.Collect, new Collect() },
            { CollectableAction.Meticulous, new Meticulous() },
            { CollectableAction.Scour, new Scour() },
            { CollectableAction.Scrutiny, new Scrutiny() },
            { CollectableAction.Wise, new Wise() }
        };

        public enum CollectableAction
        {
            Attempt,
            Brazen,
            Collect,
            Meticulous,
            Scour,
            Scrutiny,
            Wise
        }

        public int Level { get; init; }
        public uint MinerAction { get; init; }
        public uint BotanistAction { get; init; }
        public string MinerActionName { get; init; }
        public string BotanistActionName { get; init; }
        public int Gp { get; init; }
        public bool IsEndingTurn { get; init; }

        protected BaseAction(uint minerId, uint botanistId)
        {
            var minerAction = Service.DataManager.Excel.GetSheet<Action>().GetRow(minerId);
            var botanistAction = Service.DataManager.Excel.GetSheet<Action>().GetRow(botanistId);
            Level = minerAction.ClassJobLevel;
            if (minerAction.PrimaryCostType == 7) // GP cost
            {
                Gp = minerAction.PrimaryCostValue;
            }

            MinerAction = minerId;
            BotanistAction = botanistId;
            MinerActionName = minerAction.Name.ToDalamudString().ToString();
            BotanistActionName = botanistAction.Name.ToDalamudString().ToString();
            IsEndingTurn = minerAction.CastType != 1;
        }

        public virtual bool CanExecute(Rotation rotation)
        {
            var context = rotation.Context;
            if (context.CharacterLevel < Level)
            {
                return false;
            }

            return context.AvailableGp >= Gp;
        }

        public uint GetJobAction()
        {
            var player = Service.ClientState.LocalPlayer;
            var job = (Job)player!.ClassJob.RowId;
            return job switch
            {
                Job.Min => MinerAction,
                Job.Bot => BotanistAction,
                _ => 0
            };
        }

        public override string ToString()
        {
            var player = Service.ClientState.LocalPlayer;
            var job = (Job)player!.ClassJob.RowId;
            return job switch
            {
                Job.Min => MinerActionName,
                Job.Bot => BotanistActionName,
                _ => "Unknown job"
            };
        }

        public CollectableAction ToPreset()
        {
            return Actions.First(a => a.Value == this).Key;
        }

        public static BaseAction FromPreset(CollectableAction refAction)
        {
            return Actions[refAction];
        }
    }
}
