using Dalamud.Utility;
using LazyGatherer.Models;
using LazyGatherer.Solver.Gathering.Models;
using Action = Lumina.Excel.Sheets.Action;

namespace LazyGatherer.Solver.Gathering.Actions
{
    public abstract class BaseAction
    {
        protected abstract int Level { get; }
        public abstract Action BotanistAction { get; }
        public abstract Action MinerAction { get; }
        public abstract bool IsRepeatable { get; }
        public abstract int Gp { get; }
        public abstract int ExecutionOrder { get; }

        public virtual bool CanExecute(Rotation rotation)
        {
            var context = rotation.Context;
            if (context.CharacterLevel < Level)
            {
                return false;
            }

            if (context.AvailableGp >= Gp)
            {
                return true;
            }

            if (IsRepeatable && !rotation.Context.OneTurnRotation)
            {
                return (context.Attempts - 1) * context.GpRegenPerAttempt + context.AvailableGp >= Gp;
            }

            return false;
        }

        public virtual void Execute(GatheringContext context)
        {
            context.AvailableGp -= Gp;
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
