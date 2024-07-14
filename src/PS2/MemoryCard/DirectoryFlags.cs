using System;

namespace BinarySerializer.PlayStation.PS2.MemoryCard
{
    [Flags]
    public enum DirectoryFlags : ushort
    {
        None = 0,
        Read = 1 << 0,
        Write = 1 << 1,
        Execute = 1 << 2,
        Protected = 1 << 3,
        File = 1 << 4,
        Directory = 1 << 5,
        Flag_6 = 1 << 6,
        Flag_7 = 1 << 7,
        Flag_8 = 1 << 8,
        Flag_9 = 1 << 9,
        Flag_10 = 1 << 10,
        PocketStation = 1 << 11,
        PS1 = 1 << 12,
        Hidden = 1 << 13,
        Flag_14 = 1 << 14,
        Exists = 1 << 15,
    }
}