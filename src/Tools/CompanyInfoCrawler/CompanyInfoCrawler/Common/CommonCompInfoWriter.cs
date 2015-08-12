using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CompanyInfoCrawler
{
    public class CommonCompInfoWriter : ICompInfoWriter
    {
        public string Delimiter { get; set; }
        public bool IsOpen { get; set; }
        public string DesStreamUri { get; set; }

        private FileStream m_file;
        private StreamWriter m_sw;

        public CommonCompInfoWriter(string desStreamUri = "", string delimiter = ",")
        {
            Delimiter = delimiter;
            DesStreamUri = desStreamUri;
        }
        public bool Open()
        {
            // Just overwrite the des file.
            m_file = File.Create(DesStreamUri);
            if (m_file != null && File.Exists(DesStreamUri))
            {
                m_sw = new StreamWriter(m_file);
                IsOpen = true;
                return true;
            }
            return false;
        }
        public bool Write(CompanyInfo info)
        {
            string line = 
#if DEBUG
                info.Id + Delimiter + // Id is used for debug
#endif
                info.Name + Delimiter + info.Category + Delimiter + info.Address + Delimiter
                + (info.ScaleMin != -1 ? info.ScaleMin.ToString() : "") + Delimiter + (info.ScaleMax != -1 ? info.ScaleMax.ToString() : "");
            m_sw.WriteLine(line);
            return true;
        }

        public void Flush()
        {
            m_sw.Flush();
            m_file.Flush();
        }

        public void Close()
        {
            m_sw.Flush();
            m_sw.Close();
        }


    }
}
