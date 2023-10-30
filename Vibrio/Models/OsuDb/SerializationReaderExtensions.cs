using osu.Game.IO.Legacy;

namespace Vibrio.Models.OsuDb {
    public static class SerializationReaderExtensions {
        public static IntDoublePair[] ReadStarRatingInfo(this SerializationReader reader) {
            var length = reader.ReadInt32();
            var array = new IntDoublePair[length];
            for (int i = 0; i < length; i++) {
                // int flag
                reader.ReadByte();
                var intValue = reader.ReadInt32();

                // double flag
                reader.ReadByte();
                var doubleValue = reader.ReadDouble();

                array[i] = new IntDoublePair { IntValue = intValue, DoubleValue = doubleValue };
            }
            return array;
        }

        public static TimingPoint[] ReadTimingPoints(this SerializationReader reader) {
            var length = reader.ReadInt32();
            var array = new TimingPoint[length];
            for (int i = 0; i < length; i++) {
                var bpm = reader.ReadDouble();
                var offset = reader.ReadDouble();
                var isNotInherited = reader.ReadBoolean();
                array[i] = new TimingPoint { Bpm = bpm, Offset = offset, IsNotInherited = isNotInherited };
            }
            return array;
        }
    }
}
