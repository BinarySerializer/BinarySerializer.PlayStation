namespace BinarySerializer.PlayStation.PS2
{
    public class ICO_Animation : BinarySerializable
    {
        public uint ID { get; set; }
        public uint FrameLength { get; set; }
        public float Speed { get; set; }
        public uint StartOffset { get; set; }
        public uint FrameCount { get; set; }
        public ICO_AnimationFrameData[] FrameData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ID = s.Serialize<uint>(ID, name: nameof(ID));
            FrameLength = s.Serialize<uint>(FrameLength, name: nameof(FrameLength));
            Speed = s.Serialize<float>(Speed, name: nameof(Speed));
            StartOffset = s.Serialize<uint>(StartOffset, name: nameof(StartOffset));
            FrameCount = s.Serialize<uint>(FrameCount, name: nameof(FrameCount));
            FrameData = s.SerializeObjectArray<ICO_AnimationFrameData>(FrameData, FrameCount, name: nameof(FrameData));
        }

        public class ICO_AnimationFrameData : BinarySerializable
        {
            public uint ShapeID { get; set; }
            public uint KeyframeCount { get; set; }
            public ICO_AnimationKeyframe[] Keyframes { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                ShapeID = s.Serialize<uint>(ShapeID, name: nameof(ShapeID));
                KeyframeCount = s.Serialize<uint>(KeyframeCount, name: nameof(KeyframeCount));
                Keyframes = s.SerializeObjectArray<ICO_AnimationKeyframe>(Keyframes, KeyframeCount, name: nameof(Keyframes));
            }
        }

        public class ICO_AnimationKeyframe : BinarySerializable
        {
            public float Time { get; set; }
            public float Value { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Time = s.Serialize<float>(Time, name: nameof(Time));
                Value = s.Serialize<float>(Value, name: nameof(Value));
            }
        }
    }
}