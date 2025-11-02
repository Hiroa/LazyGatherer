using Lumina.Excel.Sheets;

namespace LazyGatherer.Solver.Collectable.Model.Actions
{
    // Sagesse du fermier
    public class Wise : BaseAction
    {
        protected override int Level => 25;

        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(26522);
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(26521);
        public override int Gp => 300;
        public override bool IsEndingTurn => false;

        public override bool CanExecute(Rotation rotation)
        {
            return rotation.Context.Attempts < rotation.Context.MaxAttempts && base.CanExecute(rotation);
        }
    }
}
