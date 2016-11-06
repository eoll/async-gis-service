using System;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace AsyncRircGisService.TaskUnit
{
    public class ExportNsiListTask : OriginTask
    {
        #region Constructor

        /// Заполняет <see cref="OriginTask.TaskDataPack"/> данными задачи из Oracle.
        public ExportNsiListTask( DataPack dataPack )
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
            Contract.Requires( Regex.IsMatch( TaskDataPack.ServiceId    , @"^\d+$"                            ) );
            Contract.Requires( Regex.IsMatch( TaskDataPack.MethodId     , @"^\d+$"                            ) );
            Contract.Requires( Regex.IsMatch( TaskDataPack.TaskId       , @"^\d+$"                            ) );
            Contract.Requires( Regex.IsMatch( TaskDataPack.LastStartDate, @"^(\d+\.){2}\d+\s(\d+\:){2}\d+.+$" ) );

            /// Заполняются параметры модели из <see cref="OriginTask.TaskDataPack"/>.
            Oracle.ExportNsiListParameters exportNsiListParameters = new Oracle.ExportNsiListParameters
            {
                task_id         = TaskDataPack.TaskId,
                connectSettings = ""
            };

            // Создаётся экземпляр класса ExportNsiListModel и в него передаются параметры.
            Oracle.ExportNsiListModel exportNsiListModel = new Oracle.ExportNsiListModel(exportNsiListParameters);
            exportNsiListModel.Select();

            // Поле класса заполняется данными из Oracle.
            taskOracleData = exportNsiListModel.ResultData;
#if DEBUG
            Notificator.Write("ExportNsiListTask, параметры задачи: " + exportNsiListModel.ResultData.Rows[0][0].ToString() + " " + exportNsiListModel.ResultData.Rows[0][1].ToString() );
#endif
        }
    }
    #endregion
}
