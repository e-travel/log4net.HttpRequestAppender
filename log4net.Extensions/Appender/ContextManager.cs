using System.Web;

namespace log4net.Appender
{
    public class ContextManager : IContextManager
    {
        private readonly object _dataSlot;

        public ContextManager(object dataSlot)
        {
            _dataSlot = dataSlot;
        }

        public Context BuildContext()
        {
            return System.Web.HttpContext.Current == null
                       ? null
                       : new HttpContext(_dataSlot);

            return System.Web.HttpContext.Current == null
                       ? (Context)new ThreadContext(_dataSlot)
                       : new HttpContext(_dataSlot);
        }
    }
}