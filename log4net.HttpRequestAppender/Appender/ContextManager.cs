namespace log4net.Appender
{
    /// <summary>
    /// Context manager is used to decide what type of context the appender is using (http, thread or mock)
    /// </summary>
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

            // note: de-comment in v1.1
            /*return System.Web.HttpContext.Current == null
                       ? (Context)new ThreadContext(_dataSlot)
                       : new HttpContext(_dataSlot);*/
        }
    }
}