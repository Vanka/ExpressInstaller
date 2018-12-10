// Файл с исключениями
using System;

namespace ExpressInstaller
{
    public class CriticalErrorException : Exception
    {
        public string title;
        public string description;

        public CriticalErrorException()
        {}

        public CriticalErrorException(string message)
            : base(message)
        {
            this.title = "Критическая ошибка во время настройки рабочего места!";
            this.description = message;
        }
    }

    public class WrongBrowserException : Exception
    {
        public int version;

        public WrongBrowserException()
        {
        }

        public WrongBrowserException(int ver)
            : base("Версия Internet Explorer не соответствует требованиям: " + ver.ToString())
        {
            version = ver;
        }

        public WrongBrowserException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class WrongNETException : CriticalErrorException
    {
        public Version version;

        public WrongNETException()
        {
        }

        public WrongNETException(Version ver)
            : base("Версия NET Framework не соответствует требованиям: " + ver.ToString())
        {
            this.title = "Версия NET Framework не соответствует требованиям: " + ver.ToString();
            this.description = "Установите версию не ниже рекомендуемой";
        }
    }

    public class CSPNotInstalledException : CriticalErrorException
    {
        public CSPNotInstalledException()
            : base("Не обнаружена утилита КриптоПРО CSP")
        {
            this.title = "На вашем рабочем месте не обнаружена утилита КриптоПРО CSP";
            this.description = "Для настройки рабочего места установите сначала КриптоПРО CSP версии 4.0 и выше";
        }
    }

    public class WrongOperatingSystemException : CriticalErrorException
    {
        public Version version;

        public WrongOperatingSystemException()
        {
        }

        public WrongOperatingSystemException(Version ver)
            : base("Версия операционной системы не соответствует требованиям: " + ver.ToString())
        {
            this.version = ver;
            this.title = "Установлена неподдерживаемая версия Windows\nНастройка вашего компьютера будет прервана";
            this.description = "Для работы с электронными торговыми площадками необходима операционная система Windows версии 7, 8 или 10";
        }
    }

    public class CSPOldVersionException : CriticalErrorException
    {

        public CSPOldVersionException(int ver)
            : base("Версия КриптоПРО CSP не соответствует требованиям: " + ver.ToString() + ". Установите версию 4 и выше.")
        {
            this.title = "Обнаружена устаревшая версия КриптоПро CSP!";
            this.description = "Рекомендуем установить версию 4 с поддержкой нового криптографического алгоритма, обязательного для использования с 1 января 2019 года";
        }
    }
}

