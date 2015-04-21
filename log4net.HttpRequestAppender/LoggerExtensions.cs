using System.Linq;
using log4net.Appender;

namespace log4net
{
    public static class LoggerExtensions
    {
        public static TAppender GetAppender<TAppender>(this ILog logger)
            where TAppender : class, IAppender
        {
            var appender = LogManager.GetRepository().GetAppenders().SingleOrDefault(a => a.GetType() == typeof(TAppender));
            return appender as TAppender;
        }
    }
}
