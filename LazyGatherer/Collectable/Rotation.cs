using System.Collections.Generic;
using System.Linq;
using LazyGatherer.Collectable.Actions;
using LazyGatherer.Collectable.Presets;
using static LazyGatherer.Collectable.Actions.CollectableAction;

namespace LazyGatherer.Collectable;

public class Rotation
{
    public required Context Context { get; init; }
    public List<Step> Steps { get; init; } = [];

    public static Rotation FromPreset(Preset preset, Context ctx)
    {
        return new Rotation
        {
            Context = ctx,
            Steps = preset.Steps
        };
    }

    public CollectableAction? GetNextAction()
    {
        // Prioritize wise if not max attempts - no matter the rotation
        if (CollectableAction.Actions[Names.Wise].CanExecute(this))
        {
            return CollectableAction.Actions[Names.Wise];
        }

        foreach (var turn in Steps)
        {
            if (turn.Conditions.All(c => c.IsSatisfied(Context)))
            {
                var actions = turn.Actions.Select(a => CollectableAction.Actions[a]);
                var action = actions.FirstOrDefault(a => a.CanExecute(this));
                if (action != null)
                {
                    return action;
                }
            }
        }

        return null;
    }

    public bool IsValidRotation()
    {
        foreach (var turn in Steps)
        {
            var actions = turn.Actions
                              .Select(a => CollectableAction.Actions[a])
                              .Where(a => a.IsEndingTurn).ToList();
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
