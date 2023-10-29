using Vibrio.Models.OsuDb;

namespace Vibrio.Models {
    public static class BinaryReaderExtensions {
        public static DateTime ReadDateTime(this BinaryReader reader) => DateTime.UnixEpoch.AddTicks(reader.ReadInt64());

        public static byte[] ReadLEB128String(this BinaryReader reader) {
            var header = reader.ReadByte();
            if (header == 0x00) {
                return Array.Empty<byte>();
            }

            var length = 0;
            var shift = 0;
            byte value;
            do {
                value = reader.ReadByte();
                length |= (value & 0x7f) << shift;
                shift += 7;
            } while ((value & 0x80) != 0);

            var array = new byte[length];
            for (int i = 0; i < length; i++) {
                array[i] = reader.ReadByte();
            }
            return array;
        }

        public static IntDoublePair[] ReadStarRatingInfo(this BinaryReader reader) {
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

        public static TimingPoint[] ReadTimingPoints(this BinaryReader reader) {
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
