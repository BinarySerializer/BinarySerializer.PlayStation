using System;
using System.IO;
using System.Linq;

namespace BinarySerializer.PlayStation.PS2
{
    public class VIF_Parser
    {
        protected MemoryStream CurrentStream { get; set; }
        protected Writer Writer { get; set; }

        // Settings
        public bool IsVIF1 { get; set; } = false;
        public bool CountMaskedBytes { get; set; } = false;

        // Registers
        public bool DBF { get; set; }
        public uint MODE { get; set; }
        public uint TOPS { get; set; }
        public uint ITOPS { get; set; }
        public uint TOP { get; set; }
        public uint BASE { get; set; }
        public uint OFST { get; set; }
        public byte CYCLE_CycleLength { get; set; } = 1; // CL
        public byte CYCLE_WriteLength { get; set; } = 1; // WL
        public uint[] ROW { get; set; } = new uint[4];
        public uint[] COL { get; set; } = new uint[4];
        public uint MASK { get; set; }

        public bool IsExecutingMicroProgram { get; set; } = false;
        public bool HasPendingChanges { get; set; } = false;

        public uint ExpectedUnpackDataSize { get; set; }

        public int MemorySize => IsVIF1 ? 0x4000 : 0x1000;

        public byte[] GetCurrentBuffer() => CurrentStream?.GetBuffer();

        public bool StartsNewMicroProgram(VIF_Command command)
        {
            switch (command.VIFCode.CMD)
            {
                case VIFcode.Command.MSCAL:
                case VIFcode.Command.MSCALF:
                case VIFcode.Command.MSCNT:
                    return true;
                default:
                    return false;
            }
        }

        // Based on https://github.com/PCSX2/pcsx2/blob/943b513a525a819c96ec83ed7a505d95633c2255/pcsx2/Vif_Codes.cpp
        // Based on https://psi-rockin.github.io/ps2tek/#vifcommands
        public void ExecuteCommand(VIF_Command command, bool executeFull)
        {
            if (command.VIFCode.IsUnpack)
            {
                ExecuteUnpackCommand(command, executeFull);
                return;
            }

            switch (command.VIFCode.CMD)
            {
                case VIFcode.Command.STMOD:
                    MODE = (uint)BitHelpers.ExtractBits64(command.VIFCode.IMMEDIATE, 2, 0);
                    break;
                case VIFcode.Command.BASE:
                    BASE = (uint)BitHelpers.ExtractBits64(command.VIFCode.IMMEDIATE, 10, 0);
                    break;
                case VIFcode.Command.OFFSET:
                    OFST = (uint)BitHelpers.ExtractBits64(command.VIFCode.IMMEDIATE, 10, 0);
                    DBF = false;
                    TOPS = BASE;
                    break;
                case VIFcode.Command.ITOP:
                    ITOPS = (uint)BitHelpers.ExtractBits64(command.VIFCode.IMMEDIATE, 10, 0);
                    break;
                case VIFcode.Command.STCYCL:
                    CYCLE_CycleLength = (byte)BitHelpers.ExtractBits64(command.VIFCode.IMMEDIATE, 8, 0);
                    CYCLE_WriteLength = (byte)BitHelpers.ExtractBits64(command.VIFCode.IMMEDIATE, 8, 8);
                    break;
                case VIFcode.Command.STROW:
                    Array.Copy(command.ROW, ROW, 4);
                    break;
                case VIFcode.Command.STCOL:
                    Array.Copy(command.COL, COL, 4);
                    break;
                case VIFcode.Command.STMASK:
                    MASK = command.MASK;
                    break;
                case VIFcode.Command.MSCAL:
                case VIFcode.Command.MSCALF:
                case VIFcode.Command.MSCNT:
                    ExecuteMicroProgram();
                    break;
                case VIFcode.Command.NOP:
                case VIFcode.Command.FLUSHE:
                case VIFcode.Command.FLUSH:
                case VIFcode.Command.FLUSHA:
                case VIFcode.Command.DIRECT:
                case VIFcode.Command.DIRECTHL:
                    break;
                default:
                    throw new Exception($"Unparsed VIF command {command.VIFCode.CMD}");
            }
        }

        public void ExecuteMicroProgram()
        {
            IsExecutingMicroProgram = true;
            HasPendingChanges = false;
            if (IsVIF1)
            {
                TOP = TOPS;
                if (DBF)
                {
                    // DBF is set, so set tops with base, and clear the stat DBF flag
                    TOPS = BASE;
                    DBF = false;
                }
                else
                {
                    // it is not, so set tops with base + offset, and set stat DBF flag
                    TOPS = BASE + OFST;
                    DBF = true;
                }
            }
        }

        public void CreateStream()
        {
            CurrentStream = new MemoryStream(MemorySize);
            Writer = new Writer(CurrentStream, isLittleEndian: true, leaveOpen: true);
        }

        private int CurCL { get; set; } = 0;

        public void ExecuteUnpackCommand(VIF_Command command, bool executeFull)
        {
            VIFcode_Unpack unpack = command.VIFCode.GetUnpack();

            uint wl = CYCLE_WriteLength != 0 ? (uint)CYCLE_WriteLength : 256;
            bool isFill = CYCLE_CycleLength < wl;

            uint sourceAddress = 0;
            uint targetAddress = unpack.ADDR;
            if (IsVIF1 && unpack.FLG) targetAddress += TOPS;
            targetAddress = (uint)((targetAddress << 4) & (IsVIF1 ? 0x3ff0 : 0xff0));

            CurCL = 0;
            var cl = CurCL;

            bool doMask = unpack.M;

            uint count = unpack.VN switch
            {
                VIFcode_Unpack.UnpackVN.S => 1,
                VIFcode_Unpack.UnpackVN.V2 => 2,
                VIFcode_Unpack.UnpackVN.V3 => 3,
                VIFcode_Unpack.UnpackVN.V4 => 4,
                _ => throw new Exception($"Unknown VIF Unpack command for data type {unpack.VN}-{unpack.VL}")
            };
            uint size = unpack.VL switch
            {
                VIFcode_Unpack.UnpackVL.VL_8 => 1,
                VIFcode_Unpack.UnpackVL.VL_16 => 2,
                VIFcode_Unpack.UnpackVL.VL_32 => 4,
                _ => throw new Exception($"Unknown VIF Unpack command for data type {unpack.VN}-{unpack.VL}")
            };
            uint vSize = size * count;

            ExpectedUnpackDataSize = unpack.Count * vSize;
            uint usedBytes = 0;

            // Based on https://github.com/PCSX2/pcsx2/blob/943b513a525a819c96ec83ed7a505d95633c2255/pcsx2/x86/newVif_Unpack.cpp#L231
            // Based on https://github.com/PCSX2/pcsx2/blob/943b513a525a819c96ec83ed7a505d95633c2255/pcsx2/Vif_Unpack.cpp#L37
            for (int i = 0; i < unpack.Count; i++)
            {
                //if(executeFull)
                //UnityEngine.Debug.Log($"{i} - TAR:{targetAddress} - SRC:{sourceAddress} - IsFill:{isFill}");
                // Execute unpack
                uint curUsedBytes = ExecuteSingleUnpack(command, unpack, executeFull, cl, sourceAddress, targetAddress);
                usedBytes += curUsedBytes;
                targetAddress += 16;
                cl++;
                if (isFill)
                {
                    if (cl <= CYCLE_CycleLength)
                    {
                        sourceAddress += CountMaskedBytes ? vSize : curUsedBytes;
                    }
                    else if (cl == CYCLE_WriteLength)
                    {
                        cl = 0;
                    }
                }
                else
                {
                    sourceAddress += CountMaskedBytes ? vSize : curUsedBytes;
                    if (cl >= CYCLE_WriteLength)
                    {
                        targetAddress = (uint)(targetAddress + (CYCLE_CycleLength - CYCLE_WriteLength) * 16);
                        cl = 0;
                    }
                }
            }
            CurCL = cl;
            if (doMask)
                ExpectedUnpackDataSize = usedBytes;
        }

        public uint ExecuteSingleUnpack(VIF_Command command, VIFcode_Unpack unpack, bool executeFull, int cl, uint sourceAddress, uint targetAddress)
        {
            uint sourceDataCount = unpack.VN switch
            {
                VIFcode_Unpack.UnpackVN.S => 1,
                VIFcode_Unpack.UnpackVN.V2 => 2,
                VIFcode_Unpack.UnpackVN.V3 => 3,
                VIFcode_Unpack.UnpackVN.V4 => 4,
                _ => throw new Exception($"Unknown VIF Unpack command for data type {unpack.VN}-{unpack.VL}")
            };
            uint sourceDataSize = unpack.VL switch
            {
                VIFcode_Unpack.UnpackVL.VL_8 => 1,
                VIFcode_Unpack.UnpackVL.VL_16 => 2,
                VIFcode_Unpack.UnpackVL.VL_32 => 4,
                _ => throw new Exception($"Unknown VIF Unpack command for data type {unpack.VN}-{unpack.VL}")
            };

            uint mode = unpack.VL != VIFcode_Unpack.UnpackVL.VL_5 ? MODE : 0;
            bool[] useData = new bool[4];

            for (uint i = 0; i < 4; i++)
            {
                uint currentSourceAddress = sourceAddress;
                currentSourceAddress += (i % sourceDataCount) * sourceDataSize;
                /*if (unpack.VN == VIFcode_Unpack.UnpackVN.V2 && i % 2 == 1) {
                    currentSourceAddress += sourceDataSize;
                } else if (unpack.VN == VIFcode_Unpack.UnpackVN.V3 || unpack.VN == VIFcode_Unpack.UnpackVN.V4) {
                    currentSourceAddress += i * sourceDataSize;
                    if(unpack.VN == VIFcode_Unpack.UnpackVN.V3 && i == 3) currentSourceAddress -= sourceDataSize;
                }*/
                if (WriteXYZW(command, unpack, executeFull, cl, mode, sourceDataSize, i, currentSourceAddress, targetAddress + i * 4))
                {
                    switch (unpack.VN)
                    {
                        case VIFcode_Unpack.UnpackVN.S:
                            useData[0] = true;
                            break;
                        case VIFcode_Unpack.UnpackVN.V2:
                            useData[i % 2] = true;
                            break;
                        case VIFcode_Unpack.UnpackVN.V3:
                            if (i < 3) useData[i] = true;
                            break;
                        case VIFcode_Unpack.UnpackVN.V4:
                            useData[i] = true;
                            break;
                    }
                }
            }
            return (uint)useData.Count(u => u) * sourceDataSize;
        }
        private uint SetVifRow(uint index, uint data)
        {
            ROW[index] = data;
            return data;
        }

        public bool WriteXYZW(VIF_Command command, VIFcode_Unpack unpack, bool executeFull, int cl, uint mode, uint sourceDataSize, uint offnum, uint sourceAddress, uint targetAddress)
        {
            bool doMask = unpack.M;
            int maskMode = 0;
            var col = Math.Min(cl, 3);
            if (doMask)
            {
                maskMode = (int)BitHelpers.ExtractBits64(MASK, 2, col * 8 + (int)offnum * 2);
            }
            bool useData = maskMode == 0;
            HasPendingChanges = true;
            if (executeFull)
            {
                if (Writer == null) CreateStream();
                Writer.BaseStream.Position = targetAddress;
                switch (maskMode)
                {
                    case 0: // 0 - Data
                        byte[] dataBytes = new byte[4];
                        //UnityEngine.Debug.Log($"{command.UnpackData.Length} - {sourceAddress} - {sourceDataSize}");
                        Array.Copy(
                            command.UnpackData, sourceAddress,
                            dataBytes, 0,
                            sourceDataSize);
                        if (sourceDataSize < 4 && !unpack.USN)
                        {
                            // Sign extension
                            byte lastByte = dataBytes[sourceDataSize - 1];
                            bool hasSign = BitHelpers.ExtractBits(lastByte, 1, 7) == 1;
                            if (hasSign)
                            {
                                for (int i = (int)sourceDataSize; i < 4; i++) dataBytes[i] = 0xFF;
                            }
                        }
                        uint dataUInt = BitConverter.ToUInt32(dataBytes, 0);
                        switch (mode)
                        {
                            case 1: // Add
                                Writer.Write(dataUInt + ROW[offnum]);
                                break;
                            case 2: // AddRow
                                Writer.Write(SetVifRow(offnum, dataUInt + ROW[offnum]));
                                break;
                            case 3: // Row
                                Writer.Write(SetVifRow(offnum, dataUInt));
                                break;
                            default: // Direct (0)
                                Writer.Write(dataUInt);
                                break;
                        }
                        break;
                    case 1: // 1 - MaskRow
                        Writer.Write(ROW[offnum]);
                        break;
                    case 2: // 2 - MaskCol
                        Writer.Write(COL[col]);
                        break;
                    case 3: // 3 - Write protect
                            // Do nothing
                        break;
                }
            }
            return useData;
        }
    }
}