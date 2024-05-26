using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection.Gatherer
{
    internal class InformationGathererBIOS : PowerShell
    {
        public static string GetBiosSerialNumber()
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
        public static string GetBiosVersion()
        {
            string query = "SELECT Version FROM Win32_BIOS";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
            {
                return m["Version"]?.ToString() ?? "Unknown";
            }

            return "Unknown";
        }
        public static string GetDeviceType()
        {
            // Команда PowerShell
            string powerShellCommand = @"
$system = Get-WmiObject -Class Win32_ComputerSystem
if ($system.PCSystemType -eq 2) {
    'Ноутбук'
} elseif ($system.PCSystemType -eq 1) {
    'Настольный компьютер'
} else {
    'Неизвестно'
}";

            // Выполняем команду PowerShell и получаем результат
            return PowerShell.RunPowerShellCommand(powerShellCommand).Trim();
        }

    }
}
