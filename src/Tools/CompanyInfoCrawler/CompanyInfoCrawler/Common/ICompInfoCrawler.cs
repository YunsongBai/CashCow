using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyInfoCrawler
{
    public interface ICompInfoCrawler
    {
        bool Open();
        void Crawl(ICompListEnumerator listEnum, ICompInfoParser parser, ICompInfoWriter writer);
        void Close();
    }
}
