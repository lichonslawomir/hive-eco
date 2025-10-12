using System.IO.Ports;

string[] ports = SerialPort.GetPortNames();
Console.WriteLine("Available serial ports:");
for (int i = 0; i < ports.Length; i++)
{
    Console.WriteLine($"{i}: {ports[i]}");
}

if (ports.Length == 0)
{
    Console.WriteLine("No serial ports found.");
    return;
}

// Ask user to select a port
Console.Write("Select port number: ");
if (!int.TryParse(Console.ReadLine(), out int portIndex) || portIndex < 0 || portIndex >= ports.Length)
{
    Console.WriteLine("Invalid selection.");
    return;
}

string selectedPort = ports[portIndex];
int baudRate = 921600; // Change as needed

using (SerialPort serialPort = new SerialPort(selectedPort, baudRate))
{
    serialPort.Open();
    Console.WriteLine($"Listening on {selectedPort} at {baudRate} baud...");

    // Run the reading loop in a Task
    await Task.Run(() =>
    {
        while (true)
        {
            try
            {
                string line = serialPort.ReadLine();
                Console.WriteLine($"Received: {line}");
            }
            catch (TimeoutException)
            {
                // Ignore timeout, keep listening
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                break;
            }
        }
    });

    serialPort.Close();
}