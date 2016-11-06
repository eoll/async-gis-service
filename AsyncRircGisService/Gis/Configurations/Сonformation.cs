using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncRircGisService.Gis.Configurations.Sections;

namespace AsyncRircGisService
{
    /// <summary>
    /// Набор данных из app.config.
    /// </summary>
    public struct Сonformation
    {
        // <signingConfig>
        /// <summary>
        /// Отпечаток сертификата.
        /// </summary>
        public string certificateThumbprint;

        /// <summary>
        /// Пароль к контейнеру секретного ключа подписи.
        /// </summary>
        public string certificatePassword;

        // <GisServicesConfig>
        /// <summary>
        /// Идентификатор оператора ИС.
        /// </summary>
        public string RIRCorgPPAGUID;

        /// <summary>
        /// Ip адрес сервера ГИС.
        /// </summary>
        public string baseUrl;

        /// <summary>
        /// Текущая версия сервиса ГИС.
        /// </summary>
        public string schemaVersion;

        // <GisServicesConfig>/<SoapConfiguration>
        /// <summary>
        /// Пути для формирования конверта SOAP.
        /// </summary>
        public SoapConfiguration soapConfiguration;

        /// <summary>
        /// Логин к сервису ГИС ЖКХ
        /// </summary>
        public string login;

        /// <summary>
        /// Пароль к сервису ГИС ЖКХ
        /// </summary>
        public string password;

        /// <summary>
        /// Парметр указывает будет ли служба запущена как тест.
        /// </summary>
        public bool IsTest;

        /// <summary>
        /// Параметр содержит список email адресов, на которые будут рассылаться служебные сообщения.
        /// </summary>
        public string sendTo;

        /// <summary>
        /// Параметр содержит переодичность (в минутах), через которую будет запускаться служба.
        /// </summary>
        public int timeInterval;

        /// <summary>
        /// Показывает сколько раз может выполняться задача.
        /// </summary>
        public int amountAttempt;

    }
}
