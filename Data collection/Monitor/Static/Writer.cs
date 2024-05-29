using Data_collection.Gatherer;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection.Monitor.Static
{
    internal class Writer
    {
        public static string BIOS = InformationGathererBIOS.GetBiosSerialNumber();
        public static string type = InformationGathererBIOS.GetDeviceType();
        public static void WriteRam()
        {
            foreach (var item in AssemblyItemInfo.GetPhisicalMemoryInfo())
            {
                //DataBaseHelper.Query($"EXECUTE ДобавитьОЗУ @BIOS = '{BIOS}', @Объём='{item["Capacity"]}', @Частота = '{item["Speed"]}',@Производитель = '{item["Manufacturer"]}', @Тип = '{item["MemoryType"]}'");
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

        public static void WriteVideoCard()
        {
            // Выводим информацию в консоль
            foreach (var item in AssemblyItemInfo.GetVideoControllerInfo())
            {
                DataBaseHelper.Query($"EXECUTE InsertVideoAdapter @Device = '{BIOS}', @SerialNumber = '{item["PNPDeviceID"]}', @Model = '{item["Name"]}', @GPU = '{item["VideoProcessor"]}', @Manufacturer = '{item["AdapterCompatibility"]}', @Memory = {item["AdapterRAM"]};");
            }
        }
        public static void WriteDevice()
        {
            string deviceName = InformationGathererBIOS.GetDeviceType();       
            DataBaseHelper.Query($"INSERT INTO Устройство VALUES ('{BIOS}','{type}')");
        }
    }
}
