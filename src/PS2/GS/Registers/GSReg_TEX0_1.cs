namespace BinarySerializer.PlayStation.PS2
{
    public class GSReg_TEX0_1 : GSRegister
    {
        public override GSRegisters RegisterByte => GSRegisters.TEX0_1;

        public ushort TPB0 { get; set; }
        public byte TBW { get; set; }
        public GS.PixelStorageMode PSM { get; set; }
        public byte TW { get; set; }
        public byte TH { get; set; }
        public bool TCC { get; set; }
        public GS.TextureFunction TFX { get; set; }
        public ushort CBP { get; set; }
        public GS.CLUTPixelStorageMode CPSM { get; set; }
        public GS.ColorStorageMode CSM { get; set; }
        public byte CSA { get; set; }
        public byte CLD { get; set; }

        public override void SerializeRegisterImpl(SerializerObject s)
        {
            s.DoBits<long>(b =>
            {
                TPB0 = b.SerializeBits<ushort>(TPB0, 14, name: nameof(TPB0));
                TBW = b.SerializeBits<byte>(TBW, 6, name: nameof(TBW));
                PSM = b.SerializeBits<GS.PixelStorageMode>(PSM, 6, name: nameof(PSM));
                TW = b.SerializeBits<byte>(TW, 4, name: nameof(TW));
                TH = b.SerializeBits<byte>(TH, 4, name: nameof(TH));
                TCC = b.SerializeBits<bool>(TCC, 1, name: nameof(TCC));
                TFX = b.SerializeBits<GS.TextureFunction>(TFX, 2, name: nameof(TFX));
                CBP = b.SerializeBits<ushort>(CBP, 14, name: nameof(CBP));
                CPSM = b.SerializeBits<GS.CLUTPixelStorageMode>(CPSM, 4, name: nameof(CPSM));
                CSM = b.SerializeBits<GS.ColorStorageMode>(CSM, 1, name: nameof(CSM));
                CSA = b.SerializeBits<byte>(CSA, 5, name: nameof(CSA));
                CLD = b.SerializeBits<byte>(CLD, 3, name: nameof(CLD));
            });
        }
    }
}