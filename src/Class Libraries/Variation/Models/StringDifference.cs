namespace Cavity.Models
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public struct StringDifference : ISerializable,
                                     IEquatable<StringDifference>
    {
        public StringDifference(string difference,
                                string former,
                                string latter)
            : this()
        {
            Difference = difference;
            Former = former;
            Latter = latter;
        }

        private StringDifference(SerializationInfo info,
                                 StreamingContext context)
            : this()
        {
            Difference = info.GetString("_difference");
            Former = info.GetString("_former");
            Latter = info.GetString("_latter");
        }

        public string Difference { get; set; }

        public string Former { get; set; }

        public string Latter { get; set; }

        public static bool operator ==(StringDifference obj,
                                       StringDifference comparand)
        {
            return obj.Equals(comparand);
        }

        public static bool operator !=(StringDifference obj,
                                       StringDifference comparand)
        {
            return !obj.Equals(comparand);
        }

        public override bool Equals(object obj)
        {
            return !ReferenceEquals(null, obj) && Equals((StringDifference)obj);
        }

        public bool Equals(StringDifference other)
        {
            if (Difference != other.Difference)
            {
                return false;
            }

            if (Former != other.Former)
            {
                return false;
            }

            return Latter == other.Latter;
        }

        public override int GetHashCode()
        {
            return string.Concat(Difference, '|', Former, '|', Latter).GetHashCode();
        }

#if NET20 || NET35
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
#endif

        void ISerializable.GetObjectData(SerializationInfo info,
                                         StreamingContext context)
        {
            if (null == info)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("_difference", Difference);
            info.AddValue("_former", Former);
            info.AddValue("_latter", Latter);
        }
    }
}