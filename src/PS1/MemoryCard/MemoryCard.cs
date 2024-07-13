namespace BinarySerializer.PlayStation.PS1.MemoryCard
{
    public class MemoryCard : BinarySerializable
    {
        public const int FrameSize = 0x80;
        public const int FramesPerBlock = 0x40;
        public const int BlockSize = FrameSize * FramesPerBlock;
        public const int BlocksCount = 0x10;

        /// <summary>
        /// The first block on the card, defining header information
        /// </summary>
        public HeaderBlock HeaderBlock { get; set; }

        /// <summary>
        /// Gets the pointer for a block and frame
        /// </summary>
        /// <param name="block">The block (0-15)</param>
        /// <param name="frame">The frame within the block (0-63)</param>
        /// <returns>The pointer to the block and frame</returns>
        public Pointer GetPointer(int block, int frame)
        {
            return Offset + block * BlockSize + frame * FrameSize;
        }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoAt(GetPointer(0, 0),
                () => HeaderBlock = s.SerializeObject<HeaderBlock>(HeaderBlock, name: nameof(HeaderBlock)));
        }
    }
}