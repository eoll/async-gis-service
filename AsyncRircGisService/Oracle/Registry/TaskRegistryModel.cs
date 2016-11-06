using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace AsyncRircGisService.Oracle
{
    /// <summary>
    /// Получает список задач из базы данных Oracle
    /// </summary>
    public class TaskRegistryModel : OracleBase<TaskRegistryParameters, DataTable>
    {
        /// <summary>
        /// Конструктор настраивает параметры подключения к Oracle
        /// </summary>
        /// <param name="parameters">Список параметров запроса</param>
        public TaskRegistryModel( TaskRegistryParameters parameters )
        {
            // Заполняет строку данными для подключения к Oracle.
            parameters.connectSettings = GetConnectSettings();
#if DEBUG
            parameters.isTest = true;
#else
            parameters.isTest = ServiceConfig.Conformation.IsTest;
#endif
            Parameters = parameters;
        }

        /// <summary>
        /// Осуществляет выборку активных задач, имеющих статус на регистрации.
        /// </summary>
        /// <param name="parameters">Список параметров запроса</param>
        public override void Select()
        {
            // Заполняем свойство данными из БД.
            ResultData = GetOracleData();
        }

        /// <summary>
        /// Возвращается Datatable со значениями из БД Oracle.
        /// </summary>
        /// <returns></returns>
        private DataTable GetOracleData()
        {

            Contract.Requires<ArgumentException>( Regex.IsMatch( Parameters.connectSettings, @"^Data\sSource=\w+;User\sId=\w+;Password=\w+;$" ), "connectSettings is not correct" );

            OracleConnection conn = new OracleConnection(Parameters.connectSettings);
            conn.Open();
            OracleCommand command = conn.CreateCommand();
            command.CommandText = "gis.task_manager.provide_task_data";
            command.CommandType = CommandType.StoredProcedure;

            OracleParameter parameterRefCursor = new OracleParameter();
            {
                parameterRefCursor.ParameterName = "p_ref_cursor";
                parameterRefCursor.Direction = ParameterDirection.Output;
                parameterRefCursor.OracleDbType = OracleDbType.RefCursor;
            }
            OracleParameter parameterTest = new OracleParameter();
            {
                parameterTest.ParameterName = "p_test";
                parameterTest.Direction = ParameterDirection.Input;
                parameterTest.OracleDbType = OracleDbType.Int32;
                parameterTest.Value = Convert.ToInt32( Parameters.isTest );
            }
            command.BindByName = true;
            command.Parameters.Add( parameterRefCursor );
            command.Parameters.Add( parameterTest );

            OracleDataAdapter oraAdapt = new OracleDataAdapter(command);

            DataTable resultDataTable = new DataTable();

            oraAdapt.Fill( resultDataTable );

            conn.Close();

            return resultDataTable;
        }
    }
}
