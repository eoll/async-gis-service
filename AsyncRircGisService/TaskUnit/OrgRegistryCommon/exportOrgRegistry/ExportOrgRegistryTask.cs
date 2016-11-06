using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace AsyncRircGisService.TaskUnit
{
    public class ExportOrgRegistryTask : OriginTask
    {
        #region Constructor

        /// Заполняет <see cref="OriginTask.TaskDataPack"/> данными задачи из Oracle.
        public ExportOrgRegistryTask( DataPack dataPack )
        {
            Contract.Requires<ArgumentException>( Regex.IsMatch( dataPack.ServiceId, @"^\d+$" ), "ServiceId in DataPack is not correct" );
            Contract.Requires<ArgumentException>( Regex.IsMatch( dataPack.MethodId, @"^\d+$"  ), "MethodId in DataPack is not correct"  );
            Contract.Requires<ArgumentException>( Regex.IsMatch( dataPack.TaskId, @"^\d+$"    ), "TaskId in DataPack is not correct"    );

            Contract.Requires<ArgumentException>( Regex.IsMatch( dataPack.LastStartDate, @"^(\d+\.){2}\d+\s(\d+\:){2}\d+.+$" ), "LastStartDate in DataPack is not correct" );

            TaskDataPack = dataPack;
        }

        #endregion

        #region ProtectedMethods

        /// <summary>
        /// Поставляет данные задачи из базы Oracle в <see cref="taskOracleData"/>
        /// </summary>
        protected override void ProvideTaskOracleData()
        {
            Contract.Requires( Regex.IsMatch( TaskDataPack.ServiceId, @"^\d+$" ) );
            Contract.Requires( Regex.IsMatch( TaskDataPack.MethodId, @"^\d+$"  ) );
            Contract.Requires( Regex.IsMatch( TaskDataPack.TaskId, @"^\d+$"    ) );

            Contract.Requires( Regex.IsMatch( TaskDataPack.LastStartDate, @"^(\d+\.){2}\d+\s(\d+\:){2}\d+.+$" ) );

            /// Заполняются параметры модели из <see cref="OriginTask.TaskDataPack"/>.
            Oracle.ExportOrgRegistryParameters exportOrgRegistryParameters = new Oracle.ExportOrgRegistryParameters
            {
                task_id         = TaskDataPack.TaskId,
                connectSettings = ""
            };

            // Создаётся экземпляр класса ExportOrgRegistryModel и в него передаются параметры.
            Oracle.ExportOrgRegistryModel exportOrgRegistryModel = new Oracle.ExportOrgRegistryModel(exportOrgRegistryParameters);
            exportOrgRegistryModel.Select();

            // Поле класса заполняется данными из Oracle.
            taskOracleData = exportOrgRegistryModel.ResultData;
        }

        ///// <summary>
        ///// Выполняет асинхронно очередную подзадачу на запрос или ответ с помощью менеджера взаимодействия с ГИС ЖКХ.
        ///// </summary>
        ///// <param name="subtaskDataTable">Порция данных Oracle в форме <see cref="DataTable"/>, которая соответсвует подзадаче.</param>
        ///// <returns>Ожидаемая задача</returns>
        //protected override Task PerformSubtaskAsync( DataTable subtaskDataTable )
        //{
        //    return Task.Delay( 1 );
        //}

        #endregion
    }
}
