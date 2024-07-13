namespace BinarySerializer.PlayStation.PS2
{
    public class VIFcode_Unpack
    {
        public VIFcode_Unpack(VIFcode vifcode)
        {
            M = ((int)vifcode.CMD >> 4 & 0x01) == 1; // Bit 4
            VN = (UnpackVN)(((int)vifcode.CMD >> 2) & 0x03); // Bits 2-3
            VL = (UnpackVL)((int)vifcode.CMD & 0x03); // Bits 0-1
            SIZE = vifcode.NUM;
            ADDR = vifcode.IMMEDIATE & 0x3FF; // Bits 0-9
            USN = ((vifcode.IMMEDIATE >> 14) & 0x01) == 1; // Bit 14
            FLG = ((vifcode.IMMEDIATE >> 15) & 0x01) == 1; // Bit 15
        }

        public bool M { get; set; }
        public UnpackVN VN { get; set; }
        public UnpackVL VL { get; set; }
        public uint SIZE { get; set; }
        public uint ADDR { get; set; }

        public uint Count => SIZE != 0 ? SIZE : 256;

        /// <summary>
        /// False if it's sign-extended. For example if a 16-bit value is unpacked then the remaining
        /// 16 bits get set so the full 32-bit value remains signed.
        /// </summary>
        public bool USN { get; set; }

        public bool FLG { get; set; }

        public override string ToString() => $"{VN}_{VL}, SIZE: {SIZE}, ADDR: {ADDR}, M: {M}, USN: {USN}, FLG: {FLG}";

        public enum UnpackVN
        {
            S,
            V2,
            V3,
            V4
        }

        public enum UnpackVL
        {
            VL_32,
            VL_16,
            VL_8,
            VL_5
        }
    }
}