namespace BinarySerializer.PlayStation.PS1
{
    /// <summary>
    /// A memory mapped file with the option to allow unmapped developer pointers
    /// </summary>
    public class MemoryMappedPS1File : MemoryMappedFile 
    {
        public MemoryMappedPS1File(
            Context context, 
            string filePath, 
            uint baseAddress, 
            InvalidPointerMode currentInvalidPointerMode, 
            Endian? endianness = null, 
            long? fileLength = null) 
            : base(context, filePath, baseAddress, endianness, fileLength)
        {
            CurrentInvalidPointerMode = currentInvalidPointerMode;
        }

        public InvalidPointerMode CurrentInvalidPointerMode { get; }

		private static bool CheckIfDevPointer(uint serializedValue, Pointer anchor = null) 
        {
			var anchorOffset = anchor?.AbsoluteOffset ?? 0;
			var offset = serializedValue + anchorOffset;
			offset ^= 0xFFFFFFFF;
			return offset is >= 0x80000000 and < 0x807FFFFF;
		}

		public override bool AllowInvalidPointer(long serializedValue, Pointer anchor = null) => CurrentInvalidPointerMode switch
        {
            InvalidPointerMode.DevPointerXOR => CheckIfDevPointer((uint)serializedValue, anchor: anchor),
            InvalidPointerMode.Allow => true,
            _ => true
        };

        public enum InvalidPointerMode 
        {
			DevPointerXOR,
			Allow
		}
    }
}