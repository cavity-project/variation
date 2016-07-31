namespace Cavity.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Cavity.Collections;

    [Serializable]
    public sealed class StringDifferenceCollection : ISerializable,
                                                     IEnumerable<KeyValuePair<string, StringDifference>>,
                                                     IEquatable<StringDifferenceCollection>
    {
        public StringDifferenceCollection()
            : this(StringComparer.OrdinalIgnoreCase)
        {
        }

        public StringDifferenceCollection(IEqualityComparer<string> comparer)
        {
            Data = new Dictionary<string, StringDifference>(comparer);
        }

        private StringDifferenceCollection(SerializationInfo info,
                                           StreamingContext context)
        {
            if (null == info)
            {
                throw new ArgumentNullException("info");
            }

            Data = (Dictionary<string, StringDifference>)info.GetValue("_data", typeof(Dictionary<string, StringDifference>));
            Variation = (VariationDates)info.GetValue("_variation", typeof(VariationDates));
        }

        public int Count
        {
            get
            {
                return Data.Count;
            }
        }

        public VariationDates Variation { get; set; }

        private Dictionary<string, StringDifference> Data { get; set; }

        public StringDifference this[string key]
        {
            get
            {
                if (null == key)
                {
                    throw new ArgumentNullException("key");
                }

                key = key.Trim();
                if (key.IsEmpty())
                {
                    throw new ArgumentOutOfRangeException("key");
                }

                if (Data.NotContainsKey(key))
                {
                    throw new KeyNotFoundException("The key '{0}' was not found.".FormatWith(key));
                }

                return Data[key];
            }
        }

        public static bool operator ==(StringDifferenceCollection obj,
                                       StringDifferenceCollection comparand)
        {
            return ReferenceEquals(null, obj)
                       ? ReferenceEquals(null, comparand)
                       : obj.Equals(comparand);
        }

        public static bool operator !=(StringDifferenceCollection obj,
                                       StringDifferenceCollection comparand)
        {
            return ReferenceEquals(null, obj)
                       ? !ReferenceEquals(null, comparand)
                       : !obj.Equals(comparand);
        }

        public static StringDifferenceCollection Calculate(VariationDates variation,
                                                           KeyStringDictionary former,
                                                           KeyStringDictionary latter)
        {
            if (null == (former ?? latter))
            {
                return null;
            }

            var result = new StringDifferenceCollection
                             {
                                 Variation = variation
                             };

            var keys = new[] { former, latter }.Where(dictionary => dictionary.IsNotNull())
                                               .SelectMany(dictionary => dictionary.Keys)
                                               .ToHashSet(result.Data.Comparer);
            foreach (var key in keys)
            {
                var a = null == former || former.NotContainsKey(key) ? null : former[key];
                var b = null == latter || latter.NotContainsKey(key) ? null : latter[key];
                var difference = result.Data.Comparer.Equals(a, b) ? "repetition" : "alteration";
                result.Data.Add(key, new StringDifference(difference, a, b));
            }

            return result;
        }

        public void Add(string key,
                        StringDifference value)
        {
            if (null == key)
            {
                throw new ArgumentNullException("key");
            }

            key = key.Trim();
            if (key.IsEmpty())
            {
                throw new ArgumentOutOfRangeException("key");
            }

            Data.Add(key, value);
        }

        public IEnumerator<KeyValuePair<string, StringDifference>> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            return !ReferenceEquals(null, obj) && Equals((StringDifferenceCollection)obj);
        }

        public bool Equals(StringDifferenceCollection other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (Variation != other.Variation)
            {
                return false;
            }

            return Data.SequenceEqual(other.Data);
        }

        public override int GetHashCode()
        {
            return Data.Values.GetHashCode() ^ Variation.GetHashCode();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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

            info.AddValue("_data", Data);
            info.AddValue("_variation", Variation);
        }
    }
}