using Lumina.Excel.GeneratedSheets2;

namespace LazyGatherer.Solver.Actions
{
    // Sagesse du fermier
    public class Attempt : BaseAction
    {
        protected override int Level => 25;

        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(215)!;
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(232)!;
        public override bool IsRepeatable => true;
        public override int Gp => 300;
        public override int ExecutionOrder => 2;
        public override bool CanExecute(Data.GatheringContext context)
        {
            return base.CanExecute(context) && context.CharacterLevel < 90;
        }

        public override void Execute(Data.GatheringContext context)
        {
            context.Attempts++;

            base.Execute(context);
        }
    }
}
