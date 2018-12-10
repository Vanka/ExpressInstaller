using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;

namespace ExpressInstaller
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private NotifyIcon nIcon = new NotifyIcon();
        private Icon ee_icon = ExpressInstaller.Properties.Resources.logo_wh;
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);
                Configurator.AppFolder = Path.GetTempPath() + @"ExpressInstaller";
                Directory.CreateDirectory(Configurator.AppFolder);
                Logger.LogDir = Configurator.AppFolder + @"\log_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log";

                Logger.Log("Начало настройки");
                //create the notifyicon
                nIcon.Icon = ee_icon;
                nIcon.Visible = true;
                ShowInitialBalloon();

                // Валидация требований ПК. В случае, если какое-то требование не выполнено -
                // уведомление об ошибке и прервана установка
                Logger.Log("Валидация требований");
                PCValidator.Validate();

                // Установка всего
                Logger.Log("Установка необходимых компонентов");
                LocalInstaller.installAll();

                // Настройка всего
                Logger.Log("Настройка рабочего места");
                Configurator.configureAll();

                ShowFinalBalloon();
                if(PCValidator.warnings.Count > 0)
                {
                    List<String> warns = new List<String>();
                    PCValidator.warnings.ForEach(delegate(Exception error)
                    {
                        warns.Add(error.Message);
                    });

                    CriticalError crit = new CriticalError("Успешное завершение установки", "Во время настройки были выработаны некоторые рекомендации:\n" + String.Join("\n", warns));
                    crit.Show();
                    Logger.Log("Успешное завершение с предупреждениями");
                }
                else
                {
                    System.Windows.Application.Current.Shutdown();
                }


            }
            catch (CriticalErrorException error)
            {
                CriticalError crit = new CriticalError(error.title, error.description);
                crit.Show();
            }
            catch (Exception er)
            {
                CriticalError crit = new CriticalError("В процессе настройки вашего компьютера произошла неизвестная ошибка.", "Подробная информация в журнале " + Logger.LogDir + ".\nПопробуйте снова запустить настройку\n");
                crit.Show();
                Logger.Log("Ошибка: " + er.ToString());
            }
            Logger.Log("Окончание настройки");
        }
   
        protected override void OnExit(ExitEventArgs e)
        {
            ShowFinalBalloon();
            base.OnExit(e);
        }

        private void ShowInitialBalloon()
        {
            string title = "Экспресс-настройка";
            string text = "Мы подготовим ваш компьютер для работы с торговыми площадками\n\nЭто займет пару минут";

            nIcon.ShowBalloonTip(10000, title, text, ToolTipIcon.Info);
        }

        private void ShowFinalBalloon()
        {
            string title = "Экспресс-настройка";
            string text = "Все готово!\n\nТеперь вы можете начать работу с электронными площадками";

            nIcon.ShowBalloonTip(10000, title, text, ToolTipIcon.Info);
        }

        private void ShowErrorBalloon(string error)
        {
            string title = "Ошибка при выполнении установки";
            string text = error;

            nIcon.ShowBalloonTip(10000, title, text, ToolTipIcon.Info);
        }
    }
}
