using LazyGatherer.Solver.Models;
using Lumina.Excel.Sheets;

namespace LazyGatherer.Solver.Actions
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

        public override void Execute(GatheringContext context)
        {
            context.BoonBonus++;

            base.Execute(context);
        }
    }
}
