namespace Cavity.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Cavity.Collections;

    [Serializable]
    public class StringDifferenceDictionary : Dictionary<string, StringDifference>
    {
        public StringDifferenceDictionary()
            : this(StringComparer.OrdinalIgnoreCase)
        {
        }

        public StringDifferenceDictionary(IEqualityComparer<string> comparer)
            : base(comparer)
        {
        }

        protected StringDifferenceDictionary(SerializationInfo info,
                                             StreamingContext context)
            : base(info, context)
        {
        }

        public static StringDifferenceDictionary Calculate(KeyStringDictionary former,
                                                           KeyStringDictionary latter)
        {
            if (null == (former ?? latter))
            {
                return null;
            }

            var result = new StringDifferenceDictionary();

            var keys = new[] { former, latter }.Where(dictionary => dictionary.IsNotNull())
                                               .SelectMany(dictionary => dictionary.Keys)
                                               .ToHashSet(result.Comparer);
            foreach (var key in keys)
            {
                var a = null == former || former.NotContainsKey(key) ? null : former[key];
                var b = null == latter || latter.NotContainsKey(key) ? null : latter[key];
                var difference = result.Comparer.Equals(a, b) ? "repetition" : "alteration";
                result.Add(key, new StringDifference(difference, a, b));
            }

            return result;
        }
    }
}