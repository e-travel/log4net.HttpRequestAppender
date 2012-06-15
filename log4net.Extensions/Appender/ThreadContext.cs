using System;
using log4net.Util;

namespace log4net.Appender
{
    public class ThreadContext : Context
    {
        public ThreadContext(object dataSlot)
            : base(dataSlot)
        {
            _timeStamp = DateTime.Now;
        }

        public override DateTime Timestamp
        {
            get { return _timeStamp; }
        }

        public override GroupedEvents Events
        {
            get { return log4net.ThreadContext.Properties["xaxa"] as GroupedEvents; }
        }

        public override void AddEvents(GroupedEvents groupedEvents)
        {
            log4net.ThreadContext.Properties["xaxa"] = groupedEvents;
        }

        private readonly DateTime _timeStamp;
    }
}