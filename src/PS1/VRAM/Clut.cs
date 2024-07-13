namespace BinarySerializer.PlayStation.PS1
{
    public class Clut : BinarySerializable
    {
        public RGBA5551Color[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, 256, name: nameof(Palette));
        }
    }
}