using GlobalClass.Static_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection.Gatherer
{
    internal class InformationGathererVideoCard
    {
        public static string GetModel()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
            foreach (ManagementObject obj in searcher.Get())
            {
                return obj["Caption"].ToString();

            }
            return "\0";
        }
        public static List<VideoСardData> GetModels()
        {
            List<VideoСardData> models = new List<VideoСardData>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
            foreach (ManagementObject obj in searcher.Get())
            {
                models.Add(new VideoСardData(obj["Caption"].ToString()));
            }
            return models;
        }
        public static List<Dictionary<string, string>> GetInfo()
        {
            // Получаем значения из методов
            List<string> names = PowerShell.GetPowershellValueListClass("Win32_VideoController", "Name");
            List<string> adapters = PowerShell.GetPowershellValueListClass("Win32_VideoController", "AdapterCompatibility");
            List<string> gpus = PowerShell.GetPowershellValueListClass("Win32_VideoController", "VideoProcessor");
            List<string> adapterRAM = PowerShell.GetPowershellValueListClass("Win32_VideoController", "AdapterRAM");

            // Проверяем, что все списки имеют одинаковое количество элементов
            if (names.Count != adapters.Count || names.Count != gpus.Count || names.Count != adapterRAM.Count)
            {
                throw new Exception("Количество элементов в списках не совпадает.");
            }

            // Создаем список словарей для хранения данных
            List<Dictionary<string, string>> videoInfo = new List<Dictionary<string, string>>();

            // Объединяем значения в словари и добавляем их в список
            for (int i = 0; i < names.Count; i++)
            {
                var videoItem = new Dictionary<string, string>
        {
            { "Name", names[i] },
            { "AdapterCompatibility", adapters[i] },
            { "VideoProcessor", gpus[i] },
            { "AdapterRAM", adapterRAM[i] } // Включаем AdapterRAM в словарь
        };
                videoInfo.Add(videoItem);
            }

            return videoInfo;
        }


    }
}
