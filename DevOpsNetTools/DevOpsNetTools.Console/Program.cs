using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOpsNetTools.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            DevOpsNetTools.NUnitController controller = new NUnitController();
            var str = File.ReadAllText(@"D:\Arconte\Templar\Tests\Templar.Service.SmokeTest\bin\Debug\TestResult.xml");
            var stra = File.ReadAllText(@"D:\Arconte\Templar\Tests\Templar.Service.FunctionalTest\bin\Debug\TestResult.xml");
            var result =  controller.Merge(new List<string>() { str, stra });
            File.WriteAllText("output.xml", result);
        }
    }
}
