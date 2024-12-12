using Nerd_STF.Mathematics;

namespace AirTrajectoryBuilder.ObjectModels;

public class SceneRect : ISceneObject
{
    public Float2 From { get; set; }
    public Float2 To { get; set; }

    public bool Contains(Float2 p)
    {
        double minX = double.Min(From.x, To.x), maxX = double.Max(From.x, To.x),
               minY = double.Min(From.y, To.y), maxY = double.Max(From.y, To.y);

        return p.x >= minX && p.x <= maxX &&
               p.y >= minY && p.y <= maxY;
    }

    public ISceneObject DeepCopy() => new SceneRect()
    {
        From = From,
        To = To
    };
}
