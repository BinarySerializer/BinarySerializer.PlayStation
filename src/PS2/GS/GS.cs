namespace BinarySerializer.PlayStation.PS2
{
    public class GS
    {
        /// <summary>
        /// PS2 Graphics Synthesizer VRAM class
        /// Read/write functions reimplemented from <see href="https://sourceforge.net/projects/kernelloader">GSTextureConvert/ezSwizzle</see>
        /// </summary>
        public class VRAM
        {
            #region Constants
            static readonly int[] block32 = new int[32] {
                0,  1,  4,  5, 16, 17, 20, 21,
                2,  3,  6,  7, 18, 19, 22, 23,
                8,  9, 12, 13, 24, 25, 28, 29,
                10, 11, 14, 15, 26, 27, 30, 31
            };


            static readonly int[] columnWord32 = new int[16] {
                0,  1,  4,  5,  8,  9, 12, 13,
                2,  3,  6,  7, 10, 11, 14, 15
            };

            static readonly int[] block16 = new int[32] {
                0,  2,  8, 10,
                1,  3,  9, 11,
                4,  6, 12, 14,
                5,  7, 13, 15,
                16, 18, 24, 26,
                17, 19, 25, 27,
                20, 22, 28, 30,
                21, 23, 29, 31
            };

            static readonly int[] columnWord16 = new int[32] {
                0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
                2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15
            };

            static readonly int[] columnHalf16 = new int[32] {
                0, 0, 0, 0, 0, 0, 0, 0,  1, 1, 1, 1, 1, 1, 1, 1,
                0, 0, 0, 0, 0, 0, 0, 0,  1, 1, 1, 1, 1, 1, 1, 1
            };


            static readonly int[] block8 = new int[32] {
                0,  1,  4,  5, 16, 17, 20, 21,
                2,  3,  6,  7, 18, 19, 22, 23,
                8,  9, 12, 13, 24, 25, 28, 29,
                10, 11, 14, 15, 26, 27, 30, 31
            };

            static readonly int[][] columnWord8 = new int[2][] {
                new int[64] {
                    0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
                    2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,

                    8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,
                    10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7
                },
                new int[64] {
                    8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,
                    10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,

                    0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
                    2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15
                }
            };

            static readonly int[] columnByte8 = new int[64] {
                0, 0, 0, 0, 0, 0, 0, 0,  2, 2, 2, 2, 2, 2, 2, 2,
                0, 0, 0, 0, 0, 0, 0, 0,  2, 2, 2, 2, 2, 2, 2, 2,

                1, 1, 1, 1, 1, 1, 1, 1,  3, 3, 3, 3, 3, 3, 3, 3,
                1, 1, 1, 1, 1, 1, 1, 1,  3, 3, 3, 3, 3, 3, 3, 3
            };

            static readonly int[] block4 = new int[32] {
                0,  2,  8, 10,
                1,  3,  9, 11,
                4,  6, 12, 14,
                5,  7, 13, 15,
                16, 18, 24, 26,
                17, 19, 25, 27,
                20, 22, 28, 30,
                21, 23, 29, 31
            };

            static readonly int[][] columnWord4 = new int[2][] {
                new int[128] {
                    0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
                    2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,

                    8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,
                    10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7
                },
                new int[128] {
                    8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,
                    10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,

                    0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
                    2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15
                }
            };

            static readonly int[] columnByte4 = new int[128] {
                0, 0, 0, 0, 0, 0, 0, 0,  2, 2, 2, 2, 2, 2, 2, 2,  4, 4, 4, 4, 4, 4, 4, 4,  6, 6, 6, 6, 6, 6, 6, 6,
                0, 0, 0, 0, 0, 0, 0, 0,  2, 2, 2, 2, 2, 2, 2, 2,  4, 4, 4, 4, 4, 4, 4, 4,  6, 6, 6, 6, 6, 6, 6, 6,

                1, 1, 1, 1, 1, 1, 1, 1,  3, 3, 3, 3, 3, 3, 3, 3,  5, 5, 5, 5, 5, 5, 5, 5,  7, 7, 7, 7, 7, 7, 7, 7,
                1, 1, 1, 1, 1, 1, 1, 1,  3, 3, 3, 3, 3, 3, 3, 3,  5, 5, 5, 5, 5, 5, 5, 5,  7, 7, 7, 7, 7, 7, 7, 7
            };
            #endregion

            public byte[] VRAM_Bytes { get; set; }

            /// <summary>
            /// Creates a new PS2 Graphics Synthesizer VRAM object
            /// </summary>
            public VRAM()
            {
                VRAM_Bytes = new byte[1024 * 1024 * 4]; // 4 MB
            }

            public void writeTexPSMT4(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, byte[] data)
            {
                dbw >>= 1;
                int src = 0;
                int startBlockPos = dbp * 64;

                bool odd = false;

                for (int y = dsay; y < dsay + rrh; y++)
                {
                    for (int x = dsax; x < dsax + rrw; x++)
                    {
                        int pageX = x / 128;
                        int pageY = y / 128;
                        int page = pageX + pageY * dbw;

                        int px = x - (pageX * 128);
                        int py = y - (pageY * 128);

                        int blockX = px / 32;
                        int blockY = py / 16;
                        int block = block4[blockX + blockY * 4];

                        int bx = px - blockX * 32;
                        int by = py - blockY * 16;

                        int column = by / 4;

                        int cx = bx;
                        int cy = by - column * 4;
                        int cw = columnWord4[column & 1][cx + cy * 32];
                        int cb = columnByte4[cx + cy * 32];

                        int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw;

                        if ((cb & 1) != 0)
                        {
                            if (odd)
                                VRAM_Bytes[4 * dst + cb >> 1] = (byte)((VRAM_Bytes[4 * dst + cb >> 1] & 0x0f) | ((data[src]) & 0xf0));
                            else
                                VRAM_Bytes[4 * dst + cb >> 1] = (byte)((VRAM_Bytes[4 * dst + cb >> 1] & 0x0f) | (((data[src]) << 4) & 0xf0));
                        }
                        else
                        {
                            if (odd)
                                VRAM_Bytes[4 * dst + cb >> 1] = (byte)((VRAM_Bytes[4 * dst + cb >> 1] & 0xf0) | (((data[src]) >> 4) & 0x0f));
                            else
                                VRAM_Bytes[4 * dst + cb >> 1] = (byte)((VRAM_Bytes[4 * dst + cb >> 1] & 0xf0) | ((data[src]) & 0x0f));
                        }

                        if (odd)
                            src++;

                        odd = !odd;
                    }
                }
            }

            public void readTexPSMT4(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, ref byte[] data)
            {
                dbw >>= 1;
                int src = 0;
                int startBlockPos = dbp * 64;

                bool odd = false;

                for (int y = dsay; y < dsay + rrh; y++)
                {
                    for (int x = dsax; x < dsax + rrw; x++)
                    {
                        int pageX = x / 128;
                        int pageY = y / 128;
                        int page = pageX + pageY * dbw;

                        int px = x - (pageX * 128);
                        int py = y - (pageY * 128);

                        int blockX = px / 32;
                        int blockY = py / 16;
                        int block = block4[blockX + blockY * 4];

                        int bx = px - blockX * 32;
                        int by = py - blockY * 16;

                        int column = by / 4;

                        int cx = bx;
                        int cy = by - column * 4;
                        int cw = columnWord4[column & 1][cx + cy * 32];
                        int cb = columnByte4[cx + cy * 32];

                        int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw + cb >> 1;

                        if ((cb & 1) != 0)
                        {
                            if (odd)
                                data[src] = (byte)(((data[src]) & 0x0f) | (byte)(VRAM_Bytes[4 * dst + cb >> 1] & 0xf0));
                            else
                                data[src] = (byte)(((data[src]) & 0xf0) | ((byte)(VRAM_Bytes[4 * dst + cb >> 1] >> 4) & 0x0f));
                        }
                        else
                        {
                            if (odd)
                                data[src] = (byte)(((data[src]) & 0x0f) | (((byte)VRAM_Bytes[4 * dst + cb >> 1] << 4) & 0xf0));
                            else
                                data[src] = (byte)(((data[src]) & 0xf0) | ((byte)VRAM_Bytes[4 * dst + cb >> 1] & 0x0f));
                        }

                        if (odd)
                            src++;

                        odd = !odd;
                    }
                }
            }


            public void readTexPSMT4_mod(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, ref byte[] data)
            {
                dbw >>= 1;
                int src = 0;
                int startBlockPos = dbp * 64;

                bool odd = false;

                for (int y = dsay; y < dsay + rrh; y++)
                {
                    for (int x = dsax; x < dsax + rrw; x++)
                    {
                        int pageX = x / 128;
                        int pageY = y / 128;
                        int page = pageX + pageY * dbw;

                        int px = x - (pageX * 128);
                        int py = y - (pageY * 128);

                        int blockX = px / 32;
                        int blockY = py / 16;
                        int block = block4[blockX + blockY * 4];

                        int bx = px - blockX * 32;
                        int by = py - blockY * 16;

                        int column = by / 4;

                        int cx = bx;
                        int cy = by - column * 4;
                        int cb = columnByte4[cx + cy * 32];
                        int cw = columnWord4[column & 1][cx + cy * 32];

                        int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw;

                        if ((cb & 1) != 0)
                        {
                            if (odd)
                                data[src] = (byte)((VRAM_Bytes[4 * dst + (cb >> 1)] >> 4) & 0x0f);
                            else
                                data[src] = (byte)((VRAM_Bytes[4 * dst + (cb >> 1)] >> 4) & 0x0f);
                        }
                        else
                        {
                            if (odd)
                                data[src] = (byte)(VRAM_Bytes[4 * dst + (cb >> 1)] & 0x0f);
                            else
                                data[src] = (byte)(VRAM_Bytes[4 * dst + (cb >> 1)] & 0x0f);
                        }

                        src++;

                        odd = !odd;
                    }
                }
            }

            public void readTexPSMT8(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, ref byte[] data)
            {
                dbw >>= 1;
                int src = 0;
                int startBlockPos = dbp * 64;

                for (int y = dsay; y < dsay + rrh; y++)
                {
                    for (int x = dsax; x < dsax + rrw; x++)
                    {
                        int pageX = x / 128;
                        int pageY = y / 64;
                        int page = pageX + pageY * dbw;

                        int px = x - (pageX * 128);
                        int py = y - (pageY * 64);

                        int blockX = px / 16;
                        int blockY = py / 16;
                        int block = block8[blockX + blockY * 8];

                        int bx = px - blockX * 16;
                        int by = py - blockY * 16;

                        int column = by / 4;

                        int cx = bx;
                        int cy = by - column * 4;
                        int cw = columnWord8[column & 1][cx + cy * 16];
                        int cb = columnByte8[cx + cy * 16];

                        int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw;
                        data[src] = VRAM_Bytes[4 * dst + cb];
                        src++;
                    }
                }
            }

            public void writeTexPSMT8(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, byte[] data)
            {
                dbw >>= 1;
                int src = 0;
                int startBlockPos = dbp * 64;

                for (int y = dsay; y < dsay + rrh; y++)
                {
                    for (int x = dsax; x < dsax + rrw; x++)
                    {
                        int pageX = x / 128;
                        int pageY = y / 64;
                        int page = pageX + pageY * dbw;

                        int px = x - (pageX * 128);
                        int py = y - (pageY * 64);

                        int blockX = px / 16;
                        int blockY = py / 16;
                        int block = block8[blockX + blockY * 8];

                        int bx = px - (blockX * 16);
                        int by = py - (blockY * 16);

                        int column = by / 4;

                        int cx = bx;
                        int cy = by - column * 4;
                        int cw = columnWord8[column & 1][cx + cy * 16];
                        int cb = columnByte8[cx + cy * 16];

                        int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw;
                        VRAM_Bytes[4 * dst + cb] = data[src];
                        src++;
                    }
                }
            }

            public void writeTexPSMCT16(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, byte[] data)
            {
                int src = 0;
                int startBlockPos = dbp * 64;

                for (int y = dsay; y < dsay + rrh; y++)
                {
                    for (int x = dsax; x < dsax + rrw; x++)
                    {
                        int pageX = x / 64;
                        int pageY = y / 64;
                        int page = pageX + pageY * dbw;

                        int px = x - (pageX * 64);
                        int py = y - (pageY * 64);

                        int blockX = px / 16;
                        int blockY = py / 8;
                        int block = block16[blockX + blockY * 4];

                        int bx = px - blockX * 16;
                        int by = py - blockY * 8;

                        int column = by / 2;

                        int cx = bx;
                        int cy = by - column * 2;
                        int cw = columnWord16[cx + cy * 16];
                        int ch = columnHalf16[cx + cy * 16];

                        int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw;
                        for (int i = 0; i < 2; i++)
                        {
                            VRAM_Bytes[4 * dst + 2 * ch + i] = data[src + i];
                        }
                        src += 2;
                    }
                }
            }

            void readTexPSMCT16(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, ref byte[] data)
            {
                int src = 0;
                int startBlockPos = dbp * 64;

                for (int y = dsay; y < dsay + rrh; y++)
                {
                    for (int x = dsax; x < dsax + rrw; x++)
                    {
                        int pageX = x / 64;
                        int pageY = y / 64;
                        int page = pageX + pageY * dbw;

                        int px = x - (pageX * 64);
                        int py = y - (pageY * 64);

                        int blockX = px / 16;
                        int blockY = py / 8;
                        int block = block16[blockX + blockY * 4];

                        int bx = px - blockX * 16;
                        int by = py - blockY * 8;

                        int column = by / 2;

                        int cx = bx;
                        int cy = by - column * 2;
                        int cw = columnWord16[cx + cy * 16];
                        int ch = columnHalf16[cx + cy * 16];

                        int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw;
                        for (int i = 0; i < 2; i++)
                        {
                            data[src + i] = VRAM_Bytes[4 * dst + 2 * ch + i];
                        }
                        src += 2;
                    }
                }
            }

            public void writeTexPSMCT32(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, byte[] data)
            {
                int src = 0;
                int startBlockPos = dbp * 64;

                for (int y = dsay; y < dsay + rrh; y++)
                {
                    for (int x = dsax; x < dsax + rrw; x++)
                    {
                        int pageX = x / 64;
                        int pageY = y / 32;
                        int page = pageX + pageY * dbw;

                        int px = x - (pageX * 64);
                        int py = y - (pageY * 32);

                        int blockX = px / 8;
                        int blockY = py / 8;
                        int block = block32[blockX + blockY * 8];

                        int bx = px - blockX * 8;
                        int by = py - blockY * 8;

                        int column = by / 2;

                        int cx = bx;
                        int cy = by - column * 2;
                        int cw = columnWord32[cx + cy * 8];

                        int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw;
                        for (int i = 0; i < 4; i++)
                        {
                            VRAM_Bytes[4 * dst + i] = data[src + i];
                        }
                        src += 4;
                    }
                }
            }

            public void readTexPSMCT32(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, ref byte[] data)
            {
                int src = 0;
                int startBlockPos = dbp * 64;

                for (int y = dsay; y < dsay + rrh; y++)
                {
                    for (int x = dsax; x < dsax + rrw; x++)
                    {
                        int pageX = x / 64;
                        int pageY = y / 32;
                        int page = pageX + pageY * dbw;

                        int px = x - (pageX * 64);
                        int py = y - (pageY * 32);

                        int blockX = px / 8;
                        int blockY = py / 8;
                        int block = block32[blockX + blockY * 8];

                        int bx = px - blockX * 8;
                        int by = py - blockY * 8;

                        int column = by / 2;

                        int cx = bx;
                        int cy = by - column * 2;
                        int cw = columnWord32[cx + cy * 8];


                        int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw;
                        for (int i = 0; i < 4; i++)
                        {
                            data[src + i] = VRAM_Bytes[4 * dst + i];
                        }
                        src += 4;
                    }
                }
            }
        }

        /// <see href="https://openkh.dev/common/tm2.html#psm-register-pixel-storage-mode">PSM documentation</see>
        public enum PixelStorageMode
        {
            PSMCT32 = 0x00,
            PSMCT24 = 0x01,
            PSMCT16 = 0x02,
            PSMCT16S = 0x0A,
            PSMT8 = 0x13,
            PSMT4 = 0x14,
            PSMT8H = 0x1B,
            PSMT4HL = 0x24,
            PSMT4HH = 0x2C,
            PSMZ32 = 0x30,
            PSMZ24 = 0x31,
            PSMZ16 = 0x32,
            PSMZ16S = 0x3A
        }

        /// <see href="https://openkh.dev/common/tm2.html#cpsm-register-color-look-up-pixel-storage-mode">CPSM documentation</see>
        public enum CLUTPixelStorageMode
        {
            PSMCT32 = 0x00,
            PSMCT24 = 0x01,
            PSMCT16 = 0x02,
            PSMCT16S = 0x03
        }

        /// <see href="https://openkh.dev/common/tm2.html#csm-register-color-storage-mode">CSM documentation</see>
        public enum ColorStorageMode
        {
            CSM1 = 0x00,
            CSM2 = 0x01
        }

        /// <see href="https://openkh.dev/common/tm2.html#tfx-register-texture-function">TFX documentation</see>
        public enum TextureFunction
        {
            MODULATE = 0x00,
            DECAL = 0x01,
            HIGHLIGHT = 0x02,
            HIGHLIGHT2 = 0x03
        }
    }
}