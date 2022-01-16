
using System.IO.Ports;

var ArrayComPortsNames = SerialPort.GetPortNames();

Console.WriteLine($"COM ports; {ArrayComPortsNames.Length}");

foreach (var i in ArrayComPortsNames)
{

    var portName = i.ToString();
    Console.WriteLine(portName);
}

var _serialPort = new SerialPort();

_serialPort.PortName = "/dev/ttyUSB2";
_serialPort.BaudRate = 115200;

//_serialPort.Parity = Parity.None;
//_serialPort.StopBits = StopBits.One;
//_serialPort.DataBits = 8;
//_serialPort.Handshake = Handshake.None;
//_serialPort.DataReceived += port_DataReceived;

Console.WriteLine($"PortName: {_serialPort.PortName}");
Console.WriteLine($"BaudRate: {_serialPort.BaudRate}");
Console.WriteLine($"Parity: {_serialPort.Parity}");
Console.WriteLine($"StopBits: {_serialPort.StopBits}");
Console.WriteLine($"DataBits: {_serialPort.DataBits}");
Console.WriteLine($"Handshake: {_serialPort.Handshake}");

_serialPort.ReadTimeout = 2000;
_serialPort.Open();

bool _continue = true;

Task.Run(() =>
{
    while (_continue)
    {
        try
        {
            var message = _serialPort.ReadLine();
            Console.WriteLine($"output: {message}");
        }
        catch (TimeoutException) { }
    }
});

//while (_continue)
//{
//    var message = Console.ReadLine();
//    if (message == "quit")
//    {
//        _continue = false;
//    }
//    else
//    {
//        _serialPort.WriteLine(message);
//    }
//}

//void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
//{
//    // Show all the incoming data in the port's buffer
//    Console.WriteLine($"{_serialPort.ReadExisting()}");
//}

Console.WriteLine("Ready?");
Console.ReadLine();
_serialPort.WriteLine("AT");
Console.ReadLine();
