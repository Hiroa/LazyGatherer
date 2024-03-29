﻿
using Lumina.Excel.GeneratedSheets2;

namespace LazyGatherer.Solver.Actions
{
    // Récolte abondante II
    public class Bountiful2 : BaseAction
    {
        public override Action BotanistAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(273)!;
        public override Action MinerAction => Service.DataManager.Excel.GetSheet<Action>()!.GetRow(272)!;
        public override bool IsRepeatable => true;

        public override int Gp => 100;

        protected override int Level => 68;

        public override int ExecutionOrder => 3;

        public override void Execute(Data.GatheringContext context)
        {
            context.BountifulAttempts++;

            base.Execute(context);
        }
    }
}
