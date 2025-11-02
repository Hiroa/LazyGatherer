using Lumina.Excel.Sheets;

namespace LazyGatherer.Solver.Collectable.Model.Actions
{
    // Sagesse du fermier
    public class Attempt : BaseAction
    {
        protected override int Level => 25;

        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(215);
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(232);
        public override int Gp => 300;
        public override bool IsEndingTurn => false;

        public override void Execute(Context context)
        {
            context.Attempts++;

            base.Execute(context);
        }
    }
}
