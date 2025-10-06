using BeeHive.App.Extensions.DependencyInjection;
using BeeHive.Cloud.Service.Components;
using BeeHive.Infra.Extensions.DependencyInjection;
using Core.Infra.Backgrounds;

namespace BeeHive.Cloud.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddLogging(logbuilder =>
            {
                logbuilder.AddConsole();
                logbuilder.AddFile(builder.Configuration.GetSection("FileLogging"));
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAppServices()
                .AddInfrastructureServices(builder.Configuration)
                .AddHostedService<StartupService>();

            // Add services to the container.
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
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseBlazorFrameworkFiles();
            app.UseAntiforgery();

            app.MapControllers();

            app.MapRazorComponents<Application>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            app.Run();
        }
    }
}