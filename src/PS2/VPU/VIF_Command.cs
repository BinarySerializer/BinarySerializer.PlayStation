namespace BinarySerializer.PlayStation.PS2
{
    public class VIF_Command : BinarySerializable
    {
        public VIF_Parser Pre_Parser { get; set; }

        public VIFcode VIFCode { get; set; }

        public uint[] ROW { get; set; }
        public uint[] COL { get; set; }
        public uint MASK { get; set; }

        public byte[] DirectData { get; set; }
        public byte[] UnpackData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (Pre_Parser == null)
                throw new MissingPreValueException(this, nameof(Pre_Parser));

            VIFCode = s.SerializeObject<VIFcode>(VIFCode, name: nameof(VIFCode));

            if (VIFCode.IsUnpack)
            {
                // UNPACK command
                VIFcode_Unpack unpack = VIFCode.GetUnpack();

                s.Log("VIF Command: [UNPACK] {0}", unpack);

                Pre_Parser.ExecuteCommand(this, false);
                UnpackData = s.SerializeArray<byte>(UnpackData, Pre_Parser.ExpectedUnpackDataSize, name: nameof(UnpackData));
                s.Align(4, baseOffset: Offset);
            }
            else
            {
                s.Log("VIF Command: [{0}]", VIFCode.CMD);

                switch (VIFCode.CMD)
                {
                    case VIFcode.Command.NOP:
                    case VIFcode.Command.OFFSET:
                    case VIFcode.Command.BASE:
                    case VIFcode.Command.STMOD:
                    case VIFcode.Command.FLUSHE:
                    case VIFcode.Command.FLUSH:
                    case VIFcode.Command.FLUSHA:
                    case VIFcode.Command.MSCAL:
                    case VIFcode.Command.MSCALF:
                    case VIFcode.Command.MSCNT: // Transfer data
                    case VIFcode.Command.STCYCL:
                        break;

                    case VIFcode.Command.STROW:
                        ROW = s.SerializeArray<uint>(ROW, 4, name: nameof(ROW));
                        break;

                    case VIFcode.Command.STCOL:
                        COL = s.SerializeArray<uint>(COL, 4, name: nameof(COL));
                        break;

                    case VIFcode.Command.STMASK:
                        MASK = s.Serialize<uint>(MASK, name: nameof(MASK));
                        break;
                    
                    // Transfers IMMEDIATE quadwords to GIF
                    case VIFcode.Command.DIRECT:
                    case VIFcode.Command.DIRECTHL:
                        DirectData = s.SerializeArray<byte>(DirectData, VIFCode.IMMEDIATE * 16, name: nameof(DirectData));
                        break;

                    default:
                        throw new BinarySerializableException(this, $"Unexpected VIF command: {VIFCode.CMD} ({(int)VIFCode.CMD:X2})");
                }
                Pre_Parser.ExecuteCommand(this, false);
            }
        }
    }
}