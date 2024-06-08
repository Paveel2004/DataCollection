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
using System.Text.RegularExpressions;
using Data_collection.Control;
using System.Text.Json;
using static Data_collection.Gatherer.InformationGathererProcess;
using GlobalClass;
using Newtonsoft.Json.Converters;
using Server;
using System.Runtime.Intrinsics.Arm;
using System.Timers;
using System.Threading;
using System.CodeDom.Compiler;
using Microsoft.VisualBasic;
using Microsoft.Data.SqlClient;
using System.Data;
using Data_collection.Gatherer;
using Data_collection.Monitor;
using Data_collection.Monitor.Usage;
using Data_collection.Monitor.Static;
namespace Data_collection
{

    internal class Program
    {
        private static string connectionString = "Server=WIN-5CLMGM4LR48\\SQLEXPRESS; Database=Server; User Id=Name; Password=12345QWERTasdfg; TrustServerCertificate=true";

        //static string connectionString = "Data Source = DESKTOP-LVEJL0B\\SQLEXPRESS;Initial Catalog=S6ClientDB;Integrated Security=true;TrustServerCertificate=True "; // Замените на свой строку подключения
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

        public static List<ProcessInfo> GetProcessInfo()
        {
            List<ProcessInfo> processInfoList = new List<ProcessInfo>();

            // Получаем все процессы
            Process[] processlist = Process.GetProcesses();

            // Выводим информацию о каждом процессе
            foreach (Process theprocess in processlist)
            {
                string title = theprocess.MainWindowTitle != "" ? theprocess.MainWindowTitle : "—";
                double memoryUsageMB = theprocess.WorkingSet64 / Math.Pow(1024, 2);
                ProcessInfo processInfo = new ProcessInfo(theprocess.ProcessName, title, memoryUsageMB);

                processInfoList.Add(processInfo);
            }

            return processInfoList;
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
                    string json = null;
                    byte[] response = response = Encoding.UTF8.GetBytes("Неверная команда");
                    string clientIP = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
                    string site = null;
                    string siteBlockPattern = @"blockSite \[(.*?)\]";
                    string siteUnBlockPattern = @"unBlockSite \[(.*?)\]";
                    Match matchBlock = Regex.Match(message, siteBlockPattern);
                    Match matchUnBlock = Regex.Match(message, siteUnBlockPattern);
                    if (matchBlock.Success)
                    {
                        site = matchBlock.Groups[1].Value;
                        message = "blockSite";
                    }
                    if (matchUnBlock.Success)
                    {
                        site = matchUnBlock.Groups[1].Value;
                        message = "unBlockSite";
                    }

                    switch (message)
                    {
                        case "getUserName":
                            response = Encoding.UTF8.GetBytes(InformationGathererUser.GetUserName().ToString());
                            break;
                        case "getComputerName":
                            response = Encoding.UTF8.GetBytes(OSInformationGatherer.GetComputerName().ToString());
                            break;                   
                        case "getTotalRAM":
                            response = Encoding.UTF8.GetBytes((double.Parse(InformationGathererRAM.GetTotalPhysicalMemory().ToString()) / (1024 * 1024)).ToString());
                            break;
                        case "getUsers":
                            response = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize<string[]>(OSInformationGatherer.GetActiveUserNames()));
                            break;
                        case "blockSite":
                            WebControl.BlockedWebsite(new StringBuilder(site));
                            break;
                        case "unBlockSite":
                            WebControl.UnBlockedWebSite(site);
                            break;
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
                            json = JsonConvert.SerializeObject(NetworkInterfase);
                            response = Encoding.UTF8.GetBytes(json);
                            break;
                        case "getProcesses":
                            response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(GetProcessInfo()));
                            break;       
                        case "getApplications":
                            response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(GetInstalledApps()));
                            break;
                        case "AdditionalInformation":
                            var runtime = GetSystemUpTime();
                            var Info = new Dictionary<string, string>
{
    {"Загруженность ОЗУ", InformationGathererRAM.GetUsageRam() + " %"},
    {"Состояние ОС", OSInformationGatherer.GetSystemState().ToString()},
    {"Версия ОС", OSInformationGatherer.GetOperatingSystemVersion().ToString()},
    {"Свободное место", InformationGathererDrive.TotalFreeSpace().ToString()},
    {"Время работы", runtime.ToString(@"hh\:mm\:ss")}
};
                            json = JsonConvert.SerializeObject(Info, Formatting.Indented);
                            response = Encoding.UTF8.GetBytes(json);
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
        class ComputerData
        {
            public string ComputerName { get; set; }
            public string ProcessorModel { get; set; }
            public string ProcessorArchitecture { get; set; }
            public string ProcessorCores { get; set; }
            public string RAMSize { get; set; }
            public string RAMFrequency { get; set; }
            public string RAMType { get; set; }
            public string GPUModel { get; set; }
            public string OS { get; set; }
            public string OSVersion { get; set; }
            public string OSArchitecture { get; set; }
            public string TotalSpaceDisk { get; set; }
        }
        public static async Task ReceiveBroadcastMessages(string address, int port)
        {

            using var udpClient = new UdpClient(port);
            var brodcastAddress = IPAddress.Parse(address);
            udpClient.JoinMulticastGroup(brodcastAddress);
            string json;
            Console.WriteLine("Начало прослушивания сообщений");
            while (true)
            {
                var result = await udpClient.ReceiveAsync();
                string message = Encoding.UTF8.GetString(result.Buffer);
                if (message == "END") break;
                switch (message)
                {
                    case "getBios":
                        ServerMessageSender.SendMessage(result.RemoteEndPoint.Address.ToString(), 2222, InformationGathererBIOS.GetBiosSerialNumber());
                        break;
                    case "getAll":
                        var data = new Dictionary<string, string>
                        {
                            {"Имя компьютера", OSInformationGatherer.GetComputerName().ToString()},
                            {"Текущий пользователь", InformationGathererUser.GetUserName().ToString()},
                               {"IP Адрес", NetworkInformationGatherer.GetIPAddress().ToString()},
                               {"MAC Адрес", NetworkInformationGatherer.GetMacAddress().ToString()},
                               {"Процессор", InformationGathererCPU.GetProcessorName().ToString()},
                               {"Количество ядер в процессоре", InformationGathererCPU.GetProcessorCoreCount().ToString()},
                               {"Архитектура процессора", InformationGathererCPU.GetProcessorArchitecture().ToString()},                               
                               {"Операционная система", OSInformationGatherer.GetOperatingSystem().ToString()},                              
                               {"Оперативная память", (double.Parse(InformationGathererRAM.GetTotalPhysicalMemory().ToString()) / (1024 * 1024)).ToString() + " МБ"},
                               {"Объём диска", InformationGathererDrive.TotalSpace().ToString()},
                               {"Видеокарта", InformationGathererVideoCard.GetModel().ToString()}
                        };

                        json = JsonConvert.SerializeObject(data, Formatting.Indented);

                        ServerMessageSender.SendMessage(result.RemoteEndPoint.Address.ToString(), 2222, json);

                        break;
                    case "getBuild":
                        {
                            DeviceCharacteristics sborka = new DeviceCharacteristics
                            {
                                ComputerName = OSInformationGatherer.GetComputerName(),
                                ProcessorModel = InformationGathererCPU.GetProcessorName(),
                                ProcessorArchitecture = InformationGathererCPU.GetProcessorArchitecture(),
                                ProcessorCores = InformationGathererCPU.GetProcessorCoreCount().ToString(),
                                RAMSize = (double.Parse(InformationGathererRAM.GetTotalPhysicalMemory().ToString()) / (1024 * 1024)).ToString(),
                                RAMFrequency = InformationGathererRAM.GetRAMSpeed().ToString(),
                                RAMType = InformationGathererRAM.GetTypeRAM().ToString(),
                                GPUModel = InformationGathererVideoCard.GetModel(),
                                OS = OSInformationGatherer.GetOperatingSystem(),
                                OSVersion = OSInformationGatherer.GetOperatingSystemVersion(),
                                OSArchitecture = OSInformationGatherer.GetSystemBitArchitecture().ToString(),
                                TotalSpaceDisk = InformationGathererDrive.TotalSpace().ToString(),
                                SerialNumberBIOS = InformationGathererBIOS.GetBiosSerialNumber().ToString(),
                            };
                            json = JsonConvert.SerializeObject(sborka, Formatting.Indented);
                            ServerMessageSender.SendMessage(result.RemoteEndPoint.Address.ToString(), 3333, json);

                        }
                        break;
                    case "getApp":
                        json = GetInstalledAppsWithBiosSerialNumberAsJson(InformationGathererBIOS.GetBiosSerialNumber());
                        ServerMessageSender.SendMessage(result.RemoteEndPoint.Address.ToString(), 4444, json);
                        

                        break;
                }
                Console.WriteLine(message);
            }
            // отсоединяемся от группы
            udpClient.DropMulticastGroup(brodcastAddress);
        }
        public static double monitoringInterval = 60*1000*5;
        static string GetInstalledAppsWithBiosSerialNumberAsJson(string biosSerialNumber)
        {
            List<AppInfo> appsList = new List<AppInfo>();

            string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        string appName = subkey.GetValue("DisplayName") as string;

                        string installDateValue = subkey.GetValue("InstallDate") as string;
                        DateTime? installDate = null;
                        if (!string.IsNullOrEmpty(installDateValue) && DateTime.TryParseExact(installDateValue, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                        {
                            installDate = parsedDate;
                        }

                        object sizeObj = subkey.GetValue("EstimatedSize");
                        int? appSize = null;
                        if (sizeObj != null)
                        {
                            appSize = Convert.ToInt32(sizeObj);
                        }

                        if (!string.IsNullOrEmpty(appName))
                        {
                            appsList.Add(new AppInfo { Name = appName, InstallDate = installDate, Size = appSize });
                        }
                    }
                }
            }

            // Добавляем серийный номер биоса в самый верхний уровень JSON
            Dictionary<string, object> jsonObject = new Dictionary<string, object>();
            jsonObject.Add("BiosSerialNumber", biosSerialNumber);
            jsonObject.Add("InstalledApps", appsList);

            string json = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            return json;
        }
        public static void OSBoot()
        {
            string dateTimeString = DateTime.Now.ToString("s");
            DataBaseHelper.Query($"EXECUTE ДобавитьИспользование @СерийныйНомерBIOS='{InformationGathererBIOS.GetBiosSerialNumber()}', @ТипХарактеристики = 'ОС', @Характеристика = 'Состояние', @Значение = '{OSInformationGatherer.GetSystemState()}', @ДатаВремя = '{dateTimeString}'");
            DataBaseHelper.Query($"EXECUTE ДобавитьИспользование @СерийныйНомерBIOS='{InformationGathererBIOS.GetBiosSerialNumber()}', @ТипХарактеристики = 'ОС', @Характеристика = 'Текущий пользователь', @Значение = '{InformationGathererUser.GetUserName()}', @ДатаВремя = '{dateTimeString}'");
        }
        public static void PCBoot()
        {
            OSBoot();
        }
        

        public static int GetLastIdFromUsage(string connectionString)
        {
            int lastId = -1; // Инициализируем переменную для хранения последнего ID

            // SQL запрос для получения последнего ID из таблицы
            string sqlQuery = $"SELECT TOP 1 Id FROM Использование ORDER BY Id DESC";

            // Создаем подключение к базе данных
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Создаем команду для выполнения SQL запроса
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    // Открываем подключение к базе данных
                    connection.Open();

                    // Выполняем запрос и получаем последний ID
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        lastId = Convert.ToInt32(result);
                    }

                    // Закрываем подключение
                    connection.Close();
                }
            }

            return lastId;
        }
        public static void SaveLastIdToJson(int lastId, string filePath)
        {
            // Создаем объект для сохранения
            var lastIdObject = new { LastId = lastId };

            // Сериализуем объект в JSON
            string json = JsonConvert.SerializeObject(lastIdObject);

            // Записываем JSON в файл
            File.WriteAllText(filePath, json);
        }
        
        public static void SendUsageMessage()
        {
            //SaveLastIdToJson(, "lastUsageID.json");
        }
        
        static void Main(string[] args)
        {


            // Вызываем метод для получения данных и конвертируем в JSON
      

            // Выводим JSON на консоль
          
            //StartupManager.HideConsoleWindow();
            //StartupManager.CreateBatStartup();
            DataBaseHelper.connectionString = connectionString;
            // Запуск мониторинга использования оперативной памяти
            /*            AssemblyWriter.WriteDevice();
                        AssemblyWriter.WriteVideoCard();
                        AssemblyWriter.WriteDrive();
                        AssemblyWriter.WriteRam();
                        AssemblyWriter.WriteProcessor();
                       // AssemblyWriter.WritePhysicalNetworkInterface();
                        AssemblyWriter.WriteOperatingSystem();
                        AssemblyWriter.WriteUser();
                        AppMonitoring.StartMonitor();*/
            RAMUsageMonitor.StartMonitoring();

            //ProcessorUsageMonitor.StartMonitoring();
            //Writer.WriteDrive();
            // Writer.WriteRam();
            // Writer.WriteVideoCard();
            //OSBoot();

            //  AppMonitoringHelper.AppMonitor();



            IPAddress localAddr = IPAddress.Parse(NetworkInformationGatherer.GetIPAddress().ToString());
            Task.Run(() => StartServer(1111, HendleClient, localAddr));
            ReceiveBroadcastMessages("224.0.0.252", 11000);
            Console.ReadKey();
            
        }
    }

}