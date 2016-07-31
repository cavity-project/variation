namespace Cavity.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Cavity.Collections;
    using Xunit;
    using Xunit.Extensions;

    public sealed class StringDifferenceCollectionFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<StringDifferenceCollection>()
                            .DerivesFrom<object>()
                            .IsConcreteClass()
                            .IsSealed()
                            .HasDefaultConstructor()
                            .Serializable()
                            .Result);
        }

        [Fact]
        public void ctor()
        {
            Assert.NotNull(new StringDifferenceCollection());
        }

        [Fact]
        public void ctor_IEqualityComparerOfString()
        {
            Assert.NotNull(new StringDifferenceCollection(StringComparer.Ordinal));
        }

        [Fact]
        public void ctor_IEqualityComparerOfStringNull()
        {
            Assert.NotNull(new StringDifferenceCollection(null));
        }

        [Fact]
        public void ctor_SerializationInfo_StreamingContext()
        {
            var expected = new StringDifferenceCollection
                               {
                                   { "example", new StringDifference("difference", "former", "latter") }
                               };
            expected.Variation = new VariationDates("2000-01-05T00:00:00Z", string.Empty, string.Empty, string.Empty, string.Empty);
            StringDifferenceCollection actual;

            using (Stream stream = new MemoryStream())
            {
                var obj = new StringDifferenceCollection
                              {
                                  { "example", new StringDifference("difference", "former", "latter") }
                              };
                obj.Variation = new VariationDates("2000-01-05T00:00:00Z", string.Empty, string.Empty, string.Empty, string.Empty);

                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                actual = (StringDifferenceCollection)formatter.Deserialize(stream);
            }

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void opIndexer_string()
        {
            var expected = new StringDifference("difference", "former", "latter");
            var obj = new StringDifferenceCollection
                          {
                              { "example", expected }
                          };
            var actual = obj["example"];

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void opIndexer_stringEmpty(string key)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new StringDifferenceCollection()[key]);
        }

        [Fact]
        public void opIndexer_stringNotFound()
        {
            Assert.Throws<KeyNotFoundException>(() => new StringDifferenceCollection()["example"]);
        }

        [Fact]
        public void opIndexer_stringNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StringDifferenceCollection()[null]);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void op_Add_stringEmpty_StringDifference(string key)
        {
            var obj = new StringDifferenceCollection();
            var value = new StringDifference("difference", "former", "latter");

            Assert.Throws<ArgumentOutOfRangeException>(() => obj.Add(key, value));
        }

        [Fact]
        public void op_Add_stringExists_StringDifference()
        {
            var value = new StringDifference("difference", "former", "latter");
            var obj = new StringDifferenceCollection
                          {
                              { "example", value }
                          };

            Assert.Throws<ArgumentException>(() => obj.Add("example", value));
        }

        [Fact]
        public void op_Add_stringNull_StringDifference()
        {
            var obj = new StringDifferenceCollection();
            var value = new StringDifference("difference", "former", "latter");

            Assert.Throws<ArgumentNullException>(() => obj.Add(null, value));
        }

        [Fact]
        public void op_Add_string_StringDifference()
        {
            var obj = new StringDifferenceCollection();
            var expected = new StringDifference("difference", "former", "latter");
            obj.Add("example", expected);
            var actual = obj["example"];

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_Calculate_VariationDates_KeyStringDictionaryEmpty_KeyStringDictionary()
        {
            var former = new KeyStringDictionary();
            var latter = new KeyStringDictionary
                             {
                                 { "key", "latter" }
                             };

            var obj = StringDifferenceCollection.Calculate(new VariationDates(), former, latter);

            Assert.Equal(1, obj.Count);
            Assert.Equal("alteration", obj["key"].Difference);
            Assert.Equal(null, obj["key"].Former);
            Assert.Equal("latter", obj["key"].Latter);
        }

        [Fact]
        public void op_Calculate_VariationDates_KeyStringDictionaryEmpty_KeyStringDictionaryEmpty()
        {
            Assert.Empty(StringDifferenceCollection.Calculate(new VariationDates(), new KeyStringDictionary(), new KeyStringDictionary()));
        }

        [Fact]
        public void op_Calculate_VariationDates_KeyStringDictionaryEmpty_KeyStringDictionaryNull()
        {
            Assert.Empty(StringDifferenceCollection.Calculate(new VariationDates(), new KeyStringDictionary(), null));
        }

        [Fact]
        public void op_Calculate_VariationDates_KeyStringDictionaryNull_KeyStringDictionary()
        {
            var latter = new KeyStringDictionary
                             {
                                 { "key", "latter" }
                             };

            var obj = StringDifferenceCollection.Calculate(new VariationDates(), null, latter);

            Assert.Equal(1, obj.Count);
            Assert.Equal("alteration", obj["key"].Difference);
            Assert.Equal(null, obj["key"].Former);
            Assert.Equal("latter", obj["key"].Latter);
        }

        [Fact]
        public void op_Calculate_VariationDates_KeyStringDictionaryNull_KeyStringDictionaryEmpty()
        {
            Assert.Empty(StringDifferenceCollection.Calculate(new VariationDates(), null, new KeyStringDictionary()));
        }

        [Fact]
        public void op_Calculate_VariationDates_KeyStringDictionaryNull_KeyStringDictionaryNull()
        {
            Assert.Null(StringDifferenceCollection.Calculate(new VariationDates(), null, null));
        }

        [Fact]
        public void op_Calculate_VariationDates_KeyStringDictionary_KeyStringDictionary()
        {
            var former = new KeyStringDictionary
                             {
                                 { "A", "former" },
                                 { "B", "example" }
                             };
            var latter = new KeyStringDictionary
                             {
                                 { "B", "example" },
                                 { "C", "latter" }
                             };

            var obj = StringDifferenceCollection.Calculate(new VariationDates(), former, latter);

            Assert.Equal(3, obj.Count);

            Assert.Equal(new StringDifference("alteration", "former", null), obj["A"]);
            Assert.Equal(new StringDifference("repetition", "example", "example"), obj["B"]);
            Assert.Equal(new StringDifference("alteration", null, "latter"), obj["C"]);
        }

        [Fact]
        public void op_Calculate_VariationDates_KeyStringDictionary_KeyStringDictionaryEmpty()
        {
            var former = new KeyStringDictionary
                             {
                                 { "key", "former" }
                             };
            var latter = new KeyStringDictionary();

            var obj = StringDifferenceCollection.Calculate(new VariationDates(), former, latter);

            Assert.Equal(1, obj.Count);
            Assert.Equal("alteration", obj["key"].Difference);
            Assert.Equal("former", obj["key"].Former);
            Assert.Equal(null, obj["key"].Latter);
        }

        [Fact]
        public void op_Calculate_VariationDates_KeyStringDictionary_KeyStringDictionaryNull()
        {
            var former = new KeyStringDictionary
                             {
                                 { "key", "former" }
                             };

            var obj = StringDifferenceCollection.Calculate(new VariationDates(), former, null);

            Assert.Equal(1, obj.Count);
            Assert.Equal("alteration", obj["key"].Difference);
            Assert.Equal("former", obj["key"].Former);
            Assert.Equal(null, obj["key"].Latter);
        }

        [Theory]
        [InlineData(null, "latter")]
        [InlineData("", "latter")]
        [InlineData("former", "latter")]
        [InlineData("former", "")]
        [InlineData("former", null)]
        public void op_Calculate_VariationDates_KeyStringDictionary_KeyStringDictionary_whereAlteration(string before,
                                                                                                        string after)
        {
            var former = new KeyStringDictionary
                             {
                                 { "key", before }
                             };
            var latter = new KeyStringDictionary
                             {
                                 { "key", after }
                             };

            var obj = StringDifferenceCollection.Calculate(new VariationDates(), former, latter);

            Assert.Equal(1, obj.Count);
            Assert.Equal("alteration", obj["key"].Difference);
            Assert.Equal(before, obj["key"].Former);
            Assert.Equal(after, obj["key"].Latter);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("example", "example")]
        [InlineData("Example", "example")]
        [InlineData("example", "Example")]
        public void op_Calculate_VariationDates_KeyStringDictionary_KeyStringDictionary_whereRepetition(string before,
                                                                                                        string after)
        {
            var former = new KeyStringDictionary
                             {
                                 { "key", before }
                             };
            var latter = new KeyStringDictionary
                             {
                                 { "key", after }
                             };

            var obj = StringDifferenceCollection.Calculate(new VariationDates(), former, latter);

            Assert.Equal(1, obj.Count);
            Assert.Equal("repetition", obj["key"].Difference);
            Assert.Equal(before, obj["key"].Former);
            Assert.Equal(after, obj["key"].Latter);
        }

        [Fact]
        public void op_GetEnumerator()
        {
            const string key = "example";
            var value = new StringDifference("difference", "former", "latter");
            var obj = new StringDifferenceCollection
                          {
                              { key, value }
                          };

            foreach (var item in obj)
            {
                Assert.Equal(key, item.Key);
                Assert.Equal(value, item.Value);
            }
        }

        [Fact]
        public void prop_Count()
        {
            Assert.True(new PropertyExpectations<StringDifferenceCollection>(x => x.Count)
                            .TypeIs<int>()
                            .DefaultValueIs(0)
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void prop_Variation()
        {
            Assert.True(new PropertyExpectations<StringDifferenceCollection>(x => x.Variation)
                            .IsAutoProperty<VariationDates>()
                            .IsNotDecorated()
                            .Result);
        }
    }
}