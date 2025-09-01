using System;
using LazyGatherer.Solver.Models;
using Action = Lumina.Excel.Sheets.Action;

namespace LazyGatherer.Solver.Actions
{
    // Sols fertiles II
    public class Gift2 : BaseAction
    {
        protected override int Level => 50;

        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(25590);
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(25589);

        public override bool IsRepeatable => false;

        public override int Gp => 100;

        public override int ExecutionOrder => 1;

        public override bool CanExecute(Rotation rotation)
        {
            var context = rotation.Context;
            return base.CanExecute(rotation) && context.HasBoon;
        }

        public override void Execute(GatheringContext context)
        {
            context.Boon = Math.Min(context.Boon + 0.3, 1);

            base.Execute(context);
        }
    }
}
