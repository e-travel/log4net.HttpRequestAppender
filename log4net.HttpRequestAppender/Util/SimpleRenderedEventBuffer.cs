using System;
using System.Text;
using log4net.Core;

namespace log4net.Util
{
    public class SimpleRenderedEventBuffer
    {
        public delegate string Renderer(LoggingEvent logEvent);

        private readonly Renderer _renderer;


        private readonly StringBuilder _buffer;

        public bool IsEmpty { get { return _buffer.Length == 0; } }

        public SimpleRenderedEventBuffer(Renderer renderer)
        {
            _renderer = renderer;
            _buffer = new StringBuilder(1024);
        }

        public void AddEvent(LoggingEvent logEvent)
        {
            if (!TimeStamp.HasValue)
            {
                TimeStamp = logEvent.TimeStamp;
            }
            _buffer.Append(_renderer(logEvent));
        }

        public string GetRenderedEvents()
        {
            return _buffer.ToString();
        }

        public DateTime? TimeStamp { get; private set; }
    }
}
