﻿using System.Text;

namespace Vibrio.Models {
    public class IntDoublePair {
        public int IntValue;
        public double DoubleValue;
    }

    public class TimingPoint {
        public double Bpm;
        public double Offset;
        public Boolean IsNotInherited;
    }

    public class BeatmapInfo {
        public byte[] ArtistName;
        public byte[] ArtistNameUnicode;
        public byte[] SongTitle;
        public byte[] SongTitleUnicode;
        public byte[] CreatorName;
        public byte[] DifficultyName;
        public byte[] AudioFileName;
        public byte[] BeatmapHash;
        public byte[] OsuFileName;
        public byte RankedStatus;
        public short HitcircleCount;
        public short SliderCount;
        public short SpinnerCount;
        public long LastModificationTime;
        public Single ApproachRate;
        public Single CircleSize;
        public Single HpDrain;
        public Single OverallDifficulty;
        public double SliderVelocity;
        public IntDoublePair[] StandardStarRatingInfo;
        public IntDoublePair[] TaikoStarRatingInfo;
        public IntDoublePair[] CtbStarRatingInfo;
        public IntDoublePair[] ManiaStarRatingInfo;
        public int DrainTime;
        public int TotalTime;
        public int AudioPreviewTime;
        public TimingPoint[] TimingPoints;
        public int DifficultyId;
        public int BeatmapId;
        public int ThreadId;
        public byte StandardGrade;
        public byte TaikoGrade;
        public byte CtbGrade;
        public byte ManiaGrade;
        public short LocalOffset;
        public Single StackLeniency;
        public byte GameplayMode;
        public byte[] SongSource;
        public byte[] SongTags;
        public short OnlineOffset;
        public byte[] TitleFont;
        public bool IsUnplayed;
        public long LastPlayedTime;
        public bool IsOsz2;
        public byte[] FolderName;
        public long LastCheckedOnlineTime;
        public bool IgnoreBeatmapSound;
        public bool IgnoreBeatmapSkin;
        public bool DisableStoryboard;
        public bool DisableVideo;
        public bool VisualOverride;
        public int LastModificationTime2;
        public byte ManiaScrollSpeed;

        public BeatmapInfo(BinaryReader reader) {
            ArtistName = reader.ReadLEB128String();
            ArtistNameUnicode = reader.ReadLEB128String();
            SongTitle = reader.ReadLEB128String();
            SongTitleUnicode = reader.ReadLEB128String();
            CreatorName = reader.ReadLEB128String();
            DifficultyName = reader.ReadLEB128String();
            AudioFileName = reader.ReadLEB128String();
            BeatmapHash = reader.ReadLEB128String();
            OsuFileName = reader.ReadLEB128String();
            RankedStatus = reader.ReadByte();
            HitcircleCount = reader.ReadInt16();
            SliderCount = reader.ReadInt16();
            SpinnerCount = reader.ReadInt16();
            LastModificationTime = reader.ReadInt64();
            ApproachRate = reader.ReadInt32();
            CircleSize = reader.ReadInt32();
            HpDrain = reader.ReadInt32();
            OverallDifficulty = reader.ReadInt32();
            SliderVelocity = reader.ReadDouble();
            StandardStarRatingInfo = reader.ReadStarRatingInfo();
            TaikoStarRatingInfo = reader.ReadStarRatingInfo();
            CtbStarRatingInfo = reader.ReadStarRatingInfo();
            ManiaStarRatingInfo = reader.ReadStarRatingInfo();
            DrainTime = reader.ReadInt32();
            TotalTime = reader.ReadInt32();
            AudioPreviewTime = reader.ReadInt32();
            TimingPoints = reader.ReadTimingPoints();
            DifficultyId = reader.ReadInt32();
            BeatmapId = reader.ReadInt32();
            ThreadId = reader.ReadInt32();
            StandardGrade = reader.ReadByte();
            TaikoGrade = reader.ReadByte();
            CtbGrade = reader.ReadByte();
            ManiaGrade = reader.ReadByte();
            LocalOffset = reader.ReadInt16();
            StackLeniency = reader.ReadInt32();
            GameplayMode = reader.ReadByte();
            SongSource = reader.ReadLEB128String();
            SongTags = reader.ReadLEB128String();
            OnlineOffset = reader.ReadInt16();
            TitleFont = reader.ReadLEB128String();
            IsUnplayed = reader.ReadBoolean();
            LastPlayedTime = reader.ReadInt64();
            IsOsz2 = reader.ReadBoolean();
            FolderName = reader.ReadLEB128String();
            LastCheckedOnlineTime = reader.ReadInt64();
            IgnoreBeatmapSound = reader.ReadBoolean();
            IgnoreBeatmapSkin = reader.ReadBoolean();
            DisableStoryboard = reader.ReadBoolean();
            DisableVideo = reader.ReadBoolean();
            VisualOverride = reader.ReadBoolean();
            LastModificationTime2 = reader.ReadInt32();
            ManiaScrollSpeed = reader.ReadByte();
        }
    }

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