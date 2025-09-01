using System;
using System.Collections.Generic;
using System.Text;
using Dalamud.Utility;
using LazyGatherer.Models;
using LazyGatherer.Solver.Actions;

namespace LazyGatherer.Solver.Models
{
    public class Rotation(GatheringContext context)
    {
        public List<BaseAction> Actions { get; } = [];

        public GatheringContext Context { get; } = context;

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

        public string ToString(GatheringContext context)
        {
            var job = context.Job;
            var sb = new StringBuilder();
            sb.Append(context.Item.Name.ToDalamudString())
              .Append(" => ");
            foreach (var action in Actions)
            {
                var igAction = job switch
                {
                    Job.Bot => action.BotanistAction,
                    Job.Min => action.MinerAction,
                    _ => throw new ArgumentOutOfRangeException()
                };
                sb.Append(igAction.Name)
                  .Append(" (")
                  .Append(igAction.RowId)
                  .Append("), ");
            }

            return sb.ToString();
        }
    }
}
