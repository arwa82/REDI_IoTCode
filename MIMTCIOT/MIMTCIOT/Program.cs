using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Common.Exceptions;
using System.IO.Ports;
using Newtonsoft.Json;

namespace MIMTCIOT
{
    class Program
    {

        static RegistryManager registryManager;
        static string deviceKey;
        static DeviceClient deviceClient;
        static string connectionString = "HostName=mimtciot.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=qhcRKgaI2cy3pxd35CM2X3ip1MHD3wc6gQ7UTiOV4Zo=";
        static string iotHubUri = "mimtciot.azure-devices.net";

        static void Main(string[] args)
        {

            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            AddDeviceAsync().Wait();

            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("miiotdevice", deviceKey));

            SerialPort serialPort = new SerialPort("COM4");

            serialPort.BaudRate = 9600;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = Handshake.None;

            serialPort.DataReceived += SerialPort_DataReceived;

            serialPort.Open();

            Console.WriteLine("Press Any Key to terminate");
            Console.ReadKey();
            serialPort.Close();

        }

        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            string data = port.ReadLine().Replace("\r","");

            string[] readings = data.Split(',');

            //Trapping a scenario where we get a partial read from the sensor.
            if (readings.Length == 2)
            {
                SensorData sensor = new SensorData { Student = "StudentMI", Humidity = readings[0], Temperature = readings[1] };

                var JsonString = JsonConvert.SerializeObject(sensor);

                var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(JsonString));

                deviceClient.SendEventAsync(message);

                Console.WriteLine("data sent:  {0}", JsonString);
            }


        }


        private async static Task AddDeviceAsync()
        {

            string deviceId = "miiotdevice";
            Device device;

            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }

            deviceKey = device.Authentication.SymmetricKey.PrimaryKey;
            Console.WriteLine("Generated device key: {0}", deviceKey);
        }
    }
}
