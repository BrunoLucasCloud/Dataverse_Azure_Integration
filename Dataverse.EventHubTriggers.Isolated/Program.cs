using Dataverse.EventHubTriggers.Isolated.Service.Interface;
using Dataverse.EventHubTriggers.Isolated.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s =>
    {
        s.AddSingleton<IAccountService, AccountService>();
    })
    .Build();

host.Run();
