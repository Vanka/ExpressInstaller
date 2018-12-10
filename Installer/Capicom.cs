using System;
using System.IO;
using System.Diagnostics;

namespace ExpressInstaller.Installer
{
    class Capicom : Installator
    {
        public Capicom()
        {
            displayName = "КАПИКОМ";
            distrName = "capicom.dll";
            installParams = "/s capicom.dll -gm2";
            distr = Properties.Resources.capicom;
        }

        protected override void InstallProcess()
        {
            string exe_system_path = Environment.SystemDirectory + "\\" + @distrName;
            string exe_path = Installator.Directory + @distrName;
            using (FileStream exeFile = new FileStream(exe_path, FileMode.Create)) exeFile.Write(distr, 0, distr.Length);

            //-- HELPER DEFINITION --\\
            string helper_system_path = Environment.SystemDirectory + "\\" + @distrName;
            string helper_path = Installator.Directory + @"\capicom.inf";
            string helper_distr = Properties.Resources.capicom1;
            using (StreamWriter helperFile = new StreamWriter(helper_path)) helperFile.Write(helper_distr, 0, helper_distr.Length);
            //-- HELPER DEFINITION --\\

            // Copy all distrs to system32
            if (!File.Exists(exe_system_path)) File.Copy(exe_path, exe_system_path);
            if (!File.Exists(helper_system_path)) File.Copy(helper_path, helper_system_path);

            ProcessStartInfo pInfo = new ProcessStartInfo(@"C:\Windows\sysWOW64\regsvr32.exe", installParams);
            pInfo.Arguments = installParams;
            Process p = Process.Start(pInfo);
            p.WaitForExit();
        }

        protected override bool Installed()
        {
            string exe_system_path = Environment.SystemDirectory + "\\" + @distrName;
            return File.Exists(exe_system_path);
        }
    }
}
