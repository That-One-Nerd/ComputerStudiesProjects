using Nerd_STF.Mathematics;

namespace AirTrajectoryBuilder.ObjectModels;

public class SceneEllipse : ISceneObject
{
    public Float2 Position { get; set; }
    public Float2 Size { get; set; }

    public bool Contains(Float2 point)
    {
        Float2 p = point - Position;
        Float2 delta = p * 2 / Size;

        delta.x *= delta.x;
        delta.y *= delta.y;
        return delta.x + delta.y <= 1;
    }
    public ISceneObject DeepCopy() => new SceneEllipse()
    {
        Position = Position,
        Size = Size
    };
}
