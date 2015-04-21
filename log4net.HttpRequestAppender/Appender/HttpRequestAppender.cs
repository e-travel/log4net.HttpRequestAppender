using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net.Core;
using log4net.Web;

namespace log4net.Appender
{
    /// <summary>
    /// Buffer logs during an HttpRequest and the forwards them to another appender as one log event
    /// </summary>
    public class HttpRequestAppender : ContextAppenderBase
    {
        public void AppendLoggingEvents(IEnumerable<SerializableLoggingEventData> serializableLoggingEventsData)
        {
            var builder = new LoggingEventBuilder();
            serializableLoggingEventsData.Select(builder.Build)
                                         .ToList()
                                         .ForEach(Append);
        }

        protected override bool RequiresLayout { get { return true; } }

        public override void ActivateOptions()
        {
            base.ActivateOptions();

            Log4NetHttpModule.EndRequest += OnEndRequest;
        }
        
        private void OnEndRequest(object sender, EventArgs e)
        {
            var buffer = GetBuffer(false);
            if (buffer == null || !buffer.Any())
                return;

            // Ok, we got the buffer, prepare the event.
            var context = GetHttpContext();
            var duration = DateTime.Now - context.Timestamp;
            var renderedEvents = String.Join("", buffer.OrderBy(x => x.TimeStamp).Select(RenderLoggingEvent));

            var logEvent = new LoggingEvent(new LoggingEventData
                                                {
                                                    Level = Level.Info,
                                                    Message = duration.TotalMilliseconds.ToString("F0") + "ms" + renderedEvents
                                                });

            SendBuffer(logEvent);
        }

        private static HttpContextBase GetHttpContext()
        {
            var context = HttpContext.Current;
            if (context == null)
                return null;
            return new HttpContextWrapper(HttpContext.Current);
        }

        protected override IList<LoggingEvent> GetBuffer(bool create)
        {
            var context = GetHttpContext();
            if (context == null)
                return null;
            var buffer = context.Items[_dataSlot] as IList<LoggingEvent>;

            if (buffer == null && create)
            {
                buffer = new List<LoggingEvent>();
                context.Items[_dataSlot] = buffer;
            }

            return buffer;
        }

        private readonly object _dataSlot = new object();
    }
}