using CommandLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOpsNetTools
{
    class Options
    {
        [Option('n', "nuget", Required = true, HelpText = "Nuget Server.")]
        public string NugetServer { get; set; }

        [Option('f', "filter", Required = true, HelpText = "Filter")]
        public string Filter { get; set; }

        [Option('d', "directory", Required = true, HelpText = "directory")]
        public string  Directory { get; set; }

        [Option('t', "target", Required = true, HelpText = "Target")]
        public string Target { get; set; }

        [Option('k', "apiKey", Required = true, HelpText = "Api Key")]
        public string ApiKey { get; set; }

        [Option('e', "environment", Required = true, HelpText = "environment")]
        public string Environment { get; set; }

        [Option('c', "categories", Required = false, HelpText = "nunit categories")]
        public string Categories { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            bool error = false;
            var options =  new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                LogController log = new LogController();
                try
                {
                    var controller = new NugetController();
                    var deployment = new TestDeploymentController(
                        options.ApiKey,
                        options.Environment,
                        options.NugetServer,
                        options.Target,
                        options.Directory);

                    var nunit = new NUnitController(options.Directory);

                    log.Write("Found Packages");

                    var packages = controller.SearchPackages(options.NugetServer, options.Filter);

                    log.Write("package : {0}", packages);
                    ParallelOptions configurations= new ParallelOptions();
                    configurations.MaxDegreeOfParallelism = 5;
                    

                    Parallel.ForEach(packages, configurations,  (package) =>
                    {
                        try
                        {
                            deployment.DeployPackage(package);
                            var executed = nunit.Execute(package, options.Categories);
                            log.Write(string.Format("Executed package {0} {1}", package, executed));
                        }
                        catch (Exception ex)
                        {
                            var message = string.Format("{0} {1}", package, JsonConvert.SerializeObject(ex));                            
                            log.Write(message);
                            throw new ApplicationException("error:" + package);
                        }                         
                    });

                    var result = nunit.MergeResults(packages);

                    File.WriteAllText("TestResult.xml", result);
                }
                catch (Exception ex)
                {
                    log.Write(JsonConvert.SerializeObject(ex));
                    error = true;
                }
                finally {
                    Console.WriteLine(log.ToString());          
                }

                if (error) throw new ApplicationException("error");
            }
        }
    }
}
