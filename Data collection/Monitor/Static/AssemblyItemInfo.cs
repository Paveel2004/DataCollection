using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection.Monitor.Static
{
    internal class AssemblyItemInfo : PowerShell
    {
        public static List<Dictionary<string, string>> GetDriveInfo()
        {
            // Получаем значения из методов
            List<string> models = GetPowershellValueList("PhysicalDisk", "FriendlyName");//Модель 
            List<string> canPools = GetPowershellValueList("PhysicalDisk", "CanPool");//Пул
            List<string> mediaTypes = GetPowershellValueList("PhysicalDisk", "MediaType");//Тип
            List<string> sizes = GetPowershellValueList("PhysicalDisk", "Size");//Объём
            List<string> partitionStyle = GetPowershellValueList("Disk", "PartitionStyle");
            List<string> serialNumber = GetPowershellValueList("PhysicalDisk", "SerialNumber");

            //

            // Проверяем, что все списки имеют одинаковое количество элементов
            if (models.Count != canPools.Count || models.Count != mediaTypes.Count || models.Count != sizes.Count || models.Count != partitionStyle.Count)
            {
                throw new Exception("Количество элементов в списках не совпадает.");
            }

            // Создаем список словарей для хранения данных
            List<Dictionary<string, string>> physicalDiskInfo = new List<Dictionary<string, string>>();

            // Объединяем значения в словари и добавляем их в список
            for (int i = 0; i < models.Count; i++)
            {
                var diskInfo = new Dictionary<string, string>
                {
                     { "FriendlyName", models[i] },
                     { "CanPool", canPools[i] },
                     { "MediaType", mediaTypes[i] },
                     { "Size", sizes[i] },
                     { "PartitionStyle", partitionStyle[i] },
                     { "SerialNumber", serialNumber[i] },
                };
                physicalDiskInfo.Add(diskInfo);
            }

            return physicalDiskInfo;
        }
        public static List<Dictionary<string, string>> GetVideoControllerInfo()
        {
            // Получаем значения из методов
            List<string> names = GetPowershellValueListClass("Win32_VideoController", "Name"); // Модель
            List<string> adapters = GetPowershellValueListClass("Win32_VideoController", "AdapterCompatibility"); // Производитель
            List<string> gpus = GetPowershellValueListClass("Win32_VideoController", "VideoProcessor"); // ВидеоПроцессор
            List<string> adapterRAM = GetPowershellValueListClass("Win32_VideoController", "AdapterRAM"); // Объём памяти
            List<string> serialNumbers = GetPowershellValueListClass("Win32_VideoController", "PNPDeviceID"); // Серийный номер

            // Проверяем, что все списки имеют одинаковое количество элементов
            if (names.Count != adapters.Count || names.Count != gpus.Count || names.Count != adapterRAM.Count || names.Count != serialNumbers.Count)
            {
                throw new Exception("Количество элементов в списках не совпадает.");
            }

            // Создаем список словарей для хранения данных
            List<Dictionary<string, string>> videoControllerInfo = new List<Dictionary<string, string>>();

            // Объединяем значения в словари и добавляем их в список
            for (int i = 0; i < names.Count; i++)
            {
                var videoControllerItem = new Dictionary<string, string>
        {
            { "Name", names[i] },
            { "AdapterCompatibility", adapters[i] },
            { "VideoProcessor", gpus[i] },
            { "AdapterRAM", adapterRAM[i] },
            { "PNPDeviceID", serialNumbers[i] } // Включаем серийный номер в словарь
        };
                videoControllerInfo.Add(videoControllerItem);
            }

            return videoControllerInfo;
        }
        public static List<Dictionary<string, string>> GetPhisicalMemoryInfo()
        {
            // Получаем значения из методов
            List<string> capacities = GetPowershellValueListClass("Win32_PhysicalMemory", "Capacity");
            List<string> speeds = GetPowershellValueListClass("Win32_PhysicalMemory", "Speed");
            List<string> memoryTypes = GetPowershellValueListClass("Win32_PhysicalMemory", "MemoryType");
            List<string> formFactors = GetPowershellValueListClass("Win32_PhysicalMemory", "Manufacturer");

            // Проверяем, что все списки имеют одинаковое количество элементов
            if (capacities.Count != speeds.Count || capacities.Count != memoryTypes.Count || capacities.Count != formFactors.Count)
            {
                throw new Exception("Количество элементов в списках не совпадает.");
            }

            // Создаем список словарей для хранения данных
            List<Dictionary<string, string>> memoryInfo = new List<Dictionary<string, string>>();

            // Объединяем значения в словари и добавляем их в список
            for (int i = 0; i < capacities.Count; i++)
            {
                var memoryItem = new Dictionary<string, string>
        {
            { "Capacity", capacities[i] },
            { "Speed", speeds[i] },
            { "MemoryType", memoryTypes[i] },
            { "Manufacturer", formFactors[i] }
        };
                memoryInfo.Add(memoryItem);
            }

            return memoryInfo;
        }


    }
}
