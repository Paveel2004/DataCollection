using Data_collection.Gatherer;
using Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection.Monitor.Static
{
    internal class RamWriter
    {
        public static void Write()
        {
            List<Dictionary<string, string>> info = InformationGathererRAM.GetInfo();
            foreach (var item in info)
            {
                DataBaseHelper.Query($"EXECUTE ДобавитьОЗУ @BIOS = '{InformationGathererBIOS.GetBiosSerialNumber()}', @Объём='{item["Capacity"]}', @Частота = '{item["Speed"]}',@Производитель = '{item["Manufacturer"]}', @Тип = '{item["MemoryType"]}'");
            }
        }
    }
}
