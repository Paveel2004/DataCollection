using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
namespace Data_collection
{
    internal static class DataCPU
    {
        public static string GetProcessorArchitecture()
        { 
            using (var searcher = new ManagementObjectSearcher("SELECT Architecture FROM Win32_Processor"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    return obj["Architecture"].ToString();
                }
            }
            return "Неизвестно";
        }
        public static string GetProcessorName()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor"))
            {
                foreach(ManagementObject obj in searcher.Get())
                {
                    return obj["Name"].ToString();
                }
            }
            return "Неизвестно";
        }
        public static int GetProcessorCoreCount()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT NumberOfCores FROM Win32_Processor"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    return Convert.ToInt32(obj["NumberOfCores"]);
                }
                return 0;
            }
        }
        public static double GetProcessorTemperature2()
        {
            double cpuTemperature = 0;

            ManagementObjectSearcher mos = new ManagementObjectSearcher(@"root\cimv2", "SELECT * FROM Win32_TemperatureProbe");

            foreach (ManagementObject mo in mos.Get())
            {
                double temperature = Convert.ToDouble(mo["CurrentReading"]);
                cpuTemperature = temperature / 10.0 - 273.15; // Преобразование в градусы Цельсия
                return cpuTemperature;
            }

            return 0;
        }

        public static double GetProcessorTemperature() 
        {
            Double CPUtprt = 0;
            ManagementObjectSearcher mos = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
            foreach (ManagementObject mo in mos.Get())
            {
                CPUtprt = Convert.ToDouble(Convert.ToDouble(mo.GetPropertyValue("CurrentTemperature").ToString()) - 2732) / 10;
                return CPUtprt;
            }
            return 0;
        }
    }
}
