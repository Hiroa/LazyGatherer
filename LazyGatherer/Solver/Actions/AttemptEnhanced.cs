using LazyGatherer.Solver.Models;
using Lumina.Excel.Sheets;

namespace LazyGatherer.Solver.Actions
{
    // Sagesse du fermier améliorer
    public class AttemptEnhanced : BaseAction
    {
        protected override int Level => 90;

        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(215);
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(232);
        public override bool IsRepeatable => true;
        public override int Gp => 300;
        public override int ExecutionOrder => 2;

        public override bool CanExecute(Rotation rotation)
        {
            var context = rotation.Context;
            return !context.OneTurnRotation && base.CanExecute(rotation);
        }

        public override void Execute(GatheringContext context)
        {
            context.Attempts++;
            context.WiseAttempts++;

            base.Execute(context);
        }
    }
}
