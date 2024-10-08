﻿namespace BinarySerializer.PlayStation.PS3
{
    public enum GTFFormat : byte
    {
        B8 = 0x81,
        A1R5G5B5 = 0x82,
        A4R4G4B4 = 0x83,
        R5G6B5 = 0x84,
        A8R8G8B8 = 0x85,
        COMPRESSED_DXT1 = 0x86,
        COMPRESSED_DXT23 = 0x87,
        COMPRESSED_DXT45 = 0x88,
        G8B8 = 0x8B,
        R6G5B5 = 0x8F,
        DEPTH24_D8 = 0x90,
        DEPTH24_D8_FLOAT = 0x91,
        DEPTH16 = 0x92,
        DEPTH16_FLOAT = 0x93,
        X16 = 0x94,
        Y16_X16 = 0x95,
        R5G5B5A1 = 0x97,
        COMPRESSED_HILO8 = 0x98,
        COMPRESSED_HILO_S8 = 0x99,
        W16_Z16_Y16_X16_FLOAT = 0x9A,
        W32_Z32_Y32_X32_FLOAT = 0x9B,
        X32_FLOAT = 0x9C,
        D1R5G5B5 = 0x9D,
        D8R8G8B8 = 0x9E,
        Y16_X16_FLOAT = 0x9F,
        COMPRESSED_B8R8_G8R8 = 0xAD,
        COMPRESSED_R8B8_R8G8 = 0xAE,
        COMPRESSED_B8R8_G8R8_RAW = 0x8D,
        COMPRESSED_R8B8_R8G8_RAW = 0x8E,
    }
}