using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dataverse.ServiceBusTrigger.Isolated.Service.Interface;
using Dataverse.ServiceBusTrigger.Isolated.Service;


var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s =>
    {
        s.AddSingleton<IAccountService, AccountService>();
    })
    .Build();

host.Run();
