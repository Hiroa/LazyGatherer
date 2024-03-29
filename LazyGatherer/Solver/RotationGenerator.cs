using System;
using System.Collections.Generic;
using System.Linq;
using LazyGatherer.Solver.Actions;
using LazyGatherer.Solver.Data;

namespace LazyGatherer.Solver
{

    public class RotationGenerator
    {
        private readonly List<BaseAction> availableActions;
        private readonly List<Rotation> rotations;

        public RotationGenerator()
        {
            availableActions = new List<BaseAction>();
            rotations = new List<Rotation>();
            foreach (var t in GetType().Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(BaseAction))))
            {
                if (t.IsAbstract) continue;
                var module = (BaseAction?)Activator.CreateInstance(t);
                if (module is null) continue;

                availableActions.Add(module);
            }
            this.availableActions = availableActions.OrderBy(a => a.ExecutionOrder).ToList();
        }

        public List<Rotation> GetRotations(GatheringContext initialContext)
        {
            rotations.Clear();
            var emptyRotation = new Rotation(initialContext);
            rotations.Add(emptyRotation);
            NextRotation(0, emptyRotation);
            return rotations;
        }

        private void NextRotation(int currActionIdx, Rotation currRotation)
        {
            if (currActionIdx >= availableActions.Count)
            {
                return;
            }

            var currAction = availableActions[currActionIdx];

            if (currAction.IsRepeatable)
            {
                // generate a rotation each time action is repeated
                var actionRepeatedRotation = currRotation;

                while (currAction.CanExecute(actionRepeatedRotation.Context))
                {
                    actionRepeatedRotation = actionRepeatedRotation.Clone();
                    actionRepeatedRotation.AddAction(currAction);
                    rotations.Add(actionRepeatedRotation);
                    NextRotation(currActionIdx + 1, actionRepeatedRotation);
                }
            }
            else if (currAction.CanExecute(currRotation.Context))
            {
                // generate rotations with action executed
                var actionExecutedRotation = currRotation.Clone();
                actionExecutedRotation.AddAction(currAction);
                rotations.Add(actionExecutedRotation);
                NextRotation(currActionIdx + 1, actionExecutedRotation);
            }

            // generate rotations without action executed
            NextRotation(currActionIdx + 1, currRotation);
        }
    }
}
