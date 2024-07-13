namespace BinarySerializer.PlayStation.PS1.Psyq
{
    public class CdlFILE : BinarySerializable
    {
        public bool Pre_HasFileName { get; set; } = true; // Older Psyq versions don't seem to include the name

        /// <summary>
        /// The file position on the disc
        /// </summary>
        public CdlLOC Pos { get; set; }
        
        /// <summary>
        /// The file size
        /// </summary>
        public uint Size { get; set; }
        
        /// <summary>
        /// The file name. This does not contain the directory path.
        /// </summary>
        public string Name { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Pos = s.SerializeObject<CdlLOC>(Pos, name: nameof(Pos));
            Size = s.Serialize<uint>(Size, name: nameof(Size));

            if (Pre_HasFileName)
                Name = s.SerializeString(Name, length: 16, name: nameof(Name));
        }
    }
}