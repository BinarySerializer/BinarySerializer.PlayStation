namespace BinarySerializer.PlayStation.PS2
{
    public class GIF_Packed_STQ : BinarySerializable
    {
        public float S { get; set; }
        public float T { get; set; }
        public float Q { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            S = s.Serialize<float>(S, name: nameof(S));
            T = s.Serialize<float>(T, name: nameof(T));
            Q = s.Serialize<float>(Q, name: nameof(Q));
            s.SerializePadding(4);
        }
    }
}