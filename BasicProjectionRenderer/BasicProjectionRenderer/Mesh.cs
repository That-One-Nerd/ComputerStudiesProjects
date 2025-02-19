using Nerd_STF.Mathematics;
using Nerd_STF.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.IO;

namespace BasicProjectionRenderer;

public record class Line(int IndA, int IndB) : IEquatable<Line>
{
    public virtual bool Equals(Line? other)
    {
        if (other is null) return false;
        return (IndA == other.IndA && IndB == other.IndB) ||
               (IndA == other.IndB && IndB == other.IndA);
    }
    public override int GetHashCode() => base.GetHashCode();
}
public record class Face(int IndA, int IndB, int IndC);

public class Mesh
{
    public required Float3[] points;
    public required Line[] lines;
    public required Face[] faces;

    public Float3 Location { get; set; } = (0, 0, 0);
    public (Angle, Angle, Angle) Rotation
    {
        get => _rotation;
        set
        {
            if (_rotation != value)
            {
                _rotation = value;
                CalculateRotationMatrix();
            }
        }
    }
    public Float3 Scale { get; set; }    = (1, 1, 1);

    private readonly Dictionary<int, Float3> _pointCache = [];
    private (Angle, Angle, Angle) _rotation = (Angle.Zero, Angle.Zero, Angle.Zero);
    private Matrix3x3 _rotMatrix = Matrix3x3.Identity;

    private void CalculateRotationMatrix()
    {
        double radX = _rotation.Item1.Radians,
               radY = _rotation.Item2.Radians,
               radZ = _rotation.Item3.Radians;

        (double cosX, double sinX) = (MathE.Cos(radX), MathE.Sin(radX));
        (double cosY, double sinY) = (MathE.Cos(radY), MathE.Sin(radY));
        (double cosZ, double sinZ) = (MathE.Cos(radZ), MathE.Sin(radZ));
        Matrix3x3 rotX = new([
            [ 1,    0,     0 ],
            [ 0, cosX, -sinX ],
            [ 0, sinX,  cosX ]
        ]);
        Matrix3x3 rotY = new([
            [  cosY, 0, sinY ],
            [     0, 1,    0 ],
            [ -sinY, 0, cosY ]
        ]);
        Matrix3x3 rotZ = new([
            [ cosZ, -sinZ, 0 ],
            [ sinZ,  cosZ, 0 ],
            [    0,     0, 1 ]
        ]);
        _rotMatrix = rotX * rotY * rotZ;
        _pointCache.Clear();
        Array.Sort(faces, SortFace);
    }

    public Float3 GetPoint(int index)
    {
        if (_pointCache.TryGetValue(index, out Float3 cached)) return cached;

        Float3 p = points[index];
        p = _rotMatrix * p;
        p *= Scale;
        p += Location;
        _pointCache.Add(index, p);

        return p;
    }

    private int SortFace(Face f1, Face f2)
    {
        Float3 f1a = GetPoint(f1.IndA), f2a = GetPoint(f2.IndA),
               f1b = GetPoint(f1.IndB), f2b = GetPoint(f2.IndB),
               f1c = GetPoint(f1.IndC), f2c = GetPoint(f2.IndC);
        Float3 avg1 = (f1a + f1b + f1c) / 3,
               avg2 = (f2a + f2b + f2c) / 3;
        return avg1.z.CompareTo(avg2.z);
    }

    public static Mesh FromObj(Stream data)
    {
        StreamReader reader = new(data, leaveOpen: true);
        string? line;
        List<Float3> points = [];
        List<Line> lines = [];
        List<Face> faces = [];
        while ((line = reader.ReadLine()) is not null)
        {
            line = line.Trim();
            if (line.StartsWith('#')) continue;

            string[] parts = line.Split(' ');
            switch (parts[0])
            {
                case "v":
                    points.Add((double.Parse(parts[1]), double.Parse(parts[2]), double.Parse(parts[3])));
                    break;

                case "f":
                    int indA = int.Parse(parts[1]) - 1,
                        indB = int.Parse(parts[2]) - 1,
                        indC = int.Parse(parts[3]) - 1;

                    /*Line l1 = new(indA, indB), l2 = new(indB, indC), l3 = new(indC, indA);
                    if (!lines.Contains(l1)) lines.Add(l1);
                    if (!lines.Contains(l2)) lines.Add(l2);
                    if (!lines.Contains(l3)) lines.Add(l3);*/

                    faces.Add(new(indA, indB, indC));
                    break;
            }
        }
        reader.Close();
        Console.WriteLine($"{points.Count} verts, {lines.Count} edges, {faces.Count} faces");
        return new()
        {
            points = [.. points],
            lines = [.. lines],
            faces = [.. faces]
        };
    }

    public override int GetHashCode() => points.GetHashCode();
}
