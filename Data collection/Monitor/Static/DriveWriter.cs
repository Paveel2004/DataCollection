using Data_collection.Gatherer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection.Monitor.Static
{
    internal class DriveWriter
    {

        public static void Write()
        {
            List<Dictionary<string, string>> physicalDiskInfo = InformationGathererDrive.GetPhysicalDiskInfo();

            // Выводим информацию в консоль
            foreach (var diskInfo in physicalDiskInfo)
            {
                Console.WriteLine("Model: " + diskInfo["Model"]);
                Console.WriteLine("CanPool: " + diskInfo["CanPool"]);
                Console.WriteLine("MediaType: " + diskInfo["MediaType"]);
                Console.WriteLine();
            }
        }

    }
}
