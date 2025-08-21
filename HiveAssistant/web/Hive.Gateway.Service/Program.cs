using BeeHive.App.Extensions.DependencyInjection;
using BeeHive.Infra.Extensions.DependencyInjection;
using Core.Infra.Backgrounds;
using Core.Infra.Schedule.Extensioms.DependencyInjection;
using Hive.Gateway.Service.Components;
using Hive.Gateway.Service.Extensions;
using Hive.Gateway.Service.Models;
using Hive.Gateway.Service.OsUtils;

namespace Hive.Gateway.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //builder.Services.AddHostedService<FirewallService>();

            builder.Services.AddAppServices()
                .AddInfrastructureServices(builder.Configuration)
                .Configure<BeeGardenConfig>(builder.Configuration.GetSection(nameof(BeeGardenConfig)))
                .AddHostedService<StartupService>()
                .AddJobSchedule()
                .AddGatewayServices(builder.Configuration);

            builder.Services.AddHttpClient();
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

            app.UseAuthorization();

            app.UseStaticFiles();
            app.UseBlazorFrameworkFiles();
            app.UseAntiforgery();

            app.MapControllers();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode(); ;

            app.Run();
        }
    }
}