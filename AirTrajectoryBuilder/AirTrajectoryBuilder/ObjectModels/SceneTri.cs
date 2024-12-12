using Nerd_STF.Mathematics;
using System;

namespace AirTrajectoryBuilder.ObjectModels;

public class SceneTri : ISceneObject
{
    public Float2 A { get; set; }
    public Float2 B { get; set; }
    public Float2 C { get; set; }

    private static double Area(Float2 a, Float2 b, Float2 c)
    {
        return Math.Abs((a.x * (b.y - c.y) +
                         b.x * (c.y - a.y) +
                         c.x * (a.y - b.y)) * 0.5);
    }
    public bool Contains(Float2 p)
    {
        double area = Area(A, B, C),
               a1 = Area(p, B, C),
               a2 = Area(A, p, C),
               a3 = Area(A, B, p);
        return area == a1 + a2 + a3;
    }

    public ISceneObject DeepCopy() => new SceneTri()
    {
        A = A,
        B = B,
        C = C
    };
}
