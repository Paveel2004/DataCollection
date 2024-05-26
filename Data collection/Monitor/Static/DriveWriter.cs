using Data_collection.Gatherer;
using Server;
using System;
using System.CodeDom.Compiler;
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
            List<Dictionary<string, string>> physicalDiskInfo = InformationGathererDrive.GetInfo();


            // Выводим информацию в консоль
            foreach (var diskInfo in physicalDiskInfo)
            {
                DataBaseHelper.Query($"EXECUTE ДобавитьДиск @BIOS = '{InformationGathererBIOS.GetBiosSerialNumber()}', @Модель = '{diskInfo["Model"]}', @Пул = '{diskInfo["CanPool"]}', @Тип = '{diskInfo["MediaType"]}', @Объём = '{diskInfo["Size"]}'");
            }

        }

    }
}
