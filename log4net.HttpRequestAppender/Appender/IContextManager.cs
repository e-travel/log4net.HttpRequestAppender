namespace log4net.Appender
{
    /// <summary>
    /// Context manager is used to decide what type of context the appender is using (http, thread or mock)
    /// This interface helps in unit testing
    /// </summary>
    public interface IContextManager
    {
        Context BuildContext();
    }
}