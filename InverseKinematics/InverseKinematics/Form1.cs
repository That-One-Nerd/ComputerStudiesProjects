using Nerd_STF.Mathematics;
using System.Drawing.Drawing2D;

namespace InverseKinematics;

public partial class Form1 : Form
{
    public Float2 DesiredPoint { get; set; }

    public Float2[] Points { get; set; }

    public Float2 ZoomLevel { get; set; } = (3, 3);
    public Float2 ScreenCenter { get; set; } = (0, 0);

    public Form1()
    {
        InitializeComponent();
        Points = new Float2[5];
        for (int i = 0; i < Points.Length; i++)
        {
            Points[i] = (0, i);
        }

        SetStyle(ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
    }

    public Int2 ToScreenSpace(Float2 worldPoint)
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
    public Float2 FromScreenSpace(Int2 screenPoint)
    {
        Float2 result = (screenPoint.x, screenPoint.y);

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

    protected override void OnMouseMove(MouseEventArgs e)
    {
        DesiredPoint = FromScreenSpace(e.Location);
        Invalidate();
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        RunFabrik();

        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.HighQuality;
        float scaleFactor = g.DpiX / 96.0f;

        SolidBrush brush = new(Color.Black);
        Pen pen = new(Color.Black, scaleFactor);

        Float2 radius = (2, 2);
        Int2 prevPointPix = (0, 0);
        for (int i = 0; i < Points.Length; i++)
        {
            Int2 pointPix = ToScreenSpace(Points[i]);
            g.FillEllipse(brush, new RectangleF(pointPix - radius * scaleFactor, radius * scaleFactor * 2));
        
            if (i > 0)
            {
                g.DrawLine(pen, prevPointPix, pointPix);
            }
            prevPointPix = pointPix;
        }

        Int2 desiredPix = ToScreenSpace(DesiredPoint);
        radius = (5, 5);

        brush.Color = Color.Red;
        g.FillEllipse(brush, new RectangleF(desiredPix - radius * scaleFactor, radius * scaleFactor * 2));
    }

    private void RunFabrik()
    {
        // Initial setup.
        for (int i = 0; i < Points.Length; i++)
        {
            // Reset points to 90 degrees away from goal.
            // This produces a "nice" rotation no matter where
            // you are.
            Float2 dir = (-DesiredPoint.y, DesiredPoint.x);
            dir.Normalize();

            Points[i] = dir * i;
        }

        RunFabrik(true, 16);
    }

    // True = forwards, false = backwards.
    private void RunFabrik(bool direction, int maxIters)
    {
        if (maxIters <= 0) return;

        const double maxLength = 1;
        if (direction)
        {
            Points[^1] = DesiredPoint;
            for (int i = Points.Length - 2; i >= 0; i--)
            {
                Float2 thisPoint = Points[i],
                       connected = Points[i + 1],
                       diff = connected - thisPoint;

                double step = maxLength - diff.Magnitude;
                Float2 dir = diff.Normalized;
                Points[i] = thisPoint - dir * step;
            }
        }
        else
        {
            Points[0] = (0, 0);
            for (int i = 1; i < Points.Length; i++)
            {
                Float2 thisPoint = Points[i],
                       connected = Points[i - 1],
                       diff = connected - thisPoint;

                double step = maxLength - diff.Magnitude;
                Float2 dir = diff.Normalized;
                Points[i] = thisPoint - dir * step;
            }
        }

        RunFabrik(!direction, maxIters - 1);
    }
}
