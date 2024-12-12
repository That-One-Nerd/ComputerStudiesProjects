using AirTrajectoryBuilder.ObjectModels;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System;

namespace AirTrajectoryBuilder;

public partial class SweepParametersForm : Form
{
    private static readonly FileMode[] fileModeValues = Enum.GetValues<FileMode>();

    public double AngleDelta = 0.1, 
                  SpeedDelta = 0.5,
                  SpeedMin = 10,
                  SpeedMax = 100,
                  Gravity = -9.81,
                  TimeDelta = 0.01;

    public double ObjectRadius = 0,
                  DragCoefficient = 0,
                  Mass = 0,
                  AirDensity = 1.225;

    public ResultsFileMode FileMode = ResultsFileMode.None;

    private static SweepParameters? previous = null;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SweepParameters Result
    {
        get
        {
            SweepParameters result = new()
            {
                AngleDelta = AngleDelta,
                SpeedDelta = SpeedDelta,
                SpeedMin = SpeedMin,
                SpeedMax = SpeedMax,
                Gravity = Gravity,
                TimeDelta = TimeDelta,
                ObjectRadius = ObjectRadius,
                DragCoefficient = DragCoefficient,
                Mass = Mass,
                AirDensity = AirDensity,
                Tolerance = 1e-1, // TODO
                Scene = form.Scene,
                FileMode = FileMode
            };
            previous = result;
            return result;
        }
        set
        {
            AngleDelta = value.AngleDelta;
            SpeedDelta = value.SpeedDelta;
            SpeedMin = value.SpeedMin;
            SpeedMax = value.SpeedMax;
            Gravity = value.Gravity;
            TimeDelta = value.TimeDelta;
            ObjectRadius = value.ObjectRadius;
            DragCoefficient = value.DragCoefficient;
            Mass = value.Mass;
            AirDensity = value.AirDensity;
            FileMode = value.FileMode;

            previous = value;
            SetValues();
        }
    }
    private readonly MainForm form;

    public SweepParametersForm(MainForm form)
    {
        InitializeComponent();
        this.form = form;

        AngleSweepValue.Leave += (o, e) =>
        {
            if (!double.TryParse(AngleSweepValue.Text, out AngleDelta))
                AngleSweepValue.Text = AngleDelta.ToString();
        };
        SpeedDeltaValue.Leave += (o, e) =>
        {
            if (!double.TryParse(SpeedDeltaValue.Text, out SpeedDelta))
                SpeedDeltaValue.Text = SpeedDelta.ToString();
        };
        SpeedMinValue.Leave += (o, e) =>
        {
            if (!double.TryParse(SpeedMinValue.Text, out SpeedMin))
                SpeedMinValue.Text = SpeedMin.ToString();
        };
        SpeedMaxValue.Leave += (o, e) =>
        {
            if (!double.TryParse(SpeedMaxValue.Text, out SpeedMax))
                SpeedMaxValue.Text = SpeedMax.ToString();
        };
        GravityValue.Leave += (o, e) =>
        {
            if (!double.TryParse(GravityValue.Text, out Gravity))
                GravityValue.Text = Gravity.ToString();
        };
        TimeDeltaValue.Leave += (o, e) =>
        {
            if (!double.TryParse(TimeDeltaValue.Text, out TimeDelta))
                TimeDeltaValue.Text = TimeDelta.ToString();
        };
        ObjectRadiusValue.Leave += (o, e) =>
        {
            if (!double.TryParse(ObjectRadiusValue.Text, out ObjectRadius))
                ObjectRadiusValue.Text = ObjectRadius.ToString();
        };
        MassValue.Leave += (o, e) =>
        {
            if (!double.TryParse(MassValue.Text, out Mass))
                MassValue.Text = Mass.ToString();
        };
        DragCoefficientValue.Leave += (o, e) =>
        {
            if (!double.TryParse(DragCoefficientValue.Text, out DragCoefficient))
                DragCoefficientValue.Text = DragCoefficient.ToString();
        };
        AirDensityValue.Leave += (o, e) =>
        {
            if (!double.TryParse(AirDensityValue.Text, out AirDensity))
                AirDensityValue.Text = AirDensity.ToString();
        };
        FileOutputValue.Leave += (o, e) =>
        {
            int index = FileOutputValue.SelectedIndex;
            if (index < 0 || index >= FileOutputValue.Items.Count)
            {
                FileOutputValue.SelectedIndex = (int)FileMode;
            }
            else FileMode = (ResultsFileMode)FileOutputValue.SelectedIndex;
        };

        if (previous is null) SetValues();
        else Result = previous;
    }

    private void SetValues()
    {
        AngleSweepValue.Text = AngleDelta.ToString();
        SpeedDeltaValue.Text = SpeedDelta.ToString();
        SpeedMinValue.Text = SpeedMin.ToString();
        SpeedMaxValue.Text = SpeedMax.ToString();
        GravityValue.Text = Gravity.ToString();
        TimeDeltaValue.Text = TimeDelta.ToString();
        ObjectRadiusValue.Text = ObjectRadius.ToString();
        MassValue.Text = Mass.ToString();
        DragCoefficientValue.Text = DragCoefficient.ToString();
        AirDensityValue.Text = AirDensity.ToString();
        FileOutputValue.SelectedIndex = (int)FileMode;
    }
}
