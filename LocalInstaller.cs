using System.IO;

namespace ExpressInstaller
{
    class LocalInstaller
    {
        private static string tmp;
        public static void installAll()
        {
            tmp = Configurator.AppFolder + @"\tmp\";
            DirectoryInfo directory = Directory.CreateDirectory(tmp);

            Installer.Installator.Directory = tmp;
            Installer.Installator.getDistrs().ForEach(delegate (Installer.Installator distr)
            {
                distr.Install();
            });
            
            directory.Delete(true);
        }      
    }
}
