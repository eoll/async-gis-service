namespace AsyncRircGisService.TaskUnit
{
    /// <summary>
    /// Параметры задачи.
    /// </summary>
    public struct DataPack
    {
        /// <summary>
        /// Код задачи в журнале Oracle.
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        /// Дата последнего запуска задачи.
        /// </summary>
        public string LastStartDate { get; set; }

        /// <summary>
        /// Код сервиса ГИС ЖКХ.
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// Код метода ГИС ЖКХ.
        /// </summary>
        public string MethodId { get; set; }

        /// <summary>
        /// Идентификатор организации в БД РИРЦ.
        /// </summary>
        public string OrgId { get; set; }

        //public string MugkxId { get; set; }

        //public string DuId { get; set; }

        /// <summary>
        /// Приоритет выполнения задач (1 - самый низкий).
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Код домоуправления в ГИС ЖКХ.
        /// </summary>
        public string OrgPPAGUID { get; set; }

        /// <summary>
        /// Путь к сервису ГИС ЖКХ.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Использовать код организации или нет.
        /// </summary>
        public bool AddOrgPpaGuid { get; set; }

        /// <summary>
        /// Добавлять сигнатуру к запросу или нет.
        /// </summary>
        public bool AddSignature { get; set; }

        /// <summary>
        /// Путь к шаблону XML тела SOAP конверта.
        /// </summary>
        public string TemplatePath { get; set; }

        /// <summary>
        /// Единообразное название ресурса (действия).
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Путь к шаблону XML тела SOAP конверта при запросе на получение результатов. (get-result)
        /// </summary>
        public string TemplateResponsePath { get; set; }

        /// <summary>
        /// Единообразное название ресурса (действия) при запросе на получение ответа. (get-result)
        /// </summary>
        public string ActionResponce { get; set; }

        /// <summary>
        /// Сколько раз задача была на выполнении.
        /// </summary>
        public int Attempt { get; set; }

        /// <summary>
        /// Версия метода ГИС ЖКХ (необходимо для указания в XML).  
        /// </summary>
        public string MethodVersion { get; set; }



    }

}
