#nullable enable
using System.IO;

namespace BinarySerializer.PlayStation.PS2.MemoryCard
{
    public class MemoryCardFile : PhysicalFile
    {
        public MemoryCardFile(
            Context context,
            string filePath,
            Endian? endianness = null,
            long? fileLength = null)
            : base(context, filePath, endianness, fileLength: fileLength) 
        { }
        
        public override bool IsMemoryMapped => false;

        public override Reader CreateReader()
        {
            Stream s = FileManager.GetFileReadStream(SourcePath);
            FileLength = s.Length;
            Reader reader = new(new MemoryCardStream(s), isLittleEndian: Endianness == Endian.Little);
            Context.SystemLogger?.LogTrace("Created reader for file {0}", FilePath);
            return reader;

        }

        public override Writer CreateWriter()
        {
            CreateBackupFile();
            Stream s = FileManager.GetFileWriteStream(DestinationPath, RecreateOnWrite);
            FileLength = s.Length;
            Writer writer = new(new MemoryCardStream(s), isLittleEndian: Endianness == Endian.Little);
            Context.SystemLogger?.LogTrace("Created writer for file {0}", FilePath);
            return writer;
        }
    }
}