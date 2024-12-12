using AirTrajectoryBuilder.ObjectModels;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System;
using Nerd_STF.Mathematics;
using System.IO;
using System.Reflection;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Threading;
using AirTrajectoryBuilder.Forms;

namespace AirTrajectoryBuilder;

public partial class MainForm : Form
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Scene Scene { get; set; }

    private readonly float scalingFactor;
    private readonly string baseFolder, sceneFolder;

    public SweepParameters? SweepParameters;

    private CancellationTokenSource? simCancel;
    private SimulationResult? simResult;
    private SweepStatus simStatus;
    private SweepCancelForm? simCancelForm;
    internal SweepInfoViewer? simViewer;

    private readonly Font statusFont;

    public MainForm(Scene? initialScene)
    {
        InitializeComponent();

        SetStyle(ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

        Graphics tempG = CreateGraphics();
        scalingFactor = tempG.DpiX / 96;
        tempG.Dispose();

        baseFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        sceneFolder = Path.Combine(baseFolder, "./scenes/");
        Directory.CreateDirectory(sceneFolder);
        FileOpener.InitialDirectory = sceneFolder;

        Scene = initialScene ?? Scene.Default;

        statusFont = new("Segoe UI", 10);
        MenuRunCancel.Enabled = false;
    }

    public Int2 PlotToScreen(Float2 plot)
    {
        int menuHeight = Menu.Height;
        int buffer = (int)(20 * scalingFactor);

        double bufferX, bufferY, maxPix, pixPerUnit;
        double clientAspect = (double)(ClientRectangle.Height - menuHeight - buffer * 2) / (ClientRectangle.Width - buffer * 2),
               sceneAspect = Scene.Height / Scene.Width;

        if (clientAspect > sceneAspect)
        {
            // Client is taller than scene, use width as max.
            maxPix = ClientRectangle.Width - 2 * buffer;
            pixPerUnit = maxPix / Scene.Width;

            bufferX = buffer;
            bufferY = (ClientRectangle.Height + menuHeight - Scene.Height * pixPerUnit) * 0.5f;
        }
        else
        {
            // Client is wider than scene, use height as max.
            maxPix = ClientRectangle.Height - menuHeight - 2 * buffer;
            pixPerUnit = maxPix / Scene.Height;

            bufferX = (ClientRectangle.Width - Scene.Width * pixPerUnit) * 0.5f;
            bufferY = buffer + menuHeight;
        }

        return ((int)(plot.x * pixPerUnit + bufferX),
                (int)((Scene.Height - plot.y) * pixPerUnit + bufferY));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.HighQuality;
        Pen pen = new(Color.Black, scalingFactor);
        SolidBrush fill = new(Color.Black);

        const int fillAlpha = 64;

        // Draw components.
        foreach (ISceneObject obj in Scene.Objects)
        {
            if (obj is SceneRect objRect)
            {
                pen.Color = Color.Green;
                fill.Color = Color.FromArgb(fillAlpha, pen.Color);
                Int2 rectFrom = PlotToScreen(objRect.From), rectTo = PlotToScreen(objRect.To);
                int minX = int.Min(rectFrom.x, rectTo.x), sizeX = MathE.Absolute(rectFrom.x - rectTo.x),
                    minY = int.Min(rectFrom.y, rectTo.y), sizeY = MathE.Absolute(rectFrom.y - rectTo.y);
                Rectangle rect = new(new Point(minX, minY), new Size(sizeX, sizeY));
                g.FillRectangle(fill, rect);
                g.DrawRectangle(pen, rect);
            }
            else if (obj is SceneTri objTri)
            {
                pen.Color = Color.Orange;
                fill.Color = Color.FromArgb(fillAlpha, pen.Color);
                Int2 triA = PlotToScreen(objTri.A),
                     triB = PlotToScreen(objTri.B),
                     triC = PlotToScreen(objTri.C);
                g.FillPolygon(fill, [triA, triB, triC]);
                g.DrawPolygon(pen, [triA, triB, triC]);
            }
            else if (obj is SceneEllipse objEllipse)
            {
                pen.Color = Color.Purple;
                fill.Color = Color.FromArgb(fillAlpha, pen.Color);
                Int2 min = PlotToScreen(objEllipse.Position - objEllipse.Size * 0.5),
                     max = PlotToScreen(objEllipse.Position + objEllipse.Size * 0.5);
                Rectangle ellipseRect = new(min, max - min);
                g.FillEllipse(fill, ellipseRect);
                g.DrawEllipse(pen, ellipseRect);
            }
        }

        // Draw scene border.
        pen.Color = Color.Blue;
        pen.Width = scalingFactor * 2;
        Int2 sceneMin = PlotToScreen((0, Scene.Height)), sceneMax = PlotToScreen((Scene.Width, 0));
        g.DrawRectangle(pen, new Rectangle(sceneMin, sceneMax - sceneMin));

        // Draw starting position.
        int startPosSize = (int)(6 * scalingFactor);
        pen.Color = Color.Red;
        pen.Width = scalingFactor;
        fill.Color = Color.FromArgb(fillAlpha, pen.Color);
        Int2 startPos = PlotToScreen(Scene.StartAt);
        startPos.x -= startPosSize;
        startPos.y -= startPosSize;
        Rectangle startRect = new(startPos, Int2.One * startPosSize * 2);
        g.FillEllipse(fill, startRect);
        g.DrawEllipse(pen, startRect);

        // Draw ending position.
        pen.Color = Color.Lime;
        pen.Width = scalingFactor;
        fill.Color = Color.FromArgb(fillAlpha, pen.Color);
        Int2 endPos = PlotToScreen(Scene.EndAt);
        endPos.x -= startPosSize;
        endPos.y -= startPosSize;
        Rectangle endRect = new(endPos, Int2.One * startPosSize * 2);
        g.FillEllipse(fill, endRect);
        g.DrawEllipse(pen, endRect);

        // If there's a trail, draw it.
        pen.Color = Color.Red;
        if (simResult is not null)
        {
            Point[] points = new Point[simResult.Trail.Count];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = PlotToScreen(simResult.Trail[i]);
            }
            g.DrawLines(pen, points);

            // Draw X at end point (if it's a crash and not a finish).
            if (simResult.EndDistanceSquared >= SweepParameters!.Tolerance * SweepParameters.Tolerance)
            {
                Int2 end = points[^1];
                PointF[] xShape = [
                    new Float2(-5, -5) * scalingFactor + end,
                    new Float2(5, 5) * scalingFactor + end,
                    new Float2(0, 0) * scalingFactor + end,
                    new Float2(-5, 5) * scalingFactor + end,
                    new Float2(5, -5) * scalingFactor + end,
                ];
                g.DrawLines(pen, xShape);
            }
        }

        string message = simStatus switch
        {
            SweepStatus.NoSweep => "No Sweep",
            SweepStatus.Sweeping => $"Sweeping... Best {simResult?.StartingConditions.StartAngle:0.000} deg, {simResult?.StartingConditions.StartVelocity:0.0} m/s",
            SweepStatus.FinishedSweep => $"Done Sweeping. Best {simResult?.StartingConditions.StartAngle:0.000} deg, {simResult?.StartingConditions.StartVelocity:0.0} m/s",
            SweepStatus.CancelledSweep => $"Cancelled Sweep. Best {simResult?.StartingConditions.StartAngle:0.000} deg, {simResult?.StartingConditions.StartVelocity:0.0} m/s",
            _ => "???",
        };
        fill.Color = Color.Blue;
        SizeF size = g.MeasureString(message, statusFont);
        const float spacing = 1;
        g.DrawString(message, statusFont, fill, new PointF(spacing * scalingFactor, ClientRectangle.Height - size.Height - spacing * scalingFactor));

        e.Dispose();
    }
    protected override void OnClientSizeChanged(EventArgs e)
    {
        base.OnClientSizeChanged(e);
        Invalidate(true);
    }

    private void ResetSceneData()
    {
        simCancel?.Cancel();

        simResult = null;
        simCancel = null;
        SweepParameters = null;
        simStatus = SweepStatus.NoSweep;
        simViewer?.Close();
        Invalidate(true);
    }

    private void MenuFileNew_Click(object? sender, EventArgs e)
    {
        if (!TryCancelSweep()) return;

        if (!Scene.HasBeenSaved)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to discard your changes?", "Lose changes?",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.No) return;
        }
        Scene = Scene.Default;
        ResetSceneData();
    }
    private void MenuFileOpen_Click(object? sender, EventArgs e)
    {
        if (!TryCancelSweep()) return;

        DialogResult result;
        if (!Scene.HasBeenSaved)
        {
            result = MessageBox.Show(
                "Are you sure you want to discard your changes?", "Lose changes?",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.No) return;
        }
        result = FileOpener.ShowDialog();
        if (result == DialogResult.Cancel) return;

        try
        {
            Scene = Scene.Read(FileOpener.FileName);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error opening scene file: {ex.GetType().Name}");
        }
        ResetSceneData();
    }

    private void MenuRunSweep_Click(object? sender, EventArgs e)
    {
        if (!TryCancelSweep()) return;

        SweepParametersForm param = new(this);
        DialogResult result = param.ShowDialog();
        if (result == DialogResult.Cancel) return;

        SweepParameters = param.Result;
        simCancel = new();
        simStatus = SweepStatus.Sweeping;
        MenuRunCancel.Enabled = true;
        simViewer ??= new SweepInfoViewer(this);
        simViewer.UncompleteSweep();
        simViewer.Show();
        Task.Run(() => SweepSimulation(SweepParameters, simCancel.Token));
    }

    public bool TryCancelSweep()
    {
        if (simStatus == SweepStatus.Sweeping && simCancel is not null)
        {
            SweepCancelForm form = new();
            simCancelForm = form;
            DialogResult result = form.ShowDialog();
            if (result == DialogResult.No) return false;

            simCancel?.Cancel();
            return true;
        }
        return true;
    }

    private void SweepSimulation(SweepParameters param, CancellationToken token)
    {
        Float2 diff = param.Scene.EndAt - param.Scene.StartAt;
        double minAngle = Math.Atan2(diff.y, diff.x) * Constants.Pi / 180,
               maxAngle = Constants.Pi / 2,
               angleStep = param.AngleDelta * Constants.Pi / 180;

        double closest = double.MaxValue, tolSquared = param.Tolerance * param.Tolerance;
        bool end = false;
        int angleSteps = (int)((maxAngle - minAngle) / angleStep);
        int steps = 0;
        simViewer?.SetMaxIters(angleSteps);
        for (double ang = minAngle; ang <= maxAngle; ang += angleStep)
        {
            for (double vel = param.SpeedMin; vel <= param.SpeedMax; vel += param.SpeedDelta)
            {
                if (token.IsCancellationRequested)
                {
                    end = true;
                    break;
                }
                SimulationParameters simParams = new()
                {
                    DeltaTime = param.TimeDelta,
                    Gravity = param.Gravity,
                    StartAngle = ang,
                    StartVelocity = vel,
                    Scene = Scene,
                    ToleranceSquared = tolSquared,
                    ObjectRadius = param.ObjectRadius,
                    DragCoefficient = param.DragCoefficient,
                    Mass = param.Mass,
                    AirDensity = param.AirDensity,
                    GenerateTable = param.FileMode != ResultsFileMode.None
                };
                SimulationResult result = SimulateTrajectory(simParams, token);
                if (result.EndDistanceSquared < closest)
                {
                    Invoke(() =>
                    {
                        simResult = result;
                    });
                    Invalidate(true);
                    closest = result.EndDistanceSquared;
                }
            }
            if (end) break;
            simViewer?.SetCurrentIters(steps);
            steps++;
        }
        simCancel = null;
        simStatus = token.IsCancellationRequested ? SweepStatus.CancelledSweep : SweepStatus.FinishedSweep;
        if (simCancelForm is not null)
        {
            simCancelForm.DialogResult = DialogResult.No;
            simCancelForm.Close();
        }
        MenuRunCancel.Enabled = false;
        simViewer?.Invoke(() => simViewer?.CompleteSweep(simResult!));
        Invalidate(true);
    }
    private static SimulationResult SimulateTrajectory(SimulationParameters param, CancellationToken token)
    {
        Float2 pos = param.Scene.StartAt;
        Float2 vel = (Math.Cos(param.StartAngle) * param.StartVelocity,
                      Math.Sin(param.StartAngle) * param.StartVelocity);
        Float2 gravity = (0, param.Gravity);
        double halfArea = 0.5 * param.ObjectRadius * param.ObjectRadius * Constants.Pi;

        SimulationResult result = new(param);
        double trailPointsPerSecond = 5;
        int ticksPerTrailPoint = (int)(1 / (trailPointsPerSecond * param.DeltaTime));

        if (param.GenerateTable) result.Table = [];

        int ticks = 0;
        double time = 0;
        while (true)
        {
            Float2 air = (halfArea * param.DragCoefficient * param.AirDensity * vel.x * vel.x,
                          halfArea * param.DragCoefficient * param.AirDensity * vel.y * vel.y);

            Float2 acc = gravity;
            if (param.Mass > 0)
            {
                air /= param.Mass;
                acc.x -= air.x * Math.Sign(vel.x);
                acc.y -= air.y * Math.Sign(vel.y);
            }

            if (param.GenerateTable) result.Table!.Add(new(pos, vel, acc));

            if (pos.x < 0 || pos.x >= param.Scene.Width ||
                pos.y < 0 || pos.y >= param.Scene.Height)
            {
                result.Trail.Add(pos);
                break;
            }

            if (token.IsCancellationRequested) break;

            bool collide = false;
            for (int i = 0; i < param.Scene.Objects.Count; i++)
            {
                if (param.Scene.Objects[i].Contains(pos))
                {
                    collide = true;
                    break;
                }
            }
            if (collide)
            {
                result.Trail.Add(pos);
                break;
            }

            Float2 diff = param.Scene.EndAt - pos;
            result.EndDistanceSquared = diff.x * diff.x + diff.y * diff.y;
            if (result.EndDistanceSquared <= param.ToleranceSquared)
            {
                result.Trail.Add(pos);
                break;
            }

            if (ticks % ticksPerTrailPoint == 0) result.Trail.Add(pos);

            pos += vel * param.DeltaTime;
            vel += acc * param.DeltaTime;

            time += param.DeltaTime;
        }
        result.StartingConditions.StartAngle *= 180 / Constants.Pi;
        result.EndSpeed = vel.Magnitude;
        result.Duration = time;

        return result;
    }

    private void MenuRunCancel_Click(object sender, EventArgs e)
    {
        simCancel?.Cancel();
    }

    public enum SweepStatus
    {
        NoSweep,
        Sweeping,
        FinishedSweep,
        CancelledSweep
    }
}
