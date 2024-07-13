namespace BinarySerializer.PlayStation.PS2
{
    public class GSReg_BITBLTBUF : GSRegister
    {
        public override GSRegisters RegisterByte => GSRegisters.BITBLTBUF;

        public ushort SBP { get; set; }
        public ushort SBW { get; set; }
        public GS.PixelStorageMode SPSM { get; set; }
        public ushort DBP { get; set; }
        public ushort DBW { get; set; }
        public GS.PixelStorageMode DPSM { get; set; }

        public override void SerializeRegisterImpl(SerializerObject s)
        {
            s.DoBits<long>(b =>
            {
                SBP = b.SerializeBits<ushort>(SBP, 14, name: nameof(SBP));
                b.SerializePadding(2);
                SBW = b.SerializeBits<ushort>(SBW, 6, name: nameof(SBW));
                b.SerializePadding(2);
                SPSM = b.SerializeBits<GS.PixelStorageMode>(SPSM, 6, name: nameof(SPSM));
                b.SerializePadding(2);
                DBP = b.SerializeBits<ushort>(DBP, 14, name: nameof(DBP));
                b.SerializePadding(2);
                DBW = b.SerializeBits<ushort>(DBW, 6, name: nameof(DBW));
                b.SerializePadding(2);
                DPSM = b.SerializeBits<GS.PixelStorageMode>(DPSM, 6, name: nameof(DPSM));
                b.SerializePadding(2);
            });
        }
    }
}