using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_collection.Gatherer;
using System.Text.Json;

namespace Data_collection.Monitor.Static
{
    internal class DeviceWriter
    {
        public static void Write()
        {
            const string fileName = "Device.json"; // Имя файла, который вы хотите проверить
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory; // Папка, где находится EXE файл
            string filePath = Path.Combine(exeDirectory, fileName); // Полный путь к файлу
            var device = new
            {
                deviceName = InformationGathererBIOS.GetDeviceType(),
                biosSerialNumber = InformationGathererBIOS.GetBiosSerialNumber(),
            };

                DataBaseHelper.Query($"EXECUTE ДобавитьУстройство @BIOS = '{device.biosSerialNumber}', @Имя = '{device.deviceName}'");
                JsonWriter.WriteToJsonFile(device, filePath);
                            

        }
    }
}
