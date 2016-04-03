using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DevOpsNetTools
{
    public class NUnitController
    {
        public string Merge(IEnumerable<string> Paths)
        {
            var Elements = new List<XElement>();

            foreach (var item in Paths)
            {
                Elements.Add(XElement.Parse(item));
            }

            return this.Merge(Elements).ToString(); 
        }
        public XElement Merge(IEnumerable<XElement> Paths)
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
