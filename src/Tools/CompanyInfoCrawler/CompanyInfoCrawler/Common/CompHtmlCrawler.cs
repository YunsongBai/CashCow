using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace CompanyInfoCrawler
{
    public class CompHtmlCrawler : ICompInfoCrawler
    {
        public bool IsOpen { get; protected set; }

        private class WorkingThreadParameter
        {
            public ICompListEnumerator CompListEnumerator { get; set; }
            public ICompInfoParser Parser { get; set; }
            public ICompInfoWriter Writer { get; set; }
            public AutoResetEvent FinishEvt { get; set; }
        }
        private Thread m_workingThread;

        private class NetIOParsingThreadParam
        {
            public int ThreadIdx { get; set; }
            public List<HtmlDataSrcDesc> HtmlDSDescList { get; set; }
            public List<CompanyInfo>[] CompanyInfos { get; set; }
            public ICompInfoParser Parser { get; set; }
            public CountdownEvent Evt { get; set; }
        }
        private const int NETIO_THREAD_COUNT = 32;
        private const int BATCH_PROC_COUNT = 2048;

        public bool Open()
        {
            m_workingThread = new Thread(new ParameterizedThreadStart(WorkingThreadProc));
            IsOpen = true;
            return true;
        }
        public void Close()
        {
            if (IsOpen)
            {
                if (m_workingThread.IsAlive)
                {
                    m_workingThread.Abort();
                }
                IsOpen = false;
            }
        }

        public void Crawl(ICompListEnumerator listEnum, ICompInfoParser parser, ICompInfoWriter writer)
        {
            Debug.Assert(IsOpen);
            Debug.Assert(listEnum != null && parser != null && writer != null);

            AutoResetEvent evt = new AutoResetEvent(false);
            m_workingThread.Start(new WorkingThreadParameter()
            {
                CompListEnumerator = listEnum,
                Parser = parser,
                Writer = writer,
                FinishEvt = evt
            });

            // Just wait working thread done.
            evt.WaitOne();
        }

        private void WorkingThreadProc(object parameter)
        {
            WorkingThreadParameter par = parameter as WorkingThreadParameter;

            try
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Begin write company infos.\r\n");
                if (!par.Writer.Open())
                {
                    par.FinishEvt.Set();
                    return;
                }

                // Batch parse companies.
                ConcurrentQueue<HtmlDataSrcDesc> q = new ConcurrentQueue<HtmlDataSrcDesc>();
                List<HtmlDataSrcDesc> srcDescBuf = new List<HtmlDataSrcDesc>();
                while (par.CompListEnumerator.MoveNext())
                {
                    HtmlDataSrcDesc desc = par.CompListEnumerator.GetCompDataSrcDesc();
                    srcDescBuf.Add(desc);
                    if (srcDescBuf.Count == BATCH_PROC_COUNT)
                    {
                        // Use other working threads for net io and parsing.
                        BatchParse(par, srcDescBuf);
                        srcDescBuf.Clear();
                    }
                }
                if (srcDescBuf.Count > 0)
                {
                    BatchParse(par, srcDescBuf);
                    srcDescBuf.Clear();
                }

                par.Writer.Close();
                Console.WriteLine("End write company infos.\r\n");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Error in Crawling: {0}", ex.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            finally
            {
                par.FinishEvt.Set();
            }
        }

        private static void BatchParse(WorkingThreadParameter par, List<HtmlDataSrcDesc> srcDescBuf)
        {
            List<CompanyInfo>[] infos = new List<CompanyInfo>[NETIO_THREAD_COUNT];
            for (int i = 0; i < NETIO_THREAD_COUNT; i++)
            {
                infos[i] = new List<CompanyInfo>();
            }

            CountdownEvent evt = new CountdownEvent(NETIO_THREAD_COUNT);
            for (int i = 0; i < NETIO_THREAD_COUNT; i++)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(NetIOParseThreadProc));
                thread.Start(new NetIOParsingThreadParam()
                {
                    ThreadIdx = i,
                    HtmlDSDescList = srcDescBuf,
                    CompanyInfos = infos,
                    Parser = par.Parser,
                    Evt = evt
                });
            }
            evt.Wait();

            // Write to file.
            for (int i = 0; i < NETIO_THREAD_COUNT; i++)
            {
                List<CompanyInfo> list = infos[i];
                foreach (CompanyInfo info in list)
                {
                    par.Writer.Write(info);
                }
            }
            par.Writer.Flush();
        }

        private static void NetIOParseThreadProc(object parameter)
        {
            NetIOParsingThreadParam par = parameter as NetIOParsingThreadParam;

            for (int i = 0; i < par.HtmlDSDescList.Count; ++i)
            {
                if (i % NETIO_THREAD_COUNT == par.ThreadIdx)
                {
                    HtmlDataSrcDesc desc = par.HtmlDSDescList[i];
                    CompanyInfo info = par.Parser.Parse(desc);
                    if (info != null)
                    {
                        par.CompanyInfos[par.ThreadIdx].Add(info);
                    }
                }
            }
            par.Evt.Signal();
        }
    }
}
