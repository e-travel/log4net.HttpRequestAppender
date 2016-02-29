using System;
using System.Collections.Concurrent;
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

        private void OnEndRequest(object sender, EventArgs e)
        {
            #region set logging properties from httpcontext to callcontext

            var httpContextBuffer = GetHttpContextBuffer(false);
            if (httpContextBuffer != null)
            {
                foreach (var element in httpContextBuffer)
                {
                    LogicalThreadContext.Properties[element.Key] = element.Value;
                }
            }

            #endregion

            #region create one large logging event from multiple appended smaller events

            var buffer = GetBuffer(false, _dataSlot, () => new SimpleRenderedEventBuffer(RenderLoggingEvent));
            if (buffer == null || buffer.IsEmpty)
                return;

            // Ok, we got the buffer, prepare the event.
            var context = GetHttpContext();
            var duration = DateTime.Now - context.Timestamp;

            var logEvent = new LoggingEvent(new LoggingEventData
                                                {
                                                    Level = Level.Info,
                                                    Message = duration.TotalMilliseconds + "ms" + buffer.GetRenderedEvents()
                                                });

            #endregion

            SendBuffer(logEvent);
        }

        private static T GetBuffer<T>(bool create, object dataSlot, Func<T> creator)
            where T : class
        {
            var context = GetHttpContext();
            if (context == null)
            {
                return null;
            }
            var buffer = context.Items[dataSlot] as T;

            if (buffer == null && create)
            {
                buffer = creator();
                context.Items[dataSlot] = buffer;
            }

            return buffer;
        }

        public static ConcurrentDictionary<string, string> GetHttpContextBuffer(bool create)
        {
            return GetBuffer(create, LoggingPropertiesBufferKey, () => new ConcurrentDictionary<string, string>());
        }

        private static HttpContextBase GetHttpContext()
        {
            var context = HttpContext.Current;
            if (context == null)
                return null;
            return new HttpContextWrapper(HttpContext.Current);
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            var buffer = GetBuffer(true, _dataSlot, () => new SimpleRenderedEventBuffer(RenderLoggingEvent));
            if (buffer == null)
            {
                SendBuffer(new LoggingEvent(new LoggingEventData
                                                {
                                                    Level = Level.Info,
                                                    Message = RenderLoggingEvent(loggingEvent)
                                                }));
            }
            else
            {
                buffer.AddEvent(loggingEvent);
            }
        }

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
        private void SendBuffer(LoggingEvent logEvent)
        {
            // Pass the logging event on to the attached appenders
            if (_mAppenderAttachedImpl != null)
            {
                _mAppenderAttachedImpl.AppendLoopOnAppenders(logEvent);
            }
        }

        #region Implementation of IAppenderAttachable

        /// <summary>
        /// Implementation of the <see cref="IAppenderAttachable"/> interface
        /// </summary>
        private AppenderAttachedImpl _mAppenderAttachedImpl;

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
        public virtual void AddAppender(IAppender newAppender)
        {
            if (newAppender == null)
            {
                throw new ArgumentNullException("newAppender");
            }
            lock (this)
            {
                if (_mAppenderAttachedImpl == null)
                {
                    _mAppenderAttachedImpl = new AppenderAttachedImpl();
                }
                _mAppenderAttachedImpl.AddAppender(newAppender);
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
        public virtual AppenderCollection Appenders
        {
            get
            {
                lock (this)
                {
                    if (_mAppenderAttachedImpl == null)
                    {
                        return AppenderCollection.EmptyCollection;
                    }
                    else
                    {
                        return _mAppenderAttachedImpl.Appenders;
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
        public virtual IAppender GetAppender(string name)
        {
            lock (this)
            {
                if (_mAppenderAttachedImpl == null || name == null)
                {
                    return null;
                }

                return _mAppenderAttachedImpl.GetAppender(name);
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
        public virtual void RemoveAllAppenders()
        {
            lock (this)
            {
                if (_mAppenderAttachedImpl != null)
                {
                    _mAppenderAttachedImpl.RemoveAllAppenders();
                    _mAppenderAttachedImpl = null;
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
        public virtual IAppender RemoveAppender(IAppender appender)
        {
            lock (this)
            {
                if (appender != null && _mAppenderAttachedImpl != null)
                {
                    return _mAppenderAttachedImpl.RemoveAppender(appender);
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
        public virtual IAppender RemoveAppender(string name)
        {
            lock (this)
            {
                if (name != null && _mAppenderAttachedImpl != null)
                {
                    return _mAppenderAttachedImpl.RemoveAppender(name);
                }
            }
            return null;
        }

        #endregion Implementation of IAppenderAttachable

        private readonly object _dataSlot = new object();
        private const string LoggingPropertiesBufferKey = "logging_properties_buffer";
    }
}