namespace BinarySerializer.PlayStation.PS1
{
    public class TMD_Color : BinarySerializable
    {
        // Note: The color values have a range between 0-2 rather than 0-1
        public RGB888Color Color { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Color = s.SerializeObject<RGB888Color>(Color, name: nameof(Color));
            s.SerializePadding(1);
        }
    }
}