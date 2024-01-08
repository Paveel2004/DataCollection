using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection
{
    internal class DataBIOS
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
    }
}
