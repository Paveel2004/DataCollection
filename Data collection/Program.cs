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
using System.Net.Sockets;
using System.Net;
using System.Text;


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
        static List<string> GetInstalledApps()
        {
            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            List<string> installedApps = new List<string>();

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        if (subkey.GetValue("DisplayName") != null)
                        {
                            installedApps.Add(subkey.GetValue("DisplayName").ToString());
                        }
                    }
                }
            }

            return installedApps;
        }

        public static List<string> GetProcessInfo()
        {
            List<string> processInfoWithTitle = new List<string>();
            List<string> processInfoWithoutTitle = new List<string>();

            // Получаем все процессы
            Process[] processlist = Process.GetProcesses();

            // Выводим информацию о каждом процессе
            foreach (Process theprocess in processlist)
            {
                string title = theprocess.MainWindowTitle != "" ? theprocess.MainWindowTitle : "—";
                string processDetails = $"Process:{theprocess.ProcessName}\nTitle:{title}\nMemory:{theprocess.WorkingSet64} B\n";

                if (theprocess.MainWindowTitle != "")
                {
                    processInfoWithTitle.Add(processDetails);
                }
                else
                {
                    processInfoWithoutTitle.Add(processDetails);
                }
            }

            // Сначала добавляем процессы с названиями, а затем все остальные
            processInfoWithTitle.AddRange(processInfoWithoutTitle);

            return processInfoWithTitle;
        }

        static TimeSpan GetSystemUpTime()
        {
            return TimeSpan.FromMilliseconds(Environment.TickCount64);
        }
        static void HendleClient(TcpClient tcpClient)
        {
            try
            {
                using NetworkStream stream = tcpClient.GetStream();

                byte[] data = new byte[999999];
                int bytesRead;

                // Читаем данные из потока
                while ((bytesRead = stream.Read(data, 0, data.Length)) != 0)
                {
                    string message = Encoding.UTF8.GetString(data, 0, bytesRead);

                    byte[] response = response = Encoding.UTF8.GetBytes("Неверная команда");
                    string clientIP = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();

                    switch (message)
                    {
                        case "getTemperatureCPU":
                            response = Encoding.UTF8.GetBytes(InformationGathererCPU.GetProcessorTemperature().ToString());
                            break;
                        case "getUsageCPU":
                            response = Encoding.UTF8.GetBytes(InformationGathererCPU.GetCpuUsage().ToString());
                            break;                                     
                        case "getSpeedEthernet":
                            response = Encoding.UTF8.GetBytes(NetworkInformationGatherer.EthernetSpeed().ToString());
                            break;
                        case "getTraffic [Network Interfase Name]":
                            Sniffer.StartSend("Intel(R) Wi-Fi 6 AX201 160MHz", clientIP, 2222);
                            break;
                        case "getNetworkInterfases":
                            var NetworkInterfase = NetworkInformationGatherer.GetNetworkInterfaces();
                            string json = JsonConvert.SerializeObject(NetworkInterfase);
                            response = Encoding.UTF8.GetBytes(json);
                            break;
                        case "getProcesses":
                            response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(GetProcessInfo()));
                            break;
                        case "closeProcess [Name]":
                            break;
                        case "getKeye":
                            break;
                        case "getUsageRAM":
                            break;
                        case "getApplications":
                            response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(GetInstalledApps()));
                            break;
                        case "AdditionalInformation":
                            var runtime = GetSystemUpTime();
                            response = Encoding.UTF8.GetBytes(
                                $"Загруженность ОЗУ: {InformationGathererRAM.GetUsageRam()} %\n" +
                                $"Состояние ОС: {OSInformationGatherer.GetSystemState()}\n" +
                                $"Версия ОС: {OSInformationGatherer.GetOperatingSystemVersion()} \n" +
                                $"Свободное место: {InformationGathererDisk.TotalFreeSpace()} \n" +
                                $"Время работы {runtime.ToString(@"hh\:mm\:ss")}");
                            break;
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
        public static async Task ReceiveBroadcastMessages(string address, int port)
        {
            
            using var udpClient = new UdpClient(port);
            var brodcastAddress = IPAddress.Parse(address); 
            udpClient.JoinMulticastGroup(brodcastAddress);
            Console.WriteLine("Начало прослушивания сообщений");
            while (true)
            {
                var result = await udpClient.ReceiveAsync();
                string message = Encoding.UTF8.GetString(result.Buffer);
                if (message == "END") break;
                switch(message)
                {
                    case "getAll":
                        ServerMessageSender.SendMessage(result.RemoteEndPoint.Address.ToString(), 2222,
                            $"IP Адрес: {NetworkInformationGatherer.GetIPAddress().ToString()}\n" +
                            $"MAC Адрес: {NetworkInformationGatherer.GetMacAddress().ToString()}\n" +
                            $"Процессор: {InformationGathererCPU.GetProcessorName().ToString()}\n" +
                            $"Количество ядер в процессоре: {InformationGathererCPU.GetProcessorCoreCount().ToString()}\n" +
                            $"Архитектура процессора: {InformationGathererCPU.GetProcessorArchitecture().ToString()}\n" +
                            $"Имя компьютера: {OSInformationGatherer.GetComputerName().ToString()}\n" +
                            $"Операционная система: {OSInformationGatherer.GetOperatingSystem().ToString()}\n" +
                            $"Текущий пользователь: {InformationGathererUser.GetUserName().ToString()}\n" +
                            $"Оперативная память: {InformationGathererRAM.GetTotalPhysicalMemory().ToString()}\n" +
                            $"Объём диска: {InformationGathererDisk.TotalSpace().ToString()}\n" +
                            $"Видеокарта: {InformationGathererVideoCard.GetModel().ToString()}\n");
                            
                        break;
                }
                Console.WriteLine(message);
            }
            // отсоединяемся от группы
            udpClient.DropMulticastGroup(brodcastAddress);
        }

        static void Main(string[] args)
        {
            //StartupManager.HideConsoleWindow();
            //StartupManager.CreateBatStartup();

            IPAddress localAddr = IPAddress.Parse("192.168.1.52");

            Task.Run(() => StartServer(1111, HendleClient, localAddr));
            ReceiveBroadcastMessages("224.0.0.252",11000);
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