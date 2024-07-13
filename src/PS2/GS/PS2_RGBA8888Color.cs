namespace BinarySerializer.PlayStation.PS2
{
    /// <summary>
    /// RGBA8888 color wrapper for PS2. Max alpha value is 128 (0x80) rather than 255.
    /// </summary>
    public class PS2_RGBA8888Color : RGBA8888Color
    {
        public override float Alpha
        {
            get => A / 128f;
            set => A = (byte)(value * 128);
        }
    }
}