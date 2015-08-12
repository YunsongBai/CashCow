using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CompanyInfoCrawler
{
    public class _51CompListEnumerator : ICompListEnumerator
    {
        private const string c_urlPattern = @"http://search.51job.com/list/co,c,*,000000,10,1.html";
        public int MinCompId { get; private set; }
        //private const int c_minCompId = 141790;
        //private const int c_maxCompId = 35000000; 141798
        public int MaxCompId { get; private set; }
        private int m_curId = -1;
        public _51CompListEnumerator()
        {
            MinCompId = 110000;
            MaxCompId = 210000;
            Reset();
        }

        public void Reset()
        {
            m_curId = MinCompId;
        }

        public bool MoveNext()
        {
            Debug.Assert(m_curId != -1);
            return m_curId++ <= MaxCompId;
        }

        HtmlDataSrcDesc ICompListEnumerator.GetCompDataSrcDesc()
        {
            return new HtmlDataSrcDesc()
            {
                Id = m_curId,
                Url = c_urlPattern.Replace("*", m_curId.ToString())
            };
        }
    }
}
