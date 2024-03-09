using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection
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
    }
}
