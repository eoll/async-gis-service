namespace AsyncRircGisService.Oracle
{
    /// <summary>
    /// Параметры  модели, которая обслуживает метод ExportOrgRegistry сервиса OrgRegistryCommon
    /// <para/>
    /// connectSettings - параметры подключения к Oracle
    /// </summary>
    public struct TaskRegistryParameters
    {
        // Строка подключения к БД.
        public string connectSettings;

        // Данная переменная указывает будет ли запрос являться тестовым или боевым.
        public bool isTest;
    }
}
