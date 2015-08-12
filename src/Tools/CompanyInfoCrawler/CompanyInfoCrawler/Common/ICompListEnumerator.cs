using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyInfoCrawler
{
    public interface ICompListEnumerator
    {
        void Reset();
        bool MoveNext();
        HtmlDataSrcDesc GetCompDataSrcDesc();
    }
}
