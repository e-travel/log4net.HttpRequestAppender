using System;
using System.Web;
using log4net.Util;

namespace log4net.Appender
{
    public class HttpContext : Context
    {
        public HttpContext(object dataSlot) 
            : base(dataSlot)
        {}

        public override DateTime Timestamp
        {
            get { return System.Web.HttpContext.Current.Timestamp; }
        }

        public override GroupedEvents Events {
            get
            {
                return System.Web.HttpContext.Current.Items[DataSlot] != null
                           ? System.Web.HttpContext.Current.Items[DataSlot] as GroupedEvents
                           : null;
            }
        }

        public override void AddEvents(GroupedEvents groupedEvents)
        {
            System.Web.HttpContext.Current.Items[DataSlot] = groupedEvents;
        }
    }
}