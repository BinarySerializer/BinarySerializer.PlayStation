namespace BinarySerializer.PlayStation.PS1
{
    public class TMD_Object : BinarySerializable
    {
        public Pointer Pre_PointerAnchor { get; set; }
        public bool Pre_HasBones { get; set; }
        public bool Pre_HasColorTable { get; set; } // Undocumented, but used in Klonoa
        public bool Pre_HasBonePositions { get; set; }

        public Pointer VerticesPointer { get; set; }
        public uint VerticesCount { get; set; }
        public Pointer NormalsPointer { get; set; }
        public uint NormalsCount { get; set; }
        public Pointer ColorsPointer { get; set; }
        public uint ColorsCount { get; set; }
        public Pointer PrimitivesPointer { get; set; }
        public uint PrimitivesCount { get; set; }
        public int Scale { get; set; } // Raised to the second power
        public int BonesCount { get; set; }
        public TMD_Bone[] Bones { get; set; }

        // Serialized from pointers
        public TMD_Vertex[] Vertices { get; set; }
        public TMD_Normal[] Normals { get; set; }
        public TMD_Color[] Colors { get; set; }
        public TMD_Packet[] Primitives { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            VerticesPointer = s.SerializePointer(VerticesPointer, anchor: Pre_PointerAnchor, name: nameof(VerticesPointer));
            VerticesCount = s.Serialize<uint>(VerticesCount, name: nameof(VerticesCount));
            NormalsPointer = s.SerializePointer(NormalsPointer, anchor: Pre_PointerAnchor, name: nameof(NormalsPointer));
            NormalsCount = s.Serialize<uint>(NormalsCount, name: nameof(NormalsCount));

            if (Pre_HasColorTable)
            {
                ColorsPointer = s.SerializePointer(ColorsPointer, anchor: Pre_PointerAnchor, name: nameof(ColorsPointer));
                ColorsCount = s.Serialize<uint>(ColorsCount, name: nameof(ColorsCount));
            }

            PrimitivesPointer = s.SerializePointer(PrimitivesPointer, anchor: Pre_PointerAnchor, name: nameof(PrimitivesPointer));
            PrimitivesCount = s.Serialize<uint>(PrimitivesCount, name: nameof(PrimitivesCount));

            if (Pre_HasBones)
            {
                BonesCount = s.Serialize<int>(BonesCount, name: nameof(BonesCount));
                Bones = s.SerializeObjectArray<TMD_Bone>(Bones, BonesCount, x =>
                {
                    x.Pre_HasColorTable = Pre_HasColorTable;
                    x.Pre_HasBonePositions = Pre_HasBonePositions;
                }, name: nameof(Bones));
            }
            else
            {
                Scale = s.Serialize<int>(Scale, name: nameof(Scale));
            }

            s.DoAt(VerticesPointer, () => Vertices = s.SerializeObjectArray<TMD_Vertex>(Vertices, VerticesCount, name: nameof(Vertices)));
            s.DoAt(NormalsPointer, () => Normals = s.SerializeObjectArray<TMD_Normal>(Normals, NormalsCount, name: nameof(Normals)));
            s.DoAt(ColorsPointer, () => Colors = s.SerializeObjectArray<TMD_Color>(Colors, ColorsCount, name: nameof(Colors)));
            s.DoAt(PrimitivesPointer, () => Primitives = s.SerializeObjectArray<TMD_Packet>(Primitives, PrimitivesCount, onPreSerialize: x => x.Pre_HasColorTable = Pre_HasColorTable, name: nameof(Primitives)));
        }
    }
}