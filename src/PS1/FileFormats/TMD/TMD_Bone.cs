namespace BinarySerializer.PlayStation.PS1
{
    /// <summary>
    /// Undocumented, but used in Klonoa
    /// </summary>
    public class TMD_Bone : BinarySerializable
    {
        public bool Pre_HasColorTable { get; set; }
        public bool Pre_HasBonePositions { get; set; }

        public int VerticesIndex { get; set; }
        public int VerticesCount { get; set; }
        public int NormalsIndex { get; set; }
        public int NormalsCount { get; set; }
        public int ColorsIndex { get; set; }
        public int ColorsCount { get; set; }
        public int ParentIndex { get; set; } // Index - 1, 0 is no parent
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int ZPos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            VerticesIndex = s.Serialize<int>(VerticesIndex, name: nameof(VerticesIndex));
            VerticesCount = s.Serialize<int>(VerticesCount, name: nameof(VerticesCount));
            NormalsIndex = s.Serialize<int>(NormalsIndex, name: nameof(NormalsIndex));
            NormalsCount = s.Serialize<int>(NormalsCount, name: nameof(NormalsCount));

            if (Pre_HasColorTable)
            {
                ColorsIndex = s.Serialize<int>(ColorsIndex, name: nameof(ColorsIndex));
                ColorsCount = s.Serialize<int>(ColorsCount, name: nameof(ColorsCount));
            }

            ParentIndex = s.Serialize<int>(ParentIndex, name: nameof(ParentIndex));

            if (Pre_HasBonePositions)
            {
                XPos = s.Serialize<int>(XPos, name: nameof(XPos));
                YPos = s.Serialize<int>(YPos, name: nameof(YPos));
                ZPos = s.Serialize<int>(ZPos, name: nameof(ZPos));
            }
        }
    }
}