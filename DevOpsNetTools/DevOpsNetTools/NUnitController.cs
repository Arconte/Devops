using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DevOpsNetTools
{
    public class NUnitController
    {
        private string Destination;
        private ProcessController ProcessController = new ProcessController(); 
        public NUnitController(string Destination)
        {
            this.Destination = Destination;
        }    
        public bool Execute(string Package, string Categories)
        {
            var directory =  Path.Combine(this.Destination, Package);
            var target =  Path.Combine(directory, "RunTest.ps1");

            if (!File.Exists(target))
                return false;

            if (string.IsNullOrWhiteSpace(Categories))
            {
                var args = string.Format(@" -ExecutionPolicy ByPass -file ""{0}"" ", target );
                this.ProcessController.Run(args, directory );            
            }
            else {
                var args = string.Format(@" -ExecutionPolicy ByPass -file ""{0}"" -Categories ""{1}""", target, Categories);
                this.ProcessController.Run(args, directory);            
            }
            return true;
        }


        public string MergeResults(IEnumerable<string> Packages)
        {
            List<string> elements = new List<string>();
            foreach (var item in Packages)
            {
                var directory = Path.Combine(Destination, item);
                var target = Path.Combine(directory, "TestResult.xml");
                if (File.Exists(target))
                {
                    var str = File.ReadAllText(target);
                    elements.Add(str);  
                }                
            }
            return Merge(elements); 
        }
        private string Merge(IEnumerable<string> Contents)
        {
            var Elements = new List<XElement>();
            foreach (var item in Contents)
            {
                Elements.Add(XElement.Parse(item));
            }
            return this.Merge(Elements).ToString(); 
        }
        private  XElement Merge(IEnumerable<XElement> Paths)
        {
            //first item  mark the common attributes             
            var root = new XElement(Paths.ElementAt(0));
            var env = root.Element("environment");
            var culture = root.Element("culture-info");
            root.RemoveNodes();
            root.Add(env);
            root.Add(culture);
            foreach (var item in Paths)
            {
                var suite = item.Elements("test-suite").ToList();
                root.Add(suite);
            }
            return root;
        }        
    }
}
