using System;
using LazyGatherer.Solver.Gathering.Models;
using Action = Lumina.Excel.Sheets.Action;

namespace LazyGatherer.Solver.Gathering.Actions
{
    // Maîtrise du terrain III
    public class Chance3 : BaseAction
    {
        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(294);
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(295);

        public override bool IsRepeatable => false;

        public override int Gp => 250;

        protected override int Level => 10;

        public override int ExecutionOrder => 1;

        public override bool CanExecute(Rotation rotation)
        {
            foreach (var action in rotation.Actions)
            {
                if (action is Chance1 or Chance2)
                {
                    return false;
                }
            }

            return base.CanExecute(rotation);
        }

        public override void Execute(GatheringContext context)
        {
            context.Chance = Math.Min(context.Chance + 0.5, 1);

            base.Execute(context);
        }
    }
}
