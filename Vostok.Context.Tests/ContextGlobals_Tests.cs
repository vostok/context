using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Vostok.Context.Tests
{
    [TestFixture]
    internal class ContextGlobals_Tests
    {
        private ContextGlobals globals;

        [SetUp]
        public void TestSetup()
        {
            globals = new ContextGlobals();
        }

        [Test]
        public void Get_should_return_a_default_value_if_nothing_was_set_earlier()
        {
            globals.Get<Type1>().Should().BeNull();
        }

        [Test]
        public void Get_should_return_current_value()
        {
            globals.Set(new Type2("value1"));
            globals.Set(new Type2("value2"));

            globals.Get<Type2>()?.Value.Should().Be("value2");
        }

        [Test]
        public void Get_can_return_null_value()
        {
            globals.Set(new Type2("value1"));
            globals.Set<Type2>(null);

            globals.Get<Type2>()?.Value.Should().BeNull();
        }

        [Test]
        public void Changes_made_after_running_a_new_task_should_not_affect_upstream_context()
        {
            globals.Set(new Type2("value1"));

            Task.Run(() => globals.Set(new Type2("value2"))).Wait();

            globals.Get<Type2>()?.Value.Should().Be("value1");
        }

        [Test]
        public void Changes_made_after_running_a_new_thread_should_not_affect_upstream_context()
        {
            globals.Set(new Type2("value1"));

            var thread = new Thread(() => globals.Set(new Type2("value2")));

            thread.Start();
            thread.Join();

            globals.Get<Type2>()?.Value.Should().Be("value1");
        }

        [Test]
        public void Changes_made_after_entering_an_async_method_should_not_affect_upstream_context()
        {
            globals.Set(new Type2("value1"));

            SpoilContextAsync().Wait();

            globals.Get<Type2>()?.Value.Should().Be("value1");
        }

        [Test]
        public void Changes_made_after_yielding_control_in_async_method_should_not_affect_upstream_context()
        {
            globals.Set(new Type2("value1"));

            YieldAndSpoilContextAsync().Wait();

            globals.Get<Type2>()?.Value.Should().Be("value1");
        }

        [Test]
        public void Changes_made_after_sleeping_in_async_method_should_not_affect_upstream_context()
        {
            globals.Set(new Type2("value1"));

            SleepAndSpoilContextAsync().Wait();

            globals.Get<Type2>()?.Value.Should().Be("value1");
        }

        [Test]
        public void Get_and_set_with_runtime_type_parameters_should_work_correctly_with_each_other()
        {
            globals.Set(typeof(Type2), new Type2("custom-value"));

            globals.Get(typeof(Type2)).Should().BeOfType<Type2>().Which.Value.Should().Be("custom-value");
        }

        [Test]
        public void Get_with_runtime_type_parameters_should_work_correctly_with_ordinary_set()
        {
            globals.Set(new Type2("7C0D64CF-9CBD-4B28-A45B-236760770271"));

            globals.Get(typeof(Type2)).Should().BeOfType<Type2>().Which.Value.Should().Be("7C0D64CF-9CBD-4B28-A45B-236760770271");
        }

        [Test]
        public void Set_with_runtime_type_parameters_should_work_correctly_with_ordinary_get()
        {
            globals.Set(typeof(Type2), new Type2("1E466DED-FAF7-46BD-97B7-5372A040D16F"));

            globals.Get<Type2>()?.Value.Should().Be("1E466DED-FAF7-46BD-97B7-5372A040D16F");
        }

        private async Task SpoilContextAsync()
        {
            globals.Set(new Type2("value2"));
        }

        private async Task YieldAndSpoilContextAsync()
        {
            await Task.Yield();

            globals.Set(new Type2("value2"));
        }

        private async Task SleepAndSpoilContextAsync()
        {
            await Task.Delay(1);

            globals.Set(new Type2("value2"));
        }

        [UsedImplicitly]
        private class Type1 { }

        private class Type2
        {
            public Type2(string value)
            {
                Value = value;
            }

            [UsedImplicitly]
            public string Value { get; }
        }
    }
}