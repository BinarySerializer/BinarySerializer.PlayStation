namespace BinarySerializer.PlayStation.PS2
{
    public class ICO_File : BinarySerializable
    {
        /// <summary>
        /// Should always be 0x00010000
        /// </summary>
        public uint ID { get; set; }

        /// <summary>
        /// Number of frames in the icon's animation
        /// </summary>
        public uint FrameCount { get; set; }

        /// <summary>
        /// Defines the shading
        /// </summary>
        public PRIM_IIP IIP { get; set; }

        /// <summary>
        /// Indicates if anti-aliasing is enabled
        /// </summary>
        public bool AA1 { get; set; }

        /// <summary>
        /// Indicates if the icon has a texture
        /// </summary>
        public bool TEX { get; set; }

        /// <summary>
        /// Indicates if the icon's texture is RLE-compressed
        /// </summary>
        public bool RLE { get; set; }

        /// <summary>
        /// Should always be 1.0
        /// </summary>
        public float Float_0C { get; set; }

        /// <summary>
        /// Number of vertices in the icon's model
        /// </summary>
        public uint VertexCount { get; set; }

        /// <summary>
        /// Positions, normals, UVs, and colors for each vertex
        /// </summary>
        public ICO_VertexData[] VertexData { get; set; }

        /// <summary>
        /// Defines keyframes and other properties for the model's animation
        /// </summary>
        public ICO_Animation Animation { get; set; }

        /// <summary>
        /// 128x128 texture
        /// </summary>
        public RGBA5551Color[] Texture { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ID = s.Serialize<uint>(ID, name: nameof(ID));
            FrameCount = s.Serialize<uint>(FrameCount, name: nameof(FrameCount));
            s.DoBits<uint>((b) =>
            {
                IIP = b.SerializeBits<PRIM_IIP>(IIP, 1, name: nameof(IIP));
                AA1 = b.SerializeBits<bool>(AA1, 1, name: nameof(AA1));
                TEX = b.SerializeBits<bool>(TEX, 1, name: nameof(TEX));
                RLE = b.SerializeBits<bool>(RLE, 1, name: nameof(RLE));
            });
            Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
            VertexCount = s.Serialize<uint>(VertexCount, name: nameof(VertexCount));
            VertexData = s.SerializeObjectArray<ICO_VertexData>(VertexData, VertexCount, onPreSerialize: v => v.Pre_FrameCount = FrameCount, name: nameof(VertexData));
            Animation = s.SerializeObject<ICO_Animation>(Animation, name: nameof(Animation));
            
            if (TEX)
                Texture = s.SerializeObjectArray<RGBA5551Color>(Texture, 0x4000, name: nameof(Texture));
        }
    }
}
