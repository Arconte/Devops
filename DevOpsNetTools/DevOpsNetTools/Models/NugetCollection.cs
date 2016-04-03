using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOpsNetTools.Models
{
    public class NugetItem {
        public string Id { get; set; }
    }    
    public class NugetCollection
    {        
        public List<NugetItem> d { get; set; }
    }
}
