using System;
using Action = Lumina.Excel.GeneratedSheets2.Action;

namespace LazyGatherer.Solver.Actions
{
    // Maîtrise du terrain II
    public class Chance2 : BaseAction
    {
        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(220)!;
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(237)!;

        public override bool IsRepeatable => false;

        public override int Gp => 100;

        protected override int Level => 5;

        public override int ExecutionOrder => 1;

        public override void Execute(Data.GatheringContext context)
        {
            context.Chance = Math.Min(context.Chance + 0.15, 1);
            
            base.Execute(context);
        }
    }
}
