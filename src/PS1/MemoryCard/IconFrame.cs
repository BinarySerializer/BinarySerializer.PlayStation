namespace BinarySerializer.PlayStation.PS1.MemoryCard
{
    public class IconFrame : BinarySerializable
    {
        public byte[] ImgData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ImgData = s.SerializeArray<byte>(ImgData, 128, name: nameof(ImgData));
        }
    }
}