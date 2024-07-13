namespace BinarySerializer.PlayStation.PS2
{
    /// <summary>
    /// A primitive
    /// </summary>
    public class PRIM
    {
        /// <summary>
        /// Defines the type of primitive
        /// </summary>
        public PRIM_PrimitiveType PrimitiveType { get; set; }

        /// <summary>
        /// Defines the shading
        /// </summary>
        public PRIM_IIP IIP { get; set; }

        /// <summary>
        /// Indicates if texture mapping is enabled
        /// </summary>
        public bool TME { get; set; }

        /// <summary>
        /// Indicates if fog is enabled
        /// </summary>
        public bool FGE { get; set; }

        /// <summary>
        /// Indicates if alpha blending is enabled
        /// </summary>
        public bool ABE { get; set; }

        /// <summary>
        /// Indicates if anti-aliasing is enabled
        /// </summary>
        public bool AA1 { get; set; }

        /// <summary>
        /// Defines the texture coordinates format
        /// </summary>
        public PRIM_FST FST { get; set; }

        /// <summary>
        /// Defines the drawing register to use
        /// </summary>
        public PRIM_DrawingRegister CTXT { get; set; }

        /// <summary>
        /// Indicates if it is fixed
        /// </summary>
        public bool FIX { get; set; }

        public void SerializeImpl(BitSerializerObject b)
        {
            PrimitiveType = b.SerializeBits<PRIM_PrimitiveType>(PrimitiveType, 3, name: nameof(PrimitiveType));
            IIP = b.SerializeBits<PRIM_IIP>(IIP, 1, name: nameof(IIP));
            TME = b.SerializeBits<bool>(TME, 1, name: nameof(TME));
            FGE = b.SerializeBits<bool>(FGE, 1, name: nameof(FGE));
            ABE = b.SerializeBits<bool>(ABE, 1, name: nameof(ABE));
            AA1 = b.SerializeBits<bool>(AA1, 1, name: nameof(AA1));
            FST = b.SerializeBits<PRIM_FST>(FST, 1, name: nameof(FST));
            CTXT = b.SerializeBits<PRIM_DrawingRegister>(CTXT, 1, name: nameof(CTXT));
            FIX = b.SerializeBits<bool>(FIX, 1, name: nameof(FIX));
        }
    }
}