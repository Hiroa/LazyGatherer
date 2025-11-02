using Action = Lumina.Excel.Sheets.Action;

namespace LazyGatherer.Solver.Collectable.Model.Actions;

// Selection instinctive
public class Brazen : BaseAction
{
    protected override int Level => 50;
    public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(22187);
    public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(22183);
    public override int Gp => 0;
    public override bool IsEndingTurn => true;

    public override bool CanExecute(Rotation rotation)
    {
        return rotation.Context.Attempts > 0 && base.CanExecute(rotation);
    }
}
