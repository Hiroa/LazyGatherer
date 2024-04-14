using Lumina.Excel.GeneratedSheets2;

namespace LazyGatherer.Solver.Actions
{
    // Récolte abondante
    public class Bountiful1 : BaseAction
    {
        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(4087)!;
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(4073)!;
        public override bool IsRepeatable => true;

        public override int Gp => 100;

        protected override int Level => 24;

        public override int ExecutionOrder => 3;

        public override bool CanExecute(Data.GatheringContext context)
        {
            return base.CanExecute(context) && context.CharacterLevel < 68;
        }

        public override void Execute(Data.GatheringContext context)
        {
            context.BountifulAttempts++;

            base.Execute(context);
        }
    }
}
