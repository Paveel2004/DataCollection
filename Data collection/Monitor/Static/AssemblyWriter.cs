using Data_collection.Gatherer;
using Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.Win32;
using System.IO;

namespace Data_collection.Monitor.Static
{
    internal class AssemblyWriter
    {
        public static string BIOS = InformationGathererBIOS.GetBiosSerialNumber();
        public static string type = InformationGathererBIOS.GetDeviceType();
        public static string computerName = OSInformationGatherer.GetComputerName();
        public static string serialNumberOS = OSInformationGatherer.GetOperatingSystemSerialNumber();
        public static void WriteRam()
        {
            foreach (var item in AssemblyItemInfo.GetPhisicalMemoryInfo())
            {
                DataBaseHelper.Query($"EXECUTE InsertRAM @Device = '{BIOS}', @SerialNumber = '{item["SerialNumber"]}', @Type = '{item["MemoryType"]}', @Speed = {item["Speed"]}, @Manufacturer = '{item["Manufacturer"]}', @Memory = {item["Capacity"]}, @Lot = '{item["DeviceLocator"]}';\r\n");
            }
        }
        public static void WriteOperatingSystem()
        {
            foreach (var item in AssemblyItemInfo.GetOperatingSystemInfo())
            {
                DataBaseHelper.Query($"EXECUTE InsertOS @Device = '{BIOS}', @SerialNumber = '{item["SerialNumber"]}', @Version = '{item["Version"]}', @Architecture = '{item["OSArchitecture"]}', @Caption = '{item["Caption"]}', @ComputerName = '{computerName}'");
            }
        }
        public static void WriteUser()
        {
            foreach (var item in AssemblyItemInfo.GetUsersInfo())
            {
                DataBaseHelper.Query($"EXECUTE InsertUser @SID = '{item["SID"]}', @Name = '{item["Name"]}', @SerialNumberOS = '{serialNumberOS}';");
            }
        }

        public static void WriteVolume()
        {
            var volumeInfo = AssemblyItemInfo.GetVolumeInfo();
            foreach (var item in volumeInfo)
            {
                string driveLetter = item["DriveLetter"];
                string fileSystem = item["FileSystem"];
                string capacity = item["Capacity (GB)"];
                string freeSpace = item["FreeSpace (GB)"];

                // Преобразуем строковые значения в числовые, если это необходимо
                double capacityValue = double.Parse(capacity);
                double freeSpaceValue = double.Parse(freeSpace);

                // Выполняем запрос к базе данных, используя хранимую процедуру InsertOrUpdateDisk
                DataBaseHelper.Query($"EXECUTE InsertOrUpdateDisk " +
                                     $"@Имя = '{driveLetter}', " +
                                     $"@ОС = '{serialNumberOS}', " +
                                     $"@ФайловаяСистема = '{fileSystem}', " +
                                     $"@Размер = {capacityValue}, " +
                                     $"@СвободноеМесто = {freeSpaceValue};");
            }
        }

        public static void WriteDrive()
        {
            // Выводим информацию в консоль
            foreach (var item in AssemblyItemInfo.GetDriveInfo())
            {
                int poolValue = item["CanPool"] == "True" ? 1 : 0;

                DataBaseHelper.Query($"EXECUTE InsertDrive @Device = '{BIOS}', @SerialNumber = '{item["SerialNumber"]}', @Type = '{item["MediaType"]}', @Model = '{item["FriendlyName"]}', @Memory = {item["Size"]}, @PartitionStyle = '{item["PartitionStyle"]}', @Pool = {poolValue};");
            }
        }
        public static void WritePhysicalNetworkInterface()
        {
            foreach (var item in AssemblyItemInfo.GetNetworkInterfaceInfo())
            {
                DataBaseHelper.Query($"EXECUTE InsertPhysicalNetworkInterfase @Device = '{BIOS}', @MAC = '{item["MACAddress"]}', @Name = '{item["Name"]}', @Description = '{item["Description"]}';");
            }
        }

        public static void WriteVideoCard()
        {
            // Выводим информацию в консоль
            foreach (var item in AssemblyItemInfo.GetVideoControllerInfo())
            {
                DataBaseHelper.Query($"EXECUTE InsertVideoAdapter @Device = '{BIOS}', @SerialNumber = '{item["PNPDeviceID"]}', @Model = '{item["Name"]}', @GPU = '{item["VideoProcessor"]}', @Manufacturer = '{item["AdapterCompatibility"]}', @Memory = {item["AdapterRAM"]};");
            }
        }
        public static void WriteProcessor()
        {
            foreach (var item in AssemblyItemInfo.GetProcessorInfo())
            {
                DataBaseHelper.Query($"EXECUTE InsertProcessor " +
                    $"@Device = '{BIOS}', " +
                    $"@SerialNumber = '{item["ProcessorId"]}', " +
                    $"@Model = '{item["Name"]}', " +
                    $"@Architecture = '{item["Architecture"]}', " +
                    $"@Manufacturer = '{item["Manufacturer"]}', " +
                    $"@SocketDesignation = '{item["SocketDesignation"]}', " +
                    $"@CoreCount = {item["NumberOfCores"]}, " +
                    $"@NumberOfLogicalProcessors = {item["NumberOfLogicalProcessors"]}," +
                    $"@Speed = {item["MaxClockSpeed"]}");
            }
        }


        public static void WriteDevice()
        {
            string deviceName = InformationGathererBIOS.GetDeviceType();       
            DataBaseHelper.Query($"INSERT INTO Устройство VALUES ('{BIOS}','{type}')");
        }
  
    }
}

