using LazyGatherer.Solver.Models;
using Lumina.Excel.Sheets;

namespace LazyGatherer.Solver.Actions
{
    // Récolte bénie II
    public class Yield2 : BaseAction
    {
        protected override int Level => 40;

        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(224);
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(241);

        public override bool IsRepeatable => false;

        public override int Gp => 500;

        public override int ExecutionOrder => 1;

        public override bool CanExecute(Rotation rotation)
        {
            return base.CanExecute(rotation) && !rotation.Context.YieldUsed;
        }

        public override void Execute(GatheringContext context)
        {
            context.BaseAmount += 2;
            context.YieldUsed = true;

            base.Execute(context);
        }
    }
}
