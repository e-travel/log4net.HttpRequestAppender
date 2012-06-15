using System;
using log4net.Core;
using log4net.Util;
using log4net.Web;

namespace log4net.Appender
{
    public class HttpRequestAppender : AppenderSkeleton, IAppenderAttachable
    {
        private IContextManager _contextManager;
        public IContextManager ContextManager
        {
            set { _contextManager = value; }
            private get { return _contextManager ?? (_contextManager = new ContextManager(dataSlot)); }
        }
        
        private Context Context
        {
            get { return ContextManager.BuildContext(); }
        }

        private readonly object dataSlot = new object();


        public virtual bool DoesRequireLayout { get { return true; } }
        protected override bool RequiresLayout { get { return DoesRequireLayout; } }

        public override void ActivateOptions()
        {
            base.ActivateOptions();

            Log4NetHttpModule.BeginRequest += this.OnBeginRequest;
            Log4NetHttpModule.EndRequest += this.OnEndRequest;

            // we are in the context already, it's too late for OnBeginRequest to be called, so let's
            // just call it ourselves
            this.OnBeginRequest(null, null);
        }

        #region HttpModule Events
        private void OnEndRequest(object sender, EventArgs e)
        {
            var buffer = GetBuffer();
            if (buffer == null || buffer.IsEmpty)
                return;

            SendBuffer( new []{buffer.GetEvent()});
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            //var context = HttpContext.Current;
            //context.Items[this.dataSlot] = null;
        }

        #endregion HttpModule Events

        protected GroupedEvents GetBuffer()
        {
            if (this.Context == null)
                return null;

            if (this.Context.Events == null)
            {
                this.Context.AddEvents(new GroupedEvents(this.Context.Timestamp, RenderLoggingEvent));
            }

            return this.Context.Events;
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            var buffer = GetBuffer();
            if (buffer == null)
                SendBuffer(new []{loggingEvent});
            else
                buffer.AddEvent(loggingEvent);
        }

        #region klopy of BufferingAppenderSkeleton

        /// <summary>
        /// Send the events.
        /// </summary>
        /// <param name="events">The events that need to be send.</param>
        /// <remarks>
        /// <para>
        /// Forwards the events to the attached appenders.
        /// </para>
        /// </remarks>
        /// 
        protected void SendBuffer(LoggingEvent[] events)
        {
            // Pass the logging event on to the attached appenders
            if (m_appenderAttachedImpl != null)
            {
                m_appenderAttachedImpl.AppendLoopOnAppenders(events);
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
    }
}
