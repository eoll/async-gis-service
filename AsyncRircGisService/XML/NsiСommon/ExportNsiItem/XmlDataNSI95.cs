namespace AsyncRircGisService.XML
{
    /// <summary>
    /// Класс разбора XML справочника ГИС ЖКХ - NSI 95.
    /// </summary>
    public class XmlDataNSI95 : XMLDataCommonAs10
    {

        /// <summary>
        /// Преобразует XML в DataTable.
        /// </summary>
        /// <returns>Таблица типа DataTable.</returns>
        public XmlDataNSI95( string bodyXML )
        {

            this.bodyXML = bodyXML;

        }
    }
}
