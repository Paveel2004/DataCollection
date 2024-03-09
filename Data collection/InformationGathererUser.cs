using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection
{
    internal class InformationGathererUser
    {
        public static string GetUserName()
        {
            string query = "SELECT Name FROM Win32_UserAccount";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
            {
                return m["Name"]?.ToString() ?? "Unknown";
            }

            return "Unknown";
        }
        public static string GetUserSID()
        {
            string query = "SELECT SID FROM Win32_UserAccount";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
            {
                return m["SID"]?.ToString() ?? "Unknown";
            }

            return "Unknown";
        }
        public static string GetUserStatus()
        {
            string query = "SELECT Status FROM Win32_UserAccount";
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
