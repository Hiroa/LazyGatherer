using System;
using Action = Lumina.Excel.GeneratedSheets2.Action;

namespace LazyGatherer.Solver.Actions
{
    // Maîtrise du terrain
    public class Chance1 : BaseAction
    {
        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(218)!;
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(235)!;
        public override bool IsRepeatable => false;

        public override int Gp => 50;

        protected override int Level => 4;

        public override int ExecutionOrder => 1;

        public override void Execute(Data.GatheringContext context)
        {
            context.Chance = Math.Min(context.Chance + 0.05, 1);

            base.Execute(context);
        }
    }
}
