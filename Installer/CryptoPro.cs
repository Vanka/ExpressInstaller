namespace ExpressInstaller.Installer
{
    class CryptoPro : Installator
    {
        public CryptoPro()
        {
            displayName = "КриптоПро";
            distrName = "CSPSetup.exe";
            installParams = "-gm2 -kc kc1 -lang rus -root -noreboot -args \"/quiet\"";
            distr = Properties.Resources.CSPSetup;
        }

        protected override bool Installed()
        {
            return PCValidator.CheckCryptoPro();
        }
    }
}
