using System;

namespace BinarySerializer.PlayStation.PS1
{
    /// <summary>
    /// Color table position
    /// </summary>
    public class CBA : BinarySerializable, IEquatable<CBA>, ISerializerShortLog
    {
        public int ClutX { get; set; } // Multiply by 16 to get actual value
        public int ClutY { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<ushort>(b =>
            {
                ClutX = b.SerializeBits<int>(ClutX, 6, name: nameof(ClutX));
                ClutY = b.SerializeBits<int>(ClutY, 9, name: nameof(ClutY));
                b.SerializePadding(1, logIfNotNull: true);
            });
        }

        public string ShortLog => ToString();
        public override string ToString() => $"CBA({ClutX}, {ClutY})";

        #region Equality

        public override bool Equals(object other) 
        {
            if (other is CBA cba)
                return Equals(cba);
            else
                return false;
        }

        public bool Equals(CBA other) 
        {
            if (other == null) 
                return false;
            if (other.ClutX != ClutX || other.ClutY != ClutY)
                return false;
            return true;
        }

        public override int GetHashCode() => (ClutX, ClutY).GetHashCode();

        public static bool operator ==(CBA term1, CBA term2) 
        {
            if ((object)term1 == null) 
                return (object)term2 == null;

            return term1.Equals(term2);
        }

        public static bool operator !=(CBA term1, CBA term2) => !(term1 == term2);

        #endregion
    }
}