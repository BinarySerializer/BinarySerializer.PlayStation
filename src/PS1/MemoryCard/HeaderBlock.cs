namespace BinarySerializer.PlayStation.PS1.MemoryCard
{
    public class HeaderBlock : BinarySerializable
    {
        public HeaderFrame Header { get; set; }
        public DirectoryFrame[] Directories { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Header = s.SerializeObject<HeaderFrame>(Header, name: nameof(Header));
            Directories = s.SerializeObjectArray<DirectoryFrame>(Directories, 15, name: nameof(Directories));
        }
    }
}