namespace BinarySerializer.PlayStation.PS1
{
    public class TMD_Vertex : BinarySerializable, ISerializerShortLog
    {
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<short>(X, name: nameof(X));
            Y = s.Serialize<short>(Y, name: nameof(Y));
            Z = s.Serialize<short>(Z, name: nameof(Z));
            s.SerializePadding(2, logIfNotNull: true);
        }

        public string ShortLog => ToString();
        public override string ToString() => $"Vertex({X}, {Y}, {Z})";
    }
}