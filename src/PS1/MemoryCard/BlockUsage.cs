namespace BinarySerializer.PlayStation.PS1.MemoryCard
{
    public enum BlockUsage : byte
    {
        Unused = 0x0,
        NoLink = 0x1,
        MidLink = 0x2,
        TerminatingLink = 0x3,
        Unusable = 0xF,
    }
}