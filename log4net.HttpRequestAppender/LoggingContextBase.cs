using log4net.Appender;

namespace log4net
{
    public abstract class LoggingContextBase
    {
        public static bool SaveInHttpContext = false;

        public static void SetProperty(string key, string value)
        {
            var httpContextBuffer = HttpRequestAppender.GetHttpContextBuffer(true);
            if (SaveInHttpContext && httpContextBuffer != null)
            {
                httpContextBuffer[key] = value;
            }
            else
            {
                LogicalThreadContext.Properties[key] = value ?? string.Empty;
            }
        }

        public static string GetProperty(string key)
        {
            var httpContextBuffer = HttpRequestAppender.GetHttpContextBuffer(true);
            if (SaveInHttpContext && httpContextBuffer != null)
            {
                return httpContextBuffer[key];
            }

            return (string) LogicalThreadContext.Properties[key];
        }

        public static void SetProperty(string key, int? value)
        {
            SetProperty(key, value  != null ? value.ToString() : string.Empty);
        }

        public static int GetIntProperty(string key)
        {
            return int.Parse(GetProperty(key));
        }
    }
}