using System.Text;
using log4net.Core;

namespace log4net.Appender
{
    public class SimpleRenderedEventBuffer
    {
        public delegate string Renderer(LoggingEvent logEvent);

        private readonly Renderer _renderer;
        

        public StringBuilder Buffer { get; private set; }

        public bool IsEmpty { get { return Buffer.Length == 0; } }

        public SimpleRenderedEventBuffer(Renderer renderer)
        {
            _renderer = renderer;
            Buffer = new StringBuilder(1024);
        }

        public void AddEvent(LoggingEvent logEvent)
        {
            Buffer.Append(_renderer(logEvent));
        }

        public string GetRenderedEvents()
        {
            return Buffer.ToString();
        }
    }
}
