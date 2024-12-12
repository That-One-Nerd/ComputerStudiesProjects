using AirTrajectoryBuilder.ObjectModels;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AirTrajectoryBuilder.Forms;

public partial class SweepInfoViewer : Form
{
    private readonly MainForm mainForm;
    private SimulationResult? results;

    public SweepInfoViewer(MainForm mainForm)
    {
        InitializeComponent();
        ResultsPanel.Visible = false;
        this.mainForm = mainForm;

        Location =
            new Point(mainForm.Location.X + mainForm.Size.Width,
                      mainForm.Location.Y + (mainForm.Size.Height - Size.Height) / 2);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!mainForm.TryCancelSweep())
        {
            e.Cancel = true;
            return;
        }
        base.OnClosing(e);
        mainForm.simViewer = null;
    }

    public void SetMaxIters(int max)
    {
        SweepProgress.Maximum = max;
    }
    public void SetCurrentIters(int val)
    {
        SweepProgress.Value = val;
    }

    public void CompleteSweep(SimulationResult best)
    {
        results = best;

        SweepProgress.Visible = false;
        ProgressLabel.Visible = false;

        DisplayResults();
    }
    public void UncompleteSweep()
    {
        SweepProgress.Visible = true;
        ProgressLabel.Visible = true;
        ResultsPanel.Visible = false;
        Invalidate(true);
    }

    private void SetLabelSize()
    {
        ResultsLabel.Location = new Point(0, 0);
        ResultsLabel.Size = new(ResultsPanel.ClientRectangle.Width,
                                ResultsLabel.PreferredHeight);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        SetLabelSize();
    }

    private void DisplayResults()
    {
        ResultsPanel.Visible = true;
        if (results is null) return;

        ResultsLabel.Text = $"""

            Initial Angle: {results.StartingConditions.StartAngle:0.00} degrees
            Initial Velocity: {results.StartingConditions.StartVelocity:0.0} m/s
            Gravity: {results.StartingConditions.Gravity:0.000} m/s^2
            Delta Time: {results.StartingConditions.DeltaTime:0.000} seconds

            Final Velocity: {results.EndSpeed:0.0} m/s

            Duration: {results.Duration:0.00} seconds

            Error: off by {results.EndDistanceSquared:0.000} meters

            """;
        SetLabelSize();
        SetLabelSize(); // ...nice. But required!
    }
}
