
using System.Drawing.Drawing2D;

namespace BonusTicTacToe;

public partial class MainForm : Form
{
    public Game ActiveGame { get; set; } = Game.Load();
    public Preferences Preferences { get; } = Preferences.Load();

    public float ScalingFactor { get; private set; }

    private Font fontOutfit;

    public MainForm()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SetStyle(ControlStyles.UserPaint, true);

        InitializeComponent();

        // Get DPI for the scaling factor.
        Graphics tempG = CreateGraphics();
        ScalingFactor = tempG.DpiX / 192;
        tempG.Dispose();

        // Load main font from memory.
        fontOutfit = new(Program.fonts.Families.Single(x => x.Name == "Outfit SemiBold"), 12);

        SetWindowSizeFromGame();
    }

    public void SetWindowSizeFromGame()
    {
        Size = new Size(
            (int)(ScalingFactor * (Preferences.TileSize * ActiveGame.TilesX + Preferences.WindowBuffer * 2)),
            (int)(ScalingFactor * (Preferences.TileSize * ActiveGame.TilesY + Preferences.WindowBuffer * 2)));
    }
    public void SetupBoard()
    {
        ActiveGame.Board = new byte[ActiveGame.TilesX * ActiveGame.TilesY];
        ActiveGame.CurrentTurn = 0;
        ActiveGame.HasFinished = 0;
        ActiveGame.IsDraw = false;
        ActiveGame.TotalMoves = 0;

        Invalidate(true);
        ActiveGame.Save();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        // Full invalidation when the window size is changed,
        // since the grid is centered.
        Invalidate(true);
    }

    private Point lastHighlightedTile;
    protected override void OnMouseMove(MouseEventArgs e)
    {
        // Check if the tile you're highlighting has changed,
        // redraw the screen with the new one.
        Point highlightedTile = ScreenToTileInt(Cursor.Position);
        if (highlightedTile != lastHighlightedTile) Invalidate(true);
        lastHighlightedTile = highlightedTile;
    }
    protected override void OnMouseClick(MouseEventArgs e)
    {
        // Add a player move to the current selected tile and redraw.
        Point highlightedTile = ScreenToTileInt(Cursor.Position);
        PlayMove(highlightedTile);
    }

    private void PlayMove(Point tile)
    {
        if (ActiveGame.HasFinished != 0 || ActiveGame.IsDraw ||
            !TileWithinRange(tile) || ActiveGame[tile.X, tile.Y] != 0) return;

        ActiveGame[tile.X, tile.Y] = (byte)((ActiveGame.CurrentTurn / ActiveGame.MovesPerTurn) + 1);
        ActiveGame.CurrentTurn = (ActiveGame.CurrentTurn + 1) % (ActiveGame.Players * ActiveGame.MovesPerTurn);
        ActiveGame.TotalMoves++;

        // Check for wins.
        List<WinObject> wins = ActiveGame.GetWins();
        for (int i = 0; i < ActiveGame.Players; i++)
        {
            if (wins.Count(x => x.player == i) >= ActiveGame.WinsToFinish)
            {
                ActiveGame.HasFinished = (byte)(i + 1);
                break;
            }
        }
        ActiveGame.IsDraw = ActiveGame.HasFinished == 0 && ActiveGame.TotalMoves >= ActiveGame.Board.Length;

        Invalidate(true);
        ActiveGame.Save();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.HighQuality;

        float centerX = ClientRectangle.Width / 2, centerY = ClientRectangle.Height / 2;
        float tileSize = Preferences.TileSize * ScalingFactor;
        float lenX = tileSize * ActiveGame.TilesX,
              lenY = tileSize * ActiveGame.TilesY;

        float left = centerX - lenX * 0.5f, top = centerY - lenY * 0.5f,
              right = centerX + lenX * 0.5f, bottom = centerY + lenY * 0.5f;

        // Step 1: Draw highlighted segment.
        Point mouseTilePos = ScreenToTileInt(Cursor.Position);
        if (ActiveGame.HasFinished == 0 && !ActiveGame.IsDraw && TileWithinRange(mouseTilePos))
        {
            SolidBrush highlightColor = new(Color.FromArgb(0x18000000));
            Point tl = TileToScreen(mouseTilePos);
            g.FillRectangle(highlightColor, new RectangleF(tl, new SizeF(tileSize, tileSize)));
        }

        // Step 2: Draw grid lines.
        Pen gridPen = new(Color.FromArgb(unchecked((int)0xFF000000)), Preferences.LineThickness * ScalingFactor);
        g.DrawRectangle(gridPen, new RectangleF(left, top, lenX, lenY));

        for (int x = 0; x <= ActiveGame.TilesX; x++)
        {
            float t = (float)x / ActiveGame.TilesX;
            float lerpX = left + t * lenX;
            g.DrawLine(gridPen, new PointF(lerpX, top), new PointF(lerpX, bottom));
        }
        for (int y = 0; y <= ActiveGame.TilesY; y++)
        {
            float t = (float)y / ActiveGame.TilesY;
            float lerpY = top + t * lenY;
            g.DrawLine(gridPen, new PointF(left, lerpY), new PointF(right, lerpY));
        }

        // Step 3: Draw player inputs.
        for (int x = 0; x < ActiveGame.TilesX; x++)
        {
            for (int y = 0; y < ActiveGame.TilesY; y++)
            {
                RenderPlayerInput(g, x, y);
            }
        }

        // Step 4: Draw text stating whose turn it is.
        //         (Or who has won).
        string text;
        byte value;
        if (ActiveGame.HasFinished != 0)
        {
            int turns = 1 + (ActiveGame.TotalMoves - 1) / (ActiveGame.Players * ActiveGame.MovesPerTurn);
            text = $" has won in {turns} turn{(turns == 1 ? "" : "s")}!";
            value = ActiveGame.HasFinished;
        }
        else if (ActiveGame.IsDraw)
        {
            int turns = 1 + (ActiveGame.TotalMoves - 1) / (ActiveGame.Players * ActiveGame.MovesPerTurn);
            text = $"It's a draw in {turns} turn{(turns == 1 ? "" : "s")}!";
            value = 0;
        }
        else
        {
            text = " to Move";
            if (ActiveGame.MovesPerTurn > 1) text += $" ({ActiveGame.CurrentTurn % ActiveGame.MovesPerTurn + 1}/{ActiveGame.MovesPerTurn})";
            value = (byte)((ActiveGame.CurrentTurn / ActiveGame.MovesPerTurn) + 1);
        }
        SolidBrush textBrush = new(Color.FromArgb(unchecked((int)0xFF_101010)));
        SizeF length = g.MeasureString(text, fontOutfit);
        float textSpriteSize = 20 * ScalingFactor,
              textSpriteThickness = 6 * ScalingFactor;
        length.Width += textSpriteSize;

        float fullTextLeft = centerX - length.Width * 0.5f,
              trueTextLeft = fullTextLeft + textSpriteSize;
        float padding = 10 * ScalingFactor;
        float fullTextBottom = top - padding;
        StringFormat textFormat = new()
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Far,
        };
        g.DrawString(text, fontOutfit, textBrush, new PointF(trueTextLeft, fullTextBottom), textFormat);
        RenderMoveSprite(g, value, new PointF(fullTextLeft, fullTextBottom - textSpriteSize * 1.7f), textSpriteSize, textSpriteThickness);

        // Step 5: If there are wins, draw them.
        List<WinObject> wins = ActiveGame.GetWins();
        float lineThickness = 15 * ScalingFactor;
        foreach (WinObject w in wins)
        {
            Color color = Color.FromArgb((int)playerColors[w.player]);
            Pen temp = new(color, lineThickness);
            PointF min, max;
            switch (w.direction)
            {
                case Direction.LeftRight:
                    min = new(w.minX + 0.1f, w.minY + 0.5f);
                    max = new(w.maxX + 0.9f, w.minY + 0.5f);
                    break;

                case Direction.UpDown:
                    min = new(w.minX + 0.5f, w.minY + 0.1f);
                    max = new(w.minX + 0.5f, w.maxY + 0.9f);
                    break;

                case Direction.DiagonalTopRightBottomLeft:
                    min = new(w.maxX + 0.9f, w.minY + 0.1f);
                    max = new(w.minX + 0.1f, w.maxY + 0.9f);
                    break;

                case Direction.DiagonalTopLeftBottomRight:
                    min = new(w.minX + 0.1f, w.minY + 0.1f);
                    max = new(w.maxX + 0.9f, w.maxY + 0.9f);
                    break;

                default:
                    min = new();
                    max = new();
                    break;
            }

            g.DrawLine(temp, TileToScreen(min), TileToScreen(max));
        }

        // Step 6: Highlight replay button if game is finished.
        if (ActiveGame.HasFinished != 0 || ActiveGame.IsDraw)
        {
            ReplayButton.Font = new Font(ReplayButton.Font, FontStyle.Bold);
        }
        else ReplayButton.Font = new Font(ReplayButton.Font, FontStyle.Regular);

        base.OnPaint(e);
    }

    private readonly uint[] playerColors = [
        0xFF_454545,
        0xFF_E83535,
        0xFF_35A9E8,
        0xFF_2AAD39,
        0xFF_9E49E3,
        0xFF_EBA121,
        0xFF_2C5AE6,
        0xFF_FA78ED,
    ];
    private void RenderMoveSprite(Graphics g, byte value, PointF tileStart, float tileSize, float thickness)
    {
        Color color;
        if (value > 0 && value <= playerColors.Length) color = Color.FromArgb((int)playerColors[value - 1]);
        else color = Color.Black;

        Pen pen = new(color, thickness);
        switch (value)
        {
            case 0: return; // No move.
            case 1:
                // Draw 'X'
                float xLeft = tileStart.X, xRight = tileStart.X + tileSize,
                      xTop = tileStart.Y, xBottom = tileStart.Y + tileSize;
                g.DrawLine(pen, xLeft, xTop, xRight, xBottom);
                g.DrawLine(pen, xRight, xTop, xLeft, xBottom);
                break;
            case 2:
                // Draw 'O'
                g.DrawEllipse(pen, new RectangleF(tileStart, new SizeF(tileSize, tileSize)));
                break;
            case 3:
                // Draw triangle (equilateral requires a little more math)
                const float halfSqrt3 = 0.866025403784f;
                float tMiddleX = tileStart.X + tileSize * 0.5f, tLeft = tileStart.X, tRight = tileStart.X + tileSize,
                      tMiddleY = tileStart.Y + tileSize * 0.5f, tRadius = tileSize * halfSqrt3 * 0.5f,
                      tTop = tMiddleY - tRadius, tBottom = tMiddleY + tRadius;

                g.DrawPolygon(pen, [
                    new PointF(tLeft, tBottom),
                    new PointF(tMiddleX, tTop),
                    new PointF(tRight, tBottom)
                ]);
                break;
            case 4:
                // Draw square.
                float sLeft = tileStart.X, sRight = tileStart.X + tileSize,
                      sTop = tileStart.Y, sBottom = tileStart.Y + tileSize;
                g.DrawPolygon(pen, [
                    new PointF(sLeft, sTop),
                    new PointF(sRight, sTop),
                    new PointF(sRight, sBottom),
                    new PointF(sLeft, sBottom)
                ]);
                break;
            case 5:
                // Draw pentagon. Uses polygon code.
                PointF[] polyArray = GeneratePolygon(value);
                float pRadius = tileSize * 0.5f,
                      pCenterX = tileStart.X + pRadius,
                      pCenterY = tileStart.Y + pRadius;
                for (int i = 0; i < polyArray.Length; i++)
                {
                    PointF point = polyArray[i];
                    polyArray[i] = new PointF(point.X * pRadius * 1.1f + pCenterX,
                                              point.Y * pRadius * 1.1f + pCenterY);
                }
                g.DrawPolygon(pen, polyArray);
                break;
            case 6:
                // Draw star. Uses polygon code.
                PointF[] starArray = GeneratePolygon(10);
                float sOuterRadius = tileSize * 0.5f,
                      sInnerRadius = sOuterRadius * 0.5f,
                      sCenterX = tileStart.X + sOuterRadius,
                      sCenterY = tileStart.Y + sOuterRadius;
                for (int i = 0; i < starArray.Length; i++)
                {
                    PointF point = starArray[i];
                    float scale = i % 2 == 0 ? sOuterRadius : sInnerRadius;
                    starArray[i] = new PointF(point.X * scale + sCenterX,
                                              point.Y * scale + sCenterY);
                }
                g.DrawPolygon(pen, starArray);
                break;
            case 7:
                // Draw asterisk. Uses polygon code.
                const int astPoints = 6;
                PointF[] astArray = GeneratePolygon(astPoints);
                float aRadius = tileSize * 0.5f,
                      aCenterX = tileStart.X + aRadius,
                      aCenterY = tileStart.Y + aRadius;
                for (int i = 0; i < astArray.Length / 2; i++)
                {
                    PointF pointA = astArray[i], pointB = astArray[i + astPoints / 2];
                    pointA = new(pointA.X * aRadius * 1.1f + aCenterX,
                                 pointA.Y * aRadius * 1.1f + aCenterY);
                    pointB = new(pointB.X * aRadius * 1.1f + aCenterX,
                                 pointB.Y * aRadius * 1.1f + aCenterY);
                    g.DrawLine(pen, pointA, pointB);
                }
                break;
            default:
                // Draw unknown.
                SolidBrush brush = new(color);
                g.FillRectangle(brush, new RectangleF(tileStart, new SizeF(tileSize, tileSize)));
                break;
        }
    }
    private void RenderPlayerInput(Graphics g, int x, int y)
    {
        byte value = ActiveGame[x, y];
        if (value == 0) return; // No move made.

        float tileSize = Preferences.TileSize * ScalingFactor;
        PointF tileStart = TileToScreen(new Point(x, y));

        float padding = 35 * ScalingFactor;
        tileStart.X += padding;
        tileStart.Y += padding;
        tileSize -= padding * 2;

        float thickness = 15 * ScalingFactor;
        RenderMoveSprite(g, value, tileStart, tileSize, thickness);
    }

    private bool TileWithinRange(Point tile) =>
        tile.X >= 0 && tile.X < ActiveGame.TilesX &&
        tile.Y >= 0 && tile.Y < ActiveGame.TilesY;

    private Point TileToScreen(PointF tile)
    {
        float centerX = ClientRectangle.Width / 2, centerY = ClientRectangle.Height / 2;
        float tileSize = Preferences.TileSize * ScalingFactor;
        float lenX = tileSize * ActiveGame.TilesX,
              lenY = tileSize * ActiveGame.TilesY;

        float left = centerX - lenX * 0.5f, top = centerY - lenY * 0.5f;
        return new((int)(left + tileSize * tile.X), (int)(top + tileSize * tile.Y));
    }
    private PointF ScreenToTile(Point screen, bool convertToClient = true)
    {
        if (convertToClient) screen = PointToClient(screen);
        float centerX = ClientRectangle.Width / 2, centerY = ClientRectangle.Height / 2;
        float tileSize = Preferences.TileSize * ScalingFactor;
        float lenX = tileSize * ActiveGame.TilesX,
              lenY = tileSize * ActiveGame.TilesY;

        float left = centerX - lenX * 0.5f, top = centerY - lenY * 0.5f;
        return new((screen.X - left) / tileSize, (screen.Y - top) / tileSize);
    }
    private Point ScreenToTileInt(Point screen, bool convertToClient = true)
    {
        PointF raw = ScreenToTile(screen, convertToClient);
        if (raw.X < 0) raw.X--;
        if (raw.Y < 0) raw.Y--;
        return new Point((int)raw.X, (int)raw.Y);
    }

    private static PointF[] GeneratePolygon(int sides)
    {
        PointF[] points = new PointF[sides];
        double anglePer = 2 * Math.PI / sides;
        for (int i = 0; i < sides; i++)
        {
            points[i] = new PointF((float)Math.Sin(anglePer * i), -(float)Math.Cos(anglePer * i));
        }
        return points;
    }

    private void ExitButton_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private void ReplayButton_Click(object? sender, EventArgs e)
    {
        SetupBoard();
    }
}
