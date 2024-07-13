namespace BinarySerializer.PlayStation.PS2
{
    public class GSReg_TRXPOS : GSRegister
    {
        public override GSRegisters RegisterByte => GSRegisters.TRXPOS;

        public ushort SSAX { get; set; }
        public ushort SSAY { get; set; }
        public ushort DSAX { get; set; }
        public ushort DSAY { get; set; }
        public TransmissionOrder DIR { get; set; }

        public override void SerializeRegisterImpl(SerializerObject s)
        {
            s.DoBits<long>(b =>
            {
                SSAX = b.SerializeBits<ushort>(SSAX, 11, name: nameof(SSAX));
                b.SerializePadding(5);
                SSAY = b.SerializeBits<ushort>(SSAY, 11, name: nameof(SSAY));
                b.SerializePadding(5);
                DSAX = b.SerializeBits<ushort>(DSAX, 11, name: nameof(DSAX));
                b.SerializePadding(5);
                DSAY = b.SerializeBits<ushort>(DSAY, 11, name: nameof(DSAY));
                DIR = b.SerializeBits<TransmissionOrder>(DIR, 2, name: nameof(DIR));
                b.SerializePadding(3);
            });
        }

        public enum TransmissionOrder
        {
            UpperLeft_LowerRight,
            LowerLeft_UpperRight,
            UpperRight_LowerLeft,
            LowerRight_UpperLeft
        }
    }
}