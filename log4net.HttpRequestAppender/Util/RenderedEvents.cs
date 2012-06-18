using System;
using System.Text;
using log4net.Core;

namespace log4net.Util
{
    public class RenderedEvents
    {
        public delegate string Renderer(LoggingEvent logEvent);

        private LoggingEventData _firstEvent;
        private readonly Renderer _renderer;
        

        public StringBuilder Buffer { get; private set; }

        public bool IsEmpty { get { return Buffer.Length == 0; } }

        public RenderedEvents(DateTime requestStart,Renderer renderer)
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
            Buffer.Append(_renderer(logEvent));
        }


        public LoggingEvent GetEvent()
        {
            var duration = (DateTime.Now - _firstEvent.TimeStamp).TotalMilliseconds;
            duration = Math.Truncate(duration);
            _firstEvent.Message = duration + "ms" + Buffer;
            return new LoggingEvent(_firstEvent);
        }
    }
}
