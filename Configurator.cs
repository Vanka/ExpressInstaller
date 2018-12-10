using System;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;

namespace ExpressInstaller
{
    class Configurator
    {
        const int TRUSTED_SITES_ZONE = 2;
        public static string AppFolder { get; set; }

        public static void configureAll()
        {
            DirectoryInfo directory = Directory.CreateDirectory(AppFolder + @"\tmp");
            try
            {
                ConfigureTrustedURLS();
                ChangeActiveXSettings();
                StartServices();
                HideUpgradeToGOST();
                ChangeCryptoProSettings();
                RegisterOIDS();
                installCertificates();
            }
            catch
            {
                directory.Delete(true);
                throw;
            }
            directory.Delete(true);
        }

        private static void ConfigureTrustedURLS()
        {
            List<string> url_list = new List<string>(new string[] {
                "https://*.etprf.ru",
                "https://webppo.etprf.ru",
                "https://webcust.etprf.ru",
                "https://gardoc.ru",
                "https://garantexpress.ru",
                "https://etp.roseltorg.ru",
                "https://sberbank-ast.ru",
                "https://b2b-center.ru",
                "http://zakupki.gov.ru",
                "http://*.zakazrf.ru",
                "http://*.tattis.ru",
                "http://*.agzrt.ru"
            });

            url_list.ForEach(delegate (string url)
            {
                AddTrustedSiteToInternetExplorer(url);
            });
        }

        // 270C     ActiveX Controls and plug-ins: Run Antimalware software on ActiveX controls
        // 1001     ActiveX controls and plug-ins: Download signed ActiveX controls
        // 1004     ActiveX controls and plug-ins: Download unsigned ActiveX controls
        // 1200     ActiveX controls and plug-ins: Run ActiveX controls and plug-ins
        // 1201     ActiveX controls and plug-ins: Initialize and script ActiveX controls not marked as safe for scripting
        // 2201     ActiveX controls and plug-ins: Automatic prompting for ActiveX controls
        // 1201     ActiveX controls and plug-ins: Initialize and script ActiveX controls not marked as safe for scripting
        // 1209     ActiveX controls and plug-ins: Allow Scriptlets
        // 1001     ActiveX controls and plug-ins: Download signed ActiveX controls
        // 1004     ActiveX controls and plug-ins: Download unsigned ActiveX controls
        private static void ChangeActiveXSettings()
        {
            IDictionary<string, int> settings = new Dictionary<string, int>();
            settings["2201"] = 0;
            settings["2702"] = 3;
            settings["1405"] = 0;
            settings["1200"] = 0;
            settings["270C"] = 3;
            settings["1201"] = 0;
            settings["2000"] = 0;
            settings["120A"] = 0;
            settings["120B"] = 0;
            settings["1208"] = 0;
            settings["1209"] = 0;
            settings["1004"] = 0;
            settings["1001"] = 0;

            RegistryKey myKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\2", true);
            RegistryKey myKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\2", true);

            foreach (string key in settings.Keys)
            {
                int value = settings[key];
                myKey.SetValue(key, value, RegistryValueKind.DWord);
                myKey2.SetValue(key, value, RegistryValueKind.DWord);
            }

            myKey.Close();
            myKey2.Close();
        }

        private static void RegisterOIDS()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Resources\express_oids.reg";
            ProcessStartInfo pInfo = new ProcessStartInfo("regedit.exe", "/s \"" + path + "\"");
            Process p = Process.Start(pInfo);
            p.WaitForExit();
        }

        private static void ChangeCryptoProSettings()
        {
            RegistryKey licence_error = Registry.LocalMachine.OpenSubKey("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Crypto Pro\\Settings\\KeyDevices", true);
            if (licence_error != null)
            {
                licence_error.SetValue("LicErrorLevel", 6, RegistryValueKind.DWord);
                licence_error.Close();
            }
        }
        

        private static void StartServices()
        {
            RegistryKey window_driver_foundation = Registry.LocalMachine.OpenSubKey("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\wudfsvc", true);
            if (window_driver_foundation != null)
            {
                window_driver_foundation.SetValue("Start", 2, RegistryValueKind.DWord);

                window_driver_foundation.Close();
            }

            RegistryKey cert_prop = Registry.LocalMachine.OpenSubKey("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\CertPropSvc", true);
            if (cert_prop != null)
            {
                cert_prop.SetValue("Start", 2, RegistryValueKind.DWord);

                cert_prop.Close();
            }
        }

        private static void HideUpgradeToGOST()
        {
            RegistryKey CSPparams = Registry.LocalMachine.OpenSubKey("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Crypto Pro\\Cryptography\\CurrentVersion\\Parameters", true);
            if (CSPparams != null)
            {
                CSPparams.SetValue("warning_time_gen_2001", "ffffffffff", RegistryValueKind.DWord);
                CSPparams.SetValue("warning_time_sign_2001", "ffffffffff", RegistryValueKind.DWord);

                CSPparams.Close();
            }
        }

        private static void installCertificates()
        {
            byte[] root_cert = Properties.Resources.rootsert;

            X509Certificate2Collection root_certs = new X509Certificate2Collection();
            root_certs.Import(root_cert);
            X509Store root_store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            root_store.Open(OpenFlags.ReadWrite);

            var sp = new StorePermission(PermissionState.Unrestricted)
            {
                Flags = StorePermissionFlags.AddToStore
            };
            sp.Assert();

            root_store.AddRange(root_certs);
            root_store.Close();

            byte[] auth_cert = Properties.Resources.casert;

            X509Certificate2Collection auth_certs = new X509Certificate2Collection();
            auth_certs.Import(auth_cert);
            X509Store auth_store = new X509Store(StoreName.CertificateAuthority, StoreLocation.LocalMachine);
            auth_store.Open(OpenFlags.ReadWrite);
            auth_store.AddRange(auth_certs);
            auth_store.Close();
        }


        private static void AddTrustedSiteToInternetExplorer(string url)
        {
            Match match = Regex.Match(url, @"\A(.+)://((.+)\.)?([^.]+\.[^.]+)\Z");
            string protocol = match.Groups[1].Value;
            string subdomain = match.Groups[3].Value;
            string domain = match.Groups[4].Value;
            if (domain == "")
            {
                match = Regex.Match(url, @"\A(.+)://(.+)\Z");
                protocol = match.Groups[1].Value;
                subdomain = "";
                domain = match.Groups[2].Value;
            }
            if (protocol == "" || domain == "")
            {
                throw new Exception("Неверный URL:" + url);
            }
            string key = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\ZoneMap\\Domains\\" + domain;
            RegistryKey regKeyDomain = Registry.CurrentUser.CreateSubKey(key);
            using (regKeyDomain)
            {
                if (subdomain == "")
                {
                    regKeyDomain.SetValue(protocol, TRUSTED_SITES_ZONE);
                }
                else
                {
                    RegistryKey regKeySubdomain = regKeyDomain.CreateSubKey(subdomain);
                    using (regKeySubdomain)
                    {
                        regKeySubdomain.SetValue(protocol, TRUSTED_SITES_ZONE);
                    }
                }
            }
        }

    }
}
