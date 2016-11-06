using System;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
namespace AsyncRircGisService.Gis.Abstractions
{
    /// <summary>
    /// Менеджер по взаимодействию с сервисом ГИС ЖКХ. 
    /// <para/>
    /// Получает набор входящих параметров, описывающих частную операцию в ГИС ЖКХ.
    /// <para/>
    /// Выполняет асинхронные запросы на исполение методов того или иного сервиса ГИС ЖКХ.
    /// <para/>
    /// Выполняет асинхронные запросы на получение результатов работы методов сервиса ГИС ЖКХ.
    /// </summary>
    [ContractClass(typeof(ContractForIManager))]
    interface IManager
    {
        /// <summary>
        /// Набор входящих параметров.
        /// </summary>
        DataPack DataPack { get; set; }

        /// <summary>
        /// 
        /// Выполняет асинхронный запрос к методу сервиса ГИС ЖКХ.
        /// </summary>
        string RequestMethod(DataPack gisDataPack);

        /// <summary>
        /// Выполняет асинхронный запрос на получение результатов метода сервиса ГИС ЖКХ.
        /// </summary>
        void RequestMethodResults(DataPack gisDataPack);

    }

    [ContractClassFor(typeof(IManager))]
    sealed class ContractForIManager : IManager
    {
        /// <summary>
        /// Набор входящих параметров.
        /// </summary>
        public DataPack DataPack { get; set; }

        /// <summary>
        /// Выполняет асинхронный запрос к методу сервиса ГИС ЖКХ.
        /// </summary>
        public string RequestMethod(DataPack gisDataPack)
        {
            Contract.Requires(!string.IsNullOrEmpty(gisDataPack.RequesterMessageGuid));
            Contract.Ensures(!string.IsNullOrEmpty(DataPack.MessageGuid));
            return DataPack.MessageGuid;
        }

        /// <summary>
        /// Выполняет асинхронный запрос на получение результатов метода сервиса ГИС ЖКХ.
        /// </summary>
        public void RequestMethodResults(DataPack gisDataPack)
        {
            Contract.Requires(!string.IsNullOrEmpty(gisDataPack.MessageGuid));
        }

    }
}
