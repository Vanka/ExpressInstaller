using Microsoft.Win32;
using System;

namespace ExpressInstaller.Installer
{
    class RuToken : Installator
    {
        public RuToken()
        {
            displayName = "RuToken";
            distrName = "rtDrivers.exe";
            installParams = "/NORESTART /QUIET";
            distr = Properties.Resources.rtDrivers;
        }

        protected override bool Installed()
        {
            try
            {
                RegistryKey myKey = Registry.CurrentUser.OpenSubKey("Software\\Aktiv Co.\\Rutoken", true);
                Logger.Log("RuToken registry Key: " + myKey.ToString());
                return myKey != null;
            } catch (Exception er) {
                Logger.Log("Ошибка при вычислении значения RuToken в регистре. Скорей всего, RuToken не установлен. Error: " + er.Message);
                return false;
            }
        }
    }
}
