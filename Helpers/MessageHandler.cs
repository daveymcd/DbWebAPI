using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DbWebAPI.Helpers
{
    /// <summary>
    /// 
    ///     DbWebApi.Helpers.MessageHandler - Output Message methods
    ///     
    /// </summary>
    public class MessageHandler
    {
        /// <summary>
        /// 
        ///     Helpers.MessageHandler.MessageLog(string, string)
        ///     Outputs a message to the Windows EventViewer or Nlog (default).
        ///     
        /// </summary>
        /// <remarks>
        ///     
        ///     Nlog writes messages to a flat file in the project directory 'logs'...
        ///     
        ///         ~\Visual Studio 2019\Projects\DbWebAPI\logs\DbWebAPIlog-'DateTime.log
        ///     
        /// </remarks>
        /// <example>
        ///     Helpers.MessageLog("Exception Name or Message", "Nlog");
        /// </example>
        /// <param name="exceptionMsg">Exeption Name</param>
        /// <param name="logService">Nlog or EventLog</param>
        public static void MessageLog(string exceptionMsg = "Timing Log", string logService = "Nlog")
        {
            var source = "DbWebAPI";
            var log = "Application";

            if (logService == "EventLog")
            {
                if (!EventLog.SourceExists(source))
                    EventLog.CreateEventSource(source, log);
                EventLog.WriteEntry(source, exceptionMsg);
                EventLog.WriteEntry(source, exceptionMsg, EventLogEntryType.Error);
            }
            else
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error(exceptionMsg, DateTime.Now);
            }
        }

        /// <summary>
        /// 
        ///     Helpers.MessageHandler.DebugLog(string, bool, string, string, int)
        ///     Trace the calling sequence for a method.
        ///     
        /// </summary>
        /// <remarks>
        ///     
        ///     Loops through the call stack of the current thread, tracing the methods 
        ///     calling sequence. Logs to output window only in debug mode
        ///     
        /// </remarks>
        /// <example>
        ///     MessageHandler.DebugLog("Starting", true);
        /// </example>
        /// <param name="msgBody">Main Message</param>
        /// <param name="fullTrace">Full Trace History of just calling method name</param>
        /// <param name="memberName">Method Name</param>
        /// <param name="fileName">File Method defined in</param>
        /// <param name="lineNumber">File Line Number call made from</param>
        [STAThread]
        public static async Task DebugLog(string msgBody = "",
                                            bool fullTrace = false,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string fileName = "",
                                            [CallerLineNumber] int lineNumber = 0)
        { // Debug logging
            if (!Debugger.IsAttached)
                return;

            try
            {
                if (!string.IsNullOrEmpty(memberName) && !fullTrace)
                {
                    var len = (40 - (fileName.Split(' ', '\\').LastOrDefault().Length + memberName.Length));
                    len = (len >= 0) ? len : 1;
                    string spacer = "                                                                          ";
                    spacer = string.Concat(spacer.Remove(len), "\u0009");
                    var indent = ".";
                    if (!string.IsNullOrEmpty(msgBody))
                        msgBody = string.Concat(" - ", msgBody);
                    Debug.WriteLine("{0}{1}.{2}[{3}]{4}{5}", indent, fileName.Split(' ', '\\').LastOrDefault(), memberName, lineNumber, spacer, msgBody);
                }
                else
                { //Create a StackTrace that captures filename, line number, and column information for the current thread.
                    StackTrace st = new StackTrace(true);
                    string stackIndent = ".";
                    if (fullTrace)
                    { //full call history
                        for (int i = 1; i < st.FrameCount; i++)
                        { //Note: high up the call stack, there is only one stack frame.
                            StackFrame sf = st.GetFrame(i);
                            if (!string.IsNullOrEmpty(msgBody))
                                msgBody = string.Concat(" - ", msgBody);
                            Debug.WriteLine("{0}{1}[{2}]{3}", stackIndent, sf.GetMethod().Name, sf.GetFileLineNumber(), msgBody);
                            stackIndent = string.Concat(stackIndent, ".");
                        }
                    }
                    else
                    { //Just get the calling methods details
                        StackFrame sf = st.GetFrame(1);
                        if (!string.IsNullOrEmpty(msgBody))
                            msgBody = string.Concat(" - ", msgBody);
                        Debug.WriteLine("{0}{1}[{2}]{3}", stackIndent, sf.GetMethod().Name, sf.GetFileLineNumber(), msgBody);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw new NotSupportedException("DebugLog: " + ex);
            }
        }
    }
}
