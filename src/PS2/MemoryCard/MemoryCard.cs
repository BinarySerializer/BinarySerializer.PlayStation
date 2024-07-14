namespace BinarySerializer.PlayStation.PS2.MemoryCard
{
    // Useful links:
    // https://www.psdevwiki.com/ps2/Memory_Card
    // http://www.csclub.uwaterloo.ca:11068/mymc/ps2mcfs.html
    // https://github.com/FranciscoDA/ps2mcfs
    // https://github.com/Adubbz/mymcplusplus

    // TODO: The current implementation of this won't allow writing back. We should probably parse all the FAT tables in here before using them.
    public class MemoryCard : BinarySerializable
    {
        public SuperBlock SuperBlock { get; set; }
        public DirectoryEntry RootDirectory { get; set; }

        public int PageSize => SuperBlock.PageSize;
        public int ClusterSize => SuperBlock.PagesPerCluster * PageSize;

        public Pointer GetPointer(int cluster, bool allocated)
        {
            if (allocated)
                cluster += SuperBlock.AllocationStart;

            return Offset + cluster * ClusterSize;
        }

        public override void SerializeImpl(SerializerObject s)
        {
            // Always at the start
            SuperBlock = s.SerializeObject<SuperBlock>(SuperBlock, name: nameof(SuperBlock));

            // Read the root directory
            s.DoAt(GetPointer(SuperBlock.ClusterRootDirectory, true), () =>
                RootDirectory = s.SerializeObject<DirectoryEntry>(RootDirectory, x => x.Pre_MemoryCard = this, name: nameof(RootDirectory)));
        }
    }
}