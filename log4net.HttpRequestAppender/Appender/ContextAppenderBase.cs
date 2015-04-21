using System;
using System.Collections.Generic;
using log4net.Core;
using log4net.Util;

namespace log4net.Appender
{
    /// <summary>
    /// Buffer logs during an HttpRequest and the forwards them to another appender as one log event
    /// </summary>
    public abstract class ContextAppenderBase : AppenderSkeleton, IAppenderAttachable
    {
        protected override bool RequiresLayout { get { return true; } }

        protected abstract IList<LoggingEvent> GetBuffer(bool create);

        protected sealed override void Append(LoggingEvent loggingEvent)
        {
            var buffer = GetBuffer(true);
            if (buffer == null)
                SendBuffer(loggingEvent);
            else
                buffer.Add(loggingEvent);
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
        protected void SendBuffer(LoggingEvent logEvent)
        {
            // Pass the logging event on to the attached appenders
            if (_mAppenderAttachedImpl != null)
            {
                _mAppenderAttachedImpl.AppendLoopOnAppenders(logEvent);
            }
        }

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
        virtual public void AddAppender(IAppender newAppender)
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
        virtual public AppenderCollection Appenders
        {
            get
            {
                lock (this)
                {
                    return _mAppenderAttachedImpl == null 
                                ? AppenderCollection.EmptyCollection 
                                : _mAppenderAttachedImpl.Appenders;
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
        virtual public void RemoveAllAppenders()
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
        virtual public IAppender RemoveAppender(IAppender appender)
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
        virtual public IAppender RemoveAppender(string name)
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
    }
}
