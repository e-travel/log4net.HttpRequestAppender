using System;
using log4net.Util;

namespace log4net.Appender
{
    public abstract class Context
    {
        protected Context(object dataSlot)
        {
            DataSlot = dataSlot;
        }

        public abstract DateTime Timestamp { get; }
        public abstract GroupedEvents Events { get; }
        public abstract void AddEvents(GroupedEvents groupedEvents);

        protected object DataSlot { get; private set; }
    }
}