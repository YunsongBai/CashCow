using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyInfoCrawler
{
    public interface ICompInfoWriter
    {
        bool IsOpen { get; set; }

        bool Open();
        bool Write(CompanyInfo info);
        void Flush();
        void Close();
    }
}
