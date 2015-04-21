using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using log4net.Core;
using log4net.Util;

namespace log4net.Appender
{
    [Serializable]
    public class SerializableLocationInfo
    {
        public string ClassName { get; set; }
        public string FileName { get; set; }
        public string LineNumber { get; set; }
        public string MethodName { get; set; }
    }

    [Serializable]
    public sealed class SerializableLevel
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public string DisplayName { get; set; }
    }

    [Serializable]
    public class SerializableLoggingEventData
    {
        public string LoggerName { get; set; }
        public SerializableLevel SerializableLevel { get; set; }
        public string Message { get; set; }
        public string ThreadName { get; set; }
        public DateTime TimeStamp { get; set; }
        public SerializableLocationInfo SerializableLocationInfo { get; set; }
        public string UserName { get; set; }
        public string Identity { get; set; }
        public string ExceptionString { get; set; }
        public string Domain { get; set; }
        public PropertiesDictionary Properties { get; set; }
    }

    public class ThreadContextAppender : ContextAppenderBase
    {
        public static SerializableLoggingEventData[] SerializableLoggingEventData
        {
            get
            {
                if (ContextBuffer == null)
                {
                    return new SerializableLoggingEventData[]{};
                }

                var builder = new LoggingEventBuilder();
                return ContextBuffer.Select(x => builder.Build(x.GetLoggingEventData())).ToArray();
            }
        }

        protected override IList<LoggingEvent> GetBuffer(bool create)
        {
            if (ContextBuffer != null)
            {
                return ContextBuffer;
            }

            ContextBuffer = new List<LoggingEvent>();
            return ContextBuffer;
        }

        private static IList<LoggingEvent> ContextBuffer 
        {
            get
            {
                return LogicalThreadContext.Properties[ContextBufferName] as IList<LoggingEvent>;
            }
            set
            {
                LogicalThreadContext.Properties[ContextBufferName] = value;
            }
        }

        internal static string ContextBufferName = "log_events";
    }
}
