namespace BinarySerializer.PlayStation.PS2
{
    public class GSReg_TEXFLUSH : GSRegister
    {
        public override GSRegisters RegisterByte => GSRegisters.TEXFLUSH;

        public override void SerializeRegisterImpl(SerializerObject s)
        {
            s.DoBits<long>(b =>
            {
                b.SerializePadding(64);
            });
        }
    }
}