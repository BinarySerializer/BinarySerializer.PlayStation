namespace BinarySerializer.PlayStation.PS2
{
    public class GIF_Command : BinarySerializable
    {
        public GIFtag GIFTag { get; set; }

        public BinarySerializable[][] RegisterData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            GIFTag = s.SerializeObject<GIFtag>(GIFTag, name: nameof(GIFTag));
            if (GIFTag.FLG == GIF_FLG.PACKED)
            {
                // Initialize arrays
                if (RegisterData == null) RegisterData = new BinarySerializable[GIFTag.REGS.Length][];
                for (int i = 0; i < RegisterData.Length; i++)
                {
                    if (RegisterData[i] == null) RegisterData[i] = new BinarySerializable[GIFTag.NLOOP];
                }

                for (int l = 0; l < GIFTag.NLOOP; l++)
                {
                    for (int r = 0; r < RegisterData.Length; r++)
                    {
                        T SerializeRegisterData<T>() where T : BinarySerializable, new()
                        {
                            return s.SerializeObject<T>((T)RegisterData[r][l], name: $"{nameof(RegisterData)}[{r}][{l}]");
                        }
                        RegisterData[r][l] = GIFTag.REGS[r] switch
                        {
                            GIFtag.Register.ST => SerializeRegisterData<GIF_Packed_STQ>(),
                            GIFtag.Register.RGBAQ => SerializeRegisterData<GIF_Packed_RGBA>(),
                            GIFtag.Register.XYZF2 => SerializeRegisterData<GIF_Packed_XYZF2>(),
                            _ => throw new BinarySerializableException(this, $"Unsupported Register Type {GIFTag.REGS[r]}")
                        };
                    }
                }
            }
            else
            {
                throw new BinarySerializableException(this, $"Unsupported FLG {GIFTag.FLG}");
            }
        }
    }
}