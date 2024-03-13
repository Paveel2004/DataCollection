using GlobalClass.Dynamic_data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using GlobalClass.Static_data;
namespace Data_collection
{
    static class OSInformationGatherer
    {
        public static string GetStartupFolderPath()
        {
            RegistryKey shellFoldersKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders");
            if (shellFoldersKey != null)
            {
                //string startupFolderPath = shellFoldersKey.GetValue("Startup") as string;: Мы извлекаем значение ключа "Startup" из открытого раздела реестра. Если ключ существует, то мы приводим его значение к строке и сохраняем в переменной startupFolderPath.
                string startupFolderPath = shellFoldersKey.GetValue("Startup") as string;
                shellFoldersKey.Close();

                if (!string.IsNullOrEmpty(startupFolderPath))
                {
                    return startupFolderPath;
                }
            }
            return null;
        }
        private static List<Process> GetProcessesWithWindows()
        {
            List<Process> processesWithWindows = new List<Process>();
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    if (!string.IsNullOrEmpty(process.MainWindowTitle))
                    {
                        processesWithWindows.Add(process);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return processesWithWindows;
        }
        public static List<WindowData> GetWindows()
        {
            List<WindowData> usageWindows = new List<WindowData>();
            var process = GetProcessesWithWindows();
            foreach (var i in process)
            {
                usageWindows.Add(new WindowData(i.MainWindowTitle, InformationGathererBIOS.GetBiosSerialNumber()));
            }
            return usageWindows;
        }
        public static string GetOperatingSystem()
        {
            string query = "SELECT Caption FROM Win32_OperatingSystem";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
            {
                return m["Caption"]?.ToString() ?? "Unknown";
            }

            return "Unknown";
        }
        public static string GetComputerName()
        {
            string query = "SELECT Name FROM Win32_ComputerSystem";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
            {
                return m["Name"]?.ToString() ?? "Unknown";
            }

            return "Unknown";
        }
        public static int GetSystemBitArchitecture()
        {
            const string registryKeyPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
            const string registryValueName = "PROCESSOR_ARCHITECTURE";

            string architecture = Registry.GetValue(registryKeyPath, registryValueName, null) as string;

            if (!string.IsNullOrEmpty(architecture) && int.TryParse(architecture.EndsWith("64") ? "64" : "32", out int bitArchitecture))
            {
                return bitArchitecture;
            }

            return 0; // В случае ошибки возвращаем 0
        }
        public static string GetOperatingSystemSerialNumber()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_OperatingSystem");
                ManagementObjectCollection collection = searcher.Get();

                foreach (ManagementObject obj in collection)
                {
                    if (obj["SerialNumber"] != null)
                    {
                        return obj["SerialNumber"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении серийного номера операционной системы: {ex.Message}");
            }

            return "N/A";
        }
        public static int GetNumberOfUsers()
        {
            string query = "SELECT NumberOfUsers FROM Win32_OperatingSystem";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
            {
                return Convert.ToInt32(m["NumberOfUsers"]);
            }

            return 0; // В случае ошибки возвращаем 0
        }
        public static string GetOperatingSystemVersion()
        {
            string query = "SELECT Version FROM Win32_OperatingSystem";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
            {
                return m["Version"]?.ToString() ?? "Unknown";
            }

            return "Unknown";
        }
        public static string GetSystemState()
        {
            string query = "SELECT Status FROM Win32_OperatingSystem";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
            {
                return m["Status"]?.ToString() ?? "Unknown";
            }

            return "Unknown";
        }
    }
}
