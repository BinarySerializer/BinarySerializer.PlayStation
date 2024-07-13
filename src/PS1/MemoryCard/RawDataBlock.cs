namespace BinarySerializer.PlayStation.PS1.MemoryCard
{
    public class RawDataBlock : BinarySerializable
    {
        public TitleFrame Title { get; set; }
        public IconFrame[] IconFrames { get; set; }
        public byte[] SaveData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Title = s.SerializeObject<TitleFrame>(Title, name: nameof(Title));
            IconFrames = s.SerializeObjectArray<IconFrame>(IconFrames, 3, name: nameof(IconFrames));
            SaveData = s.SerializeArray<byte>(SaveData, MemoryCard.FrameSize * 60, name: nameof(SaveData));
        }
    }
}