using System;
using JetBrains.Annotations;

namespace Vostok.Context
{
    internal static class FlowingContextSerializer
    {
        [CanBeNull]
        public static string SerializeGlobals(
            [NotNull] IContextGlobals globals,
            [NotNull] ContextConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        [CanBeNull]
        public static string SerializeProperties(
            [NotNull] IContextProperties properties,
            [NotNull] ContextConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public static void RestoreGlobals(
            [NotNull] string input,
            [NotNull] IContextGlobals globals,
            [NotNull] ContextConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public static void RestoreProperties(
            [NotNull] string input,
            [NotNull] IContextProperties properties,
            [NotNull] ContextConfiguration configuration)
        {
            throw new NotImplementedException();
        }
    }
}
