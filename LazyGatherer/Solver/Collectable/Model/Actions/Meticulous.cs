using Action = Lumina.Excel.Sheets.Action;

namespace LazyGatherer.Solver.Collectable.Model.Actions;

// Selection méthodique
public class Meticulous : BaseAction
{
    protected override int Level => 50;
    public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(22188);
    public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(22184);
    public override int Gp => 0;
    public override bool IsEndingTurn => true;

    public override bool CanExecute(Rotation rotation)
    {
        return rotation.Context.Attempts > 0 && base.CanExecute(rotation);
    }
}
