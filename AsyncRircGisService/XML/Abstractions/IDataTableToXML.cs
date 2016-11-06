namespace AsyncRircGisService.XML
{
    interface IDataTableToXML
    {

        /// <summary>
        /// Таблица данных (данные Oracle) для формирования XML структуры тела запроса к методу сервиса ГИС ЖКХ.
        /// </summary>
        System.Data.DataTable DataTable { get; set; }



        /// <summary>
        /// Выстраивает XML структуру тела запроса на основе таблицы данных (данные Oracle) по спецификации wsdl.
        /// </summary>
        void BuildXML();

    }
}
