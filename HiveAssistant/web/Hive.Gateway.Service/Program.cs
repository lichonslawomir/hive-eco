using BeeHive.App.Extensions.DependencyInjection;
using BeeHive.Infra.Extensions.DependencyInjection;
using Core.Infra.Backgrounds;
using Core.Infra.Schedule.Extensioms.DependencyInjection;
using Hive.Gateway.Service.Components;
using Hive.Gateway.Service.Extensions;
using Hive.Gateway.Service.Models;
using BeeHive.Infra.Sqlite.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseWindowsService(opt =>
{
    opt.ServiceName = "Hive.Gateway.Service";
});

builder.Services.AddLogging(logbuilder =>
{
    logbuilder.AddConsole();
    logbuilder.AddFile(builder.Configuration.GetSection("FileLogging"));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services.AddAppServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddBeeHiveDbContext(builder.Configuration)
    .Configure<BeeGardenConfig>(builder.Configuration.GetSection(nameof(BeeGardenConfig)))
    .AddHostedService<StartupService>()
    .AddJobSchedule().AddJobs()
    .AddGatewayServices(builder.Configuration);

builder.Services.AddHttpClient();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();
// Get logger from DI
var logger = app.Services.GetRequiredService<ILogger<Program>>();
// Log startup info
var env = builder.Environment;
logger.LogInformation("Hive.Gateway.Service starting, Environment: {EnvironmentName}", env.EnvironmentName);
if (builder.Configuration is IConfigurationRoot root)
{
    foreach (var provider in root.Providers)
        logger.LogInformation("Loaded configuration source: {Source}", provider.ToString());
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    //app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAuthorization();
//app.UseHttpsRedirection();

app.UseAntiforgery();
app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

app.Run();