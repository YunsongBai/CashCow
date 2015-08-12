

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace CompanyInfoCrawler
{
    public class HtmlHelper
    {
        /// <summary>
        /// Get html source code from url.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GetHtmlResponseAsString(string url, string coder = "", int timeoutSeconds = 30, UInt32 retryCount = 3)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            retryCount = Math.Min(retryCount, 5);
            int tryNum = 1;
            string html = string.Empty;
            while (tryNum <= retryCount)
            {
                try
                {
                    WebRequest webRequest = WebRequest.Create(url);
                    webRequest.Timeout = timeoutSeconds * 1000;
                    using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                    {
                        if (webResponse.StatusCode == HttpStatusCode.OK)
                        {
                            Stream stream = webResponse.GetResponseStream();
                            if (string.IsNullOrEmpty(coder))
                                coder = ((HttpWebResponse)webResponse).CharacterSet;

                            StreamReader reader = new StreamReader(stream, string.IsNullOrEmpty(coder) ? Encoding.Default : Encoding.GetEncoding(coder));
                            html = reader.ReadToEnd();
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\tFailed to crawl {0}, exception: {1}. ", url, ex.GetBaseException().Message);
                    Console.WriteLine("\tRetry time: {0}.\r\n", tryNum);
                    //Request may timeout sometimes, not getting a good way to handle it
                    Random randomDelay = new Random();
                    System.Threading.Thread.Sleep(randomDelay.Next(100, 1000));
                    Console.ForegroundColor = ConsoleColor.Gray;
                    tryNum++;
                }
            }

            return html;
        }

        /// <summary>
        /// Remove script, style and comment code from html string.
        /// </summary>
        /// <param name="htmlString">Indicating the webpage source code.</param>
        /// <returns></returns>
        public static HtmlDocument InitializeHtmlDoc(string htmlString)
        {
            if (string.IsNullOrEmpty(htmlString))
            {
                return null;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlString);
            doc.DocumentNode.Descendants()
                .Where(n => n.Name == "script" || n.Name == "style" || n.Name == "#comment")
                .ToList()
                .ForEach(n => n.Remove());

            return doc;
        }
    }
}
