namespace BinarySerializer.PlayStation.PS1.MemoryCard
{
    public class DataBlock<T> : BinarySerializable
        where T : BinarySerializable, new()
    {
        public TitleFrame Title { get; set; }
        public IconFrame[] IconFrames { get; set; }
        public T SaveData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Title = s.SerializeObject<TitleFrame>(Title, name: nameof(Title));
            IconFrames = s.SerializeObjectArray<IconFrame>(IconFrames, 3, name: nameof(IconFrames));
            SaveData = s.SerializeObject<T>(SaveData, name: nameof(SaveData));
        }
    }
}