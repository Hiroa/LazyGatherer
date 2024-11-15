using System;
using LazyGatherer.Solver.Data;
using Action = Lumina.Excel.Sheets.Action;

namespace LazyGatherer.Solver.Actions
{
    // Sols fertiles I
    public class Gift1 : BaseAction
    {
        protected override int Level => 15;

        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(21178);
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(21177);

        public override bool IsRepeatable => false;

        public override int Gp => 50;

        public override int ExecutionOrder => 1;

        public override bool CanExecute(Rotation rotation)
        {
            var context = rotation.Context;
            return base.CanExecute(rotation) && context.HasBoon;
        }

        public override void Execute(GatheringContext context)
        {
            context.Boon = Math.Min(context.Boon + 0.1, 1);

            base.Execute(context);
        }
    }
}
