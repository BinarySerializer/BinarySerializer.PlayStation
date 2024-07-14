namespace BinarySerializer.PlayStation.PS2.MemoryCard
{
    public class TimeStamp : BinarySerializable
    {
        public byte Reserved { get; set; }
        public byte Seconds { get; set; }
        public byte Minutes { get; set; }
        public byte Hours { get; set; }
        public byte DayOfMonth { get; set; }
        public byte Month { get; set; }
        public ushort Year { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Reserved = s.Serialize<byte>(Reserved, name: nameof(Reserved));
            Seconds = s.Serialize<byte>(Seconds, name: nameof(Seconds));
            Minutes = s.Serialize<byte>(Minutes, name: nameof(Minutes));
            Hours = s.Serialize<byte>(Hours, name: nameof(Hours));
            DayOfMonth = s.Serialize<byte>(DayOfMonth, name: nameof(DayOfMonth));
            Month = s.Serialize<byte>(Month, name: nameof(Month));
            Year = s.Serialize<ushort>(Year, name: nameof(Year));
        }
    }
}