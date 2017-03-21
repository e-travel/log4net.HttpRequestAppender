using System;
using System.Web;
using log4net.Core;
using log4net.Util;
using log4net.Web;

namespace log4net.Appender
{
    /// <summary>
    /// Buffer logs during an HttpRequest and the forwards them to another appender as one log event
    /// </summary>
    public class HttpRequestAppender : AppenderSkeleton, IAppenderAttachable
    {
        protected override bool RequiresLayout { get { return true; } }

        public override void ActivateOptions()
        {
            base.ActivateOptions();

            Log4NetHttpModule.EndRequest += OnEndRequest;
        }

        #region HttpModule Events
        private void OnEndRequest(object sender, EventArgs e)
        {
            var buffer = GetBuffer();
            if (buffer == null || buffer.IsEmpty)
                return;

            // Ok, we got the buffer, prepare the event.
            var context = GetHttpContext();
            var duration = DateTime.Now - context.Timestamp;

            var logEvent = new LoggingEvent(new LoggingEventData
                                                {
                                                    TimeStamp = buffer.TimeStamp ?? default(DateTime),
                                                    Level = Level.Info,
                                                    Message = duration.TotalMilliseconds + "ms" + buffer.GetRenderedEvents()
                                                });

            SendBuffer(logEvent);
        }

        #endregion HttpModule Events

        protected SimpleRenderedEventBuffer GetOrCreateBuffer()
        {
            var buffer = GetBuffer();

            if (buffer != null)
                return buffer;

            var context = GetHttpContext();
            if (context == null)
                return null;

            buffer = new SimpleRenderedEventBuffer(RenderLoggingEvent);
            context.Items[this._dataSlot] = buffer;
            return buffer;
        }

        protected SimpleRenderedEventBuffer GetBuffer()
        {
            var context = GetHttpContext();
            if (context == null)
                return null;
            return context.Items[this._dataSlot] as SimpleRenderedEventBuffer;
        }

        protected virtual HttpContextBase GetHttpContext()
        {
            var context = HttpContext.Current;
            if (context == null)
                return null;
            return new HttpContextWrapper(HttpContext.Current);
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            var buffer = GetOrCreateBuffer();
            if (buffer == null)
                SendBuffer(loggingEvent);
            else
                buffer.AddEvent(loggingEvent);
        }

        #region got it from NLog BufferingAppenderSkeleton

        /// <summary>
        /// Send the events.
        /// </summary>
        /// <param name="logEvent">The events that need to be send.</param>
        /// <remarks>
        /// <para>
        /// Forwards the events to the attached appenders.
        /// </para>
        /// </remarks>
        /// 
        protected void SendBuffer(LoggingEvent logEvent)
        {
            // Pass the logging event on to the attached appenders
            if (m_appenderAttachedImpl != null)
            {
                m_appenderAttachedImpl.AppendLoopOnAppenders(logEvent);
            }
        }

        #endregion klopy of BufferingAppenderSkeleton

        #region Implementation of IAppenderAttachable

        /// <summary>
        /// Implementation of the <see cref="IAppenderAttachable"/> interface
        /// </summary>
        private AppenderAttachedImpl m_appenderAttachedImpl;

        /// <summary>
        /// Adds an <see cref="IAppender" /> to the list of appenders of this
        /// instance.
        /// </summary>
        /// <param name="newAppender">The <see cref="IAppender" /> to add to this appender.</param>
        /// <remarks>
        /// <para>
        /// If the specified <see cref="IAppender" /> is already in the list of
        /// appenders, then it won't be added again.
        /// </para>
        /// </remarks>
        virtual public void AddAppender(IAppender newAppender)
        {
            if (newAppender == null)
            {
                throw new ArgumentNullException("newAppender");
            }
            lock (this)
            {
                if (m_appenderAttachedImpl == null)
                {
                    m_appenderAttachedImpl = new log4net.Util.AppenderAttachedImpl();
                }
                m_appenderAttachedImpl.AddAppender(newAppender);
            }
        }

        /// <summary>
        /// Gets the appenders contained in this appender as an 
        /// <see cref="System.Collections.ICollection"/>.
        /// </summary>
        /// <remarks>
        /// If no appenders can be found, then an <see cref="EmptyCollection"/> 
        /// is returned.
        /// </remarks>
        /// <returns>
        /// A collection of the appenders in this appender.
        /// </returns>
        virtual public AppenderCollection Appenders
        {
            get
            {
                lock (this)
                {
                    if (m_appenderAttachedImpl == null)
                    {
                        return AppenderCollection.EmptyCollection;
                    }
                    else
                    {
                        return m_appenderAttachedImpl.Appenders;
                    }
                }
            }
        }

        /// <summary>
        /// Looks for the appender with the specified name.
        /// </summary>
        /// <param name="name">The name of the appender to lookup.</param>
        /// <returns>
        /// The appender with the specified name, or <c>null</c>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Get the named appender attached to this buffering appender.
        /// </para>
        /// </remarks>
        virtual public IAppender GetAppender(string name)
        {
            lock (this)
            {
                if (m_appenderAttachedImpl == null || name == null)
                {
                    return null;
                }

                return m_appenderAttachedImpl.GetAppender(name);
            }
        }

        /// <summary>
        /// Removes all previously added appenders from this appender.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is useful when re-reading configuration information.
        /// </para>
        /// </remarks>
        virtual public void RemoveAllAppenders()
        {
            lock (this)
            {
                if (m_appenderAttachedImpl != null)
                {
                    m_appenderAttachedImpl.RemoveAllAppenders();
                    m_appenderAttachedImpl = null;
                }
            }
        }

        /// <summary>
        /// Removes the specified appender from the list of appenders.
        /// </summary>
        /// <param name="appender">The appender to remove.</param>
        /// <returns>The appender removed from the list</returns>
        /// <remarks>
        /// The appender removed is not closed.
        /// If you are discarding the appender you must call
        /// <see cref="IAppender.Close"/> on the appender removed.
        /// </remarks>
        virtual public IAppender RemoveAppender(IAppender appender)
        {
            lock (this)
            {
                if (appender != null && m_appenderAttachedImpl != null)
                {
                    return m_appenderAttachedImpl.RemoveAppender(appender);
                }
            }
            return null;
        }

        /// <summary>
        /// Removes the appender with the specified name from the list of appenders.
        /// </summary>
        /// <param name="name">The name of the appender to remove.</param>
        /// <returns>The appender removed from the list</returns>
        /// <remarks>
        /// The appender removed is not closed.
        /// If you are discarding the appender you must call
        /// <see cref="IAppender.Close"/> on the appender removed.
        /// </remarks>
        virtual public IAppender RemoveAppender(string name)
        {
            lock (this)
            {
                if (name != null && m_appenderAttachedImpl != null)
                {
                    return m_appenderAttachedImpl.RemoveAppender(name);
                }
            }
            return null;
        }

        #endregion Implementation of IAppenderAttachable

        private readonly object _dataSlot = new object();
    }
}
