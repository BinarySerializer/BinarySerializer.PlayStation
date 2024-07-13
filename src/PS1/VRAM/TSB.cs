using System;

namespace BinarySerializer.PlayStation.PS1
{
    public class TSB : BinarySerializable, IEquatable<TSB>
    {
        // See http://hitmen.c02.at/files/docs/psx/psx.pdf page 37
        public byte TX { get; set; } // value * 64
        public byte TY { get; set; } // value * 256
        public byte ABR { get; set; } // Semi transparency mode
        public TexturePageTP TP { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<ushort>(b =>
            {
                TX = b.SerializeBits<byte>(TX, 4, name: nameof(TX));
                TY = b.SerializeBits<byte>(TY, 1, name: nameof(TY));
                ABR = b.SerializeBits<byte>(ABR, 2, name: nameof(ABR));
                TP = b.SerializeBits<TexturePageTP>(TP, 2, name: nameof(TP));
            });
        }

        public enum TexturePageTP : byte
        {
            CLUT_4Bit = 0,
            CLUT_8Bit = 1,
            Direct_15Bit = 2,
        }

        #region Equality

        public override bool Equals(object other) 
        {
            if (other is TSB tsb)
                return Equals(tsb);
            else
                return false;
        }

        public bool Equals(TSB other) 
        {
            if(other == null) 
                return false;
            if (other.TP != TP || other.TX != TX || other.TY != TY || other.ABR != ABR)
                return false;
            return true;
        }

        public override int GetHashCode() => (TP, TX, TY, ABR).GetHashCode();

        public static bool operator ==(TSB term1, TSB term2) 
        {
            if ((object)term1 == null) 
                return (object)term2 == null;

            return term1.Equals(term2);
        }

        public static bool operator !=(TSB term1, TSB term2) => !(term1 == term2);

        #endregion
    }
}