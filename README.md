# SignalR bindings for Azure Functions
[![Build status](https://ci.appveyor.com/api/projects/status/a50a86d7ynjl45s1/branch/master?svg=true)](https://ci.appveyor.com/project/fbeltrao/azurefunctions-contrib-signalr)

Facilitates the usage of Azure SignalR in Azure Functions. Implementated operations:

- broadcast messages to hub
- broadcast messages to one or more group
- broadcast messages to one or more users. 
 
For more information about Azure SignalR Service [check here](https://docs.microsoft.com/en-us/azure/azure-signalr/signalr-overview)


## Installation
Install package AzureFunctions.Contrib.SignalR

```bash
dotnet package add AzureFunctions.Contrib.SignalR
```
```PS
Install-Package AzureFunctions.Contrib.SignalR
```

## Examples
### Broadcast to a SignalR Hub

```CSharp
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
```

### Broadcast to a group

```CSharp
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
```