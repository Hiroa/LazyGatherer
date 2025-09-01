using LazyGatherer.Solver.Models;
using Lumina.Excel.Sheets;

namespace LazyGatherer.Solver.Actions
{
    // Bénédiction de la nature
    public class GivingLand : BaseAction
    {
        protected override int Level => 74;

        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(4590);
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(4589);
        public override bool IsRepeatable => false;
        public override int Gp => 200;
        public override int ExecutionOrder => 1;

        public override bool CanExecute(Rotation rotation)
        {
            var context = rotation.Context;
            return base.CanExecute(rotation) && context.Item.ItemSearchCategory.RowId == 58; // Crystal
        }

        public override void Execute(GatheringContext context)
        {
            context.BaseAmount += 15;

            base.Execute(context);
        }
    }
}
