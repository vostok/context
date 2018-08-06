using System;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Context.Tests
{
    [TestFixture]
    internal class IContextGlobalsExtensions_Tests
    {
        private IContextGlobals globals;

        [SetUp]
        public void TestSetup()
        {
            globals = new ContextGlobals();
        }

        [Test]
        public void Use_should_be_able_to_temporarily_update_the_value_of_a_global()
        {
            var originalValue = Guid.NewGuid().ToString();
            var temporaryValue = Guid.NewGuid().ToString();

            globals.Set(originalValue);

            using (globals.Use(temporaryValue))
            {
                globals.Get<string>().Should().Be(temporaryValue);
            }

            globals.Get<string>().Should().Be(originalValue);
        }
    }
}