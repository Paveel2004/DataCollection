using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection
{
    static class DataOS
    {
        public static string GetStartupFolderPath()
        {
            /*
             Registry.CurrentUser.OpenSubKey
            (@"Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders");
            : Здесь мы открываем раздел реестра 
            HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer
            \User Shell Folders. Этот раздел
            содержит информацию о различных
            системных путях, включая путь к папке 
            Startup (автозапуск).*/
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
        public static string GetSystemSerialNumber()
        {
            string query = "SELECT SerialNumber FROM Win32_BIOS";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
            {
                return m["SerialNumber"]?.ToString() ?? "Unknown";
            }

            return "Unknown";
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
