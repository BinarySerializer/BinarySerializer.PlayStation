namespace BinarySerializer.PlayStation.PS2
{
    public class Vu0IVECTOR : BinarySerializable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int W { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<int>(X, name: nameof(X));
            Y = s.Serialize<int>(Y, name: nameof(Y));
            Z = s.Serialize<int>(Z, name: nameof(Z));
            W = s.Serialize<int>(W, name: nameof(W));
        }
    }
}