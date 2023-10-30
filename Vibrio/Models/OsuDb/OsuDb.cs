using osu.Game.IO.Legacy;

namespace Vibrio.Models.OsuDb {
    public class OsuDb {
        private readonly SerializationReader reader;

        public int Version;
        public int FolderCount;
        public bool AccountUnlocked;
        public DateTime UnlockDate;
        public string PlayerName;
        public int BeatmapCount;

        public OsuDb(SerializationReader reader) {
            this.reader = reader;
            Version = reader.ReadInt32();
            FolderCount = reader.ReadInt32();
            AccountUnlocked = reader.ReadBoolean();
            UnlockDate = reader.ReadDateTime();
            PlayerName = reader.ReadString();
            BeatmapCount = reader.ReadInt32();
        }

        private Dictionary<int, string> LoadBeatmapRegistry() {
            var registry = new Dictionary<int, string>();
            for (int _ = 0; _ < BeatmapCount; _++) {
                var info = new BeatmapInfo(reader);
                registry[info.DifficultyId] = Path.Combine(info.FolderName, info.OsuFileName);
                registry[info.DifficultyId] = Path.Combine(info.FolderName, info.OsuFileName);
            }
            return registry;
        }

        public static Dictionary<int, string> LoadBeatmapRegistry(FileStream stream) {
            var reader = new SerializationReader(stream);
            var db = new OsuDb(reader);
            return db.LoadBeatmapRegistry();
        }
    }
}
