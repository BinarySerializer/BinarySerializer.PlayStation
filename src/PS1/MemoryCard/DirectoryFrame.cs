namespace BinarySerializer.PlayStation.PS1.MemoryCard
{
    public class DirectoryFrame : BinarySerializable
    {
        public BlockUsage Usage { get; set; }
        public BlockUsability Usability { get; set; }
        public byte[] Reserved { get; set; } // FF FF FF
        public byte[] UseByte { get; set; }
        public short LinkOrder { get; set; } // 0-14 or -1
        public string CountryCode { get; set; } // BI, BA, BE
        public string ProductCode { get; set; } // AAAA-00000
        public string Identifier { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoProcessed(new ChecksumXOR8Processor(), p =>
            {
                s.DoBits<byte>(b =>
                {
                    Usage = b.SerializeBits<BlockUsage>(Usage, 4, name: nameof(Usage));
                    Usability = b.SerializeBits<BlockUsability>(Usability, 4, name: nameof(Usability));
                });
                Reserved = s.SerializeArray<byte>(Reserved, 3, name: nameof(Reserved));
                UseByte = s.SerializeArray<byte>(UseByte, 4, name: nameof(UseByte));
                LinkOrder = s.Serialize<short>(LinkOrder, name: nameof(LinkOrder));
                CountryCode = s.SerializeString(CountryCode, length: 2, name: nameof(CountryCode));
                ProductCode = s.SerializeString(ProductCode, length: 10, name: nameof(ProductCode));
                Identifier = s.SerializeString(Identifier, length: 8, name: nameof(Identifier));

                s.SerializePadding(97);
                p.Serialize<byte>(s, name: "Checksum");
            });
        }
    }
}