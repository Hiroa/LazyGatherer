using System;
using System.Collections.Generic;
using LazyGatherer.Models;
using LazyGatherer.Solver.Data;
using LazyGatherer.UI;

namespace LazyGatherer.Controller;

public class UIController(
    Config config,
    List<KeyValuePair<Rotation, GatheringOutcome>> outcomes)
    : IDisposable
{
    private readonly List<RotationUI> rotationUis = [];

    public void Dispose()
    {
        ClearUi();
    }

    public void OnFrameworkUpdate()
    {
        if (outcomes.Count != rotationUis.Count)
        {
            ClearUi();
            InitUi(outcomes);
        }

        if (config.AsChanged)
        {
            rotationUis.ForEach(r => r.Update(config));
            config.AsChanged = false;
        }
        rotationUis.ForEach(r => r.OnFramework());
    }

    private void InitUi(List<KeyValuePair<Rotation, GatheringOutcome>> gatheringOutcomes)
    {
        foreach (var go in gatheringOutcomes)
        {
            rotationUis.Add(new RotationUI(go, config));
        }
    }

    private void ClearUi()
    {
        rotationUis.ForEach(r => r.Dispose());
        rotationUis.Clear();
    }
}
