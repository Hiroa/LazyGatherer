using System;
using Dalamud.Utility;
using LazyGatherer.Models;
using Action = Lumina.Excel.Sheets.Action;

namespace LazyGatherer.Solver.Collectable.Model.Actions
{
    public abstract class BaseAction
    {
        protected abstract int Level { get; }
        public abstract Action BotanistAction { get; }
        public abstract Action MinerAction { get; }
        public abstract int Gp { get; }
        public abstract bool IsEndingTurn { get; }

        public virtual bool CanExecute(Rotation rotation)
        {
            var context = rotation.Context;
            if (context.CharacterLevel < Level)
            {
                return false;
            }

            return context.AvailableGp >= Gp;
        }

        public Action GetJobAction()
        {
            var player = Service.ClientState.LocalPlayer;
            var job = (Job)player!.ClassJob.RowId;
            return job switch
            {
                Job.Min => MinerAction,
                Job.Bot => BotanistAction,
                _ => throw new ArgumentOutOfRangeException(nameof(job), $"Unsupported job: {job}")
            };
        }

        public override string ToString()
        {
            var player = Service.ClientState.LocalPlayer;
            var job = (Job)player!.ClassJob.RowId;
            return job switch
            {
                Job.Min => MinerAction.Name.ToDalamudString().ToString(),
                Job.Bot => BotanistAction.Name.ToDalamudString().ToString(),
                _ => "Unknown job"
            };
        }
    }
}
