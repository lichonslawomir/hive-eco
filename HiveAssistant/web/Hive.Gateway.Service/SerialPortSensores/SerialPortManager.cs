using Hive.Gateway.Service.Services;
using System.IO.Ports;
using System.Text;

namespace Hive.Gateway.Service.SerialPortSensores;

public class SerialPortManager(SerialPort serialPort, ISensorBuffor sensorBuffor, ILogger logger)
{
    private static string? hiveId = null;
    private int _faildCount = 0;

    public static byte[] buffer = new byte[4];

    public async Task<bool> RunAsync(string port, CancellationToken stopToken)
    {
        serialPort.DiscardInBuffer();
        serialPort.DiscardOutBuffer();

        logger.LogInformation($"Listening for audio data on {port}...");

        while (_faildCount < 2048 && !stopToken.IsCancellationRequested)
        {
            var cmd = ReadNoneZero();
            //Console.WriteLine($"cmd: {cmd}");
            switch ((char)cmd)
            {
                case 'A':
                    hiveId = GetHiveId();
                    if (hiveId is not null)
                    {
                        var rb = GetByteCount();
                        if (rb.HasValue)
                        {
                            var b = new byte[rb.Value];
                            var cc = 0;
                            while (cc < rb.Value)
                            {
                                cc += serialPort.Read(b, cc, b.Length - cc);
                            }
                            cmd = ReadNoneZero();
                            if ((char)cmd == 'C')
                            {
                                await sensorBuffor.AddAudio(hiveId, port, b, stopToken);
                                _faildCount = 0;
                            }
                            else
                            {
                                ClearBuff(cmd, "RunAsync.A", b, cc);
                            }
                        }
                    }
                    break;

                case 'H':
                    var h1 = ReadFloat();
                    var cmd2 = ReadNoneZero();
                    var t1 = ReadFloat();
                    var cmd3 = ReadNoneZero();
                    var h2 = ReadFloat();
                    var cmd4 = ReadNoneZero();
                    var t2 = ReadFloat();
                    var cmd5 = ReadNoneZero();
                    if ((char)cmd2 == 'T' && (char)cmd3 == 'G')
                    {
                        await sensorBuffor.AddData(hiveId, port, BeeHive.Domain.Data.TimeSeriesKind.Temperature, t1, stopToken);
                        await sensorBuffor.AddData(hiveId, port, BeeHive.Domain.Data.TimeSeriesKind.Humidity, h1, stopToken);
                        _faildCount = 0;
                    }
                    else
                    {
                        BitConverter.TryWriteBytes(buffer, t1);
                        ClearBuff(cmd2, "RunAsync.T1", buffer, 4);
                        BitConverter.TryWriteBytes(buffer, h1);
                        ClearBuff(cmd3, "RunAsync.H1", buffer, 4);
                    }
                    if ((char)cmd4 == 'U' && (char)cmd5 == 'E')
                    {
                        await sensorBuffor.AddData(hiveId, port, BeeHive.Domain.Data.TimeSeriesKind.OutsideTemp, t1, stopToken);
                        await sensorBuffor.AddData(hiveId, port, BeeHive.Domain.Data.TimeSeriesKind.OutsideHum, h1, stopToken);
                        _faildCount = 0;
                    }
                    else
                    {
                        BitConverter.TryWriteBytes(buffer, t2);
                        ClearBuff(cmd2, "RunAsync.T2", buffer, 4);
                        BitConverter.TryWriteBytes(buffer, h2);
                        ClearBuff(cmd3, "RunAsync.H2", buffer, 4);
                    }
                    break;

                default:
                    ClearBuff(cmd, "RunAsync.default", null, 0);
                    break;
            }
        }

        return _faildCount == 0;
    }

    private string? GetHiveId()
    {
        StringBuilder sb = new StringBuilder();
        do
        {
            var c = (char)ReadNoneZero();
            if (c == ' ')
                return sb.ToString();
            sb.Append(c);
            if (sb.Length > 100)
                break;
        }
        while (true);
        var buf = Encoding.ASCII.GetBytes(sb.ToString());
        ClearBuff(buf.LastOrDefault(), "GetHiveId", buf, sb.Length);
        return null;
    }

    private int? GetByteCount()
    {
        StringBuilder sb = new StringBuilder();
        do
        {
            var b = ReadNoneZero();
            var c = (char)b;
            if (c == ' ')
            {
                if (int.TryParse(sb.ToString().Trim(), out var value))
                    return value;
                break;
            }
            sb.Append(c);
            if (sb.Length > 100)
                break;
        }
        while (true);
        var buf = Encoding.ASCII.GetBytes(sb.ToString());
        ClearBuff(buf.LastOrDefault(), "GetByteCount", buf, sb.Length);
        return null;
    }

    private void ClearBuff(int cmd, string method, byte[]? bytes, int bc)
    {
        logger.LogWarning($"{method}: unknowCmd: d:{cmd}, c:{(char)cmd}, hex:{cmd.ToString("X")}");
        if (bytes != null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Encoding.ASCII.GetString(bytes));
            sb.Append($"\r\n{bytes.Length}\\{bc}:\r\n");
            sb.Append(Convert.ToBase64String(bytes));
            logger.LogWarning($"Buffor:{sb.ToString()}");
        }

        ++_faildCount;
        serialPort.DiscardInBuffer();
        serialPort.DiscardOutBuffer();
    }

    private float ReadFloat()
    {
        serialPort.Read(buffer, 0, 4);
        return BitConverter.ToSingle(buffer, 0);
    }

    private int ReadNoneZero()
    {
        var cmd = 0;
        do
        {
            cmd = serialPort.ReadByte();
        }
        while (cmd == 0);
        return cmd;
    }
}