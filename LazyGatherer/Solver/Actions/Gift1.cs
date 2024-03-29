using System;
using Action = Lumina.Excel.GeneratedSheets2.Action;

namespace LazyGatherer.Solver.Actions
{
    // Sols fertiles I
    public class Gift1 : BaseAction
    {
        protected override int Level => 15;

        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(21178)!;
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(21177)!;

        public override bool IsRepeatable => false;

        public override int Gp => 50;

        public override int ExecutionOrder => 1;
        
        public override bool CanExecute(Data.GatheringContext context)
        {
            return base.CanExecute(context) && context.HasBoon;
        }

        public override void Execute(Data.GatheringContext context)
        {
            context.Boon = Math.Min(context.Boon + 0.1, 1);

            base.Execute(context);
        }
    }
}
