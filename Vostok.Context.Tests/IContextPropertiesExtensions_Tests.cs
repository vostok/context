using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Context.Tests
{
    [TestFixture]
    internal class IContextPropertiesExtensions_Tests
    {
        private IContextProperties properties;

        [SetUp]
        public void TestSetup()
        {
            properties = new ContextProperties();
        }

        [Test]
        public void Get_should_return_default_value_when_no_property_with_given_name_exists()
        {
            properties.Get("name", "default").Should().Be("default");
        }

        [Test]
        public void Get_should_return_default_value_when_property_value_has_unexpected_type()
        {
            properties.Set("name", new object());

            properties.Get("name", "default").Should().Be("default");
        }

        [Test]
        public void Get_should_return_current_value_when_it_has_matching_type()
        {
            properties.Set("name", "value");

            properties.Get("name", "default").Should().Be("value");
        }

        [Test]
        public void Use_should_be_able_to_temporarily_create_a_new_property()
        {
            using (properties.Use("name", "value"))
            {
                properties.Get<string>("name").Should().Be("value");
            }

            properties.Get<string>("name").Should().BeNull();
        }

        [Test]
        public void Use_should_be_able_to_temporarily_update_property_value()
        {
            properties.Set("name", "value1");

            using (properties.Use("name", "value2"))
            {
                properties.Get<string>("name").Should().Be("value2");
            }

            properties.Get<string>("name").Should().Be("value1");
        }

        [Test]
        public void Use_should_be_able_to_temporarily_create_several_new_properties()
        {
            using (properties.Use(
                new Dictionary<string, object>
                {
                    ["name1"] = "value1",
                    ["name2"] = "value2"
                }))
            {
                properties.Get<string>("name1").Should().Be("value1");
                properties.Get<string>("name2").Should().Be("value2");
            }

            properties.Get<string>("name1").Should().BeNull();
            properties.Get<string>("name2").Should().BeNull();
        }

        [Test]
        public void Use_should_be_able_to_temporarily_update_several_property_values()
        {
            properties.Set("name1", "value1");
            properties.Set("name2", "value2");

            using (properties.Use(
                new Dictionary<string, object>
                {
                    ["name1"] = "value3",
                    ["name2"] = "value4"
                }))
            {
                properties.Get<string>("name1").Should().Be("value3");
                properties.Get<string>("name2").Should().Be("value4");
            }

            properties.Get<string>("name1").Should().Be("value1");
            properties.Get<string>("name2").Should().Be("value2");
        }
    }
}
