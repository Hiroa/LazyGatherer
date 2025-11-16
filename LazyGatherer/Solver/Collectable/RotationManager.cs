using System.Linq;
using LazyGatherer.Solver.Collectable.Model;
using LazyGatherer.Solver.Collectable.Model.Actions;

namespace LazyGatherer.Solver.Collectable;

public static class RotationManager
{
    private static readonly Wise Wise = new();

    public static BaseAction? GetNextAction(Rotation rotation)
    {
        Service.Log.Info("Determining next action...\n" + rotation.Context);
        // Prioritize wise if not max attempts - no matter the rotation
        if (Wise.CanExecute(rotation))
        {
            return Wise;
        }

        var count = 0;
        foreach (var turn in rotation.Turns)
        {
            Service.Log.Info("Checking turn " + count++);
            if (turn.Conditions.All(c => c.IsSatisfied(rotation.Context)))
            {
                var action = turn.Actions.FirstOrDefault(a => a.CanExecute(rotation));
                if (action != null)
                {
                    return action;
                }
            }
        }

        return null;
    }

    public static bool IsValidRotation(Rotation rotation)
    {
        foreach (var turn in rotation.Turns)
        {
            var actions = turn.Actions.Where(a => a.IsEndingTurn).ToList();
            if (actions.Count > 1)
            {
                Service.Log.Info("Invalid rotation: More than one ending turn action in a single turn.");
                Service.Log.Info("Turn actions: " + string.Join(", ", actions));
                return false;
            }
        }

        return true;
    }
}
