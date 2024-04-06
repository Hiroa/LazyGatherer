using Lumina.Excel.GeneratedSheets2;

namespace LazyGatherer.Solver.Actions
{
    // Bénédiction des Douze
    public class TwelveBounty : BaseAction
    {
        protected override int Level => 20;

        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(282)!;
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(280)!;
        public override bool IsRepeatable => false;
        public override int Gp => 150;
        public override int ExecutionOrder => 1;

        public override bool CanExecute(Data.GatheringContext context)
        {
            return base.CanExecute(context) && context.Item.ItemSearchCategory.Row == 58; // Crystal
        }

        public override void Execute(Data.GatheringContext context)
        {
            context.BaseAmount += 3;

            base.Execute(context);
        }
    }
}
