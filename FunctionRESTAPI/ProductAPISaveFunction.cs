using FunctionRESTAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;


namespace AzureFunctions
{
    public static class ProductAPISaveFunction
    {
        [FunctionName("SaveProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "product")] HttpRequest req, ILogger log)
        {
            String Result = string.Empty;
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var Prod = JsonConvert.DeserializeObject<Product>(requestBody);
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("DbConnection")))
                {
                    connection.Open();
                    if (!String.IsNullOrEmpty(Prod.Name) )
                    {
                        log.LogInformation("Part 2");
                        var query = $"INSERT INTO [Product] (Name,Description,UnitPrice,CategoryId) VALUES('{Prod.Name}','{Prod.Description}','{Prod.UnitPrice}','{Prod.CategoryId}' )";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.ExecuteNonQuery();
                        Result = "Save Sucessfully";
                        log.LogInformation("Part 3");
                    }
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
                return new BadRequestResult();
            }
            return new OkObjectResult(Result);
        }
    }
}
 