using Hive.Gateway.Service.Services;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Runtime.InteropServices;

namespace Hive.Gateway.Service.SerialPortSensores;

public class ComBackgroundService(IOptions<ComPortsOptions> options, ISensorBuffor sensorBuffor, ILoggerFactory loggerFactory) : BackgroundService
{
    [DllImport("kernel32.dll")]
    private static extern uint SetThreadExecutionState(uint esFlags);

    private const uint ES_CONTINUOUS = 0x80000000;
    private const uint ES_SYSTEM_REQUIRED = 0x00000001;

    private ConcurrentDictionary<string, Task> _ports = new();
    private readonly ILogger _logger = loggerFactory.CreateLogger<ComBackgroundService>();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"ExecuteAsync: {nameof(ComBackgroundService)}");
        SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED);

        do
        {
            foreach (string port in SerialPort.GetPortNames())
            {
                if (!_ports.ContainsKey(port))
                {
                    _ports[port] = Task.Factory.StartNew(() => MenagePort(port, stoppingToken));
                }
            }
#if DEBUG
            if (!_ports.Any())
                _ports["test"] = Task.Factory.StartNew(() => FakePort("test", stoppingToken));
#endif
            await Task.Delay(10000, stoppingToken);
        }
        while (true);
    }

    private int GetBaudRate(string port)
    {
        if (options.Value.Ports.TryGetValue(port, out var baudRate))
            return baudRate;

        return options.Value.DefaultBaudRate;
    }

    private async Task MenagePort(string port, CancellationToken stoppingToken)
    {
        int failCont = 0;
        do
        {
            var logger = loggerFactory.CreateLogger($"{nameof(SerialPortManager)}:{port}");
            try
            {
                using var sp = new SerialPort(port, GetBaudRate(port));
                sp.Open();
                var mngr = new SerialPortManager(sp, sensorBuffor, logger);
                var ok = await mngr.RunAsync(port, stoppingToken);
                if (!ok)
                {
                    logger.LogWarning($"{port} has unknow data");
                }
                sp.Close();
                break;
            }
            catch (Exception ex)
            {
                ++failCont;
                logger.LogWarning(ex, $"{port} is busy or unavailable: {ex.Message}");
            }
            await Task.Delay(30000, stoppingToken);
        }
        while (failCont < 1000);
        _ports.Remove(port, out _);
    }

    private async Task FakePort(string port, CancellationToken stoppingToken)
    {
        int failCont = 0;
        do
        {
            try
            {
                await sensorBuffor.AddData("1", BeeHive.Domain.Data.TimeSeriesKind.Temperature, Random.Shared.NextSingle(), stoppingToken);
                await sensorBuffor.AddData("1", BeeHive.Domain.Data.TimeSeriesKind.Humidity, Random.Shared.NextSingle(), stoppingToken);
                await sensorBuffor.AddData("2", BeeHive.Domain.Data.TimeSeriesKind.Temperature, Random.Shared.NextSingle(), stoppingToken);
                await sensorBuffor.AddData("2", BeeHive.Domain.Data.TimeSeriesKind.Humidity, Random.Shared.NextSingle(), stoppingToken);
                await sensorBuffor.AddData("3", BeeHive.Domain.Data.TimeSeriesKind.Temperature, Random.Shared.NextSingle(), stoppingToken);
                await sensorBuffor.AddData("3", BeeHive.Domain.Data.TimeSeriesKind.Humidity, Random.Shared.NextSingle(), stoppingToken);
                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception)
            {
                ++failCont;
            }
        }
        while (failCont < 1000 && !stoppingToken.IsCancellationRequested);
        _ports.Remove(port, out _);
    }
}