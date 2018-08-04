using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable AssignNullToNotNullAttribute

namespace Vostok.Context.Tests
{
    [TestFixture]
    internal class ContextProperties_Tests
    {
        private ContextProperties properties;

        [SetUp]
        public void TestSetup()
        {
            properties = new ContextProperties();
        }

        [Test]
        public void Set_should_not_tolerate_null_keys()
        {
            Action action = () => properties.Set(null, "value");

            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Set_should_create_new_properties_when_needed()
        {
            properties.Set("key1", "value1").Should().BeTrue();
            properties.Set("key2", "value2").Should().BeTrue();

            properties.Current.Should()
                .BeEquivalentTo(
                    new Dictionary<string, object>
                    {
                        ["key1"] = "value1",
                        ["key2"] = "value2"
                    });
        }

        [Test]
        public void Set_should_update_existing_properties_when_allowed()
        {
            properties.Set("key1", "value1").Should().BeTrue();
            properties.Set("key2", "value2").Should().BeTrue();

            properties.Set("key1", "value3").Should().BeTrue();
            properties.Set("key2", "value4").Should().BeTrue();

            properties.Current.Should()
                .BeEquivalentTo(
                    new Dictionary<string, object>
                    {
                        ["key1"] = "value3",
                        ["key2"] = "value4"
                    });
        }

        [Test]
        public void Set_should_not_update_existing_properties_when_forbidden()
        {
            properties.Set("key1", "value1").Should().BeTrue();
            properties.Set("key2", "value2").Should().BeTrue();

            properties.Set("key1", "value3", false).Should().BeFalse();
            properties.Set("key2", "value4", false).Should().BeFalse();

            properties.Current.Should()
                .BeEquivalentTo(
                    new Dictionary<string, object>
                    {
                        ["key1"] = "value1",
                        ["key2"] = "value2"
                    });
        }

        [Test]
        public void Set_should_be_case_sensitive_to_property_keys()
        {
            properties.Set("key", "value1").Should().BeTrue();
            properties.Set("KEY", "value2").Should().BeTrue();

            properties.Current.Should()
                .BeEquivalentTo(
                    new Dictionary<string, object>
                    {
                        ["key"] = "value1",
                        ["KEY"] = "value2"
                    });
        }

        [Test]
        public void Set_should_tolerate_null_values()
        {
            properties.Set("key1", null).Should().BeTrue();
            properties.Set("key2", null).Should().BeTrue();

            properties.Current.Should()
                .BeEquivalentTo(
                    new Dictionary<string, object>
                    {
                        ["key1"] = null,
                        ["key2"] = null
                    });
        }

        [Test]
        public void Remove_should_not_tolerate_null_keys()
        {
            Action action = () => properties.Remove(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Remove_should_delete_existing_properties()
        {
            properties.Set("key1", "value1");
            properties.Set("key2", "value2");

            properties.Remove("key2");

            properties.Current.Should()
                .BeEquivalentTo(
                    new Dictionary<string, object>
                    {
                        ["key1"] = "value1"
                    });
        }

        [Test]
        public void Remove_should_have_no_effect_on_non_existing_keys()
        {
            properties.Set("key1", "value1");
            properties.Set("key2", "value2");

            properties.Remove("key3");
            properties.Remove("key4");

            properties.Current.Should()
                .BeEquivalentTo(
                    new Dictionary<string, object>
                    {
                        ["key1"] = "value1",
                        ["key2"] = "value2",
                    });
        }

        [Test]
        public void Remove_should_be_case_sensitive_to_property_keys()
        {
            properties.Set("key", "value1");
            properties.Set("KEY", "value2");

            properties.Remove("key");

            properties.Current.Should()
                .BeEquivalentTo(
                    new Dictionary<string, object>
                    {
                        ["KEY"] = "value2"
                    });
        }

        [Test]
        public void Current_should_return_empty_snapshot_by_default()
        {
            properties.Current.Should().BeEmpty();
        }

        [Test]
        public void Current_snapshot_should_be_unaffected_by_further_sets_once_obtained()
        {
            properties.Set("key1", "value1");
            properties.Set("key2", "value2");

            var snapshot = properties.Current;

            properties.Set("key3", "value3");
            properties.Set("key2", "valueX");

            snapshot.Should()
                .BeEquivalentTo(
                    new Dictionary<string, object>
                    {
                        ["key1"] = "value1",
                        ["key2"] = "value2",
                    });
        }

        [Test]
        public void Current_snapshot_should_be_unaffected_by_further_removes_once_obtained()
        {
            properties.Set("key1", "value1");
            properties.Set("key2", "value2");

            var snapshot = properties.Current;

            properties.Remove("key1");
            properties.Remove("key2");

            snapshot.Should()
                .BeEquivalentTo(
                    new Dictionary<string, object>
                    {
                        ["key1"] = "value1",
                        ["key2"] = "value2",
                    });
        }

        [Test]
        public void Changes_made_after_running_a_new_task_should_not_affect_upstream_context()
        {
            properties.Set("key", "value");

            Task.Run(() => properties.Set("key", "value2")).Wait();

            properties.Current.Should()
                .BeEquivalentTo(new Dictionary<string, object> { ["key"] = "value" });
        }

        [Test]
        public void Changes_made_after_running_a_new_thread_should_not_affect_upstream_context()
        {
            properties.Set("key", "value");

            var thread = new Thread(() => properties.Set("key", "value2"));

            thread.Start();
            thread.Join();

            properties.Current.Should()
                .BeEquivalentTo(new Dictionary<string, object> { ["key"] = "value" });
        }

        [Test]
        public void Changes_made_after_entering_an_async_method_should_not_affect_upstream_context()
        {
            properties.Set("key", "value");

            SpoilContextAsync().Wait();

            properties.Current.Should()
                .BeEquivalentTo(new Dictionary<string, object> { ["key"] = "value" });
        }

        [Test]
        public void Changes_made_after_yielding_control_in_async_method_should_not_affect_upstream_context()
        {
            properties.Set("key", "value");

            YieldAndSpoilContextAsync().Wait();

            properties.Current.Should()
                .BeEquivalentTo(new Dictionary<string, object> { ["key"] = "value" });
        }

        [Test]
        public void Changes_made_after_sleeping_in_async_method_should_not_affect_upstream_context()
        {
            properties.Set("key", "value");

            SleepAndSpoilContextAsync().Wait();

            properties.Current.Should()
                .BeEquivalentTo(new Dictionary<string, object> { ["key"] = "value" });
        }

        private async Task SpoilContextAsync()
        {
            properties.Set("key", "value2");
        }

        private async Task YieldAndSpoilContextAsync()
        {
            await Task.Yield();

            properties.Set("key", "value2");
        }

        private async Task SleepAndSpoilContextAsync()
        {
            await Task.Delay(1);

            properties.Set("key", "value2");
        }
    }
}
