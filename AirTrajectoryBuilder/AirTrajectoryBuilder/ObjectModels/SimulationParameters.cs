namespace AirTrajectoryBuilder.ObjectModels;

public class SimulationParameters
{
    public required double StartAngle;
    public required double StartVelocity;
    public required double DeltaTime;
    public required double Gravity;
    public required double ToleranceSquared;
    public required double ObjectRadius;
    public required double DragCoefficient;
    public required double Mass;
    public required double AirDensity;
    public required Scene Scene;
    public required bool GenerateTable;
}
