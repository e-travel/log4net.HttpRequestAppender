using System;
using System.Collections.Generic;
using System.Text;
using log4net.Core;

namespace log4net.Util
{
    public class GroupedEvents
    {
        public delegate string Renderer(LoggingEvent logEvent);

        private LoggingEventData _firstEvent;
        private readonly Renderer _renderer;
        

        public StringBuilder Buffer { get; private set; }

        public bool IsEmpty { get { return Buffer.Length == 0; } }

        public GroupedEvents(DateTime requestStart,Renderer renderer)
        {
            _firstEvent = new LoggingEventData
                              {
                                  TimeStamp = requestStart,
                                  Level = Level.Info
                              };
            _renderer = renderer;
            Buffer = new StringBuilder(1024);
        }

        public void AddEvent(LoggingEvent logEvent)
        {
            this.Buffer.Append(_renderer(logEvent));
        }


        public LoggingEvent GetEvent()
        {
            var duration = (DateTime.Now - _firstEvent.TimeStamp).TotalMilliseconds;
            duration = Math.Truncate(duration);
            _firstEvent.Message = duration + "ms" + this.Buffer;
            return new LoggingEvent(_firstEvent);
        }
    }
}
