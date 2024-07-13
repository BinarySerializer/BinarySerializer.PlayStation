using System;

namespace BinarySerializer.PlayStation.PS1
{
    /// <summary>
    /// Background map data file
    /// </summary>
    public class BGD : BinarySerializable
    {
        public byte Version { get; set; }
        public bool HasATTR { get; set; }
        public bool IsATTR16Bit { get; set; }
        public byte MapWidth { get; set; }
        public byte MapHeight { get; set; }
        public byte CellWidth { get; set; }
        public byte CellHeight { get; set; }

        public ushort[] Map { get; set; } // CEL indices
        public byte[] Attr_8 { get; set; }
        public ushort[] Attr_16 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeMagic<byte>(0x23, name: "ID");

            Version = s.Serialize<byte>(Version, name: nameof(Version));

            if (Version != 0x00)
                throw new Exception($"Invalid Version {Version}");

            s.DoBits<ushort>(b =>
            {
                b.SerializeBits<int>(0, 14, name: "Reserved");
                IsATTR16Bit = b.SerializeBits<bool>(IsATTR16Bit, 1, name: nameof(IsATTR16Bit));
                HasATTR = b.SerializeBits<bool>(HasATTR, 1, name: nameof(HasATTR));
            });

            MapWidth = s.Serialize<byte>(MapWidth, name: nameof(MapWidth));
            MapHeight = s.Serialize<byte>(MapHeight, name: nameof(MapHeight));
            CellWidth = s.Serialize<byte>(CellWidth, name: nameof(CellWidth));
            CellHeight = s.Serialize<byte>(CellHeight, name: nameof(CellHeight));

            Map = s.SerializeArray<ushort>(Map, MapWidth * MapHeight, name: nameof(Map));

            if (HasATTR)
            {
                if (IsATTR16Bit)
                    Attr_16 = s.SerializeArray<ushort>(Attr_16, MapWidth * MapHeight, name: nameof(Attr_16));
                else
                    Attr_8 = s.SerializeArray<byte>(Attr_8, MapWidth * MapHeight, name: nameof(Attr_8));
            }
        }
    }
}