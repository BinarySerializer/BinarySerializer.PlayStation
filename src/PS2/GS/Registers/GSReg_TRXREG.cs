namespace BinarySerializer.PlayStation.PS2
{
    public class GSReg_TRXREG : GSRegister
    {
        public override GSRegisters RegisterByte => GSRegisters.TRXREG;

        public ushort RRW { get; set; }
        public ushort RRH { get; set; }

        public override void SerializeRegisterImpl(SerializerObject s)
        {
            s.DoBits<long>(b =>
            {
                RRW = b.SerializeBits<ushort>(RRW, 12, name: nameof(RRW));
                b.SerializePadding(20);
                RRH = b.SerializeBits<ushort>(RRH, 12, name: nameof(RRH));
                b.SerializePadding(20);
            });
        }
    }
}