using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExpressInstaller
{
    class PCValidator
    {
        public const Int32 CRYPT_VERIFYCONTEXT = -268435456;
        public static List<Exception> warnings = new List<Exception>();
        public static List<Exception> errors = new List<Exception>();

        public static void Validate()
        {
            CheckOperatingSystem();
            CheckIE();
            CheckNETFramework();
            CheckDocCreator();
        }

        // Windows 7, 8, 10
        private static void CheckOperatingSystem()
        {
            Logger.Log("Проверка Операционной системы.");
            OperatingSystem os = Environment.OSVersion;
            Version os_version = os.Version;

            // Проверим, что операционная система - Windows
            if (!os.Platform.ToString().Contains("Win")
                // Для Windows 7, 8 Мажор версия - 6, для Win10 - 10. Версия Win 6.0 - это Windows 2000.
                || (os_version.Major < 6 || (os_version.Major == 6 && os_version.Minor < 1)))
            {
                Logger.Log("[ПРЕДУПРЕЖДЕНИЕ] Версия операционной системы не соответствует требованиям: " + os.ToString());
                WrongOperatingSystemException err = new WrongOperatingSystemException(os_version);
                throw(err);
            }
            Logger.Log("Версия операционной системы соответствует требованиям: " + os.ToString());
        }

        // Internet Explorer версии 11 и выше
        private static void CheckIE()
        {
            Logger.Log("Проверка наличия браузера Internet Explorer 11 и выше");

            string strKeyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer";
            string[] ls = new string[] { "svcVersion", "svcUpdateVersion", "Version", "W2kVersion" };

            int maxVer = 0;
            for (int i = 0; i < ls.Length; ++i)
            {
                object objVal = Microsoft.Win32.Registry.GetValue(strKeyPath, ls[i], "0");
                string strVal = System.Convert.ToString(objVal);
                if (strVal != null)
                {
                    int iPos = strVal.IndexOf('.');
                    if (iPos > 0)
                        strVal = strVal.Substring(0, iPos);

                    int res = 0;
                    if (int.TryParse(strVal, out res))
                        maxVer = Math.Max(maxVer, res);
                }

            }

            if (maxVer < 11)
            {
                Logger.Log("[ПРЕДУПРЕЖДЕНИЕ ]Версия браузера IE: " + maxVer + ". Не соответствует требованиям");
                warnings.Add( new WrongBrowserException(maxVer) );
            } else {
                Logger.Log("Версия браузера IE: " + maxVer + ". Соответствует требованиям");
            }
            
        }

        // Средства для создания документов (MS Office, WordPad, Libre Office)
        private static void CheckDocCreator()
        {
            return;
        }

        // Проверка Microsoft NET Framework версии не ниже 3.5 SP1
        private static void CheckNETFramework()
        {
            Logger.Log("Проверяем версию .NET-фреймворка");
            Version net_version = Environment.Version;
            if (net_version.Major < 3 || (net_version.Major == 3 && net_version.Minor < 5))
            {
                Logger.Log("[ПРЕДУПРЕЖДЕНИЕ] Версия .NET-фреймворка: " + net_version + ". Не соответствует требованиям");
                throw (new WrongNETException(net_version));
            }
            Logger.Log("Версия .NET-фреймворка: " + net_version + ". Соответствует требованиям");
        }

        [DllImport("advapi32.dll", SetLastError = true)]
            public static extern bool CryptGetProvParam(
            IntPtr hProv,
            uint dwParam,
            [In, Out] byte[] pbData, 
            ref uint dwDataLen,
            uint dwFlags
        );

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CryptAcquireContext(
            ref IntPtr hProv,
            String pszContainer,
            String pszProvider,
            Int32 dwProvType,
            Int32 dwFlags
        );

        // Средство СКЗИ CryptoPro CSP версии 4.0 и выше.
        public static bool CheckCryptoPro()
        {
            IntPtr dProvider = new IntPtr();
            if (CryptAcquireContext(ref dProvider, null, null, 75, CRYPT_VERIFYCONTEXT))
            {
                StringBuilder sb = new StringBuilder();
                byte[] ss = new byte[4];
                uint ll = 4;
                bool r = CryptGetProvParam(dProvider, 5, ss, ref ll, 0);

                if (ss[1] < 4)
                {
                    Logger.Log("Обнаружена версия КриптоПро ниже 4.");
                    return false;
                }
            }
            else
            {
                Logger.Log("ПО КриптоПро не обнаружено.");
                return false;
            }
            return true;
        }

    }
}
