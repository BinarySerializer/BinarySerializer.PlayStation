namespace BinarySerializer.PlayStation.PS2
{
    /// <see href="https://psi-rockin.github.io/ps2tek/#dmacchainmode">DMAtag documentation</see>
    public class Chain_DMAtag : BinarySerializable, ISerializerShortLog
    {
        public ushort QWC { get; set; }
        public byte PCE { get; set; }
        public TagID ID { get; set; }
        public byte IRQ { get; set; }
        public uint ADDR { get; set; }
        public byte SPR { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<long>(b =>
            {
                QWC = b.SerializeBits<ushort>(QWC, 16, name: nameof(QWC));
                b.SerializeBits<int>(default, 10, name: "Padding");
                PCE = b.SerializeBits<byte>(PCE, 2, name: nameof(PCE));
                ID = b.SerializeBits<TagID>(ID, 3, name: nameof(ID));
                IRQ = b.SerializeBits<byte>(IRQ, 1, name: nameof(IRQ));
                ADDR = b.SerializeBits<uint>(ADDR, 31, name: nameof(ADDR));
                SPR = b.SerializeBits<byte>(SPR, 1, name: nameof(SPR));
            });
        }

        public enum TagID : int
        {
            REFE_CNTS, // refe for source chain tag, cnts for destination chain tag
            CNT,
            NEXT,
            REF,
            REFS,
            CALL,
            RET,
            END
        }

        public override string ToString() => $"DMATag(TagID: {ID}, QWC: {QWC}, ADDR: {ADDR:X8}, SPR: {SPR}, PCE: {PCE}, IRQ: {IRQ})";
        public string ShortLog => ToString();
    }
}