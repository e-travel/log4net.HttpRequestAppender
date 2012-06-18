using System;
using log4net.Util;

namespace log4net.Appender
{
    /// <summary>
    /// Used to store temporary data during a http request
    /// </summary>
    public class HttpContext : Context
    {
        public HttpContext(object dataSlot) 
            : base(dataSlot)
        {}

        public override DateTime Timestamp
        {
            get { return System.Web.HttpContext.Current.Timestamp; }
        }

        public override RenderedEvents Events {
            get
            {
                return System.Web.HttpContext.Current.Items[DataSlot] != null
                           ? System.Web.HttpContext.Current.Items[DataSlot] as RenderedEvents
                           : null;
            }
        }

        public override void AddEvents(RenderedEvents renderedEvents)
        {
            System.Web.HttpContext.Current.Items[DataSlot] = renderedEvents;
        }
    }
}