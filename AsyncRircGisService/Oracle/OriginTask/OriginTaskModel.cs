using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Diagnostics.Contracts;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AsyncRircGisService.Oracle
{
    /// <summary>
    /// Необходим для получения данных о задаче из Oracle, обслуживает класс OriginTask, метод ProvideTaskInfo.
    /// </summary>
    public class OriginTaskModel : OracleBase<OriginTaskParameters, DataTable>
    {

        #region ctor

        /// <summary>
        /// Конструктор заполняет публичное свойство <see cref="Parameters"/>.
        /// </summary>
        /// <param name="parameters">Параметры  модели OriginTaskModel, которая обслуживает класс OriginTask, метод ProvideTaskInfo.</param>
        public OriginTaskModel( OriginTaskParameters parameters )
        {
            Contract.Requires<ArgumentException>( Regex.IsMatch( parameters.taskId, @"^\d+$" ), "ContractException. Parameter taskId is not valid." );

            // Заполняет строку данными для подключения к Oracle.
            parameters.connectSettings = GetConnectSettings();
            Parameters = parameters;
        }

        #endregion


        #region PublicMethods

        /// <summary>
        /// Выборка из БД данных о задаче в рамках выполнения запроса к сервисам ГИС ЖКХ.
        /// </summary>
        public override void Select()
        {
            if ( Parameters.operationsSelect == OriginTaskParameters.selectOperation.ProvideTaskInfo )
            {
                DataTable result = GetOracleData();
                // Заполняется публичное свойство для доступа к результатам выборки.
                ResultData = result;
            }
            else if ( Parameters.operationsSelect == OriginTaskParameters.selectOperation.Check )
            {
                DataTable result = GetTaskStatus();
                ResultData = result;
            }
        }

        /// <summary>
        /// Выставляет статус задачи
        /// </summary>
        public override void Update()
        {
            Contract.Requires<ArgumentException>( Regex.IsMatch( Parameters.connectSettings, @"^Data\sSource=\w+;User\sId=\w+;Password=\w+;$" ), "OriginTaskModel.Update(), connectSettings is not correct." );
            Contract.Requires<ArgumentException>( Regex.IsMatch( Parameters.taskId, @"^\d+$" ), "OriginTaskModel.Update(), taskId is not correct." );

            OracleConnection conn = new OracleConnection(Parameters.connectSettings);
            conn.Open();
            OracleCommand command = conn.CreateCommand();
            command.CommandText = "gis.task_manager.set_task_status";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add( "p_task_id", OracleDbType.Int32 ).Value = Convert.ToInt32( Parameters.taskId );
            command.Parameters.Add( "p_status", OracleDbType.Int32 ).Value = Parameters.taskStatus;

            command.BindByName = true;

            command.ExecuteNonQuery();

            conn.Close();
        }

        #endregion


        #region privateMethods

        /// <summary>
        /// Возвращает количество задач в обработке.
        /// </summary>
        /// <returns></returns>
        private DataTable GetTaskStatus()
        {
            Contract.Requires<ArgumentException>( Regex.IsMatch( Parameters.connectSettings, @"^Data\sSource=\w+;User\sId=\w+;Password=\w+;$" ), "OriginTaskModel.GetTaskStatus. ContractException. Parameter connectSettings is not correct" );
            Contract.Requires<ArgumentException>( Regex.IsMatch( Parameters.lastStartDate, @"^(\d+\.){2}\d+\s(\d+\:){2}\d+.+$" ), "OriginTaskModel.GetTaskStatus. ContractException. Parameter lastStartDate is not valid." );

            OracleConnection conn = new OracleConnection(@"Data Source=brn;User Id=***;Password=***;");
            conn.Open();
            OracleCommand command = conn.CreateCommand();
            command.CommandText = @"gis.task_manager.task_status";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add( "p_task_id", OracleDbType.Int32 ).Value = Parameters.taskId;
            command.Parameters.Add( "p_last_start_date", OracleDbType.Varchar2 ).Value = Parameters.lastStartDate;
            command.Parameters.Add( "v_return_status", OracleDbType.Int32, ParameterDirection.ReturnValue );

            command.BindByName = true;

            command.ExecuteNonQuery();

            DataTable resultDataTable = new DataTable();
            DataRow dr = resultDataTable.NewRow();
            resultDataTable.Columns.Add( "Status" );
            dr["Status"] =  command.Parameters["v_return_status"].Value.ToString();
            resultDataTable.Rows.Add( dr );

            return resultDataTable;
        }

        /// <summary>
        /// Возвращается Datatable со значениями из БД Oracle.
        /// </summary>
        /// <returns></returns>
        private DataTable GetOracleData()
        {

            Contract.Requires( Regex.IsMatch( Parameters.connectSettings, @"^Data\sSource=\w+;User\sId=\w+;Password=\w+;$" ), "connectSettings is not correct" );

            OracleConnection conn = new OracleConnection(Parameters.connectSettings);
            conn.Open();
            OracleCommand command = conn.CreateCommand();
            command.CommandText = "gis.task_manager.provide_task_info";
            command.CommandType = CommandType.StoredProcedure;

            OracleParameter parameterRefCursor = new OracleParameter();
            {
                parameterRefCursor.ParameterName = "p_ref_cursor";
                parameterRefCursor.Direction = ParameterDirection.Output;
                parameterRefCursor.OracleDbType = OracleDbType.RefCursor;
            }
            OracleParameter parameterTaskId = new OracleParameter();
            {
                parameterTaskId.ParameterName = "p_task_id";
                parameterTaskId.Direction = ParameterDirection.Input;
                parameterTaskId.OracleDbType = OracleDbType.Int32;
                parameterTaskId.Value = Convert.ToInt32( Parameters.taskId );
            }
            command.BindByName = true;
            command.Parameters.Add( parameterRefCursor );
            command.Parameters.Add( parameterTaskId );


            OracleDataAdapter oraAdapt = new OracleDataAdapter(command);

            DataTable resultDataTable = new DataTable();

            oraAdapt.Fill( resultDataTable );

            conn.Close();

            return resultDataTable;
        }
    }

    #endregion
}
