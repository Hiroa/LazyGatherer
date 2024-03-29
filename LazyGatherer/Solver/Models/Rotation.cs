using System.Collections.Generic;
using LazyGatherer.Solver.Actions;

namespace LazyGatherer.Solver.Data
{
    public class Rotation
    {
        public Rotation(GatheringContext context)
        {
            Context = context;
            Actions = new List<BaseAction>();
        }

        public List<BaseAction> Actions { get; }

        public GatheringContext Context { get; }

        public void AddAction(BaseAction action)
        {
            Actions.Add(action);
            action.Execute(Context);
        }
        
        public Rotation Clone()
        {
            var copiedRotation = new Rotation(Context.Clone());
            copiedRotation.Actions.AddRange(Actions);
            return copiedRotation;
        }
    }
}
