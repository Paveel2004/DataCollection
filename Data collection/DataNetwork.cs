using GlobalClass;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
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
        public static string GetPhysicalMacAddress()
        {
            string query = "SELECT * FROM Win32_NetworkAdapter WHERE PhysicalAdapter = 'TRUE' AND NOT PNPDeviceID IS NULL";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
            {
                string macAddress = m["MACAddress"] as string;
                if (!string.IsNullOrEmpty(macAddress))
                {
                    return macAddress;
                }
            }

            return "N/A";
        }
        
        public static double EthernetSpeed()
        {
            var nics = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            // Select desired NIC
            var nic = nics.SingleOrDefault(n => n.Name == "Ethernet");


            var reads = Enumerable.Empty<double>();
            var sw = new Stopwatch();
            var lastBr = nic.GetIPv4Statistics().BytesReceived;

            sw.Restart();
            Thread.Sleep(100);
            var elapsed = sw.Elapsed.TotalSeconds;
            var br = nic.GetIPv4Statistics().BytesReceived;

            var local = (br - lastBr) / elapsed;
            lastBr = br;

            // Keep last 20, ~2 seconds
            reads = new[] { local }.Concat(reads).Take(20);


            var bSec = reads.Sum() / reads.Count();
            var kbs = (bSec * 8) / 1024;

            return kbs;


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
        public static List<NetworkInterfaceData> GetNetworkInterfaces()
        {
            List<NetworkInterfaceData> networkInterfaces = new List<NetworkInterfaceData>();
            ManagementObjectSearcher searcher2 = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionID != NULL");

            foreach (ManagementObject obj in searcher2.Get())
            {
                string name = obj["Name"].ToString();
                string type = obj["NetConnectionID"].ToString();
                string macAddress = obj["MACAddress"].ToString();


                networkInterfaces.Add(new NetworkInterfaceData(name, type, macAddress));
            }
            return networkInterfaces;
        }

        public static List<Dictionary<string, string>> GetMacAddressesAndDescriptions()
        {
            var result = new List<Dictionary<string, string>>();
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");

            foreach (ManagementObject obj in searcher.Get())
            {
                if (obj["MACAddress"] != null)
                {
                    var data = new Dictionary<string, string>
                    {
                        {"Description", obj["Description"].ToString()},
                        {"MAC Address", obj["MACAddress"].ToString()}
                    };
                    result.Add(data);
                }

            }
            return result;
        }
    }
}
