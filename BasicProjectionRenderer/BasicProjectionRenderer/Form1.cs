using Nerd_STF.Mathematics;
using Nerd_STF.Mathematics.Algebra;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BasicProjectionRenderer;

public partial class Form1 : Form
{
    public Mesh? Mesh { get; set; }

    public Float2 ZoomLevel = (1, 1);
    public Float3 ScreenCenter = (0, 0, -2.5);

    public Form1()
    {
        InitializeComponent();

        SetStyle(ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

        RefreshTimer.Tick += OnTick;
    }

    public Int2 ToScreenSpace(Float3 worldPoint)
    {
        float dpi = DeviceDpi;
        worldPoint.y = -worldPoint.y;

        worldPoint.x -= ScreenCenter.x;
        worldPoint.y -= ScreenCenter.y;

        worldPoint.x *= dpi / ZoomLevel.x;
        worldPoint.y *= dpi / ZoomLevel.y;

        worldPoint.x += ClientRectangle.Width * 0.5;
        worldPoint.y += ClientRectangle.Height * 0.5;

        return new((int)worldPoint.x, (int)worldPoint.y);
    }
    public Float3 FromScreenSpace(Int2 screenPoint)
    {
        Float3 result = new(screenPoint.x, screenPoint.y, ScreenCenter.z);

        result.x -= ClientRectangle.Width / 2.0;
        result.y -= ClientRectangle.Height / 2.0;

        float dpi = DeviceDpi;
        result.x /= dpi / ZoomLevel.x;
        result.y /= dpi / ZoomLevel.y;

        result.x += ScreenCenter.x;
        result.y += ScreenCenter.y;

        result.y = -result.y;

        return result;
    }

    private int paintIters;
    private double paintTime;
    protected override void OnPaint(PaintEventArgs e)
    {
        DateTime start = DateTime.Now;
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.HighQuality;
        if (Mesh is null) return;

        SolidBrush colorBrush = new(Color.Black);
        Pen colorPen = new(colorBrush);

        /*// Draw edges.
        for (int i = 0; i < Mesh.lines.Length; i++)
        {
            Line line = Mesh.lines[i];
            //Color newColor = Color.FromArgb((int)(0xFF000000 | (0x00FFFFFF & line.GetHashCode())));
            //colorPen.Color = newColor;

            Float3 pointA = Mesh.GetPoint(line.IndA),
                   pointB = Mesh.GetPoint(line.IndB);
            Int2 posA = ToScreenSpace(pointA), posB = ToScreenSpace(pointB);
            g.DrawLine(colorPen, posA, posB);
        }*/

        // Draw faces.
        for (int i = 0; i < Mesh.faces.Length; i++)
        {
            Face face = Mesh.faces[i];
            Color newColor = Color.FromArgb((int)(0xFF000000 | (0x00FFFFFF & face.GetHashCode())));
            colorBrush.Color = newColor;

            Float3 pointA = Mesh.GetPoint(face.IndA),
                   pointB = Mesh.GetPoint(face.IndB),
                   pointC = Mesh.GetPoint(face.IndC);
            Int2 posA = ToScreenSpace(pointA), posB = ToScreenSpace(pointB), posC = ToScreenSpace(pointC);
            g.FillPolygon(colorBrush, [posA, posB, posC]);
        }
        DateTime end = DateTime.Now;
        paintIters++;
        paintTime += (end - start).TotalMilliseconds;
        if (paintIters == 20)
        {
            double per = paintTime / 20;
            Console.WriteLine($"{per:0.000} ms avg.");
            paintIters = 0;
            paintTime = 0;
        }
    }

    private double elapsedTime, temp;
    private void OnTick(object? sender, EventArgs e)
    {
        elapsedTime += RefreshTimer.Interval * 1e-3;
        //temp += MathE.Sin(elapsedTime * 8) * 0.9;
        if (Mesh is not null)
        {
            DateTime start = DateTime.Now;
            LookAtCursor();
            DateTime end = DateTime.Now;
            paintTime += (end - start).TotalMilliseconds;

            void LookAtCursor()
            {
                Float3 mousePos = FromScreenSpace(PointToClient(Cursor.Position));

                Float3 dist = Mesh!.Location - mousePos;
                Angle leftRight = new(Math.Atan2(dist.x, dist.z), Angle.Units.Radians),
                      upDown = new(Math.Atan2(dist.y, dist.z), Angle.Units.Radians),
                      spin = new(temp, Angle.Units.Degrees);
                Mesh.Rotation = (upDown, -leftRight, spin);
            }
        }
        Invalidate();
    }
}
