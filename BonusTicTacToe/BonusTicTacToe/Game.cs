using System.Text.Json;

namespace BonusTicTacToe;

public class Game
{
    public const string GamePath = "./game.json";
    public static readonly bool SaveToFile = false;

    public static Game Defaults => new()
    {
        TilesX = 3,
        TilesY = 3,
        Players = 2,
        ConnectToWin = 3,
        MovesPerTurn = 1,
        WinsToFinish = 1,
    };

    public required int TilesX { get; set; }
    public required int TilesY { get; set; }

    public required int Players { get; set; }
    public required int ConnectToWin { get; set; }
    public required int MovesPerTurn { get; set; }
    public required int WinsToFinish { get; set; }

    public int CurrentTurn { get; set; } = 0;
    public byte[] Board
    {
        // We can't create the array in the constructor
        // because setting required properties happens
        // after that step.
        get => _board ??= new byte[TilesX * TilesY];
        set => _board = value;
    }
    public byte HasFinished { get; set; } = 0;
    public bool IsDraw { get; set; } = false;
    public int TotalMoves { get; set; } = 0;

    private byte[]? _board = null;

    public byte this[int x, int y]
    {
        get => Board[y * TilesX + x];
        set => Board[y * TilesX + x] = value;
    }

    public static Game Load()
    {
        if (!SaveToFile) return Defaults;

        Game game;
        if (File.Exists(GamePath))
        {
            FileStream fs = new(GamePath, FileMode.Open);
            game = JsonSerializer.Deserialize<Game>(fs)!;
            fs.Close();

            // Game is already completed.
            if (game.HasFinished != 0 || game.IsDraw) game = Defaults;
        }
        else
        {
            game = Defaults;
            game.Save();
        }
        return game;
    }

    public void Save()
    {
        if (!SaveToFile) return;

        FileStream fs = new(GamePath, FileMode.Create);
        JsonSerializer.Serialize(fs, this);
        fs.Close();
    }

    public List<WinObject> GetWins()
    {
        // Check all tiles for three/four/... of a kind either
        // left-right, top-bottom, or right/left diagonals.
        List<WinObject> wins = [];
        for (int x1 = 0; x1 < TilesX; x1++)
        {
            for (int y1 = 0; y1 < TilesY; y1++)
            {
                byte value1 = this[x1, y1];
                if (value1 == 0) continue;
                int inARow = 1;

                // Leftwards and rightwards.
                int minX = x1;
                int maxX = x1;
                for (int x2 = x1 - 1; x2 >= 0; x2--)
                {
                    byte value2 = this[x2, y1];
                    if (value1 != value2) break;
                    inARow++;
                    minX = x2;
                }
                for (int x2 = x1 + 1; x2 < TilesX; x2++)
                {
                    byte value2 = this[x2, y1];
                    if (value1 != value2) break;
                    inARow++;
                    maxX = x2;
                }
                if (inARow >= ConnectToWin) tryAdd(new()
                {
                    player = (byte)(value1 - 1),
                    direction = Direction.LeftRight,
                    minX = minX,
                    maxX = maxX,
                    minY = y1,
                    maxY = y1
                });

                // Upwards and downwards.
                int minY = y1;
                int maxY = y1;
                inARow = 1;
                for (int y2 = y1 - 1; y2 >= 0; y2--)
                {
                    byte value2 = this[x1, y2];
                    if (value1 != value2) break;
                    inARow++;
                    minY = y2;
                }
                for (int y2 = y1 + 1; y2 < TilesY; y2++)
                {
                    byte value2 = this[x1, y2];
                    if (value1 != value2) break;
                    inARow++;
                    maxY = y2;
                }
                if (inARow >= ConnectToWin) tryAdd(new()
                {
                    player = (byte)(value1 - 1),
                    direction = Direction.UpDown,
                    minX = x1,
                    maxX = x1,
                    minY = minY,
                    maxY = maxY
                });

                // Diagonal top-left to bottom-right.
                (minX, minY) = (x1, y1);
                (maxX, maxY) = (x1, y1);
                inARow = 1;
                for ((int x2, int y2) = (x1 - 1, y1 - 1); x2 >= 0 && y2 >= 0; x2--, y2--)
                {
                    byte value2 = this[x2, y2];
                    if (value1 != value2) break;
                    inARow++;
                    minX = x2;
                    minY = y2;
                }
                for ((int x2, int y2) = (x1 + 1, y1 + 1); x2 < TilesX && y2 < TilesY; x2++, y2++)
                {
                    byte value2 = this[x2, y2];
                    if (value1 != value2) break;
                    inARow++;
                    maxX = x2;
                    maxY = y2;
                }
                if (inARow >= ConnectToWin) tryAdd(new()
                {
                    player = (byte)(value1 - 1),
                    direction = Direction.DiagonalTopLeftBottomRight,
                    minX = minX,
                    maxX = maxX,
                    minY = minY,
                    maxY = maxY
                });

                // Diagonal top-right to bottom-left.
                (minX, minY) = (x1, y1);
                (maxX, maxY) = (x1, y1);
                inARow = 1;
                for ((int x2, int y2) = (x1 + 1, y1 - 1); x2 < TilesX && y2 >= 0; x2++, y2--)
                {
                    byte value2 = this[x2, y2];
                    if (value1 != value2) break;
                    inARow++;
                    maxX = x2;
                    minY = y2;
                }
                for ((int x2, int y2) = (x1 - 1, y1 + 1); x2 >= 0 && y2 < TilesY; x2--, y2++)
                {
                    byte value2 = this[x2, y2];
                    if (value1 != value2) break;
                    inARow++;
                    minX = x2;
                    maxY = y2;
                }
                if (inARow >= ConnectToWin) tryAdd(new()
                {
                    player = (byte)(value1 - 1),
                    direction = Direction.DiagonalTopRightBottomLeft,
                    minX = minX,
                    maxX = maxX,
                    minY = minY,
                    maxY = maxY
                });
            }
        }
        void tryAdd(WinObject win)
        {
            if (!wins.Any(x => x.direction == win.direction &&
                x.minX == win.minX && x.minY == win.minY &&
                x.player == win.player))
                wins.Add(win);
        }
        return wins;
    }
}
