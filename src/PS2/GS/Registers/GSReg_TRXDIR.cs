namespace BinarySerializer.PlayStation.PS2
{
    public class GSReg_TRXDIR : GSRegister
    {
        public override GSRegisters RegisterByte => GSRegisters.TRXDIR;

        public TransmissionDirection XDIR { get; set; }

        public override void SerializeRegisterImpl(SerializerObject s)
        {
            s.DoBits<long>(b =>
            {
                XDIR = b.SerializeBits<TransmissionDirection>(XDIR, 11, name: nameof(XDIR));
                b.SerializePadding(53);
            });
        }

        public enum TransmissionDirection
        {
            GIFtoVRAM,
            VRAMtoGIF,
            VRAMtoVRAM,
            DEACTIVATED
        }
    }
}