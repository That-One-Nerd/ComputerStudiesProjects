using System.Text.Json;

namespace BonusTicTacToe;

public class Preferences
{
    public const string PrefsPath = "./prefs.json";
    public static readonly bool SaveToFile = false;

    public float TileSize { get; set; } = 150;
    public float WindowBuffer { get; set; } = 200;
    public float LineThickness { get; set; } = 3;

    public static Preferences Load()
    {
        if (!SaveToFile) return new();

        Preferences prefs;
        if (File.Exists(PrefsPath))
        {
            FileStream fs = new(PrefsPath, FileMode.Open);
            prefs = JsonSerializer.Deserialize<Preferences>(fs)!;
            fs.Close();
        }
        else
        {
            prefs = new();
            prefs.Save();
        }
        return prefs;
    }

    public void Save()
    {
        if (!SaveToFile) return;

        FileStream fs = new(PrefsPath, FileMode.Create);
        JsonSerializer.Serialize(fs, this);
        fs.Close();
    }
}
