namespace BinarySerializer.PlayStation.PS1.MemoryCard
{
    public class HeaderFrame : BinarySerializable
    {
        public override void SerializeImpl(SerializerObject s)
        {
            s.DoProcessed(new ChecksumXOR8Processor(), p =>
            {
                s.SerializeMagicString("MC", 2);
                s.SerializePadding(125);
                p.Serialize<byte>(s, name: "Checksum");
            });
        }
    }
}