using Nerd_STF.Mathematics;
using System.Collections.Generic;
using System.IO;

namespace AirTrajectoryBuilder.ObjectModels;

public class SimulationResult
{
    public double Duration;

    public double EndDistanceSquared;

    public double EndSpeed;

    public List<Float2> Trail = [];
    public List<TableEntry>? Table = null;

    public SimulationParameters StartingConditions;

    public SimulationResult(SimulationParameters starting)
    {
        StartingConditions = starting;
    }
}
