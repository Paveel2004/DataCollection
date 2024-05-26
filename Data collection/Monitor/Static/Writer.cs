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
        public static void WriteRam()
        {
            foreach (var item in InformationGathererRAM.GetInfo())
            {
                DataBaseHelper.Query($"EXECUTE ДобавитьОЗУ @BIOS = '{BIOS}', @Объём='{item["Capacity"]}', @Частота = '{item["Speed"]}',@Производитель = '{item["Manufacturer"]}', @Тип = '{item["MemoryType"]}'");
            }
        }
        public static void WriteDrive()
        {
            // Выводим информацию в консоль
            foreach (var diskInfo in InformationGathererDrive.GetInfo())
            {
                DataBaseHelper.Query($"EXECUTE ДобавитьДиск @BIOS = '{BIOS}', @Модель = '{diskInfo["Model"]}', @Пул = '{diskInfo["CanPool"]}', @Тип = '{diskInfo["MediaType"]}', @Объём = '{diskInfo["Size"]}'");
            }
        }
        public static void WriteVideoCard()
        {
            // Выводим информацию в консоль
            foreach (var item in InformationGathererVideoCard.GetInfo())
            {
                DataBaseHelper.Query($"EXECUTE ДобавитьДиск @BIOS = '{BIOS}', @Модель = '{item["Model"]}', @Пул = '{diskInfo["CanPool"]}', @Тип = '{diskInfo["MediaType"]}', @Объём = '{diskInfo["Size"]}'");
            }
        }
        public static void WriteDevice()
        {
            string deviceName = InformationGathererBIOS.GetDeviceType();       
            DataBaseHelper.Query($"EXECUTE ДобавитьУстройство @BIOS = '{BIOS}', @Имя = '{deviceName}'");
        }
    }
}
