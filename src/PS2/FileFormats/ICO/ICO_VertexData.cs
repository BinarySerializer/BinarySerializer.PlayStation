namespace BinarySerializer.PlayStation.PS2
{
    public class ICO_VertexData : BinarySerializable
    {
        public uint Pre_FrameCount { get; set; }

        /// <summary>
        /// Icon vertex positions for each frame in the icon animation
        /// </summary>
        public ICO_Vector[] Vertices { get; set; }

        /// <summary>
        /// Normal vector of this vertex
        /// </summary>
        public ICO_Vector Normal { get; set; }

        /// <summary>
        /// U value for texture mapping
        /// </summary>
        public short U { get; set; }

        /// <summary>
        /// V value for texture mapping
        /// </summary>
        public short V { get; set; }

        /// <summary>
        /// Color of this vertex
        /// </summary>
        public RGBA8888Color Color { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Vertices = s.SerializeObjectArray<ICO_Vector>(Vertices, Pre_FrameCount, name: nameof(Vertices));
            U = s.Serialize<short>(U, name: nameof(U));
            V = s.Serialize<short>(V, name: nameof(V));
            Normal = s.SerializeObject<ICO_Vector>(Normal, name: nameof(Normal));
            Color = s.SerializeObject<RGBA8888Color>(Color, name: nameof(Color));
        }
    }
}