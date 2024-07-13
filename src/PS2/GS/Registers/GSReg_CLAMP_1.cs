namespace BinarySerializer.PlayStation.PS2
{
    public class GSReg_CLAMP_1 : GSRegister
    {
        public override GSRegisters RegisterByte => GSRegisters.CLAMP_1;

        public WrapMode WMS { get; set; }
        public WrapMode WMT { get; set; }
        public int MINU { get; set; }
        public int MAXU { get; set; }
        public int MINV { get; set; }
        public int MAXV { get; set; }

        public override void SerializeRegisterImpl(SerializerObject s)
        {
            s.DoBits<long>(b =>
            {
                WMS = b.SerializeBits<WrapMode>(WMS, 2, name: nameof(WMS));
                WMT = b.SerializeBits<WrapMode>(WMT, 2, name: nameof(WMT));
                MINU = b.SerializeBits<int>(MINU, 10, name: nameof(MINU));
                MAXU = b.SerializeBits<int>(MAXU, 10, name: nameof(MAXU));
                MINV = b.SerializeBits<int>(MINV, 10, name: nameof(MINV));
                MAXV = b.SerializeBits<int>(MAXV, 10, name: nameof(MAXV));
                b.SerializePadding(20);
            });
        }
    }

    public enum WrapMode
    {
        REPEAT,
        CLAMP,
        REGION_CLAMP,
        REGION_REPEAT
    }
}