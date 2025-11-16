using System.Text.Json.Serialization;
using BeeHive.App.Extensions.DependencyInjection;
using BeeHive.Cloud.Service.Components;
using BeeHive.Cloud.Service.Extensions;
using BeeHive.Cloud.Service.Hubs;
using BeeHive.Infra.Extensions.DependencyInjection;
using Core.Infra.Backgrounds;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logbuilder =>
{
    logbuilder.AddConsole();
    logbuilder.AddFile(builder.Configuration.GetSection("FileLogging"));
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAppServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddHostedService<StartupService>()
    .AddCloudServices(builder.Configuration);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapHub<RefreshHub>("/refresh-hub");
app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BeeHive.Cloud.Service.Client._Imports).Assembly);

app.Run();