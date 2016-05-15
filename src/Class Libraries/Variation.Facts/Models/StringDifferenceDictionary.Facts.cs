namespace Cavity.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Cavity.Collections;
    using Xunit;
    using Xunit.Extensions;

    public sealed class StringDifferenceDictionaryFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<StringDifferenceDictionary>()
                            .DerivesFrom<Dictionary<string, StringDifference>>()
                            .IsConcreteClass()
                            .IsUnsealed()
                            .HasDefaultConstructor()
                            .Serializable()
                            .Result);
        }

        [Fact]
        public void ctor()
        {
            Assert.NotNull(new StringDifferenceDictionary());
        }

        [Fact]
        public void ctor_IEqualityComparerOfString()
        {
            Assert.NotNull(new StringDifferenceDictionary(StringComparer.Ordinal));
        }

        [Fact]
        public void ctor_IEqualityComparerOfStringNull()
        {
            Assert.NotNull(new StringDifferenceDictionary(null));
        }

        [Fact]
        public void ctor_SerializationInfo_StreamingContext()
        {
            var expected = new StringDifferenceDictionary
                               {
                                   { "example", new StringDifference("difference", "former", "latter") }
                               };
            StringDifferenceDictionary actual;

            using (Stream stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                var obj = new StringDifferenceDictionary
                              {
                                  { "example", new StringDifference("difference", "former", "latter") }
                              };
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                actual = (StringDifferenceDictionary)formatter.Deserialize(stream);
            }

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_Calculate_KeyStringDictionaryEmpty_KeyStringDictionary()
        {
            var former = new KeyStringDictionary();
            var latter = new KeyStringDictionary
                             {
                                 { "123", "latter" }
                             };

            var obj = StringDifferenceDictionary.Calculate(former, latter);

            Assert.Equal(1, obj.Count);
            Assert.Equal("alteration", obj["123"].Difference);
            Assert.Equal(null, obj["123"].Former);
            Assert.Equal("latter", obj["123"].Latter);
        }

        [Fact]
        public void op_Calculate_KeyStringDictionaryEmpty_KeyStringDictionaryEmpty()
        {
            Assert.Empty(StringDifferenceDictionary.Calculate(new KeyStringDictionary(), new KeyStringDictionary()));
        }

        [Fact]
        public void op_Calculate_KeyStringDictionaryEmpty_KeyStringDictionaryNull()
        {
            Assert.Empty(StringDifferenceDictionary.Calculate(new KeyStringDictionary(), null));
        }

        [Fact]
        public void op_Calculate_KeyStringDictionaryNull_KeyStringDictionary()
        {
            var latter = new KeyStringDictionary
                             {
                                 { "123", "latter" }
                             };

            var obj = StringDifferenceDictionary.Calculate(null, latter);

            Assert.Equal(1, obj.Count);
            Assert.Equal("alteration", obj["123"].Difference);
            Assert.Equal(null, obj["123"].Former);
            Assert.Equal("latter", obj["123"].Latter);
        }

        [Fact]
        public void op_Calculate_KeyStringDictionaryNull_KeyStringDictionaryEmpty()
        {
            Assert.Empty(StringDifferenceDictionary.Calculate(null, new KeyStringDictionary()));
        }

        [Fact]
        public void op_Calculate_KeyStringDictionaryNull_KeyStringDictionaryNull()
        {
            Assert.Null(StringDifferenceDictionary.Calculate(null, null));
        }

        [Fact]
        public void op_Calculate_KeyStringDictionary_KeyStringDictionary()
        {
            var former = new KeyStringDictionary
                             {
                                 { "123", "former" },
                                 { "456", "example" }
                             };
            var latter = new KeyStringDictionary
                             {
                                 { "456", "example" },
                                 { "789", "latter" }
                             };

            var obj = StringDifferenceDictionary.Calculate(former, latter);

            Assert.Equal(3, obj.Count);

            Assert.Equal(new StringDifference("alteration", "former", null), obj["123"]);
            Assert.Equal(new StringDifference("repetition", "example", "example"), obj["456"]);
            Assert.Equal(new StringDifference("alteration", null, "latter"), obj["789"]);
        }

        [Fact]
        public void op_Calculate_KeyStringDictionary_KeyStringDictionaryEmpty()
        {
            var former = new KeyStringDictionary
                             {
                                 { "123", "former" }
                             };
            var latter = new KeyStringDictionary();

            var obj = StringDifferenceDictionary.Calculate(former, latter);

            Assert.Equal(1, obj.Count);
            Assert.Equal("alteration", obj["123"].Difference);
            Assert.Equal("former", obj["123"].Former);
            Assert.Equal(null, obj["123"].Latter);
        }

        [Fact]
        public void op_Calculate_KeyStringDictionary_KeyStringDictionaryNull()
        {
            var former = new KeyStringDictionary
                             {
                                 { "123", "former" }
                             };

            var obj = StringDifferenceDictionary.Calculate(former, null);

            Assert.Equal(1, obj.Count);
            Assert.Equal("alteration", obj["123"].Difference);
            Assert.Equal("former", obj["123"].Former);
            Assert.Equal(null, obj["123"].Latter);
        }

        [Theory]
        [InlineData(null, "latter")]
        [InlineData("", "latter")]
        [InlineData("former", "latter")]
        [InlineData("former", "")]
        [InlineData("former", null)]
        public void op_Calculate_KeyStringDictionary_KeyStringDictionary_whereAlteration(string before,
                                                                                         string after)
        {
            var former = new KeyStringDictionary
                             {
                                 { "123", before }
                             };
            var latter = new KeyStringDictionary
                             {
                                 { "123", after }
                             };

            var obj = StringDifferenceDictionary.Calculate(former, latter);

            Assert.Equal(1, obj.Count);
            Assert.Equal("alteration", obj["123"].Difference);
            Assert.Equal(before, obj["123"].Former);
            Assert.Equal(after, obj["123"].Latter);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("example", "example")]
        [InlineData("Example", "example")]
        [InlineData("example", "Example")]
        public void op_Calculate_KeyStringDictionary_KeyStringDictionary_whereRepetition(string before,
                                                                                         string after)
        {
            var former = new KeyStringDictionary
                             {
                                 { "123", before }
                             };
            var latter = new KeyStringDictionary
                             {
                                 { "123", after }
                             };

            var obj = StringDifferenceDictionary.Calculate(former, latter);

            Assert.Equal(1, obj.Count);
            Assert.Equal("repetition", obj["123"].Difference);
            Assert.Equal(before, obj["123"].Former);
            Assert.Equal(after, obj["123"].Latter);
        }
    }
}