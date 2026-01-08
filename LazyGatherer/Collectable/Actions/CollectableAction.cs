using System.Collections.Generic;
using System.Linq;
using Dalamud.Utility;
using LazyGatherer.Models;
using Action = Lumina.Excel.Sheets.Action;

namespace LazyGatherer.Collectable.Actions
{
    public abstract class CollectableAction
    {
        public static readonly Dictionary<Names, CollectableAction> Actions = new()
        {
            { Names.Attempt, new Attempt() },
            { Names.Brazen, new Brazen() },
            { Names.Collect, new Collect() },
            { Names.Meticulous, new Meticulous() },
            { Names.Scour, new Scour() },
            { Names.Scrutiny, new Scrutiny() },
            { Names.Wise, new Wise() },
            { Names.Focus, new Focus() },
            { Names.Priming, new Priming() }
        };

        public enum Names
        {
            Attempt,
            Brazen,
            Collect,
            Meticulous,
            Scour,
            Scrutiny,
            Wise,
            Focus,
            Priming,
        }

        public int Level { get; init; }
        public uint MinerAction { get; init; }
        public uint BotanistAction { get; init; }
        public string MinerActionName { get; init; }
        public string BotanistActionName { get; init; }
        public int Gp { get; init; }
        public bool IsEndingTurn { get; init; }

        protected CollectableAction(uint minerId, uint botanistId)
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
            var player = Service.ObjectTable.LocalPlayer;
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
            var player = Service.ObjectTable.LocalPlayer;
            var job = (Job)player!.ClassJob.RowId;
            return job switch
            {
                Job.Min => MinerActionName,
                Job.Bot => BotanistActionName,
                _ => "Unknown job"
            };
        }

        public Names ToPreset()
        {
            return Actions.First(a => a.Value == this).Key;
        }

        public static CollectableAction FromPreset(Names refAction)
        {
            return Actions[refAction];
        }
    }
}
