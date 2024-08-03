namespace BinarySerializer.PlayStation.PS3
{
    public class GTF : BinarySerializable
    {
        // TODO: Should check this since the format is different between versions!
        public uint Version { get; set; } // dds2gtf converter version

        public uint TexturesDataSize { get; set; }
        public uint TexturesCount { get; set; }
        public uint Uint_0C { get; set; } // Always 0?
        public uint HeaderSize { get; set; }
        public GTFTexture[] Textures { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Version = s.Serialize<uint>(Version, name: nameof(Version));
            TexturesDataSize = s.Serialize<uint>(TexturesDataSize, name: nameof(TexturesDataSize));
            TexturesCount = s.Serialize<uint>(TexturesCount, name: nameof(TexturesCount));
            Uint_0C = s.Serialize<uint>(Uint_0C, name: nameof(Uint_0C));
            HeaderSize = s.Serialize<uint>(HeaderSize, name: nameof(HeaderSize));
            Textures = s.SerializeObjectArray<GTFTexture>(Textures, TexturesCount, x => x.Pre_DataStartPointer = Offset + HeaderSize, name: nameof(Textures));
        }
    }
}