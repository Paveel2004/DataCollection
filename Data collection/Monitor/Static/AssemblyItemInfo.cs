using Azure;
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
            List<string> capacities = GetPowershellValueListClass("Win32_PhysicalMemory", "Capacity");//Вместимость 
            List<string> speeds = GetPowershellValueListClass("Win32_PhysicalMemory", "Speed");//Частота
            List<string> memoryTypes = GetPowershellValueListClass("Win32_PhysicalMemory", "MemoryType");//Тип
            List<string> formFactors = GetPowershellValueListClass("Win32_PhysicalMemory", "Manufacturer");//Производител            
            List<string> deviceLocator = GetPowershellValueListClass("Win32_PhysicalMemory", "DeviceLocator ");
            List<string> serialNumber = GetPowershellValueListClass("Win32_PhysicalMemory", "SerialNumber ");
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
            { "Manufacturer", formFactors[i] },
            { "DeviceLocator", deviceLocator[i] },
            {  "SerialNumber", serialNumber[i] }
            };
                memoryInfo.Add(memoryItem);
            }
            return memoryInfo;
        }
        public static List<Dictionary<string, string>> GetOperatingSystemInfo()
        {
            // Получаем значения из методов
            List<string> serialNumbers = GetValueWMI("Win32_OperatingSystem", "SerialNumber");
            List<string> osArchitectures = GetValueWMI("Win32_OperatingSystem", "OSArchitecture");
            List<string> versions = GetValueWMI("Win32_OperatingSystem", "Version");
            List<string> captions = GetValueWMI("Win32_OperatingSystem", "Caption");

            // Проверяем, что все списки имеют одинаковое количество элементов
            if (serialNumbers.Count != osArchitectures.Count || serialNumbers.Count != versions.Count || serialNumbers.Count != captions.Count)
            {
                throw new Exception("Количество элементов в списках не совпадает.");
            }

            // Создаем список словарей для хранения данных
            List<Dictionary<string, string>> operatingSystemInfo = new List<Dictionary<string, string>>();

            // Объединяем значения в словари и добавляем их в список
            for (int i = 0; i < serialNumbers.Count; i++)
            {
                var osInfo = new Dictionary<string, string>
        {
            { "SerialNumber", serialNumbers[i] },
            { "OSArchitecture", osArchitectures[i] },
            { "Version", versions[i] },
            { "Caption", captions[i] }
        };
                operatingSystemInfo.Add(osInfo);
            }

            return operatingSystemInfo;
        }
        public static List<Dictionary<string, string>> GetUsersInfo()
        {
            // Получаем значения из методов
            List<string> names = GetValueWMI("Win32_UserAccount", "Name");
            List<string> sids = GetValueWMI("Win32_UserAccount", "SID");

            // Создаем список словарей для хранения данных
            List<Dictionary<string, string>> usersInfo = new List<Dictionary<string, string>>();

            // Проверяем, что все списки имеют одинаковое количество элементов
            if (names.Count != sids.Count)
            {
                throw new Exception("Количество элементов в списках не совпадает.");
            }

            // Объединяем значения в словари и добавляем их в список
            for (int i = 0; i < names.Count; i++)
            {
                var userInfo = new Dictionary<string, string>
        {
            { "Name", names[i] },
            { "SID", sids[i] }
        };
                usersInfo.Add(userInfo);
            }

            return usersInfo;
        }

        public static List<Dictionary<string, string>> GetProcessorInfo()
        {
            // Получаем значения из методов
            List<string> processorId = GetPowershellValueListClass("Win32_Processor", "ProcessorId");
            List<string> name = GetPowershellValueListClass("Win32_Processor", "Name");
            List<string> architecture = GetPowershellValueListClass("Win32_Processor", "Architecture");
            List<string> numberOfCores = GetPowershellValueListClass("Win32_Processor", "NumberOfCores");
            List<string> socketDesignation = GetPowershellValueListClass("Win32_Processor", "SocketDesignation");
            List<string> numberOfLogicalProcessors = GetPowershellValueListClass("Win32_Processor", "NumberOfLogicalProcessors");            
            List<string> manufacturer = GetPowershellValueListClass("Win32_Processor", "Manufacturer");
            List<string> speed = GetPowershellValueListClass("Win32_Processor", "MaxClockSpeed");
            // Проверяем, что все списки имеют одинаковое количество элементов
            if (processorId.Count != name.Count || processorId.Count != architecture.Count || processorId.Count != numberOfCores.Count ||
                processorId.Count != socketDesignation.Count || processorId.Count != numberOfLogicalProcessors.Count || processorId.Count != manufacturer.Count)
            {
                throw new Exception("Количество элементов в списках не совпадает.");
            }

            // Создаем список словарей для хранения данных
            List<Dictionary<string, string>> processorInfo = new List<Dictionary<string, string>>();

            // Объединяем значения в словари и добавляем их в список
            for (int i = 0; i < processorId.Count; i++)
            {
                var processorItem = new Dictionary<string, string>
        {
            { "ProcessorId", processorId[i] },
            { "Name", name[i] },
            { "Architecture", architecture[i] },
            { "NumberOfCores", numberOfCores[i] },
            { "SocketDesignation", socketDesignation[i] },
            { "NumberOfLogicalProcessors", numberOfLogicalProcessors[i] },
            { "Manufacturer", manufacturer[i] },
            { "MaxClockSpeed", speed[i] },
        };
                processorInfo.Add(processorItem);
            }

            return processorInfo;
        }

        public static List<Dictionary<string, string>> GetNetworkInterfaceInfo()
        {
            List<string> names = GetPowershellValueListClass("Win32_NetworkAdapter", "Name");
            List<string> descriptions = GetPowershellValueListClass("Win32_NetworkAdapter", "Description");
            List<string> macAddresses = GetPowershellValueListClass("Win32_NetworkAdapter", "MACAddress");
            List<string> physicalAddresses = GetPowershellValueListClass("Win32_NetworkAdapter", "PhysicalAdapter");

            if (names.Count != descriptions.Count || names.Count != macAddresses.Count ||
                names.Count != physicalAddresses.Count)
            {
                throw new Exception("Количество элементов в списках не совпадает.");
            }

            List<Dictionary<string, string>> networkInterfaceInfo = new List<Dictionary<string, string>>();

            for (int i = 0; i < names.Count; i++)
            {
                var networkInterfaceItem = new Dictionary<string, string>
        {
            { "Name", names[i] },
            { "Description", descriptions[i] },
            { "MACAddress", macAddresses[i] },
            { "PhysicalAdapter", physicalAddresses[i] }
        };
                networkInterfaceInfo.Add(networkInterfaceItem);
            }

            return networkInterfaceInfo;
        }



    }
}
