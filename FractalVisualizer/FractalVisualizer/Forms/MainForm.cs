using Nerd_STF.Mathematics;

namespace FractalVisualizer.Forms
{
    public partial class MainForm : Form
    {
        public Float2 ScreenCenter { get; set; }
        public Float2 Dpi { get; private set; }
        public Float2 ZoomLevel { get; set; }

        public Action<Complex> FuncSetup { get; set; }
        public Func<Complex, Complex> Func { get; set; }
        public int MaxIterations { get; set; }
        public double CutoffMagnitude { get; set; }
        public bool OptimizeIterations { get; set; }

        public MainForm(Action<Complex> setup, Func<Complex, Complex> func)
        {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            Graphics tempG = CreateGraphics();
            Dpi = (tempG.DpiX, tempG.DpiY);
            tempG.Dispose();

            ZoomLevel = Float2.One;
            FuncSetup = setup;
            Func = func;

            Viewer.MouseDown += (o, e) => OnMouseDown(e);
            Viewer.MouseMove += (o, e) => OnMouseMove(e);
            Viewer.MouseUp += (o, e) => OnMouseUp(e);

            UpdatePositionText();
            BeginRender();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            BeginRender();
        }

        public void BeginRender()
        {
            if (renderTask is not null)
            {
                renderCancel!.Cancel();
                renderTask.Wait();

                renderCancel.Dispose();
                renderTask.Dispose();
            }
            renderCancel = new CancellationTokenSource();
            renderTask = Task.Run(async () => await Render(8, true, renderCancel.Token));
        }

        private static Color ColorFromIters(int i, int max)
        {
            if (i == 0 || i == max) return Color.Black;

            int brightestR = 33,
                brightestG = 204,
                brightestB = 38;

            double intensity = 1 - 1.0 / i;
            int r = (int)(brightestR * intensity),
                g = (int)(brightestG * intensity),
                b = (int)(brightestB * intensity);
            return Color.FromArgb(g, r, b);
        }

        private CancellationTokenSource? renderCancel;
        private Task? renderTask;
        private Task Render(int depth, bool recurse, CancellationToken token)
        {
            DateTime start = DateTime.Now;
            Graphics g = Viewer.CreateGraphics();
            SolidBrush pen = new(Color.Black);
            double cutoff = CutoffMagnitude * CutoffMagnitude;
            int maxIters = MaxIterations;
            if (OptimizeIterations) maxIters /= depth + 1;

            int step = 1 << depth;
            for (int x = 0; x < Viewer.Width; x += step)
            {
                for (int y = 0; y < Viewer.Height; y += step)
                {
                    if (token.IsCancellationRequested) return Task.CompletedTask;

                    Int2 point = (x, y);
                    Float2 coords = ScreenSpaceToGraphSpace(point);
                    Complex num = new(coords.x, coords.y);
                    FuncSetup(num);
                    int i;
                    for (i = 0; i < maxIters; i++)
                    {
                        if (token.IsCancellationRequested) return Task.CompletedTask;
                        num = Func(num);
                        if (num.MagSq >= CutoffMagnitude) break;
                    }
                    
                    if (token.IsCancellationRequested) return Task.CompletedTask;
                    pen.Color = ColorFromIters(i, maxIters);
                    g.FillRectangle(pen, new Rectangle(x, y, step, step));
                }
            }

            g.Dispose();
            pen.Dispose();
            if (recurse && depth > 0) return Render(depth - 1, true, token);
            else return Task.CompletedTask;
        }

        public Int2 GraphSpaceToScreenSpace(Float2 graphPoint)
        {
            graphPoint.y = -graphPoint.y;

            graphPoint.x -= ScreenCenter.x;
            graphPoint.y -= ScreenCenter.y;

            graphPoint.x *= Dpi.x / ZoomLevel.x;
            graphPoint.y *= Dpi.y / ZoomLevel.y;

            graphPoint.x += Viewer.Width / 2.0;
            graphPoint.y += Viewer.Height / 2.0;

            return new((int)graphPoint.x, (int)graphPoint.y);
        }
        public Float2 ScreenSpaceToGraphSpace(Int2 screenPoint)
        {
            Float2 result = new(screenPoint.x, screenPoint.y);

            result.x -= Viewer.Width / 2.0;
            result.y -= Viewer.Height / 2.0;

            result.x /= Dpi.x / ZoomLevel.x;
            result.y /= Dpi.y / ZoomLevel.y;

            result.x += ScreenCenter.x;
            result.y += ScreenCenter.y;

            result.y = -result.y;

            return result;
        }

        private void UpdatePositionText()
        {
            PositionLabel.Text = $"{ScreenCenter.ToString("0.000")} - {100.0 * ZoomLevel.x:0.000000}%";
            PositionLabel.Invalidate();
        }

        private bool dragging;
        private Int2 initialDragPoint;
        private Float2 initialCenter;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!dragging)
            {
                initialCenter = ScreenCenter;
                initialDragPoint = new Int2(Cursor.Position.X, Cursor.Position.Y);
                dragging = true;
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (dragging)
            {
                Int2 pixelDiff = new(initialDragPoint.x - Cursor.Position.X,
                                     initialDragPoint.y - Cursor.Position.Y);
                Float2 graphDiff = new(pixelDiff.x * ZoomLevel.x / Dpi.x, pixelDiff.y * ZoomLevel.y / Dpi.y);
                ScreenCenter = new(initialCenter.x + graphDiff.x,
                                   initialCenter.y + graphDiff.y);
                BeginRender();
                dragging = false;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (dragging)
            {
                Int2 pixelDiff = new(initialDragPoint.x - Cursor.Position.X,
                                     initialDragPoint.y - Cursor.Position.Y);
                Float2 graphDiff = new(pixelDiff.x * ZoomLevel.x / Dpi.x, pixelDiff.y * ZoomLevel.y / Dpi.y);
                ScreenCenter = new(initialCenter.x + graphDiff.x,
                                   initialCenter.y + graphDiff.y);
                UpdatePositionText();
                BeginRender();
            }
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            Float2 newZoom = ZoomLevel;
            newZoom.x *= 1 - e.Delta * 0.00075; // Zoom factor.
            newZoom.y *= 1 - e.Delta * 0.00075;
            ZoomLevel = newZoom;
            UpdatePositionText();
            BeginRender();
        }
    }
}
