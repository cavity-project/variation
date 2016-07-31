namespace Cavity.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;

    [Serializable]
    public struct VariationDates : ISerializable,
                                   IEquatable<VariationDates>
    {
        public VariationDates(string addition,
                              string repetition,
                              string alteration,
                              string deletion,
                              string restoration)
            : this()
        {
            Addition = addition;
            Repetition = repetition;
            Alteration = alteration;
            Deletion = deletion;
            Restoration = restoration;
        }

        private VariationDates(SerializationInfo info,
                               StreamingContext context)
            : this()
        {
            Addition = info.GetString("_addition");
            Repetition = info.GetString("_repetition");
            Alteration = info.GetString("_alteration");
            Deletion = info.GetString("_deletion");
            Restoration = info.GetString("_restoration");
        }

        public string Addition { get; set; }

        public string Alteration { get; set; }

        public string Deletion { get; set; }

        public string Repetition { get; set; }

        public string Restoration { get; set; }

        public static bool operator ==(VariationDates obj,
                                       VariationDates comparand)
        {
            return obj.Equals(comparand);
        }

        public static bool operator !=(VariationDates obj,
                                       VariationDates comparand)
        {
            return !obj.Equals(comparand);
        }

        public override bool Equals(object obj)
        {
            return !ReferenceEquals(null, obj) && Equals((VariationDates)obj);
        }

        public bool Equals(VariationDates other)
        {
            if (Addition != other.Addition)
            {
                return false;
            }

            if (Repetition != other.Repetition)
            {
                return false;
            }

            if (Alteration != other.Alteration)
            {
                return false;
            }

            if (Deletion != other.Deletion)
            {
                return false;
            }

            return Restoration == other.Restoration;
        }

        public override int GetHashCode()
        {
            return string.Concat(Addition, '|', Repetition, '|', Alteration, '|', Deletion, '|', Restoration).GetHashCode();
        }

        public string Variance()
        {
            return Dates().Where(tuple => DateTime.MinValue != tuple.Item2)
                          .OrderBy(tuple => tuple.Item2)
                          .Select(tuple => tuple.Item1)
                          .LastOrDefault();
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

            info.AddValue("_addition", Addition);
            info.AddValue("_repetition", Repetition);
            info.AddValue("_alteration", Alteration);
            info.AddValue("_deletion", Deletion);
            info.AddValue("_restoration", Restoration);
        }

        private static Tuple<string, DateTime> Date(string key,
                                                    string value)
        {
            if (null == value)
            {
                return new Tuple<string, DateTime>(key, DateTime.MinValue);
            }

            value = value.Trim();
            return 0 == value.Length
                       ? new Tuple<string, DateTime>(key, DateTime.MinValue)
                       : new Tuple<string, DateTime>(key, XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Utc));
        }

        private IEnumerable<Tuple<string, DateTime>> Dates()
        {
            yield return Date("addition", Addition);
            yield return Date("repetition", Repetition);
            yield return Date("alteration", Alteration);
            yield return Date("deletion", Deletion);
            yield return Date("restoration", Restoration);
        }
    }
}