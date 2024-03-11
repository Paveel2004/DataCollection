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

namespace Data_collection
{

    internal class Program
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void HideConsoleWindow()
        {
            IntPtr handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
        }


        public static void CreateBatStartup()
        {
            try
            {

                string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                using (StreamWriter sw = File.CreateText(Path.Combine(startupFolderPath, "MyStartup.bat")))
                {
                    sw.WriteLine("@echo off");
                    sw.WriteLine($"start \"\" \"{System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName}\"");
                }

                Console.WriteLine("Файл .bat успешно создан в папке автозапуска.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
        class DiskInfo
        {
            public string DeviceID { get; set; }
            public ulong Size { get; set; }
            public ulong FreeSpace { get; set; }
            public string VolumeName { get; set; }
        }
        static DiskInfo[] GetDiskInformation()
        {
            string query = "SELECT * FROM Win32_LogicalDisk";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection queryCollection = searcher.Get();

            DiskInfo[] diskInfo = new DiskInfo[queryCollection.Count];
            int index = 0;

            foreach (ManagementObject m in queryCollection)
            {
                diskInfo[index] = new DiskInfo
                {
                    DeviceID = m["DeviceID"].ToString(),
                    Size = Convert.ToUInt64(m["Size"]),
                    FreeSpace = Convert.ToUInt64(m["FreeSpace"]),
                    VolumeName = m["VolumeName"]?.ToString() ?? "N/A"
                };
                index++;
            }

            return diskInfo;
        }

        static void SendMessage(string serverAddress, int port, string message)
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
        static void SendMessageUsage<T>(string serverAddress, int port, string message)
        {
            T obj = JsonConvert.DeserializeObject<T>(message);
            var type = typeof(T);
            PropertyInfo property;

            if (obj is UsageRAM) {
                property = type.GetProperty("Workload");
                string currentWorkload = property.GetValue(obj)?.ToString();
                if (lastUsageRam != currentWorkload)
                {
                    SendMessage(serverAddress, port, message);
                    lastUsageRam = currentWorkload;
                }
            }
            if (obj is UsageOS) 
            {
                property = type.GetProperty("Status");
                string currentStatus = property.GetValue(obj)?.ToString();
                if(lastUsageOS != currentStatus)
                {
                    SendMessage(serverAddress, port, message);
                    lastUsageOS = currentStatus;
                }
            }
            if (obj is UsageCPU)
            {
                var property2 = type.GetProperty("Temperature");
                property = type.GetProperty("Workload");

                string currentTemperature = property2.GetValue(obj)?.ToString();

                string currentWorkload = property.GetValue(obj)?.ToString();

                string currentStatus = currentWorkload + '|' + currentTemperature;
                if (lastUsageCPU != currentStatus)
                {
                    SendMessage(serverAddress, port, message);
                    lastUsageCPU = currentStatus;
                }
            }
            if(obj is UsageEthernet) 
            {
                property = type.GetProperty("Speed");
                string cerrentSpeed = property.GetValue(obj)?.ToString();
                if (lastEthernetSpeed != cerrentSpeed)
                {
                    SendMessage(serverAddress, port, message);
                    lastEthernetSpeed = cerrentSpeed;
                }
            }

        }
        private static string lastEthernetSpeed = null;
        private static string lastUsageRam = null;
        private static string lastUsageOS = null;
        private static string lastUsageCPU = null;
        static void Main(string[] args)
        {
            //HideConsoleWindow();
            //CreateBatStartup();
          
                try
                {

                    string jsonFilePath = @"C:\Users\ASUS\source\repos\ClientS6\ClientS6\bin\Debug\net6.0-windows\data.json";
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    Server adress = JsonConvert.DeserializeObject<Server>(jsonContent);
                    DeviceData<NetworkInterfaceData> networkData = new()
                    {
                        Data = NetworkInformationGatherer.GetNetworkInterfaces(),
                        SerialNumberBIOS = InformationGathererBIOS.GetBiosSerialNumber()
                    };

                    DeviceData<CPUData> DataCPU = new()
                    {
                        SerialNumberBIOS = InformationGathererBIOS.GetBiosSerialNumber(),
                        Data = InformationGathererCPU.GetCPU()
                    };
                    DeviceData<RAMData> DataRAM = new()
                    {
                        SerialNumberBIOS = InformationGathererBIOS.GetBiosSerialNumber(),
                        Data = InformationGathererRAM.GetRAM()
                    };
                    string serverAddress = adress.serverAddress;

                    SendMessage(serverAddress, 9930, JsonHelper.SerializeDeviceData(networkData));
                    SendMessage(serverAddress, 9860, JsonHelper.SerializeDeviceData(DataCPU));
                    SendMessage(serverAddress, 9790, JsonHelper.SerializeDeviceData(DataRAM));

                    while (true)
                    {
                        SendMessageUsage<UsageRAM>(serverAddress, 9720, JsonConvert.SerializeObject(new UsageRAM(InformationGathererRAM.GetUsageRam(),InformationGathererBIOS.GetBiosSerialNumber())));
                        SendMessageUsage<UsageOS>(serverAddress, 9650, JsonConvert.SerializeObject(new UsageOS(InformationGathererUser.GetUserName(), OSInformationGatherer.GetSystemState(), InformationGathererBIOS.GetBiosSerialNumber())));
                        SendMessageUsage<UsageCPU>(serverAddress, 9580, JsonConvert.SerializeObject(new UsageCPU(InformationGathererCPU.GetProcessorTemperature(), InformationGathererCPU.GetCpuUsage(), InformationGathererBIOS.GetBiosSerialNumber())));
                        SendMessageUsage<UsageEthernet>(serverAddress, 9510, JsonConvert.SerializeObject(new UsageEthernet(NetworkInformationGatherer.EthernetSpeed(), InformationGathererBIOS.GetBiosSerialNumber())));
                        
                    }            
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
        }
    }

}