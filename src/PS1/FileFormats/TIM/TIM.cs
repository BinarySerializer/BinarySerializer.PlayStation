using System;

namespace BinarySerializer.PlayStation.PS1
{
    /// <summary>
    /// Image data file
    /// </summary>
    public class TIM : BinarySerializable
    {
        public byte Version { get; set; }
        public TIM_ColorFormat ColorFormat { get; set; }
        public bool HasClut { get; set; }
        public TIM_Clut Clut { get; set; }
        public int Length { get; set; }
        public Rect Region { get; set; }
        public byte[] ImgData { get; set; }
        public RGBA5551Color[] ImgData_16 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeMagic<byte>(0x10, name: "ID");

            Version = s.Serialize<byte>(Version, name: nameof(Version));

            if (Version != 0)
                throw new Exception($"Invalid version: {Version} - should be 0x00");

            s.SerializePadding(2, logIfNotNull: true);

            s.DoBits<uint>(b =>
            {
                ColorFormat = b.SerializeBits<TIM_ColorFormat>(ColorFormat, 3, name: nameof(ColorFormat));
                HasClut = b.SerializeBits<bool>(HasClut, 1, name: nameof(HasClut));
                b.SerializePadding(28, logIfNotNull: true);
            });

            if (HasClut)
                Clut = s.SerializeObject<TIM_Clut>(Clut, name: nameof(Clut));

            Length = s.Serialize<int>(Length, name: nameof(Length));
            Region = s.SerializeObject<Rect>(Region, name: nameof(Region));

            var imgDataLength = Length - 12; // width * height * 2

            if (Region.Width == 0 || Region.Height == 0)
                imgDataLength = 4;

            if (ColorFormat is TIM_ColorFormat.BPP_4 or TIM_ColorFormat.BPP_8)
                ImgData = s.SerializeArray<byte>(ImgData, imgDataLength, name: nameof(ImgData));
            else if (ColorFormat == TIM_ColorFormat.BPP_16)
                ImgData_16 = s.SerializeObjectArray<RGBA5551Color>(ImgData_16, Region.Width * Region.Height, name: nameof(ImgData_16));
            else
                throw new NotImplementedException("Raw 24-bit image data is currently not supported");
        }

        public enum TIM_ColorFormat : byte
        {
            BPP_4 = 0,
            BPP_8 = 1,
            BPP_16 = 2,
            BPP_24 = 3,
        }

        public class TIM_Clut : BinarySerializable
        {
            public int Length { get; set; }
            public Rect Region { get; set; }
            public RGBA5551Color[] Palette { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Length = s.Serialize<int>(Length, name: nameof(Length));
                Region = s.SerializeObject<Rect>(Region, name: nameof(Region));
                Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, Region.Width * Region.Height, name: nameof(Palette));
            }
        }
    }
}