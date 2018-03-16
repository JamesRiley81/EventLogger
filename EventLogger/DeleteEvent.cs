using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogger
{
    public class DeleteEvent
    {
        public string FileName;
        public string FilePath;
        public string User;
        public string Day;
        public DeleteEvent(string fName, string fPath, string u, string d)
        {
            FileName = fName; FilePath = fPath; User = u; Day = d;
        }
        public DeleteEvent()
        {

        }
    }
}
