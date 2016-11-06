using System;
using System.Collections.Generic;

namespace AsyncRircGisService.Oracle
{
    /// <summary>
    /// Параметры  модели, которая обслуживает метод ExportOrgRegistry сервиса OrgRegistryCommon
    /// <para/>
    /// connectSettings - параметры подключения к Oracle
    /// <para/>
    /// mugkx_id - код участка, du_id - код домоуправления
    /// <para/>
    /// </summary>
    public struct ExportOrgRegistryParameters
    {
        public string task_id;
        public string requesterMessageGuid;
        public string connectSettings;


        // В данное поле помещаются данные, для записи в БД.
        public IEnumerable<  Tuple< string, string >  > insertData;
    }
}
