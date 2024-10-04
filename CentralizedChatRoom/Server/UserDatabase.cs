using ChatRoom.Centralized.Shared.ObjectModels;
using NLog;
using System.Collections;

namespace ChatRoom.Centralized.Server;

public class UserDatabase : IEnumerable<PrivateUserInfo>
{
    public string FilePath { get; set; }
    public TimeSpan SaveEvery { get; set; }
    public DateTime LastSave { get; private set; }

    public List<PrivateUserInfo> Users { get; set; }

    private readonly object LOCK = new();
    private readonly Logger logger = ProgramServer.logFactory.GetCurrentClassLogger();

    public UserDatabase(string filePath, TimeSpan saveEvery)
    {
        logger.Debug("Starting user database");

        FilePath = filePath;
        SaveEvery = saveEvery;
        LastSave = DateTime.Now;
        Users = new();

        if (File.Exists(filePath))
        {
            // Load file.
            logger.Debug($"Loading user data from {FilePath}");
            FileStream fs = new(FilePath, FileMode.Open);
            BinaryReader reader = new(fs);

            while (fs.Position < fs.Length - 1)
                Users.Add(PrivateUserInfo.ReadData(reader));

            reader.Close();
        }

        logger.Debug("Done");
        Task.Run(AutosaveLoop);
    }

    public IEnumerator<PrivateUserInfo> GetEnumerator() => Users.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private async Task AutosaveLoop()
    {
        logger.Debug($"User database autosave is every {SaveEvery}");
        while (true)
        {
            await Task.Delay(SaveEvery);
            SaveData();
        }
    }

    public void Save() => Task.Run(SaveData);

    private void SaveData()
    {
        lock (LOCK)
        {
            logger.Debug($"Saving user database");
            FileStream fs = new(FilePath, FileMode.Create);
            BinaryWriter writer = new(fs);

            for (int i = 0; i < Users.Count; i++)
            {
                PrivateUserInfo user = Users[i];
                user.WriteData(writer);
            }

            writer.Close();
            LastSave = DateTime.Now;
            logger.Debug("Done");
        }
    }
}
