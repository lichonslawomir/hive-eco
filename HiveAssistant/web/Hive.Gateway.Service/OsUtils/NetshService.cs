using System.Diagnostics;
using System.Security.Principal;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Hive.Gateway.Service.OsUtils;

public class NetshService
{
    private readonly ILogger _logger;

    public NetshService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<NetshService>();
    }

    public void ExecuteCommandAsAdmin(IEnumerable<string> commands, string? nonAdministratorRoleLogWarning = null)
    {
        try
        {
            if (!IsAdministrator())
            {
                _logger.LogWarning("Axence nVision Web is not running with admin privileges.");
                if (!string.IsNullOrEmpty(nonAdministratorRoleLogWarning))
                {
                    _logger.LogWarning(nonAdministratorRoleLogWarning);
                }
                return;
            }

            _logger.LogInformation("Axence nVision Web is running with admin privileges");

            foreach (var command in commands)
            {
                var startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = "netsh",
                    Arguments = command,
                    Verb = "runas"
                };
                using (var process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                    _logger.LogInformation($"Executed Netsh service command with exit code: {process.ExitCode}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(ExecuteCommandAsAdmin));
        }
    }

    private bool IsAdministrator()
    {
#pragma warning disable CA1416
        return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
            .IsInRole(WindowsBuiltInRole.Administrator);
#pragma warning restore CA1416
    }
}