using Nerd_STF.Mathematics;

namespace AirTrajectoryBuilder.ObjectModels;

public interface ISceneObject
{
    public bool Contains(Float2 point);
    public ISceneObject DeepCopy();
}
