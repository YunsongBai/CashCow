using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyInfoCrawler
{
    public class _51CompParser : ICompInfoParser
    {
        public CompanyInfo Parse(HtmlDataSrcDesc ds)
        {
            CompanyInfo res = null;

            Console.WriteLine("\tBegin parse company {0} with Url: {1}.\r\n", ds.Id, ds.Url);

            try
            {
                string htmlStr = HtmlHelper.GetHtmlResponseAsString(ds.Url, "GB2312", 10);
                if (string.IsNullOrEmpty(htmlStr))
                {   // There might be 404 code.
                    Console.WriteLine("\tEnd parse company {0}. No Info!\r\n", ds.Id);
                    return res;
                }
                htmlStr = HtmlEntity.DeEntitize(htmlStr);
                HtmlDocument doc = HtmlHelper.InitializeHtmlDoc(htmlStr);
                if (doc != null)
                {
                    HtmlNode rootNode = doc.DocumentNode;
                    HtmlNode containerNode = rootNode.SelectSingleNode("//div[@class='maincenter bgjob1']/div[@class='sr_ad']/div[@class='s_txt_jobs']");
                    if (containerNode != null)
                    {
                        HtmlNode tableNode = containerNode.SelectSingleNode("./table");
                        if (tableNode != null)
                        {
                            List<HtmlNode> trs = tableNode.SelectNodes("./tr").ToList();
                            if (trs != null && trs.Count > 1)
                            {
                                res = new CompanyInfo();
#if DEBUG
                            res.Id = ds.Id;
#endif
                                // Name.
                                HtmlNode compNameNode = trs[0].SelectSingleNode("./td");
                                if (compNameNode != null)
                                {
                                    res.Name = compNameNode.InnerText.Trim();
                                    res.Name = res.Name.Substring(0, res.Name.IndexOf("查看")).Trim();
                                }

                                // Cat/Scale..
                                HtmlNode catScaleContainerNode = trs[1].SelectSingleNode("./td");
                                foreach (HtmlNode tmpN in catScaleContainerNode.SelectNodes("./strong"))
                                    tmpN.Remove();
                                HtmlNodeCollection collection = catScaleContainerNode.SelectNodes("./text()");
                                if (collection != null)
                                {
                                    List<HtmlNode> attrNodes = collection.ToList();
                                    if (attrNodes != null && attrNodes.Count == 3)
                                    {
                                        res.Category = attrNodes[0].InnerText.Trim();
                                        // TODO. Fill other attributes of company.       
                                    }
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = null;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\tError in parse company {0}.\r\n{1}\r\n", ds.Id, ex.Message);
            }

            if (res != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\tEnd parse company {0}. Succeeded!\r\n", ds.Id);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\tEnd parse company {0}. No Info!\r\n", ds.Id);
            }
            Console.ForegroundColor = ConsoleColor.Gray;

            return res;
        }
    }
}
