namespace BinarySerializer.PlayStation.PS2.MemoryCard
{
    public class SuperBlock : BinarySerializable
    {
        public string Version { get; set; }
        public short PageSize { get; set; }
        public short PagesPerCluster { get; set; }
        public short PagesPerBlock { get; set; }
        public byte[] Reserved1 { get; set; } // 00 FF
        public int ClustersCount { get; set; }
        public int AllocationStart { get; set; }
        public int AllocationEnd { get; set; }
        public int ClusterRootDirectory { get; set; }
        public int BackupBlock1 { get; set; }
        public int BackupBlock2 { get; set; }
        public byte[] Reserved2 { get; set; }
        public int[] IndirectFatClusters { get; set; }
        public int[] BadBlocks { get; set; }
        public byte CardType { get; set; }
        public MemoryCardFlags CardFlags { get; set; }
        public byte[] Reserved3 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeMagicString("Sony PS2 Memory Card Format ", 28);
            Version = s.SerializeString(Version, length: 12, name: nameof(Version));
            PageSize = s.Serialize<short>(PageSize, name: nameof(PageSize));
            PagesPerCluster = s.Serialize<short>(PagesPerCluster, name: nameof(PagesPerCluster));
            PagesPerBlock = s.Serialize<short>(PagesPerBlock, name: nameof(PagesPerBlock));
            Reserved1 = s.SerializeArray<byte>(Reserved1, 2, name: nameof(Reserved1));
            ClustersCount = s.Serialize<int>(ClustersCount, name: nameof(ClustersCount));
            AllocationStart = s.Serialize<int>(AllocationStart, name: nameof(AllocationStart));
            AllocationEnd = s.Serialize<int>(AllocationEnd, name: nameof(AllocationEnd));
            ClusterRootDirectory = s.Serialize<int>(ClusterRootDirectory, name: nameof(ClusterRootDirectory));
            BackupBlock1 = s.Serialize<int>(BackupBlock1, name: nameof(BackupBlock1));
            BackupBlock2 = s.Serialize<int>(BackupBlock2, name: nameof(BackupBlock2));
            Reserved2 = s.SerializeArray<byte>(Reserved2, 8, name: nameof(Reserved2));
            IndirectFatClusters = s.SerializeArray<int>(IndirectFatClusters, 32, name: nameof(IndirectFatClusters));
            BadBlocks = s.SerializeArray<int>(BadBlocks, 32, name: nameof(BadBlocks));
            CardType = s.Serialize<byte>(CardType, name: nameof(CardType));
            CardFlags = s.Serialize<MemoryCardFlags>(CardFlags, name: nameof(CardFlags));
            Reserved3 = s.SerializeArray<byte>(Reserved3, 190, name: nameof(Reserved3));
        }
    }
}