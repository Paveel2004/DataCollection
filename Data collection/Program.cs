using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using GlobalClass.Static_data;
using GlobalClass.Dynamic_data;
using GlobalClass;
using System.Runtime.CompilerServices;
using System.CodeDom;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Data_collection.Connection;
using System.Net;

namespace Data_collection
{

    internal class Program
    {
        static async void StartServer(int port, Action<TcpClient> handleClient, IPAddress localAddr)
        {
            TcpListener server = null;
            try
            {
                server = new TcpListener(localAddr, port);
                server.Start();
                Console.WriteLine($"Сервер запущен на порту {port}. Ожидание подключений...");
                while (true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    Console.WriteLine("Подключен клмент!");
                    Task.Run(() => handleClient(client));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server?.Stop();
            }
        }
        static void HendleClient(TcpClient tcpClient)
        {
            try
            {
                using NetworkStream stream = tcpClient.GetStream();

                byte[] data = new byte[50];
                int bytesRead;

                // Читаем данные из потока
                while ((bytesRead = stream.Read(data, 0, data.Length)) != 0)
                {
                    string message = Encoding.UTF8.GetString(data, 0, bytesRead);

                    byte[] response;
                    if (message == "Привет!")
                    {
                        response = Encoding.UTF8.GetBytes("Покаы");
                    }
                    else
                    {
                        response = Encoding.UTF8.GetBytes("Неверная команда");
                    }

                   
                    stream.Write(response, 0, response.Length);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Закрываем соединение при завершении работы с клиентом
                tcpClient.Close();
            }
        }
        public static void SendMessage(string serverAddress, int port, string message)
        {
            try
            {
                // Создаем TcpClient и подключаемся к серверу
                using TcpClient client = new TcpClient(serverAddress, port);
                Console.WriteLine($"Подключено к серверу на порту {port}...");

                // Получаем поток для обмена данными с сервером
                using NetworkStream stream = client.GetStream();

                // Отправляем сообщение серверу
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
                Console.WriteLine($"Отправлено сообщение: {message}");

                // Читаем ответ от сервера
                data = new byte[256];
                int bytesRead = stream.Read(data, 0, data.Length);
                string response = Encoding.UTF8.GetString(data, 0, bytesRead);
                Console.WriteLine($"Ответ от сервера: {response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void ReceiveBroadcastMessages()
        {
            UdpClient udpClient = new UdpClient(11000);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 11000);

            try
            {
                while (true)
                {
                    byte[] buffer = udpClient.Receive(ref endPoint);
                    string message = Encoding.UTF8.GetString(buffer);
                    Console.WriteLine($"Received broadcast message: {message}");
                    if (message == "getAll")
                    {
                        SendMessage(endPoint.Address.ToString(), 2222, InformationGathererUser.GetUserName());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                udpClient.Close();
            }
        }

        static void Main(string[] args)
        {
            //StartupManager.HideConsoleWindow();
            //StartupManager.CreateBatStartup();

            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            Task.Run(() => StartServer(1111, HendleClient, localAddr));
            ReceiveBroadcastMessages();
            Console.ReadKey();

            try
            {
                //tring jsonFilePath = @"C:\Users\User\Desktop\ClientS6\ClientS6\bin\Debug\net6.0-windows\data.json";

                // string jsonContent = File.ReadAllText(jsonFilePath);
                //  dynamic data = JsonConvert.DeserializeObject(jsonContent);

                // var obj = JObject.Parse(jsonContent);
/*                string serverAddress = "127.0.0.1";//(string)obj["serverAddress"];

                DeviceData<NetworkInterfaceData> DataNetwork = new() { Data = NetworkInformationGatherer.GetNetworkInterfaces(), SerialNumberBIOS = InformationGathererBIOS.GetBiosSerialNumber() };
                DeviceData<CPUData> DataCPU = new() { SerialNumberBIOS = InformationGathererBIOS.GetBiosSerialNumber(), Data = InformationGathererCPU.GetCPU() };
                DeviceData<RAMData> DataRAM = new() { SerialNumberBIOS = InformationGathererBIOS.GetBiosSerialNumber(), Data = InformationGathererRAM.GetRAM() };
                DeviceData<VideoСardData> DataVideoCard = new() { SerialNumberBIOS = InformationGathererBIOS.GetBiosSerialNumber(), Data = InformationGathererVideoCard.GetModels() };


                Console.WriteLine(JsonHelper.SerializeDeviceData(new DeviceData<WindowData> { SerialNumberBIOS = InformationGathererBIOS.GetBiosSerialNumber(), Data = OSInformationGatherer.GetWindows() }));


                ServerMessageSender.SendMessage(serverAddress, 9440, JsonConvert.SerializeObject(new DeviceInitialization(InformationGathererBIOS.GetBiosSerialNumber(), OSInformationGatherer.GetComputerName())));
                ServerMessageSender.SendMessage(serverAddress, 9930, JsonHelper.SerializeDeviceData(DataNetwork));
                ServerMessageSender.SendMessage(serverAddress, 9860, JsonHelper.SerializeDeviceData(DataCPU));
                ServerMessageSender.SendMessage(serverAddress, 9790, JsonHelper.SerializeDeviceData(DataRAM));
                ServerMessageSender.SendMessage(serverAddress, 9370, JsonHelper.SerializeDeviceData(DataVideoCard));
                ServerMessageSender.SendMessage(serverAddress, 9230, JsonConvert.SerializeObject(new DiskData(InformationGathererDisk.TotalSpace(), InformationGathererBIOS.GetBiosSerialNumber())));
                ServerMessageSender.SendMessage(serverAddress, 9160, JsonConvert.SerializeObject(new OSData(OSInformationGatherer.GetOperatingSystem(), InformationGathererBIOS.GetBiosSerialNumber())));
                ServerMessageSender.SendMessageUsage<UsageRAM>(serverAddress, 9720, JsonConvert.SerializeObject(new UsageRAM(InformationGathererRAM.GetUsageRam(), InformationGathererBIOS.GetBiosSerialNumber())));
                ServerMessageSender.SendMessageUsage<UsageOS>(serverAddress, 9650, JsonConvert.SerializeObject(new UsageOS(InformationGathererUser.GetUserName(), OSInformationGatherer.GetSystemState(), InformationGathererBIOS.GetBiosSerialNumber())));
                ServerMessageSender.SendMessageUsage<UsageCPU>(serverAddress, 9580, JsonConvert.SerializeObject(new UsageCPU(InformationGathererCPU.GetProcessorTemperature(), InformationGathererCPU.GetCpuUsage(), InformationGathererBIOS.GetBiosSerialNumber())));
                ServerMessageSender.SendMessageUsage<UsageEthernet>(serverAddress, 9510, JsonConvert.SerializeObject(new UsageEthernet(NetworkInformationGatherer.EthernetSpeed(), InformationGathererBIOS.GetBiosSerialNumber())));
                ServerMessageSender.SendMessageUsage<UsageDisk>(serverAddress, 9300, JsonConvert.SerializeObject(new UsageDisk(InformationGathererDisk.TotalFreeSpace(), InformationGathererBIOS.GetBiosSerialNumber())));
                ServerMessageSender.SendMessageWindow(serverAddress, 9090, OSInformationGatherer.GetWindows());*/

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

}