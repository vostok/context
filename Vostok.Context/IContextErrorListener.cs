using System;
using JetBrains.Annotations;

namespace Vostok.Context
{
    /// <summary>
    /// Represents a listener subscribed to internal errors in ambient context management.
    /// </summary>
    [PublicAPI]
    public interface IContextErrorListener
    {
        /// <summary>
        /// This method gets called on every internal error. A good idea is to log those.
        /// </summary>
        void OnError([NotNull] string errorMessage, [CanBeNull] Exception exception);
    }
}