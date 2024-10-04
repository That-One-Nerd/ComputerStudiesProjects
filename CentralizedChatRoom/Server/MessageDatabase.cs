using ChatRoom.Centralized.Shared.ObjectModels;
using NLog;
using System.Collections;

namespace ChatRoom.Centralized.Server;

public class MessageDatabase : IEnumerable<MessageInfo>
{
    public string FilePath { get; set; }
    public TimeSpan SaveEvery { get; set; }
    public DateTime LastSave { get; private set; }

    public List<MessageInfo> Messages { get; set; }

    private readonly object LOCK = new();
    private readonly Logger logger = ProgramServer.logFactory.GetCurrentClassLogger();

    public MessageDatabase(string filePath, TimeSpan saveEvery)
    {
        logger.Debug("Starting message database");

        FilePath = filePath;
        SaveEvery = saveEvery;
        LastSave = DateTime.Now;
        Messages = new();

        if (File.Exists(filePath))
        {
            // Load file.
            logger.Debug($"Loading message data from {FilePath}");
            FileStream fs = new(FilePath, FileMode.Open);
            BinaryReader reader = new(fs);

            while (fs.Position < fs.Length - 1)
                Messages.Add(MessageInfo.ReadData(reader));

            reader.Close();
        }

        logger.Debug("Done");
        Task.Run(AutosaveLoop);
    }

    public IEnumerator<MessageInfo> GetEnumerator() => Messages.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private async Task AutosaveLoop()
    {
        logger.Debug($"Message database autosave is every {SaveEvery}");
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
            logger.Debug($"Saving message database");
            FileStream fs = new(FilePath, FileMode.Create);
            BinaryWriter writer = new(fs);

            for (int i = 0; i < Messages.Count; i++)
            {
                MessageInfo msg = Messages[i];
                msg.WriteData(writer);
            }

            writer.Close();
            LastSave = DateTime.Now;
            logger.Debug("Done");
        }
    }
}
