namespace BinarySerializer.PlayStation.PS3
{
    public class GTFTexture : BinarySerializable
    {
        public Pointer Pre_DataStartPointer { get; set; }

        public uint TextureDataSize { get; set; }
        public GTFFormat Format { get; set; }
        public byte MipmapLevels { get; set; }
        public GTFDimension Dimension { get; set; }
        public bool Cubemap { get; set; }
        public uint Remap { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public ushort Depth { get; set; }
        public GTFLocation Location { get; set; }
        public uint Pitch { get; set; }
        public uint DataOffset { get; set; }

        public byte[] TextureData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TextureDataSize = s.Serialize<uint>(TextureDataSize, name: nameof(TextureDataSize));
            Format = s.Serialize<GTFFormat>(Format, name: nameof(Format));
            MipmapLevels = s.Serialize<byte>(MipmapLevels, name: nameof(MipmapLevels));
            Dimension = s.Serialize<GTFDimension>(Dimension, name: nameof(Dimension));
            Cubemap = s.Serialize<bool>(Cubemap, name: nameof(Cubemap));
            Remap = s.Serialize<uint>(Remap, name: nameof(Remap));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            Depth = s.Serialize<ushort>(Depth, name: nameof(Depth));
            Location = s.Serialize<GTFLocation>(Location, name: nameof(Location));
            s.SerializePadding(1, logIfNotNull: true);
            Pitch = s.Serialize<uint>(Pitch, name: nameof(Pitch));
            DataOffset = s.Serialize<uint>(DataOffset, name: nameof(DataOffset));

            s.DoAt(Pre_DataStartPointer + DataOffset, () =>
                TextureData = s.SerializeArray<byte>(TextureData, TextureDataSize, name: nameof(TextureData)));
        }
    }
}