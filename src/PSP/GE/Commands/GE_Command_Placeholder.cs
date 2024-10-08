﻿namespace BinarySerializer.PlayStation.PSP
{
    public class GE_Command_Placeholder : GE_CommandData
    {
        public override void SerializeImpl(BitSerializerObject b)
        {
            b.Context.SystemLogger?.LogWarning("{0}: Unparsed RSP Command: {1}", Offset, Pre_Command?.Command);
            b.SerializePadding(3 * 8);
        }
    }
}