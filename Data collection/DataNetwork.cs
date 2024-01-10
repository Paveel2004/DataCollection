using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection
{
    internal class DataNetwork
    {
        public static string GetMacAddress()
        {
            string query = "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
            {
                // Проверяем, чтобы MAC-адрес не был null и не содержал только нули
                string macAddress = m["MacAddress"] as string;
                if (!string.IsNullOrEmpty(macAddress) && !macAddress.Equals("000000000000", StringComparison.OrdinalIgnoreCase))
                {
                    return macAddress;
                }
            }

            // Если не удалось получить MAC-адрес, возвращаем "N/A" или другое значение по вашему усмотрению
            return "N/A";
        }
        public static string GetIPAddress()
        {
            string query = "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
            {
                // Получаем массив IP-адресов (может быть несколько)
                string[] ipAddresses = m["IPAddress"] as string[];

                // Выбираем первый не-null и не-пустой IP-адрес
                if (ipAddresses != null && ipAddresses.Length > 0 && !string.IsNullOrEmpty(ipAddresses[0]))
                {
                    return ipAddresses[0];
                }
            }

            // Если не удалось получить IP-адрес, возвращаем "N/A" или другое значение по вашему усмотрению
            return "N/A";
        }
        
    }
}
