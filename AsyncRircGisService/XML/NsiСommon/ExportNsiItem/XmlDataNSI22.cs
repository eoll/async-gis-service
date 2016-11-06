namespace AsyncRircGisService.XML
{
    /// <summary>
    /// Класс разбора XML справочника ГИС ЖКХ - NSI 22.
    /// </summary>
    public class XmlDataNSI22 : XMLDataCommonAs10
    {

        /// <summary>
        /// Преобразует XML в DataTable.
        /// </summary>
        /// <returns>Таблица типа DataTable.</returns>
        public XmlDataNSI22( string bodyXML )
        {

            this.bodyXML = bodyXML;

        }
    }
}
