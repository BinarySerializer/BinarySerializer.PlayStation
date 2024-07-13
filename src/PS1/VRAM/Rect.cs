namespace BinarySerializer.PlayStation.PS1
{
    /// <summary>
    /// Used by several Psyq library functions to specify a rectangular area of the frame buffer. Neither negative values,
    /// nor values exceeding the size of the frame buffer (1024x512), may be specified.
    /// </summary>
    public class Rect : BinarySerializable, ISerializerShortLog
    {
        public Rect() { }

        public Rect(short xPos, short yPos, short width, short height)
        {
            XPos = xPos;
            YPos = yPos;
            Width = width;
            Height = height;
        }

        public short XPos { get; set; }
        public short YPos { get; set; }
        public short Width { get; set; }
        public short Height { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));
            Width = s.Serialize<short>(Width, name: nameof(Width));
            Height = s.Serialize<short>(Height, name: nameof(Height));
        }

        public string ShortLog => ToString();
        public override string ToString() => $"(x: {XPos}, y:{YPos}, w:{Width}, h:{Height})";
    }
}