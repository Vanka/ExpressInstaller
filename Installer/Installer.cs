using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExpressInstaller.Installer
{
    class Installator
    {
        public static string Directory;

        protected string displayName;
        protected string distrName;
        protected string installParams;
        protected byte[] distr;

        public static List<Installator> getDistrs()
        {
            return new List<Installator>(new Installator[]
            {
                new RuToken(),
                new CryptoPro(),
                new CryptoProBrowserPlugin(),
                new CryptoProOfficePlugin(),
                new Capicom(),
                new AgzrtPlugin(),
                new ICLCrypt()
            });
        }

        public void Install()
        {
            if (Installator.Directory.Length == 0) throw new Exception("You must specify Temp Directory (Installator.Directory) for Installator!");
            if (Installed())
            {
                Logger.Log("Компонент " + displayName + " уже был установлен. Пропускаем установку");
                return;
            }

            InstallProcess();

            Logger.Log("Компонент " + displayName + " успешно установлен");
        }

        protected virtual void InstallProcess()
        {
            string exe_path = Installator.Directory + @distrName;

            using (FileStream exeFile = new FileStream(exe_path, FileMode.Create)) exeFile.Write(distr, 0, distr.Length);

            ProcessStartInfo pInfo = new ProcessStartInfo(exe_path, installParams);
            pInfo.Arguments = installParams;
            Process p = Process.Start(pInfo);
            p.WaitForExit();
        }

        protected virtual bool Installed()
        {
            return false;
        }
    }
}
