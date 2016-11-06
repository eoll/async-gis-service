namespace AsyncRircGisService.XML
{
    /// <summary>
    /// Класс разбора XML справочника ГИС ЖКХ - NSI 10.
    /// </summary>
    public class XmlDataNSI10 : XMLDataCommonAs10
    {

        /// <summary>
        /// Преобразует XML в DataTable.
        /// </summary>
        /// <returns>Таблица типа DataTable.</returns>
        public XmlDataNSI10( string bodyXML )
        {

            this.bodyXML = bodyXML;

        }
    }
}
