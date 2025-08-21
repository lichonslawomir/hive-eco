namespace Hive.Gateway.Service.OsUtils;

public class FirewallService : IHostedService
{
    public const string RuleName = "Hive.Gateway.Service";

    private readonly NetshService _netsh;
    private readonly IReadOnlyCollection<int> _httpPorts;

    public FirewallService(ILoggerFactory loggerFactory, IConfiguration configuration)
    {
        _httpPorts = GetPorts(configuration);
        _netsh = new NetshService(loggerFactory);
    }

    public void AddRule()
    {
        _netsh.ExecuteCommandAsAdmin(
            _httpPorts.Select(httpPort => $"advfirewall firewall add rule name=\"{RuleName}\" enable=yes profile=any action=allow dir=in protocol=tcp localport={httpPort}"),
            $"For proper operation Axence nVision Web requires a rule for the port {string.Join(", ", _httpPorts)} to be added manually.");
    }

    public void DeleteRule()
    {
        _netsh.ExecuteCommandAsAdmin(
            _httpPorts.Select(httpPort =>
                $"advfirewall firewall delete rule name=\"{RuleName}\" protocol=tcp localport={httpPort}"));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        AddRule();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        DeleteRule();
        return Task.CompletedTask;
    }

    public static IReadOnlyCollection<int> GetPorts(IConfiguration configuration)
    {
        var urls = configuration["Urls"] ?? "";
        return urls.Split(";")
            .Where(x => x.Contains(":"))
            .Select(x => x.Split(":").Last())
            .Where(x => int.TryParse(x, out var _))
            .Select(int.Parse)
            .Distinct()
            .ToList();
    }
}