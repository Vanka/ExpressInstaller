using System;
using System.IO;

namespace ExpressInstaller.Installer
{
    class CryptoProOfficePlugin : Installator
    {
        public CryptoProOfficePlugin()
        {
            displayName = "CryptoProOfficePlugin";
            distrName = "XMLDSigAddIn_x64.msi";
            installParams = "/NORESTART /QUIET";
            distr = Properties.Resources.XMLDSigAddIn_x64;
        }

        protected override bool Installed()
        {
            try
            {
                string dll_location = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Crypto Pro\XMLDSigAddIn\cpXMLDSigAddIn.dll";
                return File.Exists(dll_location);
            } catch (Exception ex) {
                Logger.Log("Ошибка при попытке найти CryptoProOfficePlugin DLL на локальной машине. Скорей всего, он не установлен. Ошибка: " + ex.Message);
                return false;
            }
        }
    }
}
