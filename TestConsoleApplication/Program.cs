using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using log4net;
using log4net.Appender;

namespace TestConsoleApplication
{
    public class Program
    {
        protected static ILog Log = LogManager.GetLogger(typeof (Program));

        static void Main()
        {
            LogicalThreadContext.Properties["searchid"] = 123;
            LogicalThreadContext.Properties["brand"] = "brand?";
            LogicalThreadContext.Properties["language"] = "greek";
            LogicalThreadContext.Properties["message"] = "here is my message";

            Log.Info("We're in the main function 1");
            Log.Info("We're in the main function 2");
            var serializedLoggingEventsData = new JavaScriptSerializer().Serialize(ThreadContextAppender.SerializableLoggingEventData);
            Log.Info("We're in the main function 3");

            var deserializedSerializableLoggingEventData = new JavaScriptSerializer().Deserialize<IList<SerializableLoggingEventData>>(serializedLoggingEventsData);

            var httpRequestAppender = Log.GetAppender<HttpRequestAppender>();
            if (httpRequestAppender != null)
            {
                httpRequestAppender.AppendLoggingEvents(deserializedSerializableLoggingEventData);
            }
        }
    }
}
