﻿using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Context
{
    // TODO(iloktionov): 1. Serialization/deserialization for distributed properties.
    // TODO(iloktionov): 2. Global typed properties as an alternative to named properties.

    /// <summary>
    /// <para>Allows to store and retrieve arbitrary objects from ambient context.</para>
    /// <para>Ambient context propagates by itself into synchronous and asynchronous call chains by the means of <see cref="ExecutionContext"/>.</para>
    /// <para></para>
    /// <para>See <see cref="Properties"/> and <see cref="IContextProperties"/> for more info on properties.</para>
    /// <para>See <see cref="Configuration"/> and <see cref="IContextConfiguration"/> for more info on configuration.</para>
    /// </summary>
    [PublicAPI]
    public static class FlowingContext
    {
        static FlowingContext()
        {
            Properties = new ContextProperties();
            Configuration = new ContextConfiguration();
        }

        [NotNull]
        public static IContextProperties Properties { get; }

        [NotNull]
        public static IContextConfiguration Configuration { get; }
    }
}