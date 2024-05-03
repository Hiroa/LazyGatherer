using LazyGatherer.Solver.Data;
using Lumina.Excel.GeneratedSheets2;

namespace LazyGatherer.Solver.Actions
{
    // Récolte bénie
    public class Yield1 : BaseAction
    {
        protected override int Level => 30;

        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(222)!;
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(239)!;

        public override bool IsRepeatable => false;

        public override int Gp => 400;

        public override int ExecutionOrder => 1;

        public override void Execute(GatheringContext context)
        {
            context.BaseAmount++;

            base.Execute(context);
        }
    }
}
