namespace Cavity.Models
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Xunit;
    using Xunit.Extensions;

    public sealed class StringDifferenceFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<StringDifference>()
                            .IsValueType()
                            .Implements<ISerializable>()
                            .Implements<IEquatable<StringDifference>>()
                            .Serializable()
                            .Result);
        }

        [Fact]
        public void ctor()
        {
            Assert.NotNull(new StringDifference());
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", "", "")]
        [InlineData("difference", "former", "latter")]
        public void ctor_SerializationInfo_StreamingContext(string difference,
                                                            string former,
                                                            string latter)
        {
            var expected = new StringDifference(difference, former, latter);
            StringDifference actual;

            using (Stream stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, new StringDifference(difference, former, latter));
                stream.Position = 0;
                actual = (StringDifference)formatter.Deserialize(stream);
            }

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", "", "")]
        [InlineData("difference", "former", "latter")]
        public void ctor_string_string_string(string difference,
                                              string former,
                                              string latter)
        {
            Assert.NotNull(new StringDifference(difference, former, latter));
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", "", "")]
        [InlineData("difference", "former", "latter")]
        public void opEquality_StringDifference_StringDifference(string difference,
                                                                 string former,
                                                                 string latter)
        {
            var obj = new StringDifference(difference, former, latter);
            var comparand = new StringDifference(difference, former, latter);

            Assert.True(obj == comparand);
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", "", "")]
        [InlineData("difference", "former", "latter")]
        public void opInequality_StringDifference_StringDifference(string difference,
                                                                   string former,
                                                                   string latter)
        {
            var obj = new StringDifference(difference, former, latter);
            var comparand = new StringDifference(difference, former, latter);

            Assert.False(obj != comparand);
        }

        [Fact]
        public void op_Equals_StringDifference()
        {
            var obj = new StringDifference();

            Assert.True(new StringDifference().Equals(obj));
        }

        [Fact]
        public void op_Equals_object()
        {
            object obj = new StringDifference();

            Assert.True(new StringDifference().Equals(obj));
        }

        [Theory]
        [InlineData("example", null, null)]
        [InlineData(null, "former", null)]
        [InlineData(null, null, "latter")]
        public void op_Equals_objectDiffers(string difference,
                                            string former,
                                            string latter)
        {
            var obj = new StringDifference
                          {
                              Difference = difference,
                              Former = former,
                              Latter = latter
                          };

            Assert.False(new StringDifference().Equals(obj));
        }

        [Fact]
        public void op_Equals_objectInvalidCast()
        {
            var obj = new Uri("http://example.com/");

            // ReSharper disable SuspiciousTypeConversion.Global
            Assert.Throws<InvalidCastException>(() => new StringDifference().Equals(obj));

            // ReSharper restore SuspiciousTypeConversion.Global
        }

        [Fact]
        public void op_Equals_objectNull()
        {
            Assert.False(new StringDifference().Equals(null));
        }

        [Fact]
        public void op_GetHashCode()
        {
            const int expected = -838944812;
            var actual = new StringDifference().GetHashCode();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void prop_Difference()
        {
            Assert.True(new PropertyExpectations<StringDifference>(x => x.Difference)
                            .IsAutoProperty<string>()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void prop_Former()
        {
            Assert.True(new PropertyExpectations<StringDifference>(x => x.Former)
                            .IsAutoProperty<string>()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void prop_Latter()
        {
            Assert.True(new PropertyExpectations<StringDifference>(x => x.Latter)
                            .IsAutoProperty<string>()
                            .IsNotDecorated()
                            .Result);
        }
    }
}