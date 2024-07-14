namespace BinarySerializer.PlayStation.PS2.MemoryCard
{
    public class DirectoryEntry : BinarySerializable
    {
        public MemoryCard Pre_MemoryCard { get; set; }

        public DirectoryFlags DirectoryFlags { get; set; }
        public byte[] Reserved1 { get; set; }
        public int Length { get; set; }
        public TimeStamp CreationTime { get; set; }
        public int Cluster { get; set; }
        public int DirEntry { get; set; }
        public TimeStamp ModificationTime { get; set; }
        public uint Attributes { get; set; }
        public byte[] Reserved2 { get; set; }
        public string Name { get; set; }
        public byte[] Reserved3 { get; set; }

        public DirectoryEntry[] SubDirectories { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            DirectoryFlags = s.Serialize<DirectoryFlags>(DirectoryFlags, name: nameof(DirectoryFlags));
            Reserved1 = s.SerializeArray<byte>(Reserved1, 2, name: nameof(Reserved1));
            Length = s.Serialize<int>(Length, name: nameof(Length));
            CreationTime = s.SerializeObject<TimeStamp>(CreationTime, name: nameof(CreationTime));
            Cluster = s.Serialize<int>(Cluster, name: nameof(Cluster));
            DirEntry = s.Serialize<int>(DirEntry, name: nameof(DirEntry));
            ModificationTime = s.SerializeObject<TimeStamp>(ModificationTime, name: nameof(ModificationTime));
            Attributes = s.Serialize<uint>(Attributes, name: nameof(Attributes));
            Reserved2 = s.SerializeArray<byte>(Reserved2, 28, name: nameof(Reserved2));
            Name = s.SerializeString(Name, length: 32, name: nameof(Name));
            Reserved3 = s.SerializeArray<byte>(Reserved3, 416 + 16, name: nameof(Reserved3));

            // Serialize sub-directories
            if ((DirectoryFlags & DirectoryFlags.Directory) != 0 && 
                (DirectoryFlags & DirectoryFlags.Exists) != 0 &&
                Length != 0)
            {
                Pointer p = s.CurrentPointer;

                SubDirectories = s.InitializeArray(SubDirectories, Length);

                int cluster = Cluster;
                int dirsPerCluster = Pre_MemoryCard.SuperBlock.PagesPerCluster; // One dir entry per page

                s.DoArray(SubDirectories, (obj, i, name) =>
                {
                    // TODO: This won't work for writing
                    if (i % dirsPerCluster == 0 && i != 0)
                    {
                        int indirectFatEntriesPerCluster = Pre_MemoryCard.ClusterSize / 4;
                        int fatOffset = cluster % indirectFatEntriesPerCluster;
                        int indirectIndex = cluster / indirectFatEntriesPerCluster;
                        int indirectOffset = indirectIndex % indirectFatEntriesPerCluster;
                        int dblIndirectIndex = indirectIndex / indirectFatEntriesPerCluster;
                        int indirectClusterIndex = Pre_MemoryCard.SuperBlock.IndirectFatClusters[dblIndirectIndex];

                        int fatClusterOffset = indirectClusterIndex * Pre_MemoryCard.ClusterSize + indirectOffset * 4;
                        s.Goto(Pre_MemoryCard.Offset + fatClusterOffset);
                        int fatClusterIndex = s.Serialize<int>(default, "FatClusterIndex");
                        
                        int fatEntryOffset = fatClusterIndex * Pre_MemoryCard.ClusterSize + fatOffset * 4;
                        s.Goto(Pre_MemoryCard.Offset + fatEntryOffset);
                        int fatEntry = s.Serialize<int>(default, "FatEntry");

                        cluster = fatEntry & 0x7FFFFFFF;
                    }

                    s.Goto(Pre_MemoryCard.GetPointer(cluster, true) + (i % dirsPerCluster) * Pre_MemoryCard.PageSize);

                    // Itself, "."
                    if (s.CurrentPointer == Offset)
                        return this;

                    return s.SerializeObject<DirectoryEntry>(obj, x => x.Pre_MemoryCard = Pre_MemoryCard, name: name);
                }, name: nameof(SubDirectories));

                s.Goto(p);
            }
        }
    }
}