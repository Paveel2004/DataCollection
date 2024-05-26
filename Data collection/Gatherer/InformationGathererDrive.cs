using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Data_collection.Gatherer
{
    public class InformationGathererDrive
    {
        public static double TotalFreeSpace()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            long totalFreeSpace = 0;
            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady == true)
                {
                    totalFreeSpace += d.AvailableFreeSpace;
                }
            }
            return totalFreeSpace / Math.Pow(1024, 3);
        }
        public static double TotalSpace()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            long totalSpace = 0;
            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady == true)
                {
                    totalSpace += d.TotalSize;
                }
            }
            return totalSpace / Math.Pow(1024, 3);
        }
        public static List<Dictionary<string, string>> GetPhysicalDiskInfo()
        {
            // Получаем значения из методов
            List<string> models = PowerShell.GetPowershellValueList("PhysicalDisk", "FriendlyName");
            List<string> canPools = PowerShell.GetPowershellValueList("PhysicalDisk", "CanPool");
            List<string> mediaTypes = PowerShell.GetPowershellValueList("PhysicalDisk", "MediaType");

            // Проверяем, что все списки имеют одинаковое количество элементов
            if (models.Count != canPools.Count || models.Count != mediaTypes.Count)
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
                { "Model", models[i] },
                { "CanPool", canPools[i] },
                { "MediaType", mediaTypes[i] }
            };
                physicalDiskInfo.Add(diskInfo);
            }

            return physicalDiskInfo;
        }
    
        public static List<string> GetModel() => PowerShell.GetPowershellValueList("PhysicalDisk", "FriendlyName");
        public static List<string> GetPul() => PowerShell.GetPowershellValueList("PhysicalDisk", "CanPool");
        public static List<string> GetType() => PowerShell.GetPowershellValueList("PhysicalDisk", "MediaType");


    }
}
