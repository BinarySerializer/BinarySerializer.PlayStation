namespace BinarySerializer.PlayStation.PS2
{
    /// <see href=https://www.ps2savetools.com/documents/iconsys-format/>icon.sys format</see>
    public class IconSys_File : BinarySerializable
    {
        public ushort LinebreakOffset { get; set; } // Offset of where to put a line break in the title
        public uint BackgroundTransparency { get; set; } // 0x00 to 0x80
        public Vu0IVECTOR BackgroundColorUpperLeft { get; set; }
        public Vu0IVECTOR BackgroundColorUpperRight { get; set; }
        public Vu0IVECTOR BackgroundColorLowerLeft { get; set; }
        public Vu0IVECTOR BackgroundColorLowerRight { get; set; }
        public Vu0FVECTOR[] LightDirections { get; set; }
        public Vu0FVECTOR[] LightColors { get; set; }
        public Vu0FVECTOR AmbientColor { get; set; }
        public string Title { get; set; } // Shift JIS encoding; TODO: Shift JIS is not natively supported in .NET
        public string BaseIconName { get; set; }
        public string CopyIconName { get; set; }
        public string DeleteIconName { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeMagicString("PS2D", 4);
            s.SerializePadding(2);
            LinebreakOffset = s.Serialize<ushort>(LinebreakOffset, name: nameof(LinebreakOffset));
            s.SerializePadding(4);
            BackgroundTransparency = s.Serialize<uint>(BackgroundTransparency, name: nameof(BackgroundTransparency));
            BackgroundColorUpperLeft = s.SerializeObject<Vu0IVECTOR>(BackgroundColorUpperLeft, name: nameof(BackgroundColorUpperLeft));
            BackgroundColorUpperRight = s.SerializeObject<Vu0IVECTOR>(BackgroundColorUpperRight, name: nameof(BackgroundColorUpperRight));
            BackgroundColorLowerLeft = s.SerializeObject<Vu0IVECTOR>(BackgroundColorLowerLeft, name: nameof(BackgroundColorLowerLeft));
            BackgroundColorLowerRight = s.SerializeObject<Vu0IVECTOR>(BackgroundColorLowerRight, name: nameof(BackgroundColorLowerRight));
            LightDirections = s.SerializeObjectArray<Vu0FVECTOR>(LightDirections, 3, name: nameof(LightDirections));
            LightColors = s.SerializeObjectArray<Vu0FVECTOR>(LightColors, 3, name: nameof(LightColors));
            AmbientColor = s.SerializeObject<Vu0FVECTOR>(AmbientColor, name: nameof(AmbientColor));
            Title = s.SerializeString(Title, name: nameof(Title));
            s.DoAt(Offset + 0x104, () => BaseIconName = s.SerializeString(BaseIconName, name: nameof(BaseIconName)));
            s.DoAt(Offset + 0x144, () => CopyIconName = s.SerializeString(CopyIconName, name: nameof(CopyIconName)));
            s.DoAt(Offset + 0x184, () => DeleteIconName = s.SerializeString(DeleteIconName, name: nameof(DeleteIconName)));
        }
    }
}