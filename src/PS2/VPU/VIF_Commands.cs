namespace BinarySerializer.PlayStation.PS2
{
    public class VIF_Commands : BinarySerializable
    {
        public VIF_Command[] Commands { get; set; }

        public VIF_Parser Pre_Parser { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Commands = s.SerializeObjectArrayUntil<VIF_Command>(Commands,
                _ => s.CurrentFileOffset >= s.CurrentLength,
                onPreSerialize: (v,_) => v.Pre_Parser = Pre_Parser, name: nameof(Commands));
        }
    }
}