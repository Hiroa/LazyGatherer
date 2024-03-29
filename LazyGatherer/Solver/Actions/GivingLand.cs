using Lumina.Excel.GeneratedSheets2;
using MathNet.Numerics.Distributions;

namespace LazyGatherer.Solver.Actions
{
    // Bénédiction de la nature
    public class GivingLand : BaseAction
    {
        protected override int Level => 74;

        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(4590)!;
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(4589)!;
        public override bool IsRepeatable => false;
        public override int Gp => 200;
        public override int ExecutionOrder => 1;
        public override bool CanExecute(Data.GatheringContext context)
        {
            return base.CanExecute(context) && context.Item.ItemSearchCategory.Row == 58; // Crystal
        }

        public override void Execute(Data.GatheringContext context)
        {
            context.BaseAmount += 15;

            base.Execute(context);
        }
    }
}
