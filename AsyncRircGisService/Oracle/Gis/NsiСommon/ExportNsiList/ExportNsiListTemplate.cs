using System;
namespace AsyncRircGisService.Oracle
{
    /// <summary>
    /// Шаблон для модели, полностью повторяет структуру таблицы оракл gis.ST_RES_EXPORT_NSI_LIST.
    /// </summary>
    public struct ExportNsiListTemplate
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
        /// Наименование справочника.
        /// </summary>
        public string name;

        /// <summary>
        /// Дата и время последнего изменения элемента справочника (в том числе создания).
        /// </summary>
        public string modified;
    }
}
