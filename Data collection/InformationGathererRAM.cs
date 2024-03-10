using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq.Expressions;
using GlobalClass.Static_data;
using System.Runtime.CompilerServices;

namespace Data_collection
{
    public class InformationGathererRAM
    {
        public static string RamType
        {
            get
            {
                int type = 0;

                ConnectionOptions connection = new ConnectionOptions();
                connection.Impersonation = ImpersonationLevel.Impersonate;
                ManagementScope scope = new ManagementScope("\\\\.\\root\\CIMV2", connection);
                scope.Connect();
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_PhysicalMemory");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    type = Convert.ToInt32(queryObj["MemoryType"]);
                }

                return TypeString(type);
            }
        }

        private static string TypeString(int type)
        {
            string outValue = string.Empty;

            switch (type)
            {
                case 0x0: outValue = "Unknown"; break;
                case 0x1: outValue = "Other"; break;
                case 0x2: outValue = "DRAM"; break;
                case 0x3: outValue = "Synchronous DRAM"; break;
                case 0x4: outValue = "Cache DRAM"; break;
                case 0x5: outValue = "EDO"; break;
                case 0x6: outValue = "EDRAM"; break;
                case 0x7: outValue = "VRAM"; break;
                case 0x8: outValue = "SRAM"; break;
                case 0x9: outValue = "RAM"; break;
                case 0xa: outValue = "ROM"; break;
                case 0xb: outValue = "Flash"; break;
                case 0xc: outValue = "EEPROM"; break;
                case 0xd: outValue = "FEPROM"; break;
                case 0xe: outValue = "EPROM"; break;
                case 0xf: outValue = "CDRAM"; break;
                case 0x10: outValue = "3DRAM"; break;
                case 0x11: outValue = "SDRAM"; break;
                case 0x12: outValue = "SGRAM"; break;
                case 0x13: outValue = "RDRAM"; break;
                case 0x14: outValue = "DDR"; break;
                case 0x15: outValue = "DDR2"; break;
                case 0x16: outValue = "DDR2 FB-DIMM"; break;
                case 0x17: outValue = "Undefined 23"; break;
                case 0x18: outValue = "DDR3"; break;
                case 0x19: outValue = "FBD2"; break;
                case 0x1a: outValue = "DDR4"; break;
                default: outValue = "Undefined"; break;
            }

            return outValue;
        }
        public static int GetTypeRAM()
        {

            ManagementObjectSearcher searcher2 = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
            foreach (ManagementObject obj in searcher2.Get())
            {
                return int.Parse(obj["SMBIOSMemoryType"].ToString());

            }
            return 0;
        }
        public static int GetConfiguredClockSpeed()
        {

            ManagementObjectSearcher searcher2 = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
            foreach (ManagementObject obj in searcher2.Get())
            {
                return int.Parse(obj["ConfiguredClockSpeed"].ToString());
            }
            return 0;
        }
        public static ulong GetTotalPhysicalMemory()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                ManagementObjectCollection collection = searcher.Get();

                foreach (ManagementObject obj in collection)
                {
                    ulong totalMemoryBytes = Convert.ToUInt64(obj["TotalPhysicalMemory"]);
                    return totalMemoryBytes;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении объема оперативной памяти: {ex.Message}");
            }

            return 0;
        }
        public static float GetMemoryUsage()
        {
            try
            {
                string categoryName = "Memory";
                string counterName = "% Committed Bytes In Use";
                using (PerformanceCounter counter = new PerformanceCounter(categoryName, counterName))
                {
                    return counter.NextValue();
                }

            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public static List<RAMData> GetRAM()
        {
            List<RAMData> RAMs = new List<RAMData>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");

            foreach (ManagementObject obj in searcher.Get())
            {
                string Type = obj["SMBIOSMemoryType"].ToString();
                ulong Volume = GetTotalPhysicalMemory();
                int Speed = int.Parse(obj["Speed"].ToString());
                RAMs.Add(new RAMData(Type, Volume, Speed));
            }
            return RAMs;
        }

    }
}
