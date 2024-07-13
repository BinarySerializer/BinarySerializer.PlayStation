namespace BinarySerializer.PlayStation.PS1.MemoryCard
{
    public enum BlockUsability : byte
    {
        PartiallyUsed = 0x5,
        Available = 0xA,
        Unusable = 0xF,
    }
}