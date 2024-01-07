using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection
{
    static class DataOS
    {
        public static string GetStartupFolderPath()
        {
            /*
             Registry.CurrentUser.OpenSubKey
            (@"Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders");
            : Здесь мы открываем раздел реестра 
            HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer
            \User Shell Folders. Этот раздел
            содержит информацию о различных
            системных путях, включая путь к папке 
            Startup (автозапуск).*/
            RegistryKey shellFoldersKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders");
            if (shellFoldersKey != null)
            {
                //string startupFolderPath = shellFoldersKey.GetValue("Startup") as string;: Мы извлекаем значение ключа "Startup" из открытого раздела реестра. Если ключ существует, то мы приводим его значение к строке и сохраняем в переменной startupFolderPath.
                string startupFolderPath = shellFoldersKey.GetValue("Startup") as string;
                shellFoldersKey.Close();

                if (!string.IsNullOrEmpty(startupFolderPath))
                {
                    return startupFolderPath;
                }
            }

            return null;
        }

    }
}
