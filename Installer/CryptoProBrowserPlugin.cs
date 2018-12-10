using Microsoft.Win32;
using System;

namespace ExpressInstaller.Installer
{
    class CryptoProBrowserPlugin : Installator
    {
        public CryptoProBrowserPlugin()
        {
            displayName = "CryptoProBrowserPlugin";
            distrName = "cadesplugin.exe";
            installParams = "-gm2 -silent -norestart -cadesargs \"/quiet\"";
            distr = Properties.Resources.cadesplugin;
        }

        protected override bool Installed()
        {
            try
            {
                RegistryKey myKey = Registry.ClassesRoot.OpenSubKey("CAdESCOM.Certificate", true);
                Logger.Log("CryptoProBrowserPlugin registry Key: " + myKey.ToString());
                return myKey != null;
            }
            catch (Exception er)
            {
                Logger.Log("Ошибка при вычислении значения Cades в регистре. Скорей всего, CryptoProBrowserPlugin не установлен. Error: " + er.Message);
                return false;
            }
        }
    }
}
