namespace BinarySerializer.PlayStation.PS1
{
    public class TMD_Normal : BinarySerializable
    {
        public FixedPointInt16 X { get; set; }
        public FixedPointInt16 Y { get; set; }
        public FixedPointInt16 Z { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.SerializeObject<FixedPointInt16>(X, x => x.Pre_PointPosition = 12, name: nameof(X));
            Y = s.SerializeObject<FixedPointInt16>(Y, x => x.Pre_PointPosition = 12, name: nameof(Y));
            Z = s.SerializeObject<FixedPointInt16>(Z, x => x.Pre_PointPosition = 12, name: nameof(Z));
            s.SerializePadding(2, logIfNotNull: true);
        }
    }
}