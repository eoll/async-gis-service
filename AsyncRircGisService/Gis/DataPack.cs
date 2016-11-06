using System;
using System.Collections.Generic;
using AsyncRircGisService.Gis.Configurations.Sections;

namespace AsyncRircGisService.Gis
{
    /// <summary>
    /// Набор данных для формирования запроса к методу сервиса ГИС ЖКХ и получения ответа.
    /// </summary>
    public struct DataPack
    {
        /// <summary>
        /// Идентификатор задачи в Oracle.
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        /// Код сообщения-запроса в ГИС. Иcпользуется для первого запроса к методу сервиса.
        /// </summary>
        public string RequesterMessageGuid { get; set; }

        /// <summary>
        /// Код сообщения-ответа в ГИС. Используется для получения результатов работы метода сервиса.
        /// </summary>
        public string MessageGuid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LoginService { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PassService { get; set; }

        /// <summary>
        /// Путь к файлу шаблона для формирования XML конверта запроса к методу сервиса.
        /// </summary>
        public string TemplatePath { get; set; }

        /// <summary>
        /// Версия сервера, к которому отправляется запрос.
        /// </summary>
        public string SchemaVersion { get; set; }

        /// <summary>
        /// Флаг - Использовать orgPPAGuid или нет.
        /// <summary>
        public bool AddOrgPPAGUID { get; set; }

        /// <summary>
        /// Значение текущего orgPPAGuid организации (ду) от имени котрой будут отправляться запросы.
        /// </summary>
        public string OrgPPAGUID { get; set; }

        /// <summary>
        /// Флаг - использовать orgPPAGuid РИРЦ.
        /// </summary>
        public string AddRircOrgPPAGUID { get; set; }

        /// <summary>
        /// Значение orgPPAGuid РИРЦ.
        /// </summary>
        public string RircOrgPPAGUID { get; set; }

        /// <summary>
        /// Параметры, которые будут передаваться методу.
        /// </summary>
        public IEnumerable<Tuple<string, string>> Xpath2Values { get; set; }

        /// <summary>
        /// Путь к шаблонам для формирования SOAP пакета.
        /// </summary>
        public SoapConfiguration SoapConfig { get; set; }

        /// <summary>
        /// Отпечаток сертификата.
        /// </summary>
        public string CertificateThumbprint { get; set; }

        /// <summary>
        /// Пароль для контейнера с подписью.
        /// </summary>
        public string SignPasswd { get; set; }

        /// <summary>
        /// Флаг - использовать подпись в XML или нет.
        /// </summary>
        public bool AddSignature { get; set; }

        /// <summary>
        /// Значение адреса, куда будет отправлен запрос.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Action метода из файла конфигурации.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Ответ метода сервиса ГИС ЖКХ.
        /// </summary>
        public IEnumerable<Tuple<string, string>> GisResponseData { get; set; }

        /// <summary>
        /// Указывает на выполение запроса к тестовому сервису или к боевому.(1- тест, 2 - боевой запрос).
        /// </summary>
        public bool Test { get; set; }

    }
}
