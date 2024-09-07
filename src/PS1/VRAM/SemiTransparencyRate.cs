namespace BinarySerializer.PlayStation.PS1
{
    public enum SemiTransparencyRate : byte
    {
        Mode0 = 0, // 0.5 x Back + 0.5  x Forward
        Mode1 = 1, // 1.0 x Back + 1.0  x Forward
        Mode2 = 2, // 1.0 x Back - 1.0  x Forward
        Mode3 = 3, // 1.0 x Back + 0.25 x Forward
    }
}