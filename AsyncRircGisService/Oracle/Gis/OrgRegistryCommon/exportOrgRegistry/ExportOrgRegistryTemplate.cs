using System;
namespace AsyncRircGisService.Oracle
{
    /// <summary>
    /// Шаблон для модели, полностью повторяет структуру таблицы оракл gis.st_res_export_org_registry.
    /// </summary>
    public struct ExportOrgRegistryTemplate
    {
        /// <summary>
        /// Статус обработки.
        /// </summary>
        public string requestState;

        /// <summary>
        /// Идентификатор сообщения, присвоенный РИРЦ.
        /// </summary>
        public string requesterMessageGuid;

        /// <summary>
        /// Идентификатор сообщения.
        /// </summary>
        public string messageGuid;

        /// <summary>
        /// Идентификатор корневой сущности организации в реестре организаций.
        /// </summary>
        public string orgRootEntityGuid;

        /// <summary>
        /// Идентификатор версии записи в реестре организаций.
        /// </summary>
        public string orgVersionGuid;

        /// <summary>
        /// Время последнего изменения.
        /// </summary>
        public string lastEditingDate;

        /// <summary>
        /// Признак актуальности записи.
        /// </summary>
        public Nullable<int> isActual;

        /// <summary>
        /// Сокращенное наименование.
        /// </summary>
        public string shortName;

        /// <summary>
        /// Полное наименование.
        /// </summary>
        public string fullName;

        /// <summary>
        /// ОГРН.
        /// </summary>
        public Nullable<long> ogrn;

        /// <summary>
        /// Дата регистрации организации.
        /// </summary>
        public string stateRegistrationDate;

        /// <summary>
        /// ИНН.
        /// </summary>
        public Nullable<long> inn;

        /// <summary>
        /// КПП.
        /// </summary>
        public Nullable<long> kpp;

        /// <summary>
        /// ОКОПФ.
        /// </summary>
        public Nullable<long> okopf;

        /// <summary>
        /// Статус: (P) PUBLISHED - опубликована в одном из документов в рамках раскрытия.
        /// </summary>
        public string organizationStatus;

        /// <summary>
        /// Идентификатор зарегистрированной организации.
        /// </summary>
        public string orgPpaguid;

        /// <summary>
        /// Код записи справочника organizationRoles.
        /// </summary>
        public string code;

        /// <summary>
        /// Идентификатор в ГИС ЖКХ.
        /// </summary>
        public string guid;

        /// <summary>
        /// Значение справочника organizationRoles.
        /// </summary>
        public string name;

        /// <summary>
        /// Зарегистрирована в ГИС ЖКХ.
        /// </summary>
        public  Nullable<int> isRegistered;

        /// <summary>
        /// Код ошибки ( при наличиии ).
        /// </summary>
        public string errorCode;

        /// <summary>
        /// Расшифровка ошибки ( при наличии ).
        /// </summary>
        public string errorMessage;
    }
}
