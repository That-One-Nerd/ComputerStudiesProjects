using Nerd_STF.Mathematics;
using System.Collections.Generic;
using System.IO;

namespace AirTrajectoryBuilder.ObjectModels
{
    public class Scene
    {
        public static Scene Default => new()
        {
            Width = 100,
            Height = 60,
            Objects = [],
            HasBeenSaved = true,
            FilePath = null,
            EndAt = (100, 0)
        };

        public double Width { get; set; }
        public double Height { get; set; }

        public Float2 StartAt { get; set; }
        public Float2 EndAt { get; set; }

        public bool HasBeenSaved { get; set; }
        public string? FilePath { get; set; }

        public List<ISceneObject> Objects { get; private set; } = [];

        public Scene DeepCopy()
        {
            Scene copy = new()
            {
                Width = Width,
                Height = Height,
                StartAt = StartAt,
                EndAt = EndAt,
                HasBeenSaved = HasBeenSaved,
                FilePath = FilePath,
                Objects = []
            };
            foreach (ISceneObject obj in Objects) copy.Objects.Add(obj.DeepCopy());
            return copy;
        }

        public static Scene Read(string path)
        {
            if (!File.Exists(path)) throw new IOException();
            StreamReader reader = new(path);

            string? line;
            Scene? scene = null;
            while ((line = reader.ReadLine()) is not null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] parts = line.Split(' ');

                switch (parts[0])
                {
                    case "Scene":
                        if (parts.Length != 4) throw new IOException();
                        else if (parts[2] != "by") throw new IOException();
                        scene = new()
                        {
                            Width = double.Parse(parts[1]),
                            Height = double.Parse(parts[3]),
                            Objects = [],
                            HasBeenSaved = true,
                            FilePath = path,
                        };
                        break;
                    case "Rect":
                        if (scene is null) throw new IOException();
                        else if (parts.Length != 6) throw new IOException();
                        else if (parts[3] != "to") throw new IOException();
                        SceneRect rect = new()
                        {
                            From = (double.Parse(parts[1]), double.Parse(parts[2])),
                            To = (double.Parse(parts[4]), double.Parse(parts[5]))
                        };
                        scene.Objects.Add(rect);
                        break;
                    case "Tri":
                        if (scene is null) throw new IOException();
                        else if (parts.Length != 9) throw new IOException();
                        else if (parts[3] != "and" || parts[6] != "and") throw new IOException();
                        SceneTri tri = new()
                        {
                            A = (double.Parse(parts[1]), double.Parse(parts[2])),
                            B = (double.Parse(parts[4]), double.Parse(parts[5])),
                            C = (double.Parse(parts[7]), double.Parse(parts[8]))
                        };
                        scene.Objects.Add(tri);
                        break;
                    case "Ellipse":
                        if (scene is null) throw new IOException();
                        else if (parts.Length != 7) throw new IOException();
                        else if (parts[3] != "and" || parts[5] != "by") throw new IOException();
                        SceneEllipse ellipse = new()
                        {
                            Position = (double.Parse(parts[1]), double.Parse(parts[2])),
                            Size = (double.Parse(parts[4]), double.Parse(parts[6]))
                        };
                        scene.Objects.Add(ellipse);
                        break;
                    case "Start":
                        if (scene is null) throw new IOException();
                        else if (parts.Length != 3) throw new IOException();
                        scene.StartAt = (double.Parse(parts[1]), double.Parse(parts[2]));
                        break;
                    case "End":
                        if (scene is null) throw new IOException();
                        else if (parts.Length != 3) throw new IOException();
                        scene.EndAt = (double.Parse(parts[1]), double.Parse(parts[2]));
                        break;
                    default: throw new IOException();
                }
            }
            reader.Close();
            return scene ?? Default;
        }
        public void Write()
        {
            StreamWriter writer = new(FilePath ?? throw new IOException());

            writer.WriteLine($"Scene {Width} by {Height}\n");
            foreach (ISceneObject obj in Objects)
            {
                if (obj is SceneRect objRect)
                {
                    writer.WriteLine($"Rect {objRect.From.x} {objRect.From.y} to {objRect.To.x} {objRect.To.y}");
                }
                else if (obj is SceneTri objTri)
                {
                    writer.WriteLine($"Tri {objTri.A.x} {objTri.A.y} and {objTri.B.x} {objTri.B.y} and {objTri.C.x} {objTri.C.y}");
                }
                else if (obj is SceneEllipse objEllipse)
                {
                    writer.WriteLine($"Ellipse {objEllipse.Position.x} {objEllipse.Position.y} and {objEllipse.Size.x} by {objEllipse.Size.y}");
                }
            }

            writer.Close();
        }
    }
}
