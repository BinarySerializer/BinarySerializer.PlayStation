namespace BinarySerializer.PlayStation.PS2
{
    /// <see href="https://psi-rockin.github.io/ps2tek/#giftags">GIFtag documentation</see>
    public class GIFtag : BinarySerializable
    {
        /// <summary>
        /// Data per register to transfer
        /// </summary>
        public ushort NLOOP { get; set; }

        /// <summary>
        /// End of packet
        /// </summary>
        public bool EOP { get; set; }

        /// <summary>
        /// Indicates if <see cref="PRIM"/> is enabled
        /// </summary>
        public bool PRE { get; set; }

        /// <summary>
        /// PRIM, only set if <see cref="PRE"/> is true
        /// </summary>
        public PRIM PRIM { get; set; }

        /// <summary>
        /// The data format
        /// </summary>
        public GIF_FLG FLG { get; set; }
        
        /// <summary>
        /// The number of registers
        /// </summary>
        public byte NREG { get; set; }

        /// <summary>
        /// The registers
        /// </summary>
        public Register[] REGS { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<long>(b =>
            {
                NLOOP = b.SerializeBits<ushort>(NLOOP, 15, name: nameof(NLOOP));
                EOP = b.SerializeBits<bool>(EOP, 1, name: nameof(EOP));
                b.SerializePadding(30);
                PRE = b.SerializeBits<bool>(PRE, 1, name: nameof(PRE));

                if (PRE)
                {
                    // TODO: Implement support for serializing object using bit-serializer?
                    PRIM ??= new PRIM();
                    PRIM.SerializeImpl(b);
                }
                else
                {
                    // TODO: Is this always padding or should we save the data?
                    b.SerializePadding(11, logIfNotNull: true);
                }

                FLG = b.SerializeBits<GIF_FLG>(FLG, 2, name: nameof(FLG));
                NREG = b.SerializeBits<byte>(NREG, 4, name: nameof(NREG));
            });
            s.DoBits<long>(b =>
            {
                // If it's 0 then there are 16 registers defined
                int regCount = NREG == 0 ? 16 : NREG;

                REGS = new Register[regCount];
                for (int i = 0; i < regCount; i++)
                    REGS[i] = b.SerializeBits<Register>(REGS[i], 4, name: $"{nameof(REGS)}[{i}]");

                if (64 - regCount * 4 != 0)
                    b.SerializePadding(64 - regCount * 4, logIfNotNull: true);
            });
        }

        public enum Register
        {
            PRIM = 0,
            RGBAQ = 1,
            ST = 2,
            UV = 3,
            XYZF2 = 4,
            XYZ2 = 5,
            TEX0_1 = 6,
            TEX0_2 = 7,
            CLAMP_1 = 8,
            CLAMP_2 = 9,
            FOG = 10,
            RESERVED = 11,
            XYZF3 = 12,
            XYZ3 = 13,
            AD = 14,
            NOP = 15,
        }
    }
}