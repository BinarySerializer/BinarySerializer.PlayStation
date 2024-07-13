namespace BinarySerializer.PlayStation.PS2
{
    /// <see href="https://psi-rockin.github.io/ps2tek/#gsregisterlist">GS register list</see>
    public abstract class GSRegister : BinarySerializable
    {
        /// <summary>
        /// Byte identifier of the register
        /// </summary>
        public abstract GSRegisters RegisterByte { get; }

        /// <summary>
        /// Whether or not to serialize the identifier after the register data bytes (8 bytes)
        /// Used for games that store textures in raw GS transfer data, such as Klonoa 2: Lunatea's Veil and Kingdom Hearts 2
        /// </summary>
        public bool SerializeTag { get; set; }

        public ulong RegisterTag { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            SerializeRegisterImpl(s);
            if (SerializeTag)
                SerializeRegisterTag(s);
        }

        public abstract void SerializeRegisterImpl(SerializerObject s);

        protected void SerializeRegisterTag(SerializerObject s)
        {
            RegisterTag = s.Serialize<ulong>(RegisterTag, name: nameof(RegisterTag));
            if (RegisterTag != (ulong)RegisterByte)
                throw new BinarySerializableException(this, $"Invalid tag {RegisterTag} for register {RegisterByte}");
        }
    }

    public enum GSRegisters : byte
    {
        PRIM = 0x00,
        RGBAQ = 0x01,
        ST = 0x02,
        UV = 0x03,
        XYZF2 = 0x04,
        XYZ2 = 0x05,
        TEX0_1 = 0x06,
        TEX0_2 = 0x07,
        CLAMP_1 = 0x08,
        CLAMP_2 = 0x09,
        FOG = 0x0A,
        XYZF3 = 0x0C,
        XYZ3 = 0x0D,
        TEX1_1 = 0x14,
        TEX1_2 = 0x15,
        TEX2_1 = 0x16,
        TEX2_2 = 0x17,
        XYOFFSET_1 = 0x18,
        XYOFFSET_2 = 0x19,
        PRMODECONT = 0x1A,
        PRMODE = 0x1B,
        TEXCLUT = 0x1C,
        SCANMSK = 0x22,
        MIPTBP1_1 = 0x34,
        MIPTBP1_2 = 0x35,
        MIPTBP2_1 = 0x36,
        MIPTBP2_2 = 0x37,
        TEXA = 0x3B,
        FOGCOL = 0x3D,
        TEXFLUSH = 0x3F,
        SCISSOR_1 = 0x40,
        SCISSOR_2 = 0x41,
        ALPHA_1 = 0x42,
        ALPHA_2 = 0x43,
        DIMX = 0x44,
        DTHE = 0x45,
        COLCLAMP = 0x46,
        TEST_1 = 0x47,
        TEST2 = 0x48,
        PABE = 0x49,
        FBA_1 = 0x4A,
        FBA_2 = 0x4B,
        FRAME_1 = 0x4C,
        FRAME_2 = 0x4D,
        ZBUF_1 = 0x4E,
        ZBUF_2 = 0x4F,
        BITBLTBUF = 0x50,
        TRXPOS = 0x51,
        TRXREG = 0x52,
        TRXDIR = 0x53,
        HWREG = 0x54,
        SIGNAL = 0x60,
        FINISH = 0x61,
        LABEL = 0x62
    }
}