using System.IO;

namespace BinarySerializer.PlayStation.PS2
{
    public class IPU_File : BinarySerializable
    {
        public uint DataSize { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public uint FrameCount { get; set; }
        public byte[] FrameData { get; set; }
        public virtual int FPS => 30;
        public virtual bool IsAligned => false;

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeMagicString("ipum", 4);
            DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            FrameCount = s.Serialize<uint>(FrameCount, name: nameof(FrameCount));
            FrameData = s.SerializeArray<byte>(FrameData, DataSize - 8, name: nameof(FrameData));
        }

        /// <summary>
        /// Converts IPU file to an M2V/MPEG-2 stream.
        /// Implementation from <see href="https://github.com/samehb/IPUDecoder/blob/master/IPUDecoder/IPUDecoder.cpp">IPUDecoder</see>.
        /// </summary>
        public Stream ToM2V()
        {
            MemoryStream m2v = new MemoryStream();

            for (int frame = 0; frame < FrameCount; frame++)
            {
                int flag = getBits(8);

                if (frame == 0)
                {
                    putBits(m2v, 0x1b3, 32);
                    putBits(m2v, Width, 12);
                    putBits(m2v, Height, 12);
                    putBits(m2v, 0x1, 4);
                    putBits(m2v, 0x4, 4);
                    putBits(m2v, 0x30d4, 18);
                    putBits(m2v, 1, 1);
                    putBits(m2v, 112, 10);
                    putBits(m2v, 0, 1);
                    putBits(m2v, 0, 1);
                    putBits(m2v, 0, 1);

                    if ((flag & 0x80) == 0)
                    {
                        putBits(m2v, 0x1b5, 32);
                        putBits(m2v, 0x1, 4);
                        putBits(m2v, 0x4, 4);
                        putBits(m2v, 0x8, 4);
                        putBits(m2v, 0x1, 1);
                        putBits(m2v, 0x1, 2);
                        putBits(m2v, 0x0, 2);
                        putBits(m2v, 0x0, 2);
                        putBits(m2v, 0x0, 12);
                        putBits(m2v, 0x1, 1);
                        putBits(m2v, 0x0, 8);
                        putBits(m2v, 0x0, 1);
                        putBits(m2v, 0x0, 2);
                        putBits(m2v, 0x0, 5);
                    }
                }

                // Write GOP Header
                putBits(m2v, 0x1b8, 32);
                putBits(m2v, 0, 1);
                putBits(m2v, frame / FPS / 60 / 60, 5);
                putBits(m2v, (frame % (FPS * 60 * 60)) / FPS / 60, 6);
                putBits(m2v, 1, 1);
                putBits(m2v, (frame % (FPS * 60)) / FPS, 6);
                putBits(m2v, frame % FPS, 6);
                putBits(m2v, 1, 1);
                putBits(m2v, 0, 6);

                // Write Picture Header
                putBits(m2v, 0x100, 32);
                putBits(m2v, 0x0, 10);
                putBits(m2v, 0x1, 3);
                putBits(m2v, 0xffff, 16);
                putBits(m2v, 0, 3);

                // Write Picture Coding Extension
                if ((flag & 0x80) == 0)
                {
                    putBits(m2v, 0x1b5, 32);
                    putBits(m2v, 0x8ffff, 20);
                    putBits(m2v, flag & 3, 2);
                    putBits(m2v, 3, 2);
                    putBits(m2v, 2, 3);
                    putBits(m2v, (flag & 64) / 64, 1);
                    putBits(m2v, (flag & 32) / 32, 1);
                    putBits(m2v, (flag & 16) / 16, 1);
                    putBits(m2v, 1, 2);
                    putBits(m2v, 0x80, 8);
                }

                int dct_dc_y = 0;
                int dct_dc_cb = 0;
                int dct_dc_cr = 0;
                int quant = 1;
                int intraquant = 0;
                MBData[] macroblocks = new MBData[(Width / 16) * (Height / 16)];
                for (int mb = 0; mb < (Width / 16) * (Height / 16); mb++)
                {
                    MBData macroblock = new MBData();
                    if (mb > 0)
                    {
                        if (getBits(1) == 0)
                            throw new BinarySerializableException(this, "MBT_Incr is incorrect");
                    }

                    getPos(ref macroblock.Byte, ref macroblock.Bit);

                    if (getBits(1) == 1)
                        intraquant = 0;
                    else
                    {
                        if (getBits(1) == 0)
                            throw new BinarySerializableException(this, "MBT is incorrect");;
                        intraquant = 1;
                    }

                    if ((flag & 4) > 0)
                        getBits(1);

                    if (intraquant == 1)
                        quant = getBits(5);
                    macroblock.quant = quant;

                    for (int block = 0; block < 6; block++)
                    {
                        if (block < 4)
                        {
                            int size = get_dcs_y();
                            if (size > 0)
                            {
                                int diff = getBits(size);
                                if ((diff & (1 << (size - 1))) == 0)
                                    diff = (-1 << size) | (diff + 1);
                                dct_dc_y += diff;
                            }
                            if (block == 0)
                                macroblock.dct_dc_y = dct_dc_y;
                        }
                        else
                        {
                            int size = get_dcs_c();
                            int diff = 0;
                            if (size > 0)
                            {
                                diff = getBits(size);
                                if ((diff & (1 << (size - 1))) == 0)
                                    diff = (-1 << size) | (diff + 1);
                            }
                            if (block == 4)
                            {
                                if (size > 0)
                                    dct_dc_cb += diff;
                                macroblock.dct_dc_cb = dct_dc_cb;
                            }
                            else
                            {
                                if (size > 0)
                                    dct_dc_cr += diff;
                                macroblock.dct_dc_cr = dct_dc_cr;
                            }
                        }

                        int eob = 0;
                        do
                        {
                            if ((flag & 32) > 0)
                                eob = ivlc(m2v, 0);
                            else
                                eob = vlc(m2v, 0);
                            if (eob == 0)
                                getBits(1);
                        } while (eob != 1);
                    }

                    macroblocks[mb] = macroblock;
                }

                int frameByte = 0, frameBit = 0;
                getPos(ref frameByte, ref frameBit);

                dct_dc_y = 0;
                dct_dc_cb = 0;
                dct_dc_cr = 0;
                quant = 1;

                int slicescnt = 1;
		        int counter = -1;
                for (int mb = 0; mb < (Width / 16) * (Height / 16); mb++)
                {
                    int mb_source = mb;
                    counter++;

                    if ((counter % (Width / 16)) == 0)
                    {
                        if (writeBufferOffset % 8 > 0 && counter != 0)
                        {
                            int temp = writeBufferOffset;
                            for (int i = 0; i < 8 - (temp % 8); i++)
                                putBits(m2v, 0, 1);
                        }

                        putBits(m2v, 0x1, 24);
                        putBits(m2v, slicescnt, 8);
                        putBits(m2v, macroblocks[mb_source].quant, 5);
                        putBits(m2v, 0, 1);
                        slicescnt++;
                    }


                    putBits(m2v, 1, 1);

                    setPos(macroblocks[mb_source].Byte, macroblocks[mb_source].Bit);

                    if (getBits(1) > 0)
                        intraquant = 0;
                    else
                    {
                        if (getBits(1) == 0)
                            throw new BinarySerializableException(this, "Incorrect MBT");
                        intraquant = 1;
                    }

                    putBits(m2v, 1, 1);

                    if ((flag & 4) > 0)
                        putBits(m2v, getBits(1), 1);

                    if (intraquant > 0)
                        getBits(5);

                    for (int block = 0; block<6; block++)
                    {
                        int diff = 0;
                        if (block == 0)
                        {
                            getBits(get_dcs_y());
                            diff = macroblocks[mb_source].dct_dc_y - dct_dc_y;
                            dct_dc_y = macroblocks[mb_source].dct_dc_y;

                            if (counter % (Width / 16) == 0 && counter != 0)
                            {
                                diff = macroblocks[mb_source].dct_dc_y;
                            }

                            int absval = (diff < 0) ? -diff : diff;
                            int size = 0;
                            while (absval > 0)
                            {
                                absval >>= 1;
                                size++;
                            }
                            put_dcs_y(m2v, size);
                            absval = diff;
                            if (absval <= 0)
                                absval += (1 << size) - 1;
                            putBits(m2v, absval, size);
                        }
                        else if (block > 3) 
                        {
                            getBits(get_dcs_c());
                            if (block == 4)
                            {
                                diff = macroblocks[mb_source].dct_dc_cb - dct_dc_cb;
                                dct_dc_cb = macroblocks[mb_source].dct_dc_cb;
                                if (counter % (Width / 16) == 0 && counter != 0)
                                {
                                    diff = macroblocks[mb_source].dct_dc_cb;
                                }
                            }
                            else
                            {
                                diff = macroblocks[mb_source].dct_dc_cr - dct_dc_cr;
                                dct_dc_cr = macroblocks[mb_source].dct_dc_cr;
                                if (counter % (Width / 16) == 0 && counter != 0)
                                {
                                    diff = macroblocks[mb_source].dct_dc_cr;
                                }
                            }
                            int absval = (diff < 0) ? -diff : diff;
                            int size = 0;
                            while (absval > 0)
                            {
                                absval >>= 1;
                                size++;
                            }
                            put_dcs_c(m2v, size);
                            absval = diff;
                            if (absval <= 0)
                                absval += (1 << size) - 1;
                            putBits(m2v, absval, size);
                        }
                        else
                        {
                            int size = get_dcs_y();
                            put_dcs_y(m2v, size);
                            diff = getBits(size);
                            putBits(m2v, diff, size);
                            if (size > 0)
                            {
                                if ((diff & (1 << (size - 1))) == 0)
                                    diff = (-1 << size) | (diff + 1);
                                dct_dc_y += diff;
                            }
                        }

                        int eob = 0;
                        do
                        {
                            if ((flag & 0x20) > 0)
                                eob = ivlc(m2v, 1);
                            else
                                eob = vlc(m2v, 1);
                            if (eob == 0)
                                putBits(m2v, getBits(1), 1);
                        } while (eob != 1);
                    }
                }

                putBuf(m2v);
                setPos(frameByte, frameBit);

                Next_Start_Code();

                if (getBits(32) != 0x000001b0)
                    throw new BinarySerializableException(this, "Invalid frame delimiter");
                
                if (IsAligned)
                    while ((bitOffset / 8) % 16 != 0)
                        getBits(1);
            }

            putBits(m2v, 0x1b7, 32);
            
            return m2v;
        }

        int Next_Start_Code()
        {
            int buf = 0;
            bitOffset = bitOffset - (bitOffset % 8) - 8;
            while (buf != 0x000001)
            {
                getBits(8);
                buf = nextBits(24);
            }

            return(1);
        }

        int get_dcs_y()
        {
            int bits = getBits(2);

            if (bits == 0)
                return(1);
            if (bits == 1)
                return(2);
            bits <<= 1;
            bits |= getBits(1);
            if (bits == 4)
                return(0);
            if (bits == 5)
                return(3);
            if (bits == 6)
                return(4);
            if (getBits(1) == 0)
                return(5);
            if (getBits(1) == 0)
                return(6);
            if (getBits(1) == 0)
                return(7);
            if (getBits(1) == 0)
                return(8);
            if (getBits(1) == 0)
                return(9);
            if (getBits(1) == 0)
                return(10);
            return(11);
        }

        int get_dcs_c()
        {
            int bits = getBits(2);

            if (bits == 0)
                return(0);
            if (bits == 1)
                return(1);
            if (bits == 2)
                return(2);
            if (getBits(1) == 0)
                return(3);
            if (getBits(1) == 0)
                return(4);
            if (getBits(1) == 0)
                return(5);
            if (getBits(1) == 0)
                return(6);
            if (getBits(1) == 0)
                return(7);
            if (getBits(1) == 0)
                return(8);
            if (getBits(1) == 0)
                return(9);
            if (getBits(1) == 0)
                return(10);
            return(11);
        }

        int vlc(MemoryStream stream, int write)
        {
            int bits = getBits(2);
            int level = 0;

            if (write > 0)
                putBits(stream, bits, 2);
            if (bits == 2)
                return(1);
            if (bits == 3)
                return(0);
            if (bits == 1) {
                bits = getBits(1);
                if (write > 0)
                    putBits(stream, bits, 1);
                if (bits > 0)
                    return(0);
                else {
                    bits = getBits(1);
                    if (write > 0)
                        putBits(stream, bits, 1);
                    return(0);
                }
            }

            bits = getBits(1);
            if (write > 0)
                putBits(stream, bits, 1);
            if (bits > 0) {
                bits = getBits(2);
                if (write > 0) putBits(stream, bits, 2);
                if (bits < 1) {
                    bits = getBits(3);
                    if (write > 0)
                        putBits(stream, bits, 3);
                }
                return(0);
            }
            else {
                bits = getBits(3);
                if (write > 0)
                    putBits(stream, bits, 3);
                if (bits >= 4)
                    return(0);
                if (bits >= 2) {
                    bits = getBits(1);
                    if (write > 0)
                        putBits(stream, bits, 1);
                    return(0);
                }
                if (bits > 0) {
                    bits = getBits(18);
                    if (write > 0)
                        putBits(stream, bits, 18);
                    return(2);
                }
                bits = getBits(1);
                if (write > 0)
                    putBits(stream, bits, 1);
                if (bits > 0) {
                    bits = getBits(3);
                    if (write > 0)
                        putBits(stream, bits, 3);
                    return(0);
                }
                do {
                    bits = getBits(1);
                    if (write > 0)
                        putBits(stream, bits, 1);
                    level++;
                } while (bits == 0 && level<  6);
                if (level<6) {
                    bits = getBits(4);
                    if (write > 0)
                        putBits(stream, bits, 4);
                    return(0);
                }
                else {
                    throw new BinarySerializableException(this, "Invalid VLC");
                }
            }
        }

        int ivlc(MemoryStream stream, int write)
        {
            int bits = getBits(2);
            int level = 0;

            if (write > 0)
                putBits(stream, bits, 2);

            if (bits == 3)
            {
                bits = getBits(1);
                if (write > 0)
                    putBits(stream, bits, 1);
                if (bits == 0)
                    return(0);

                bits = getBits(1);
                if (write > 0)
                    putBits(stream, bits, 1);
                if (bits == 0)
                {
                    bits = getBits(1);
                    if (write > 0)
                        putBits(stream, bits, 1);
                    return(0);
                }

                bits = getBits(1);
                if (write > 0)
                    putBits(stream, bits, 1);
                if (bits == 0)
                {
                    bits = getBits(2);
                    if (write > 0)
                        putBits(stream, bits, 2);
                    return(0);
                }

                bits = getBits(2);
                if (write > 0)
                    putBits(stream, bits, 2);
                if (bits == 0)
                    return(0);
                bits = getBits(1);
                if (write > 0)
                    putBits(stream, bits, 1);
                return(0);
            }

            if (bits == 2)
                return(0);
            if (bits == 1)
            {
                bits = getBits(1);
                if (write > 0)
                    putBits(stream, bits, 1);
                if (bits > 0)
                {
                    bits = getBits(1);
                    if (write > 0) putBits(stream, bits, 1);
                    if (bits > 0)
                        return(0);
                    return(1);
                }
                return(0);
            }

            bits = getBits(1);
            if (write > 0) putBits(stream, bits, 1);
            if (bits > 0)
            {
                bits = getBits(2);
                if (write > 0)
                    putBits(stream, bits, 2);
                if (bits == 0) {
                    bits = getBits(3);
                    if (write > 0) putBits(stream, bits, 3);
                }
                return(0);
            }
            else
            {
                bits = getBits(3);
                if (write > 0)
                    putBits(stream, bits, 3);
                if (bits >= 4)
                    return(0);
                if (bits >= 2)
                {
                    bits = getBits(1);
                    if (write > 0) putBits(stream, bits, 1);
                    return(0);
                }
                if (bits > 0)
                {
                    bits = getBits(18);
                    if (write > 0) putBits(stream, bits, 18);
                    return(2);
                }
                bits = getBits(1);
                if (write > 0)
                    putBits(stream, bits, 1);
                if (bits > 0)
                {
                    bits = getBits(2);
                    if (write > 0) putBits(stream, bits, 2);
                    if (bits != 2)
                        return(0);
                    bits = getBits(1);
                    if (write > 0) putBits(stream, bits, 1);
                    return(0);
                }
                do
                {
                    bits = getBits(1);
                    if (write > 0) putBits(stream, bits, 1);
                    level++;
                } while (bits == 0 && level<6);
                if (level<6)
                {
                    bits = getBits(4);
                    if (write > 0) putBits(stream, bits, 4);
                    return(0);
                }
                else
                    throw new BinarySerializableException(this, "Invalid VLC");
            }

        }

        private int bitOffset = 0;
        private int getBits(int numBits)
        {
            uint bits = 0;
            uint mask = (uint)1 << (8 - (bitOffset % 8) - 1);
            byte b = FrameData[bitOffset / 8];
            for (int index = 0; index < numBits; index++)
            {
                bits <<= 1;
                if ((b & mask) > 0)
                    bits++;
                mask >>= 1;
                if (mask == 0)
                {
                    mask = 0x80;
                    b = FrameData[bitOffset / 8 + 1];
                }
                bitOffset++;
            }
            return (int)bits;
        }

        private byte buffer = 0;
        private int writeBufferOffset = 0;
        private void putBits(MemoryStream stream, int data, int numBits)
        {
            uint mask = (uint)1 << (numBits - 1);
            for (int i = 0; i < numBits; i++)
            {
                buffer <<= 1;
                if ((data & mask) > 0)
                    buffer |= 1;
                
                mask >>= 1;
                writeBufferOffset++;
                if (writeBufferOffset == 8)
                    writeBuffer(stream);
            }
        }

        private void putBuf(MemoryStream stream)
        {
            if (writeBufferOffset > 0)
                putBits(stream, 0, 8 - writeBufferOffset);
        }

        private void writeBuffer(MemoryStream stream)
        {
            stream.WriteByte(buffer);
            buffer = 0;
            writeBufferOffset = 0;
        }

        private int nextBits(int numBits)
        {
            int oldOffset = bitOffset;
            int bits = getBits(numBits);
            bitOffset = oldOffset;
            return bits;
        }

        private void setPos(int bytePos, int bitPos)
        {
            bitOffset = bytePos * 8 + bitPos;
        }

        private System.Tuple<int, int> getPos()
        {
            return System.Tuple.Create(bitOffset / 8, bitOffset % 8);
        }

        private void getPos(ref int Byte, ref int Bit)
        {
            Byte = bitOffset / 8;
            Bit = bitOffset % 8;
        }

        struct MBData
        {
            public int Byte;
            public int Bit;
            public int dct_dc_y;
            public int dct_dc_cb;
            public int dct_dc_cr;
            public int quant;
        }

        void put_dcs_y(MemoryStream stream, int len)
        {
            switch (len) {
                case 0:
                    putBits(stream, 4, 3);
                    break;
                case 1:
                    putBits(stream, 0, 2);
                    break;
                case 2:
                    putBits(stream, 1, 2);
                    break;
                case 3:
                    putBits(stream, 5, 3);
                    break;
                case 4:
                    putBits(stream, 6, 3);
                    break;
                case 5:
                    putBits(stream, 14, 4);
                    break;
                case 6:
                    putBits(stream, 30, 5);
                    break;
                case 7:
                    putBits(stream, 62, 6);
                    break;
                case 8:
                    putBits(stream, 126, 7);
                    break;
                case 9:
                    putBits(stream, 254, 8);
                    break;
                case 10:
                    putBits(stream, 510, 9);
                    break;
                case 11:
                    putBits(stream, 511, 9);
                    break;
            }
        }

        void put_dcs_c(MemoryStream stream, int len)
        {
            switch (len) {
                case 0:
                    putBits(stream, 0, 2);
                    break;
                case 1:
                    putBits(stream, 1, 2);
                    break;
                case 2:
                    putBits(stream, 2, 2);
                    break;
                case 3:
                    putBits(stream, 6, 3);
                    break;
                case 4:
                    putBits(stream, 14, 4);
                    break;
                case 5:
                    putBits(stream, 30, 5);
                    break;
                case 6:
                    putBits(stream, 62, 6);
                    break;
                case 7:
                    putBits(stream, 126, 7);
                    break;
                case 8:
                    putBits(stream, 254, 8);
                    break;
                case 9:
                    putBits(stream, 510, 9);
                    break;
                case 10:
                    putBits(stream, 1022, 10);
                    break;
                case 11:
                    putBits(stream, 1023, 10);
                    break;
            }
        }
    }
}