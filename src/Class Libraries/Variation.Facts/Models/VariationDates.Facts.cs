namespace Cavity.Models
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Xunit;
    using Xunit.Extensions;

    public sealed class VariationDatesFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<VariationDates>()
                            .IsValueType()
                            .Implements<ISerializable>()
                            .Implements<IEquatable<VariationDates>>()
                            .Serializable()
                            .Result);
        }

        [Fact]
        public void ctor()
        {
            Assert.NotNull(new VariationDates());
        }

        [Theory]
        [InlineData(null, null, null, null, null)]
        [InlineData("", "", "", "", "")]
        public void ctor_SerializationInfo_StreamingContext(string addition,
                                                            string repetition,
                                                            string alteration,
                                                            string deletion,
                                                            string restoration)
        {
            var expected = new VariationDates(addition, repetition, alteration, deletion, restoration);
            VariationDates actual;

            using (Stream stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, new VariationDates(addition, repetition, alteration, deletion, restoration));
                stream.Position = 0;
                actual = (VariationDates)formatter.Deserialize(stream);
            }

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null, null, null, null)]
        [InlineData("", "", "", "", "")]
        public void ctor_string_string_string_string_string(string addition,
                                                            string repetition,
                                                            string alteration,
                                                            string deletion,
                                                            string restoration)
        {
            Assert.NotNull(new VariationDates(addition, repetition, alteration, deletion, restoration));
        }

        [Theory]
        [InlineData(null, null, null, null, null)]
        [InlineData("", "", "", "", "")]
        public void opEquality_VariationDates_VariationDates(string addition,
                                                             string repetition,
                                                             string alteration,
                                                             string deletion,
                                                             string restoration)
        {
            var obj = new VariationDates(addition, repetition, alteration, deletion, restoration);
            var comparand = new VariationDates(addition, repetition, alteration, deletion, restoration);

            Assert.True(obj == comparand);
        }

        [Theory]
        [InlineData(null, null, null, null, null)]
        [InlineData("", "", "", "", "")]
        public void opInequality_VariationDates_VariationDates(string addition,
                                                               string repetition,
                                                               string alteration,
                                                               string deletion,
                                                               string restoration)
        {
            var obj = new VariationDates(addition, repetition, alteration, deletion, restoration);
            var comparand = new VariationDates(addition, repetition, alteration, deletion, restoration);

            Assert.False(obj != comparand);
        }

        [Fact]
        public void op_Equals_StringDifference()
        {
            var obj = new VariationDates();

            Assert.True(new VariationDates().Equals(obj));
        }

        [Fact]
        public void op_Equals_object()
        {
            object obj = new VariationDates();

            Assert.True(new VariationDates().Equals(obj));
        }

        [Theory]
        [InlineData("2000-01-05T00:00:00Z", null, null, null, null)]
        [InlineData(null, "2000-01-05T00:00:00Z", null, null, null)]
        [InlineData(null, null, "2000-01-05T00:00:00Z", null, null)]
        [InlineData(null, null, null, "2000-01-05T00:00:00Z", null)]
        [InlineData(null, null, null, null, "2000-01-05T00:00:00Z")]
        public void op_Equals_objectDiffers(string addition,
                                            string repetition,
                                            string alteration,
                                            string deletion,
                                            string restoration)
        {
            var obj = new VariationDates
                          {
                              Addition = addition,
                              Repetition = repetition,
                              Alteration = alteration,
                              Deletion = deletion,
                              Restoration = restoration
                          };

            Assert.False(new VariationDates().Equals(obj));
        }

        [Fact]
        public void op_Equals_objectInvalidCast()
        {
            var obj = new Uri("http://example.com/");

            // ReSharper disable SuspiciousTypeConversion.Global
            Assert.Throws<InvalidCastException>(() => new VariationDates().Equals(obj));

            // ReSharper restore SuspiciousTypeConversion.Global
        }

        [Fact]
        public void op_Equals_objectNull()
        {
            Assert.False(new VariationDates().Equals(null));
        }

        [Fact]
        public void op_GetHashCode()
        {
            const int expected = -745513406;
            var actual = new VariationDates().GetHashCode();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null, null, null, null, null)]
        [InlineData(null, "", null, null, null, null)]
        [InlineData(null, null, "", null, null, null)]
        [InlineData(null, null, null, "", null, null)]
        [InlineData(null, null, null, null, "", null)]
        [InlineData(null, null, null, null, null, "")]
        [InlineData("addition", "2000-01-05T00:00:00Z", null, null, null, null)]
        [InlineData("repetition", null, "2000-01-05T00:00:00Z", null, null, null)]
        [InlineData("alteration", null, null, "2000-01-05T00:00:00Z", null, null)]
        [InlineData("deletion", null, null, null, "2000-01-05T00:00:00Z", null)]
        [InlineData("restoration", null, null, null, null, "2000-01-05T00:00:00Z")]
        [InlineData("repetition", "2000-01-01T00:00:00Z", "2002-01-01T00:00:00Z", null, null, null)]
        [InlineData("alteration", "2000-01-01T00:00:00Z", "2002-01-01T00:00:00Z", "2003-01-01T00:00:00Z", null, null)]
        [InlineData("deletion", "2000-01-01T00:00:00Z", "2002-01-01T00:00:00Z", "2003-01-01T00:00:00Z", "2004-01-01T00:00:00Z", null)]
        [InlineData("restoration", "2000-01-01T00:00:00Z", "2002-01-01T00:00:00Z", "2003-01-01T00:00:00Z", "2004-01-01T00:00:00Z", "2005-01-01T00:00:00Z")]
        public void op_Variance(string expected,
                                string addition,
                                string repetition,
                                string alteration,
                                string deletion,
                                string restoration)
        {
            var actual = new VariationDates(addition, repetition, alteration, deletion, restoration).Variance();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void prop_Addition()
        {
            Assert.True(new PropertyExpectations<VariationDates>(x => x.Addition)
                            .IsAutoProperty<string>()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void prop_Alteration()
        {
            Assert.True(new PropertyExpectations<VariationDates>(x => x.Alteration)
                            .IsAutoProperty<string>()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void prop_Deletion()
        {
            Assert.True(new PropertyExpectations<VariationDates>(x => x.Deletion)
                            .IsAutoProperty<string>()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void prop_Repetition()
        {
            Assert.True(new PropertyExpectations<VariationDates>(x => x.Repetition)
                            .IsAutoProperty<string>()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void prop_Restoration()
        {
            Assert.True(new PropertyExpectations<VariationDates>(x => x.Restoration)
                            .IsAutoProperty<string>()
                            .IsNotDecorated()
                            .Result);
        }
    }
}