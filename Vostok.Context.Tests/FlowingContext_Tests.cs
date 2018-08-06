using System;
using System.Net;
using FluentAssertions;
using FluentAssertions.Extensions;
using NSubstitute;
using NUnit.Framework;

// ReSharper disable AssignNullToNotNullAttribute

namespace Vostok.Context.Tests
{
    [TestFixture]
    internal class FlowingContext_Tests
    {
        private Action<string, Exception> errorCallback;
        private string name1;
        private string name2;
        private string name3;
        private string name4;
        private string name5;

        [SetUp]
        public void TestSetup()
        {
            errorCallback = Substitute.For<Action<string, Exception>>();
            errorCallback
                .When(e => e.Invoke(Arg.Any<string>(), Arg.Any<Exception>()))
                .Do(
                    info =>
                    {
                        Console.Out.WriteLine(info.Arg<string>());
                        Console.Out.WriteLine(info.Arg<Exception>());
                    });

            FlowingContext.Configuration.ErrorCallback = errorCallback;
            FlowingContext.Configuration.ClearDistributedGlobals();
            FlowingContext.Configuration.ClearDistributedProperties();

            name1 = Guid.NewGuid().ToString();
            name2 = Guid.NewGuid().ToString();
            name3 = Guid.NewGuid().ToString();
            name4 = Guid.NewGuid().ToString();
            name5 = Guid.NewGuid().ToString();
        }

        [Test]
        public void SerializeDistributedGlobals_should_return_null_when_no_distributed_globals_are_registered()
        {
            FlowingContext.Globals.Set("whatever");

            FlowingContext.SerializeDistributedGlobals().Should().BeNull();
        }

        [Test]
        public void SerializeDistributedGlobals_should_return_null_when_all_registered_distributed_globals_have_no_values()
        {
            FlowingContext.Globals.Set("whatever");

            FlowingContext.Configuration.RegisterDistributedGlobal(name1, ContextSerializers.Uri);

            FlowingContext.SerializeDistributedGlobals().Should().BeNull();
        }

         [Test]
         public void SerializeDistributedGlobals_should_return_null_when_all_registered_distributed_globals_have_null_values()
         {
             FlowingContext.Configuration.RegisterDistributedGlobal(name1, ContextSerializers.Uri);
        
             FlowingContext.Globals.Set(null as Uri);
        
             FlowingContext.SerializeDistributedGlobals().Should().BeNull();
         }
        
         [Test]
         public void SerializeDistributedProperties_should_return_null_when_no_distributed_properties_are_registered()
         {
             FlowingContext.SerializeDistributedProperties().Should().BeNull();
         }
        
         [Test]
         public void SerializeDistributedProperties_should_return_null_when_all_registered_distributed_properties_have_no_values()
         {
             FlowingContext.Configuration.RegisterDistributedProperty(name1, ContextSerializers.Uri);
        
             FlowingContext.SerializeDistributedProperties().Should().BeNull();
         }
        
         [Test]
         public void SerializeDistributedProperties_should_return_null_when_all_registered_distributed_properties_have_null_values()
         {
             FlowingContext.Configuration.RegisterDistributedProperty(name1, ContextSerializers.Uri);
        
             FlowingContext.Properties.Set(name1, null);
        
             FlowingContext.SerializeDistributedProperties().Should().BeNull();
         }

        [Test]
        public void RestoreDistributedGlobals_should_not_fail_on_null_input()
        {
            FlowingContext.RestoreDistributedGlobals(null);
        }

        [Test]
        public void RestoreDistributedGlobals_should_not_fail_on_incorrect_input()
        {
            FlowingContext.RestoreDistributedGlobals("whatever");

            errorCallback.Received(1).Invoke(Arg.Any<string>(), Arg.Any<Exception>());
        }

        [Test]
        public void RestoreDistributedProperties_should_not_fail_on_null_input()
        {
            FlowingContext.RestoreDistributedProperties(null);
        }

        [Test]
        public void RestoreDistributedProperties_should_not_fail_on_incorrect_input()
        {
            FlowingContext.RestoreDistributedProperties("whatever");

            errorCallback.Received(1).Invoke(Arg.Any<string>(), Arg.Any<Exception>());
        }

        [Test]
         public void Should_correctly_serialize_and_restore_distributed_globals_according_to_whitelist()
         {
             FlowingContext.Configuration.RegisterDistributedGlobal(name1, ContextSerializers.Uri);
             FlowingContext.Configuration.RegisterDistributedGlobal(name2, ContextSerializers.TimeSpan);
             FlowingContext.Configuration.RegisterDistributedGlobal(name4, ContextSerializers.IPAddress);
        
             FlowingContext.Globals.Set(new Uri("https://kontur.ru"));
             FlowingContext.Globals.Set(5.Hours());
             FlowingContext.Globals.Set(123);
        
             var serialized = FlowingContext.SerializeDistributedGlobals();

             // (iloktionov): Now spoil all the globals:
             FlowingContext.Globals.Set(null as Uri);
             FlowingContext.Globals.Set(default(TimeSpan));
             FlowingContext.Globals.Set(default(int));
             FlowingContext.Globals.Set(IPAddress.Loopback);
        
             FlowingContext.RestoreDistributedGlobals(serialized);
        
             FlowingContext.Globals.Get<Uri>().Should().Be(new Uri("https://kontur.ru"));
             FlowingContext.Globals.Get<TimeSpan>().Should().Be(5.Hours());
             FlowingContext.Globals.Get<int>().Should().Be(0); // should not get restored due to whitelist
             FlowingContext.Globals.Get<IPAddress>().Should().Be(IPAddress.Loopback); // should not get restored due to null value
         }

        [Test]
        public void Should_correctly_serialize_and_restore_distributed_properties_according_to_whitelist()
        {
            FlowingContext.Configuration.RegisterDistributedProperty(name1, ContextSerializers.String);
            FlowingContext.Configuration.RegisterDistributedProperty(name2, ContextSerializers.String);
            FlowingContext.Configuration.RegisterDistributedProperty(name4, ContextSerializers.String);
            FlowingContext.Configuration.RegisterDistributedProperty(name5, ContextSerializers.String);

            FlowingContext.Properties.Set(name1, "value1");
            FlowingContext.Properties.Set(name2, "value2");
            FlowingContext.Properties.Set(name3, "value3");
            FlowingContext.Properties.Set(name4, null);

            var serialized = FlowingContext.SerializeDistributedProperties();

            // (iloktionov): Now spoil all the properties:
            FlowingContext.Properties.Set(name1, null);
            FlowingContext.Properties.Set(name2, null);
            FlowingContext.Properties.Set(name3, null);
            FlowingContext.Properties.Set(name4, "value4");
            FlowingContext.Properties.Set(name5, "value5");

            FlowingContext.RestoreDistributedProperties(serialized);

            FlowingContext.Properties.Get<string>(name1).Should().Be("value1");
            FlowingContext.Properties.Get<string>(name2).Should().Be("value2");
            FlowingContext.Properties.Get<string>(name3).Should().BeNull(); // should not get restored due to whitelist
            FlowingContext.Properties.Get<string>(name4).Should().Be("value4"); // should not get restored due to null value
            FlowingContext.Properties.Get<string>(name5).Should().Be("value5"); // should not get restored due absence in original properties
        }

        [Test]
        public void Should_skip_values_with_failing_serializers_during_globals_serialization_and_report_errors_to_listener()
        {
            FlowingContext.Configuration.RegisterDistributedGlobal(name1, new FailingSerializer<Uri>());
            FlowingContext.Configuration.RegisterDistributedGlobal(name2, ContextSerializers.String);

            FlowingContext.Globals.Set(new Uri("https://kontur.ru"));
            FlowingContext.Globals.Set("whatever");

            var serialized = FlowingContext.SerializeDistributedGlobals();

            // (iloktionov): Now spoil all the globals:
            FlowingContext.Globals.Set(null as Uri);
            FlowingContext.Globals.Set(null as string);

            FlowingContext.RestoreDistributedGlobals(serialized);

            FlowingContext.Globals.Get<Uri>().Should().BeNull();
            FlowingContext.Globals.Get<string>().Should().Be("whatever");

            errorCallback.Received(1).Invoke(Arg.Any<string>(), Arg.Any<Exception>());
        }

        [Test]
        public void Should_skip_values_with_failing_serializers_during_properties_serialization_and_report_errors_to_listener()
        {
            FlowingContext.Configuration.RegisterDistributedProperty(name1, new FailingSerializer<string>());
            FlowingContext.Configuration.RegisterDistributedProperty(name2, ContextSerializers.String);

            FlowingContext.Properties.Set(name1, "value1");
            FlowingContext.Properties.Set(name2, "value2");

            var serialized = FlowingContext.SerializeDistributedProperties();

            // (iloktionov): Now spoil all the properties:
            FlowingContext.Properties.Set(name1, null);
            FlowingContext.Properties.Set(name2, null);

            FlowingContext.RestoreDistributedProperties(serialized);

            FlowingContext.Properties.Get<string>(name1).Should().BeNull();
            FlowingContext.Properties.Get<string>(name2).Should().Be("value2");

            errorCallback.Received(1).Invoke(Arg.Any<string>(), Arg.Any<Exception>());
        }

        [Test]
        public void Should_skip_values_with_failing_deserializers_during_globals_deserialization_and_report_errors_to_listener()
        {
            FlowingContext.Configuration.RegisterDistributedGlobal(name1, ContextSerializers.String);
            FlowingContext.Configuration.RegisterDistributedGlobal(name2, ContextSerializers.String);

            FlowingContext.Globals.Set(new Uri("https://kontur.ru"));
            FlowingContext.Globals.Set("whatever");

            var serialized = FlowingContext.SerializeDistributedGlobals();

            // (iloktionov): Now spoil all the globals:
            FlowingContext.Globals.Set(null as Uri);
            FlowingContext.Globals.Set(null as string);

            FlowingContext.Configuration.RegisterDistributedGlobal(name1, new FailingSerializer<Uri>());
            FlowingContext.RestoreDistributedGlobals(serialized);

            FlowingContext.Globals.Get<Uri>().Should().BeNull();
            FlowingContext.Globals.Get<string>().Should().Be("whatever");

            errorCallback.Received(1).Invoke(Arg.Any<string>(), Arg.Any<Exception>());
        }

        [Test]
        public void Should_skip_values_with_failing_deserializers_during_properties_deserialization_and_report_errors_to_listener()
        {
            FlowingContext.Configuration.RegisterDistributedProperty(name1, ContextSerializers.String);
            FlowingContext.Configuration.RegisterDistributedProperty(name2, ContextSerializers.String);

            FlowingContext.Properties.Set(name1, "value1");
            FlowingContext.Properties.Set(name2, "value2");

            var serialized = FlowingContext.SerializeDistributedProperties();

            // (iloktionov): Now spoil all the properties:
            FlowingContext.Properties.Set(name1, null);
            FlowingContext.Properties.Set(name2, null);

            FlowingContext.Configuration.RegisterDistributedProperty(name1, new FailingSerializer<string>());
            FlowingContext.RestoreDistributedProperties(serialized);

            FlowingContext.Properties.Get<string>(name1).Should().BeNull();
            FlowingContext.Properties.Get<string>(name2).Should().Be("value2");

            errorCallback.Received(1).Invoke(Arg.Any<string>(), Arg.Any<Exception>());
        }

        private class FailingSerializer<T> : IContextSerializer<T>
        {
            public string Serialize(T value)
            {
                throw new Exception("I fail, I'm disgusting.");
            }

            public T Deserialize(string input)
            {
                throw new Exception("I fail, I'm disgusting.");
            }
        }
    }
}