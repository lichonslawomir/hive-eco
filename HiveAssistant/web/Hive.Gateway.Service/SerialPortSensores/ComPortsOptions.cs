namespace Hive.Gateway.Service.SerialPortSensores;

public class ComPortsOptions
{
    public int DefaultBaudRate { get; set; }
    public Dictionary<string, int> Ports { get; set; } = new();
}