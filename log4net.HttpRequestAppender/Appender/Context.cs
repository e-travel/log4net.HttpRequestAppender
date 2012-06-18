using System;
using log4net.Util;

namespace log4net.Appender
{
    /// <summary>
    /// Abstract class defining a common interface for all different types of contexts
    /// </summary>
    public abstract class Context
    {
        protected Context(object dataSlot)
        {
            DataSlot = dataSlot;
        }

        public abstract DateTime Timestamp { get; }
        public abstract RenderedEvents Events { get; }
        public abstract void AddEvents(RenderedEvents renderedEvents);

        protected object DataSlot { get; private set; }
    }
}