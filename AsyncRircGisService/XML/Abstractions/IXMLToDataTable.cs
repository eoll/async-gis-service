namespace AsyncRircGisService.XML {
    public interface IXMLToDataTable
    {

        /// <summary>
        /// Преобразует XML в DataTable.
        /// </summary>
        /// <returns>Таблица типа DataTable.</returns>
        System.Data.DataTable BuildDataTable();
    }
}
