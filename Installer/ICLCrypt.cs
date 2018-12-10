using System;
using System.IO;

namespace ExpressInstaller.Installer
{
    class ICLCrypt : Installator
    {
        public ICLCrypt()
        {
            displayName = "ICLCrypt";
            distrName = "ICLCrypt.exe";
            installParams = "/norestart /silent";
            distr = Properties.Resources.ICLCrypt;
        }

        protected override bool Installed()
        {
            try
            {
                string exe_location = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\AgzrtCryptProvider\AgzrtCryptProviderEx.exe";
                return File.Exists(exe_location);
            } catch (Exception ex) {
                Logger.Log("Ошибка при попытке найти плагин Agzrt. Скорей всего, он не установлен. Ошибка: " + ex.Message);
                return false;
            }
        }
    }
}
