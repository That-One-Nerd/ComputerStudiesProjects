namespace AirTrajectoryBuilder.ObjectModels;

public class SweepParameters
{
    public required double AngleDelta, TimeDelta;
    public required double SpeedMin, SpeedMax, SpeedDelta;
    public required double Gravity;
    public required double Tolerance;

    public required double ObjectRadius;
    public required double DragCoefficient;
    public required double Mass;
    public required double AirDensity;

    public required ResultsFileMode FileMode;

    public required Scene Scene;
}
