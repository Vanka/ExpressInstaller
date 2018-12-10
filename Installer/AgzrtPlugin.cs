using System;
using System.IO;

namespace ExpressInstaller.Installer
{
    class AgzrtPlugin : Installator
    {
        public AgzrtPlugin()
        {
            displayName = "AgzrtPlugin";
            distrName = "AgzrtCryptProvider.exe";
            installParams = "/norestart /silent";
            distr = Properties.Resources.AgzrtCryptProvider_Setup;
        }

        protected override bool Installed()
        {
            try
            {
                string exe_location = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\AgzrtCryptProvider\AgzrtCryptProviderEx.exe";
                Logger.Log("Finding AGZRT in " + exe_location);
                return File.Exists(exe_location);
            } catch (Exception ex) {
                Logger.Log("Ошибка при попытке найти плагин Agzrt. Скорей всего, он не установлен. Ошибка: " + ex.Message);
                return false;
            }
        }
    }
}
