using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOpsNetTools
{
    public class LogController
    {
        private StringBuilder sb = new StringBuilder();

        public void Write(string message)
        {
            sb.AppendLine(message);
        }
        public void Write(string format, IEnumerable<string> Collection)
        { 
            foreach (var item in Collection)
	        {
                sb.AppendLine(string.Format(format, item));
	        }
        }
        public void Write(string format, string item)
        {
            sb.AppendLine(string.Format(format, item));
        }
        public override string ToString()
        {
            return this.sb.ToString(); 
        }

    }
}
