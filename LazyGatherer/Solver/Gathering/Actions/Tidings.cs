using LazyGatherer.Solver.Gathering.Models;
using Lumina.Excel.Sheets;

namespace LazyGatherer.Solver.Gathering.Actions
{
    // Évangile de Nophica
    public class Tidings : BaseAction
    {
        protected override int Level => 81;

        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(21204);
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(21203);

        public override bool IsRepeatable => false;

        public override int Gp => 200;

        public override int ExecutionOrder => 1;

        public override bool CanExecute(Rotation rotation)
        {
            return base.CanExecute(rotation) && !rotation.Context.TidingsUsed;
        }

        public override void Execute(GatheringContext context)
        {
            context.BoonBonus++;
            context.TidingsUsed = true;

            base.Execute(context);
        }
    }
}
