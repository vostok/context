using System;
using JetBrains.Annotations;

namespace Vostok.Context
{
    internal static class FlowingContextSerializer
    {
        [CanBeNull]
        public static string SerializeGlobals(
            [NotNull] ContextGlobals globals,
            [NotNull] ContextConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        [CanBeNull]
        public static string SerializeProperties(
            [NotNull] ContextProperties properties,
            [NotNull] ContextConfiguration configuration)
        {
            throw new NotImplementedException();
        }
    }
}
