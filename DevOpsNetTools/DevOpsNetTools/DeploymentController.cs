using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DevOpsNetTools
{
    public class TestDeploymentController
    {
        private string Server;
        private string Destination;
        private string PackageSource;
        private string Environment;
        private string ApiKey;
        public TestDeploymentController(
            string ApiKey, 
            string Environment,
            string PackageSource, 
            string Server, 
            string Destination)
        {
            this.ApiKey = ApiKey;  
            this.Environment = Environment;
            this.PackageSource = PackageSource + "api/v2/Package";
            this.Server = Server;
            this.Destination = Destination;            
        }
        public void DeployPackage(string package ) {

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Server);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("apiKey", this.ApiKey);

            var response =  client.PostAsJsonAsync("api/deployment/package", new List<object>(){ 
                new {
                    PackageSource = this.PackageSource,
                    PackageName = package,
                    Destination = string.Format("{0}/{1}", Destination, package),
                    Environment = Environment, 
                    PackageType = "Test"
                }
            }).Result;
            
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(string.Format("Install Package faild {0}", response.StatusCode));            
        }
    }
}
