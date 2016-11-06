using System;
using System.Collections.Generic;

namespace AsyncRircGisService.Oracle
{
    /// <summary>
    /// Параметры  модели, которая обслуживает метод ExportNsiItem сервиса NsiСommon
    /// <para/>
    /// connectSettings - параметры подключения к Oracle
    /// <para/>
    /// task_id - код задачи
    /// </summary>
    public struct ExportNsiItemParameters
    {
        public string task_id             ;
        public string requesterMessageGuid;
        public string connectSettings     ;


        // В данное поле помещаются данные, для записи в БД.
        public IEnumerable<  Tuple< string, string >  > insertData;
    }
}
