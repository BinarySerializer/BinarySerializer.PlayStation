namespace BinarySerializer.PlayStation.PS1.MemoryCard
{
    public class TitleFrame : BinarySerializable
    {
        public byte IconDisplayFlag { get; set; }
        public byte BlockNumber { get; set; }
        public string Title { get; set; }
        public byte[] Reserved1 { get; set; }
        public short PocketStationMCIconFramesCount { get; set; }
        public string PocketStationIdentifier { get; set; }
        public short PocketStationAPIconFramesCount { get; set; }
        public byte[] Reserved2 { get; set; }
        public RGBA5551Color[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeMagicString("SC", 2);
            IconDisplayFlag = s.Serialize<byte>(IconDisplayFlag, name: nameof(IconDisplayFlag));
            BlockNumber = s.Serialize<byte>(BlockNumber, name: nameof(BlockNumber));
            Title = s.SerializeString(Title, length: 64, name: nameof(Title));
            Reserved1 = s.SerializeArray<byte>(Reserved1, 12, name: nameof(Reserved1));
            PocketStationMCIconFramesCount = s.Serialize<short>(PocketStationMCIconFramesCount, name: nameof(PocketStationMCIconFramesCount));
            PocketStationIdentifier = s.SerializeString(PocketStationIdentifier, length: 4, name: nameof(PocketStationIdentifier));
            PocketStationAPIconFramesCount = s.Serialize<short>(PocketStationAPIconFramesCount, name: nameof(PocketStationAPIconFramesCount));
            Reserved2 = s.SerializeArray<byte>(Reserved2, 8, name: nameof(Reserved2));
            Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, 16, name: nameof(Palette));
        }
    }
}