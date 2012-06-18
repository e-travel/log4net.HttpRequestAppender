using System;
using log4net.Util;

namespace log4net.Appender
{
    public class ThreadContext : Context
    {
        /// <summary>
        /// Used to temporary store data during a thread execution
        /// </summary>
        /// <param name="dataSlot"></param>
        public ThreadContext(object dataSlot)
            : base(dataSlot)
        {
            _timeStamp = DateTime.Now;
        }

        public override DateTime Timestamp
        {
            get { return _timeStamp; }
        }

        public override RenderedEvents Events
        {
            get { return log4net.ThreadContext.Properties["xaxa"] as RenderedEvents; }
        }

        public override void AddEvents(RenderedEvents renderedEvents)
        {
            log4net.ThreadContext.Properties["xaxa"] = renderedEvents;
        }

        private readonly DateTime _timeStamp;
    }
}