using System.Text;

namespace Vibrio.Models.OsuDb {
    public class OsuDb {
        private readonly BinaryReader reader;

        public int Version;
        public int FolderCount;
        public bool AccountUnlocked;
        public DateTime UnlockDate;
        public byte[] PlayerName;
        public int BeatmapCount;

        public OsuDb(BinaryReader reader) {
            this.reader = reader;
            Version = reader.ReadInt32();
            FolderCount = reader.ReadInt32();
            AccountUnlocked = reader.ReadBoolean();
            UnlockDate = reader.ReadDateTime();
            PlayerName = reader.ReadLEB128String();
            BeatmapCount = reader.ReadInt32();
        }

        private Dictionary<int, string> LoadBeatmapRegistry() {
            var registry = new Dictionary<int, string>();
            for (int _ = 0; _ < BeatmapCount; _++) {
                var info = new BeatmapInfo(reader);
                registry[info.DifficultyId] = Path.Combine(
                    Encoding.Default.GetString(info.FolderName),
                    Encoding.Default.GetString(info.OsuFileName)
                );
            }
            return registry;
        }

        public static Dictionary<int, string> LoadBeatmapRegistry(FileStream stream) {
            var reader = new BinaryReader(stream);
            var db = new OsuDb(reader);
            return db.LoadBeatmapRegistry();
        }
    }
}
