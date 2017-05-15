/******************************************************************************
*
* Copyright (c) ReflectSoftware, Inc. All rights reserved. 
*
* See License.md in the solution root for license information.
*
******************************************************************************/
using System;
using System.Text;
using System.Reflection;

using log4net.Core;
using log4net.Appender;

using ReflectSoftware.Insight;
using ReflectSoftware.Insight.Common;

using RI.Utils.ExceptionManagement;

namespace ReflectSoftware.Insight.Extensions.Log4net
{
    /// <summary>
    /// Redirect all log4net messages to ReflectInsight.
    /// </summary>
    /// <seealso cref="log4net.Appender.AppenderSkeleton" />
    /// <seealso cref="T:log4net.Appender.AppenderSkeleton" />
    public class LogAppender : AppenderSkeleton
    {
        class ActiveStates
        {
            public IReflectInsight RI { get; set; }
            public Boolean DisplayLevel { get; set; }
            public Boolean DisplayLocation { get; set; }
        }

        static private readonly String FLine;
        static private readonly MethodInfo FSendInternalErrorMethodInfo;

        private ActiveStates CurrentActiveStates { get; set; }
        protected String InstanceName { get; set; }
        protected String DisplayLevel { get; set; }
        protected String DisplayLocation { get; set; }


        /// <summary>
        /// Initializes the <see cref="LogAppender"/> class.
        /// </summary>
        /// <remarks>
        /// Empty default constructor
        /// </remarks>
        static LogAppender()
        {
            FLine = String.Format("{0,40}", String.Empty).Replace(" ", "-");
            FSendInternalErrorMethodInfo = typeof(ReflectInsight).GetMethod("SendInternalError", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogAppender" /> class.
        /// </summary>
        /// <remarks>
        /// Empty default constructor.
        /// </remarks>
        /// --------------------------------------------------------------------
        /// --------------------------------------------------------------------
        public LogAppender()
        {
            InstanceName = String.Empty;
            DisplayLevel = String.Empty;
            DisplayLocation = String.Empty;
            CurrentActiveStates = new ActiveStates();

            RIEventManager.OnServiceConfigChange += DoOnConfigChange;
        }

        /// <summary>
        /// Raises the Close event.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Releases any resources allocated within the appender such as file handles,
        /// network connections, etc.
        /// </para>
        /// <para>
        /// It is a programming error to append to a closed appender.
        /// </para>
        /// </remarks>
        protected override void OnClose()
        {
            RIEventManager.OnServiceConfigChange -= DoOnConfigChange;
            base.OnClose();
        }

        /// <summary>
        /// Does the on configuration change.
        /// </summary>
        private void DoOnConfigChange()
        {
            OnConfigChange();
        }
        /// <summary>
        /// Activates the options.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is part of the <see cref="T:log4net.Core.IOptionHandler" /> delayed object activation
        /// scheme. The <see cref="M:log4net.Appender.AppenderSkeleton.ActivateOptions" /> method must
        /// be called on this object after the configuration properties have been set. Until
        /// <see cref="M:log4net.Appender.AppenderSkeleton.ActivateOptions" /> is called this object
        /// is in an undefined state and must not be used.
        /// </para>
        /// <para>
        /// If any of the configuration properties are modified then
        /// <see cref="M:log4net.Appender.AppenderSkeleton.ActivateOptions" /> must be called again.
        /// </para>
        /// </remarks>
        /// --------------------------------------------------------------------
        /// <seealso cref="M:log4net.Appender.AppenderSkeleton.ActivateOptions()" />
        /// --------------------------------------------------------------------
        public override void ActivateOptions()
        {
            base.ActivateOptions();
            OnConfigChange();
        }

        /// <summary>
        /// Called when [configuration change].
        /// </summary>
        private void OnConfigChange()
        {
            try
            {
                lock (this)
                {
                    ActiveStates states = new ActiveStates();
                    states.RI = RILogManager.Get(InstanceName) ?? RILogManager.Default;
                    states.DisplayLevel = String.Compare(DisplayLevel.ToLower().Trim(), "true", false) == 0;
                    states.DisplayLocation = String.Compare(DisplayLocation.ToLower().Trim(), "true", false) == 0;

                    CurrentActiveStates = states;
                }
            }
            catch (Exception ex)
            {
                RIExceptionManager.Publish(ex, "Failed during: LogAppender.OnConfigChange()");
            }
        }

        /// <summary>
        /// Sends the internal error.
        /// </summary>
        /// <param name="ri">The ri.</param>
        /// <param name="mType">Type of the m.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        static private Boolean SendInternalError(IReflectInsight ri, MessageType mType, Exception ex)
        {
            return (Boolean)FSendInternalErrorMethodInfo.Invoke(ri, new object[] { mType, ex });
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="states">The states.</param>
        /// <param name="mType">Type of the m.</param>
        /// <param name="loggingEvent">The logging event.</param>
        static private void SendMessage(ActiveStates states, MessageType mType, LoggingEvent loggingEvent)
        {
            try
            {
                // build details
                StringBuilder sb = null;

                if (loggingEvent.ExceptionObject != null)
                {
                    sb = new StringBuilder();
                    sb.Append(ExceptionBasePublisher.ConstructIndentedMessage(loggingEvent.ExceptionObject));
                    sb.AppendLine();
                    sb.AppendLine();
                }

                if (states.DisplayLevel || states.DisplayLocation)
                {
                    sb = sb ?? new StringBuilder();

                    sb.AppendLine("Log4net Details:");
                    sb.AppendLine(FLine);

                    if (states.DisplayLevel)
                        sb.AppendFormat("{0,10}: {1}{2}", "Level", loggingEvent.Level.DisplayName, Environment.NewLine);

                    if (states.DisplayLocation)
                    {
                        sb.AppendFormat("{0,10}: {1}{2}", "ClassName", loggingEvent.LocationInformation.ClassName, Environment.NewLine);
                        sb.AppendFormat("{0,10}: {1}{2}", "MethodName", loggingEvent.LocationInformation.MethodName, Environment.NewLine);
                        sb.AppendFormat("{0,10}: {1}{2}", "FileName", loggingEvent.LocationInformation.FileName, Environment.NewLine);
                        sb.AppendFormat("{0,10}: {1}{2}", "LineNumber", loggingEvent.LocationInformation.LineNumber, Environment.NewLine);
                        sb.AppendFormat("{0,10}: {1}{2}", "FullInfo", loggingEvent.LocationInformation.FullInfo, Environment.NewLine);
                    }
                }

                String details = sb != null ? sb.ToString() : null;
                states.RI.Send(mType, loggingEvent.RenderedMessage, details);
            }
            catch (Exception ex)
            {
                if (!SendInternalError(states.RI, mType, ex)) throw;
            }
        }

        /// <summary>
        /// Subclasses of <see cref="T:log4net.Appender.AppenderSkeleton" /> should implement this method
        /// to perform actual logging.
        /// </summary>
        /// <param name="loggingEvent">The event to append.</param>
        /// <remarks>
        /// <para>
        /// A subclass must implement this method to perform
        /// logging of the <paramref name="loggingEvent" />.
        /// </para>
        /// <para>This method will be called by <see cref="M:DoAppend(LoggingEvent)" />
        /// if all the conditions listed for that method are met.
        /// </para>
        /// <para>
        /// To restrict the logging of events in the appender
        /// override the <see cref="M:PreAppendCheck()" /> method.
        /// </para>
        /// </remarks>
        protected override void Append(LoggingEvent loggingEvent)
        {
            ActiveStates states = CurrentActiveStates;
            MessageType mType = MessageType.SendMessage;

            if (loggingEvent.Level == Level.Info)
            {
                if (loggingEvent.RenderedMessage.StartsWith("[Enter]"))
                {
                    states.RI.EnterMethod(loggingEvent.RenderedMessage.Replace("[Enter]", String.Empty));
                    return;
                }
                if (loggingEvent.RenderedMessage.StartsWith("[Exit]"))
                {
                    states.RI.ExitMethod(loggingEvent.RenderedMessage.Replace("[Exit]", String.Empty));
                    return;
                }

                mType = MessageType.SendInformation;
            }
            else if (loggingEvent.Level == Level.Trace)
            {
                mType = MessageType.SendTrace;
            }
            else if (loggingEvent.Level == Level.Debug
                 || loggingEvent.Level == Level.Log4Net_Debug)
            {
                mType = MessageType.SendDebug;
            }
            else if (loggingEvent.Level == Level.Warn)
            {
                mType = MessageType.SendWarning;
            }
            else if (loggingEvent.Level == Level.Error
                 || loggingEvent.Level == Level.Alert
                 || loggingEvent.Level == Level.Emergency
                 || loggingEvent.Level == Level.Severe)
            {
                mType = MessageType.SendError;
            }
            else if (loggingEvent.Level == Level.Fatal
                 || loggingEvent.Level == Level.Critical)
            {
                mType = MessageType.SendFatal;
            }
            else if (loggingEvent.Level == Level.Notice)
            {
                mType = MessageType.SendNote;
            }
            else if (loggingEvent.Level == Level.Verbose)
            {
                mType = MessageType.SendVerbose;
            }

            SendMessage(states, mType, loggingEvent);
        }

        /// <summary>
        /// Tests if this appender requires a <see cref="P:log4net.Appender.AppenderSkeleton.Layout" /> to be set.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In the rather exceptional case, where the appender
        /// implementation admits a layout but can also work without it,
        /// then the appender should return <c>true</c>.
        /// </para>
        /// <para>
        /// This default implementation always returns <c>false</c>.
        /// </para>
        /// </remarks>
        protected override bool RequiresLayout
        {
            get { return false; }
        }
    }
}
