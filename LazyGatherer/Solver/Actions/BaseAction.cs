using Dalamud.Utility;
using LazyGatherer.Solver.Data;
using Action = Lumina.Excel.GeneratedSheets2.Action;

namespace LazyGatherer.Solver.Actions
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
            else if (IsRepeatable)
            {
                return (context.Attempts - 1) * context.GpRegenPerAttempt + context.AvailableGp >= Gp;
            }
            else
            {
                return false;
            }
        }

        public virtual void Execute(GatheringContext context)
        {
            context.AvailableGp -= Gp;
        }

        public override string ToString()
        {
            var player = Service.ClientState.LocalPlayer;
            var job = (Job)player!.ClassJob.Id;
            return job switch
            {
                Job.Min => MinerAction.Name.ToDalamudString().ToString(),
                Job.Bot => BotanistAction.Name.ToDalamudString().ToString(),
                _ => "Unknown job"
            };
        }
    }
}
