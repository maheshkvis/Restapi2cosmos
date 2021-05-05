using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Web.Helpers;

namespace Api2cosmos
{
    public static class Function1
    {
        [FunctionName("Function1")]

        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
        databaseName: "",
        collectionName: "",
        ConnectionStringSetting = "CosmosDbConnectionString")]IAsyncCollector<dynamic> documentsOut,
        ILogger log)
        {
            string rvalue = "ok";
            log.LogInformation("C# HTTP trigger function processed a request.");

            using (var client = new HttpClient())
            {
               
                //HTTP GET
                var responseTask = client.GetAsync("add api URL");
                responseTask.Wait();
                
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    rvalue = result.Content.ReadAsStringAsync().Result;
                    dynamic jsonval = JsonConvert.DeserializeObject<dynamic>(rvalue);
                    
                    foreach (dynamic element in jsonval.ResponseObject)
                    {
                        var temp = element;
                        var temp2 = element.id;
                        element.id= element.id.ToString();
                        await documentsOut.AddAsync(element);
                    }

                }
            }

            return new OkObjectResult(rvalue);
        }
    }
}
