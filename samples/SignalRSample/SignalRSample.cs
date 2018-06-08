using System;
using System.IO;
using System.Threading.Tasks;
using AzureFunctions.Contrib.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace SignalRSample
{
    public static class SampleFunctions
    {
        [FunctionName(nameof(Connect))]
        public static IActionResult Connect(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "connect/{hubName}")] HttpRequest req,
            string hubName, 
            [SignalR(ServiceName = "%service_name%", AccessKey = "%access_key%")] SignalRConnectionBuilder connectionBuilder,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            return new OkObjectResult(
                connectionBuilder.CreateConnectionInfo(hubName)
                );
        }

        [FunctionName(nameof(SendMessage))]
        public static async Task SendMessage(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "send/{hubName}/{target}")] HttpRequest req,
            string hubName,
            string target,
            [SignalR(ServiceName = "%service_name%", AccessKey = "%access_key%")] IAsyncCollector<SignalRMessage> messages,
            TraceWriter log)
        {
            dynamic arg = null;
            using (var sr = new StreamReader(req.Body))
            {
                arg = JsonConvert.DeserializeObject(await sr.ReadToEndAsync());
            }

            await messages.AddAsync(new SignalRMessage
            {
               Target = target,
               Hub = hubName,
               Arguments = new object[] { arg }

            });
        }


        /// <summary>
        /// Http triggered function to broadcast to a SignalR hub
        /// </summary>
        /// <param name="req"></param>
        /// <param name="redisItem"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(HttpTriggerBroadcastToHub))]
        public static async Task<IActionResult> HttpTriggerBroadcastToHub(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [SignalR(ServiceName = "%service_name%", AccessKey = "%access_key%")] IAsyncCollector<SignalRMessage> message,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");


            await message.AddAsync(new SignalRMessage()
            {
                Target = "broadcastMessage",
                Hub = "chat",
                Arguments = new object[] { $"Hub broadcast from function {nameof(HttpTriggerBroadcastToHub)}", $"Now it is {DateTime.Now}" }
            });

            return new OkResult();
        }

        /// <summary>
        /// Http triggered function to broadcast to a SignalR hub group
        /// </summary>
        /// <param name="req"></param>
        /// <param name="redisItem"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(HttpTriggerBroadcastToGroupHub))]
        public static IActionResult HttpTriggerBroadcastToGroupHub(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [SignalR(ServiceName = "%service_name%", AccessKey = "%access_key%", Groups = "dashboard")] out SignalRMessage message,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");



            message = new SignalRMessage()
            {
                Target = "broadcastMessage",
                Hub = "chat",
                Arguments = new object[] { $"Group broadcast from function {nameof(HttpTriggerBroadcastToGroupHub)}", $"Now it is {DateTime.Now}" }
            };

            return new OkResult();
        }
    }
}
