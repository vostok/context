using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Context.Helpers;

namespace Vostok.Context.Tests
{
    [TestFixture]
    internal class BinaryStringReadWrite_Tests
    {
        [Test]
        public void Reader_should_correctly_read_strings_written_by_writer()
        {
            var writer = new BinaryStringWriter(1);

            writer.Write("");
            writer.Write("Hello!");
            writer.Write("Привет!");

            var base64 = writer.ToBase64String();

            var reader = new BinaryStringReader(Convert.FromBase64String(base64));

            reader.Read().Should().Be("");
            reader.Read().Should().Be("Hello!");
            reader.Read().Should().Be("Привет!");

            reader.HasDataLeft.Should().BeFalse();
        }
    }
}