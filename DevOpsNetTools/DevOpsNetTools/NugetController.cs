using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Headers;
using DevOpsNetTools.Models;
namespace DevOpsNetTools
{
    public class NugetController
    {
        public NugetController()
        { 

        }
        public IEnumerable<string> SearchPackages(string Url, string Term)
        {
            var packages = this.GetPackages(Url);
            packages = packages.Where(c => c.Contains(Term));
            return packages.ToList();  
        }
        public IEnumerable<string> GetPackages(string Url)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            bool finish = false;
            var collection = new List<NugetCollection>();
            int skip = 0;
            while (!finish)
            {
                var response = client.GetAsync(
                    string.Format("api/v2/Packages?$filter=IsAbsoluteLatestVersion&$select=Id&$skip={0}&$top=100", skip)).Result;

                if (!response.IsSuccessStatusCode)
                    throw new ApplicationException(string.Format("GetPackages faild {0}", response.StatusCode));

                var list = response.Content.ReadAsAsync<NugetCollection>().Result;

                if (list.d.Count > 0)
                {
                    collection.Add(list);
                    skip += 100;
                }                    
                else
                    finish = true;
            }
            return collection.SelectMany(c => c.d).Select(c => c.Id).Distinct().ToList();
        }


    }
}
