namespace BinarySerializer.PlayStation.PS1.Psyq
{
    public class CdlLOC : BinarySerializable
    {
        public byte Minute { get; set; }
        public byte Second { get; set; }
        public byte Sector { get; set; }
        public byte Track { get; set; } // Unused

        protected int MinuteValue
        {
            get => FromBCD(Minute);
            set => Minute = ToBCD(value);
        }
        protected int SecondValue
        {
            get => FromBCD(Second);
            set => Second = ToBCD(value);
        }
        protected int SectorValue
        {
            get => FromBCD(Sector);
            set => Sector = ToBCD(value);
        }

        public int LBA
        {
            get => (SectorValue + (SecondValue * 75) + (MinuteValue * 60 * 75)) - 150;
            set
            {
                var tmp = value + 150;
                SectorValue = (byte)(tmp % 75);
                SecondValue = (byte)((tmp / 75) % 60);
                MinuteValue = (byte)(tmp / 75 / 60);
            }
        }

        protected int FromBCD(byte bcd)
        {
            var result = 0;
            result *= 100;
            result += (10 * (bcd >> 4));
            result += bcd & 0xf;
            return result;
        }

        protected byte ToBCD(int value)
        {
            int tens = value / 10;
            int units = value % 10;
            return (byte)((tens << 4) | units);
        }

        public override void SerializeImpl(SerializerObject s)
        {
            Minute = s.Serialize<byte>(Minute, name: nameof(Minute));
            Second = s.Serialize<byte>(Second, name: nameof(Second));
            Sector = s.Serialize<byte>(Sector, name: nameof(Sector));
            Track = s.Serialize<byte>(Track, name: nameof(Track));
            s.Log("LBA: 0x{0:X8}", LBA);
        }
    }
}