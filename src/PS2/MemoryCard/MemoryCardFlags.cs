using System;

namespace BinarySerializer.PlayStation.PS2.MemoryCard
{
    [Flags]
    public enum MemoryCardFlags : byte
    {
        None = 0,
        ECC = 1 << 0,
        Flag_1 = 1 << 1,
        Flag_2 = 1 << 2,
        BadBlock = 1 << 3,
        ErasedStateZeroed = 1 << 4,
        Flag_5 = 1 << 5,
        Flag_6 = 1 << 6,
        Flag_7 = 1 << 7,
    }
}