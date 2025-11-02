using Action = Lumina.Excel.Sheets.Action;

namespace LazyGatherer.Solver.Collectable.Model.Actions;

// Selection
public class Scrutiny : BaseAction
{
    protected override int Level => 50;
    public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(22189);
    public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>().GetRow(22185);
    public override int Gp => 200;
    public override bool IsEndingTurn => false;

    public override bool CanExecute(Rotation rotation)
    {
        return !rotation.Context.HasScrutiny && base.CanExecute(rotation);
    }
}
