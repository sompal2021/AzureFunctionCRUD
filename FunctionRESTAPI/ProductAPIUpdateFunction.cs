using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FunctionRESTAPI;

namespace AzureFunctions
{
    public static class ProductAPIUpdateFunction
    {
        [FunctionName("UpdateProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "product/{Id}")] HttpRequest req,
            ILogger log,int Id)
        {
            String Result = string.Empty;
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var Prod = JsonConvert.DeserializeObject<Product>(requestBody);
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("DbConnection")))
                {
                    connection.Open();
                    var query = @"Update Product Set Name=@Name, Description = @Description , UnitPrice=@UnitPrice, CategoryId = @CategoryId Where ProductId = @ProductId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", Prod.Name);
                    command.Parameters.AddWithValue("@Description", Prod.Description);
                    command.Parameters.AddWithValue("@UnitPrice",Prod.UnitPrice);
                    command.Parameters.AddWithValue("@CategoryId", Prod.CategoryId);
                    command.Parameters.AddWithValue("@ProductId", Id);
                    command.ExecuteNonQuery();
                    Result = "Update Sucessfully";
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            return new OkObjectResult(Result);
        }
    }
}
