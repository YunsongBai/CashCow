
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Specialized;
using System.IO;

namespace CompanyInfoCrawler
{
    /// <summary>
    /// Crawl news and comments.
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 65535;

            _51CompListEnumerator em = new _51CompListEnumerator();
            _51CompParser parser = new _51CompParser();
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string desStreamUri = string.Format("{0}./CompanyList_From_51job_min[{1}]_max[{2}]_[{3}].txt",
                                                Path.GetDirectoryName(exePath), em.MinCompId, em.MaxCompId, DateTime.Now.ToString("yyyy-MM-dd"));
            CommonCompInfoWriter writer = new CommonCompInfoWriter();
            CompHtmlCrawler crawler = new CompHtmlCrawler();
            if (crawler.Open())
            {
                crawler.Crawl(em, parser, writer);
                crawler.Close();
            }
            



        }

      
    }
}
