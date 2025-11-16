using BeeHive.Cloud.Service.Client.Services;
using BeeHive.Contract.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services
    .AddSingleton<IAppState, AppStateSignalRClient>()
    .AddSingleton<IHiveService, HiveServiceHttpClient>()
    .AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();