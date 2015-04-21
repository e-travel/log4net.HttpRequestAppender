using System;
using System.Linq;
using log4net.Appender;
using log4net.Core;
using log4net.Util;

namespace log4net
{
    public class LoggingEventBuilder
    {
        public LoggingEvent Build(SerializableLoggingEventData serializableLoggingEventData)
        {
            return new LoggingEvent(new LoggingEventData
                                        {
                                            LoggerName = serializableLoggingEventData.LoggerName,
                                            Level = new Level(serializableLoggingEventData.SerializableLevel.Value,
                                                serializableLoggingEventData.SerializableLevel.Name,
                                                serializableLoggingEventData.SerializableLevel.DisplayName),
                                            Message = serializableLoggingEventData.Message,
                                            ThreadName = serializableLoggingEventData.ThreadName,
                                            TimeStamp = serializableLoggingEventData.TimeStamp,
                                            LocationInfo = serializableLoggingEventData.SerializableLocationInfo != null
                                                ? new LocationInfo(serializableLoggingEventData.SerializableLocationInfo.ClassName,
                                                    serializableLoggingEventData.SerializableLocationInfo.MethodName,
                                                    serializableLoggingEventData.SerializableLocationInfo.FileName,
                                                    serializableLoggingEventData.SerializableLocationInfo.LineNumber)
                                                : null,
                                            UserName = serializableLoggingEventData.UserName,
                                            Identity = serializableLoggingEventData.Identity,
                                            ExceptionString = serializableLoggingEventData.ExceptionString,
                                            Domain = serializableLoggingEventData.Domain,
                                            Properties = serializableLoggingEventData.Properties
                                        });
        }

        public SerializableLoggingEventData Build(LoggingEventData loggingEventData)
        {
            var properties = new PropertiesDictionary();
            var validPropertyNames = loggingEventData.Properties
                                                     .GetKeys()
                                                     .Where(propertyName => !String.Equals(propertyName, 
                                                                                           ThreadContextAppender.ContextBufferName));
            foreach (var propertyName in validPropertyNames)
            {
                properties[propertyName] = loggingEventData.Properties[propertyName];
            }

            var ret =  new SerializableLoggingEventData
                            {
                                LoggerName = loggingEventData.LoggerName,
                                SerializableLevel = new SerializableLevel
                                {
                                    DisplayName = loggingEventData.Level.DisplayName,
                                    Name = loggingEventData.Level.Name,
                                    Value = loggingEventData.Level.Value
                                },
                                Message = loggingEventData.Message,
                                ThreadName = loggingEventData.ThreadName,
                                TimeStamp = loggingEventData.TimeStamp,
                                SerializableLocationInfo = loggingEventData.LocationInfo != null
                                    ? new SerializableLocationInfo
                                    {
                                        ClassName = loggingEventData.LocationInfo.ClassName,
                                        FileName = loggingEventData.LocationInfo.FileName,
                                        LineNumber = loggingEventData.LocationInfo.LineNumber,
                                        MethodName = loggingEventData.LocationInfo.MethodName,
                                    }
                                    : null,
                                UserName = loggingEventData.UserName,
                                Identity = loggingEventData.Identity,
                                ExceptionString = loggingEventData.ExceptionString,
                                Domain = loggingEventData.Domain,
                                Properties = properties
                            };

            return ret;
        }
    }
}
