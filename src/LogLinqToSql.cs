using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.IO;

using RI.Utils.MemoryCache;

using ReflectSoftware.Insight;
using ReflectSoftware.Insight.Common;

namespace ReflectSoftware.Insight.Extensions.LogLinq
{
    internal class LogLinqToSqlRequest : IRequestObject
    {
        public StringBuilder StrBuilder;
        public UInt32 RequestId { get; private set; }

        //---------------------------------------------------------------------
        public void Attached(UInt32 requestId)
        {
            RequestId = requestId;
            StrBuilder = new StringBuilder();
        }
        //---------------------------------------------------------------------
        public void Detached()
        {
        }
        //---------------------------------------------------------------------
        public void Reset()
        {
        }
    }   
    
    /// <summary>
    /// Redirect all Linq to Sql messages to ReflectInsight
    /// </summary>
    public class LogLinqToSql: TextWriter
    {
        static private RequestObjectManager<LogLinqToSqlRequest> FRequestObjectManager;

        protected String FLabel;
        protected Boolean FOwnsRI;
        protected ReflectInsight FReflectInsight;

        //---------------------------------------------------------------------
        static LogLinqToSql()
		{
            FRequestObjectManager = new RequestObjectManager<LogLinqToSqlRequest>();
		}
        //---------------------------------------------------------------------
        public LogLinqToSql(String label, ReflectInsight ri)
        {
            FLabel = label;
            FReflectInsight = ri;
            FOwnsRI = ri != null;

            OnConfigChange();
            RIEventManager.OnServiceConfigChange += DoOnConfigChange;
        }
        //---------------------------------------------------------------------
        public LogLinqToSql(String label): this(label, null)
        {
        }
        //---------------------------------------------------------------------
        public LogLinqToSql(ReflectInsight ri): this(null, ri)
        {
        }
        //---------------------------------------------------------------------
        public LogLinqToSql(): this(null, null)
        {
        }
        //---------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            lock (this)
            {
                RIEventManager.OnServiceConfigChange -= DoOnConfigChange;

                if (FOwnsRI && FReflectInsight != null)
                {
                    FReflectInsight.Dispose();
                    FReflectInsight = null;
                }
            }

            base.Dispose(disposing);
        }
        //---------------------------------------------------------------------
        private void DoOnConfigChange()
        {
            OnConfigChange();
        }
        //---------------------------------------------------------------------
        private void OnConfigChange()
        {
            try
            {
                lock (this)
                {
                    if (!FOwnsRI)
                    {
                        String instanceName = ReflectInsightConfig.Settings.GetExtensionAttribute("logLinqToSql", "instance", "logLinqToSql");
                        FReflectInsight = RILogManager.Get(instanceName) ?? RILogManager.Default;

                        FRequestObjectManager.RequestDetachLifeSpan = GetRequestObjectLifeSpan();
                    }
                }
            }
            catch (Exception ex)
            {
                RIExceptionManager.Publish(ex, "Failed during: LogLinqToSql.OnConfigChange()");
            }
        }
        //---------------------------------------------------------------------------
        private Int32 GetRequestObjectLifeSpan()
        {
            Int32 requestLifeSpan = 10;
            Int32.TryParse(ReflectInsightConfig.Settings.GetBaseRequestObjectAttribute("requestLifeSpan", "10"), out requestLifeSpan);

            return requestLifeSpan;
        }
        //---------------------------------------------------------------------------
        protected void AppendMessage(String message)
        {
            FRequestObjectManager.GetRequestObject().StrBuilder.Append(message);
        }
        //---------------------------------------------------------------------------
        protected String GetFullWriteMessage()
        {
            LogLinqToSqlRequest request = FRequestObjectManager.GetRequestObject(false);
            if (request != null)
            {
                FRequestObjectManager.RemoveRequest(request);
                return request.StrBuilder.ToString();
            }

            return String.Empty;
        }
        //---------------------------------------------------------------------
        public override void Write(Char[] buffer, Int32 index, Int32 count)
        {
            String line = new String(buffer, index, count);
            if (line.Trim() != String.Empty)
            {
                AppendMessage(line);
                return;
            }

            line = GetFullWriteMessage();
            FReflectInsight.SendSQLString(FLabel ?? line, line);
        }
        //---------------------------------------------------------------------
        public override Encoding Encoding
        {
            get { return System.Text.Encoding.Default; }
        }
    }
}
