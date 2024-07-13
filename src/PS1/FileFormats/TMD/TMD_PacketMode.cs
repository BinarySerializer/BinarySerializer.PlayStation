namespace BinarySerializer.PlayStation.PS1
{
    public class TMD_PacketMode : BinarySerializable
    {
        /// <summary>
        /// Brightness calculation at time of texture mapping
        /// 0: On
        /// 1: Off (Draws texture as is)
        /// </summary>
        public bool TGE { get; set; }

        /// <summary>
        /// Translucency processing
        /// 0: On
        /// 1: Off (Draws texture as is)
        /// </summary>
        public bool ABE { get; set; }

        /// <summary>
        /// Texture specification
        /// 0: Off
        /// 1: On
        /// </summary>
        public bool TME { get; set; }

        /// <summary>
        /// Indicates if it's quad, otherwise tri
        /// </summary>
        public bool IsQuad { get; set; }

        /// <summary>
        /// Shading mode
        /// 0: Flat shading
        /// 1: Gouraud shading
        /// </summary>
        public bool IIP { get; set; }

        public PacketModeCODE Code { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<byte>(b =>
            {
                TGE = b.SerializeBits<bool>(TGE, 1, name: nameof(TGE));
                ABE = b.SerializeBits<bool>(ABE, 1, name: nameof(ABE));
                TME = b.SerializeBits<bool>(TME, 1, name: nameof(TME));
                IsQuad = b.SerializeBits<bool>(IsQuad, 1, name: nameof(IsQuad));
                IIP = b.SerializeBits<bool>(IIP, 1, name: nameof(IIP));
                Code = b.SerializeBits<PacketModeCODE>(Code, 3, name: nameof(Code));
            });
        }

        public enum PacketModeCODE : byte
        {
            Polygon = 1,
            StraightLine = 2,
            Sprite = 3,
        }
    }
}