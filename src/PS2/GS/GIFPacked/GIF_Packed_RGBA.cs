namespace BinarySerializer.PlayStation.PS2
{
    public class GIF_Packed_RGBA : BaseBytewiseRGBAColor
    {

        public GIF_Packed_RGBA() { }
        public GIF_Packed_RGBA(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }

        public override float Red
        {
            get => R / 255f;
            set => R = (byte)(value * 255);
        }
        public override float Green
        {
            get => G / 255f;
            set => G = (byte)(value * 255);
        }
        public override float Blue
        {
            get => B / 255f;
            set => B = (byte)(value * 255);
        }
        public override float Alpha
        {
            get => A / 255f;
            set => A = (byte)(value * 255);
        }

        public override void SerializeImpl(SerializerObject s)
        {
            R = s.Serialize<byte>(R, name: nameof(R));
            s.SerializePadding(3);
            G = s.Serialize<byte>(G, name: nameof(G));
            s.SerializePadding(3);
            B = s.Serialize<byte>(B, name: nameof(B));
            s.SerializePadding(3);
            A = s.Serialize<byte>(A, name: nameof(A));
            s.SerializePadding(3);
        }
    }
}