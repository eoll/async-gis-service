using AsyncRircGisService.Exceptions;

namespace AsyncRircGisService.TaskUnit
{
    /// <summary>
    /// Назначает и вызывает объект-обработчик для очередной задачи очереди.
    /// </summary>
    public static class Manager
    {
        /// <summary>
        /// Выполняет подбор обработчика исходной задачи.
        /// </summary>
        /// <returns></returns>
        public static OriginTask MethodProcessor( DataPack taskDataPack )
        {

            if      ( taskDataPack.ServiceId == "1" && taskDataPack.MethodId == "1" ) return new ExportOrgRegistryTask         ( taskDataPack );
            else if ( taskDataPack.ServiceId == "2" && taskDataPack.MethodId == "1" ) return new ExportNsiListTask             ( taskDataPack );
            else if ( taskDataPack.ServiceId == "2" && taskDataPack.MethodId == "2" ) return new ExportNsiItemTask             ( taskDataPack );
            else if ( taskDataPack.ServiceId == "3" && taskDataPack.MethodId == "1" ) return new ExportDataProviderNsiItemTask ( taskDataPack );

            else throw new NotFoundException( "По данным ServiceId = " + taskDataPack.ServiceId + " и MethodId = " + taskDataPack.MethodId + " класс задача не найден" );

        }
    }
}
