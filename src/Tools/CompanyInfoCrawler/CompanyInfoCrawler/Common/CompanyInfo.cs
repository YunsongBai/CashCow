using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyInfoCrawler
{
    public class CompanyInfo
    {
        public CompanyInfo()
        {
            ScaleMin = -1;
            ScaleMax = -1;
        }
        // The Id is used for debug.
#if DEBUG
        public int Id { get; set; }
#endif
        public string Name { get; set; }
        public string Category { get; set; }
        public int ScaleMin { get; set; }
        public int ScaleMax { get; set; }
        public string Brief { get; set; }
        public string Address { get; set; }
        public string WebSiteUrl { get; set; }
    }
}
