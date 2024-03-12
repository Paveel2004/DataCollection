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



        static void Main(string[] args)
        {
            //HideConsoleWindow();
            //CreateBatStartup();

            try
            {
                //string jsonFilePath = @". ClientS6\ClientS6\bin\Debug\net6.0-windows\data.json";

                //string jsonContent = File.ReadAllText(jsonFilePath);
                //Server adress = JsonConvert.DeserializeObject<Server>(jsonContent);
                DeviceData<NetworkInterfaceData> DataNetwork = new() { Data = NetworkInformationGatherer.GetNetworkInterfaces(), SerialNumberBIOS = InformationGathererBIOS.GetBiosSerialNumber() };
                DeviceData<CPUData> DataCPU = new() { SerialNumberBIOS = InformationGathererBIOS.GetBiosSerialNumber(), Data = InformationGathererCPU.GetCPU() };
                DeviceData<RAMData> DataRAM = new() { SerialNumberBIOS = InformationGathererBIOS.GetBiosSerialNumber(), Data = InformationGathererRAM.GetRAM() };
                DeviceData<VideoСardData> DataVideoCard = new() { SerialNumberBIOS = InformationGathererBIOS.GetBiosSerialNumber(), Data = InformationGathererVideoCard.GetModels() };

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /*///////////////////////////////////////////////////////////////////////////////////////////////////*/string serverAddress = "127.0.0.1"; ///////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Console.WriteLine();
                ServerMessageSender.SendMessage(serverAddress, 9440, JsonConvert.SerializeObject(new DeviceInitialization(InformationGathererBIOS.GetBiosSerialNumber(), OSInformationGatherer.GetComputerName())));
                ServerMessageSender.SendMessage(serverAddress, 9930, JsonHelper.SerializeDeviceData(DataNetwork));
                ServerMessageSender.SendMessage(serverAddress, 9860, JsonHelper.SerializeDeviceData(DataCPU));
                ServerMessageSender.SendMessage(serverAddress, 9790, JsonHelper.SerializeDeviceData(DataRAM));
                ServerMessageSender.SendMessage(serverAddress, 9370, JsonHelper.SerializeDeviceData(DataVideoCard));
                ServerMessageSender.SendMessage(serverAddress, 9230, JsonConvert.SerializeObject(new DiskData(InformationGathererDisk.TotalSpace(),InformationGathererBIOS.GetBiosSerialNumber())));
                ServerMessageSender.SendMessage(serverAddress, 9160, JsonConvert.SerializeObject(new OSData(OSInformationGatherer.GetOperatingSystem(), InformationGathererBIOS.GetBiosSerialNumber())));
                while (true)
                {
                    ServerMessageSender.SendMessageUsage<UsageRAM>(serverAddress, 9720, JsonConvert.SerializeObject(new UsageRAM(InformationGathererRAM.GetUsageRam(), InformationGathererBIOS.GetBiosSerialNumber())));
                    ServerMessageSender.SendMessageUsage<UsageOS>(serverAddress, 9650, JsonConvert.SerializeObject(new UsageOS(InformationGathererUser.GetUserName(), OSInformationGatherer.GetSystemState(), InformationGathererBIOS.GetBiosSerialNumber())));
                    ServerMessageSender.SendMessageUsage<UsageCPU>(serverAddress, 9580, JsonConvert.SerializeObject(new UsageCPU(InformationGathererCPU.GetProcessorTemperature(), InformationGathererCPU.GetCpuUsage(), InformationGathererBIOS.GetBiosSerialNumber())));
                    ServerMessageSender.SendMessageUsage<UsageEthernet>(serverAddress, 9510, JsonConvert.SerializeObject(new UsageEthernet(NetworkInformationGatherer.EthernetSpeed(), InformationGathererBIOS.GetBiosSerialNumber())));
                    ServerMessageSender.SendMessageUsage<UsageDisk>(serverAddress, 9300, JsonConvert.SerializeObject(new UsageDisk(InformationGathererDisk.TotalFreeSpace(),InformationGathererBIOS.GetBiosSerialNumber())));
                    ServerMessageSender.SendMessageUsage<WindowData>(serverAddress, 9090, JsonHelper.SerializeDeviceData(new DeviceData<WindowData> { SerialNumberBIOS = InformationGathererBIOS.GetBiosSerialNumber(), Data = OSInformationGatherer.GetWindows() }));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

}