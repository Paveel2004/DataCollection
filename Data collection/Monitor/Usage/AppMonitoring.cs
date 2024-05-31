using Data_collection.Gatherer;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using Newtonsoft.Json;
using Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection.Monitor.Usage
{
    internal partial class AppMonitoring
    {

        public static void StartMonitor()
        {
            const string fileName = "applications.json"; // Имя файла, который вы хотите проверить
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory; // Папка, где находится EXE файл
            string filePath = Path.Combine(exeDirectory, fileName); // Полный путь к файлу
            string SID = InformationGathererUser.GetUserSID();

            if (File.Exists(filePath))
            {
                string inFileApps = ReadJsonFile(filePath);
                string inReestrApps = GetRegistryDataAsJson();

                //Находим данные которые есть в inReestrApps, но нету в inFileApps
                List<ApplicationData> InsertMissingApps = FindMissingApplications(inFileApps, inReestrApps);
                foreach (var App in InsertMissingApps)
                {
                    DataBaseHelper.Query($"EXECUTE ВставитьПриложение @Пользователь = '{SID}',@НазваниеПриложения = '{App.DisplayName}' , @ДатаУстановки = '{App.InstallDate}', @Вес = {Convert.ToUInt32(App.SizeInMB)}");

                }

                //Наоборот 
                List<ApplicationData> DeleteMissingApps = FindMissingApplications(inReestrApps, inFileApps);
                foreach (var App in DeleteMissingApps)
                {
                    DataBaseHelper.Query($"EXECUTE УдалитьПриложение @Название = '{App.DisplayName}', @SIDПользователя='{SID}' ");
                }
                SaveRegistryDataToJson(fileName);
            }
            else
            {
                SaveRegistryDataToJson(fileName);

                string jsonAppsReest = GetRegistryDataAsJson();
                List<ApplicationData> applicationsFromReestr = DeserializeJsonToApplicationData(jsonAppsReest);
                foreach (var App in applicationsFromReestr)
                {
                    DataBaseHelper.Query($"EXECUTE ВставитьПриложение @Пользователь = '{SID}',@НазваниеПриложения = '{App.DisplayName}' , @ДатаУстановки = '{App.InstallDate}', @Вес = {Convert.ToUInt32(App.SizeInMB)}");
                }
            }

        }
        static List<ApplicationData> DeserializeJsonToApplicationData(string json)
        {
            return JsonConvert.DeserializeObject<List<ApplicationData>>(json);
        }
        static List<ApplicationData> FindMissingApplications(string inFileApps, string inReestrApps)
        {
            var fileApps = JsonConvert.DeserializeObject<List<ApplicationData>>(inFileApps);
            var reestrApps = JsonConvert.DeserializeObject<List<ApplicationData>>(inReestrApps);

            var missingApps = reestrApps
                .Where(reestrApp => !fileApps.Any(fileApp => fileApp.DisplayName == reestrApp.DisplayName))
                .ToList();

            return missingApps;
        }
        static string ReadJsonFile(string jsonFilePath)
        {
            try
            {
                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    return jsonContent;
                }
                else
                {

                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        static void SaveRegistryDataToJson(string jsonFilePath)
        {
            List<ApplicationData> applications = new List<ApplicationData>();

            string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
            {
                foreach (string subkeyName in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkeyName))
                    {
                        // Получение названия приложения
                        string displayName = (string)subkey.GetValue("DisplayName");

                        // Получение даты установки приложения
                        string installDate = (string)subkey.GetValue("InstallDate");
                        DateTime date;
                        if (!string.IsNullOrEmpty(installDate) && DateTime.TryParseExact(installDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                        {
                            installDate = date.ToString("yyyy-MM-ddTHH:mm:ss");
                        }

                        // Получение размера приложения
                        object estimatedSizeObj = subkey.GetValue("EstimatedSize");
                        double sizeInMB = 0;
                        if (estimatedSizeObj != null)
                        {
                            int estimatedSize = (int)estimatedSizeObj;
                            sizeInMB = estimatedSize / 1024.0;
                        }

                        if (!string.IsNullOrEmpty(displayName))
                        {
                            applications.Add(new ApplicationData
                            {
                                DisplayName = displayName,
                                InstallDate = installDate,
                                SizeInMB = sizeInMB
                            });
                        }
                    }
                }
            }

            string json = JsonConvert.SerializeObject(applications, Formatting.Indented);
            File.WriteAllText(jsonFilePath, json);


        }
        static string GetRegistryDataAsJson()
        {
            List<ApplicationData> applications = new List<ApplicationData>();

            string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
            {
                foreach (string subkeyName in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkeyName))
                    {
                        // Получение названия приложения
                        string displayName = (string)subkey.GetValue("DisplayName");

                        // Получение даты установки приложения
                        string installDate = (string)subkey.GetValue("InstallDate");
                        DateTime date;
                        if (!string.IsNullOrEmpty(installDate) && DateTime.TryParseExact(installDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                        {
                            installDate = date.ToString("yyyy-MM-ddTHH:mm:ss");
                        }

                        // Получение размера приложения
                        object estimatedSizeObj = subkey.GetValue("EstimatedSize");
                        double sizeInMB = 0;
                        if (estimatedSizeObj != null)
                        {
                            int estimatedSize = (int)estimatedSizeObj;
                            sizeInMB = estimatedSize / 1024.0;
                        }

                        if (!string.IsNullOrEmpty(displayName))
                        {
                            applications.Add(new ApplicationData
                            {
                                DisplayName = displayName,
                                InstallDate = installDate,
                                SizeInMB = sizeInMB
                            });
                        }
                    }
                }
            }

            string json = JsonConvert.SerializeObject(applications, Formatting.Indented);
            return json;
        }
    }
}
