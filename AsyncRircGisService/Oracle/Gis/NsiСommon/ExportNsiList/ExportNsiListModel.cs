using System;
using System.Data;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;
using System.Diagnostics;
using AsyncRircGisService.Exceptions;
using System.Collections.Generic;
using System.Collections;

namespace AsyncRircGisService.Oracle

{
    public class ExportNsiListModel : OracleBase<ExportNsiListParameters, DataTable>
    {

        #region Constructor

        /// <summary>
        /// В конструкторе передаются параметры для подключения к oracle и получения данных для задачи.
        /// </summary>
        /// <param name="parameters"><see cref="ExportNsiListParameters"/>Список параметров для запроса.</param>
        public ExportNsiListModel( ExportNsiListParameters parameters )
        {

            // Записываются параметры соединения с БД.
            parameters.connectSettings = GetConnectSettings( );
            /// Параметры добавляются в публичное свойство.
            Parameters = parameters;
        }

        #endregion


        #region PublicMethods

        /// <summary>
        /// Выборка из БД значения параметра LISTGROUP <see cref="ResultData"/>/.
        /// </summary>
        /// <param name="parameters"><see cref="ExportNsiListParameters"/>Список параметров для запроса.</param>
        public override void Select()
        {
            // Контрактами устанавливаются предусловия и постусловия входных значений.
            Contract.Requires( Regex.IsMatch( Parameters.task_id        , @"^\d+$"                                         ) );
            Contract.Requires( Regex.IsMatch( Parameters.connectSettings, @"^Data\sSource=\w+;User\sId=\w+;Password=\w+;$" ) );

            /// Устанавливается соединение с базой и заполняется <see cref="ResultData"/>.
            DataTable resultDataTable = GetOracleData();
            ResultData = resultDataTable;

#if DEBUG
            Debug.WriteLine( "ResultData = {0} и {1}", ResultData.Rows[0][0], ResultData.Rows[0][1] );
#endif
        }

        /// <summary>
        /// Запись в таблицу ST_RES_EXPORT_NSI_LIST результата из ГИС ЖКХ.
        /// </summary>
        public override void Insert()
        {
            Contract.Requires<ArgumentException>( Regex.IsMatch( Parameters.connectSettings     , @"^Data\sSource=\w+;User\sId=\w+;Password=\w+;$"                                         ) );
            Contract.Requires<ArgumentException>( Regex.IsMatch( Parameters.requesterMessageGuid, @"([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}" ) );

            // Передаём кортеж в поле базового класса для работы метода ConvertTupleErrorToDataTable.
            TupleValue = Parameters.insertData;

            // Получаем ошибку из кортежа.
            string error = TupleElementValue( "ErrorCode" );

            // Если переменная error пустая, то вызываем метод который из кортежа дастаёт xml и преобразует в DataTable.
            // Иначе получаем из кортежа ошибку, описание и без разбора XML формируем DataTable.
            if ( string.IsNullOrEmpty( error ) )
            {
                // Производим разбор кортежа и преобразование его значений в DataTable.
                ConvertTupleToDataTable();
                // Вносит данные из exportToOracle в базу данных.
                foreach ( DataRow row in exportToOracle.Rows ) InsertGisResult( row );
            }
            else
            {
                // Получаем ошибку и значение из картежа и записываем в DataTable.
                ConvertTupleErrorToDataTable();
                InsertGisError( exportToOracle.Rows[0], Parameters.requesterMessageGuid );
            }
        }

        #endregion


        #region PrivateMethods

        /// <summary>
        /// Возвращается Datatable со значениями из БД Oracle.
        /// </summary>
        /// <param name="parameters"><see cref="ExportNsiListParameters"/>Список параметров для запроса.</param>
        /// <returns>DataTable представление данных БД.</returns>
        private DataTable GetOracleData()
        {
            // Контракты кода.
            Contract.Requires( Regex.IsMatch( Parameters.task_id        , @"^\d+$"                                         ) );
            Contract.Requires( Regex.IsMatch( Parameters.connectSettings, @"^Data\sSource=\w+;User\sId=\w+;Password=\w+;$" ) );

            OracleConnection conn = new OracleConnection( Parameters.connectSettings );
            conn.Open();
            OracleCommand command = conn.CreateCommand();
            command.CommandText = "gis.export_nsi_list.provide_task_oracle_data";
            command.CommandType = CommandType.StoredProcedure;

            OracleParameter parameterV_ref_cursor = new OracleParameter();
            {
                parameterV_ref_cursor.ParameterName = "p_ref_cursor";
                parameterV_ref_cursor.Direction     = ParameterDirection.Output;
                parameterV_ref_cursor.OracleDbType  = OracleDbType.RefCursor;
            }

            OracleParameter parameterTask_id = new OracleParameter();
            {
                parameterTask_id.ParameterName = "p_task_id";
                parameterTask_id.Direction     = ParameterDirection.Input;
                parameterTask_id.OracleDbType  = OracleDbType.Int32;
                parameterTask_id.Value         = Convert.ToInt32( Parameters.task_id );
            }

            command.BindByName = true;

            command.Parameters.Add( parameterV_ref_cursor );
            command.Parameters.Add( parameterTask_id );

            OracleDataAdapter oraAdapt = new OracleDataAdapter( command );

            DataTable resultDataTable = new DataTable();

            oraAdapt.Fill( resultDataTable );

            conn.Close();

            return resultDataTable;
        }

        /// <summary>
        /// Заполняет поле <see cref="exportToOracle"/> данными кордежа.
        /// </summary>
        private void ConvertTupleToDataTable()
        {
            // Создаём объект для работы с XML.
            XML.ExportNsiListBuilder xmlBuilder = new XML.ExportNsiListBuilder();

            // Записываем сырой XML результат запроса.
            xmlBuilder.XMLResponse = TupleElementValue( "XMLstring" );

            // Результат обработки XML файла заносим в поле класса-модели.
            exportToOracle = xmlBuilder.BuildDataTable();
        }

        /// <summary>
        /// Вносит данные из шаблона <see cref="ExportNsiListTemplate"/> в базу данных.
        /// </summary>
        private void InsertGisResult(DataRow exportToOracleRow)
        {
            OracleConnection conn = new OracleConnection( Parameters.connectSettings );
            OracleCommand command = conn.CreateCommand();

            conn.Open();

            command.CommandText = @"
                                    DECLARE
                                      v_results gis.st_res_export_nsi_list%ROWTYPE;
                                    BEGIN
                                    v_results.REQUESTER_MESSAGE_GUID := :P_REQUESTER_MESSAGE_GUID;
                                    v_results.MESSAGE_GUID           := :P_MESSAGE_GUID;
                                    v_results.REGISTRY_NUMBER        := :P_REGISTRY_NUMBER;
                                    v_results.NAME                   := :P_NAME;
                                    v_results.MODIFIED               := :P_MODIFIED;

                                    gis.export_nsi_list.register_results( p_results => v_results );

                                    END;
                                ";

            command.BindByName = true;

            command.Parameters.Add( ":P_REQUESTER_MESSAGE_GUID", OracleDbType.Varchar2 ).Value = Parameters.requesterMessageGuid;

            command.Parameters.Add( ":P_MESSAGE_GUID"          , OracleDbType.Varchar2 ).Value = exportToOracleRow[ "MESSAGE_GUID"           ];
            command.Parameters.Add( ":P_REGISTRY_NUMBER"       , OracleDbType.Int32    ).Value = exportToOracleRow[ "REGISTRYNUMBER"         ];
            command.Parameters.Add( ":P_NAME"                  , OracleDbType.Varchar2 ).Value = exportToOracleRow[ "NAME"                   ];

            // Это сделано, для того что пустую дату вставить в БД невозможно, поэтому если пусто, вставляем 01.01.1990.
            command.Parameters.Add( ":P_MODIFIED"              , OracleDbType.Date     ).Value = Convert.ToDateTime( exportToOracleRow[ "MODIFIED" ].ToString() != "" ? exportToOracleRow[ "MODIFIED" ] : "01.01.1990" );

            Debug.WriteLine( "В таблицу внесено: " 
                                                + Parameters.requesterMessageGuid
                                                + " " + exportToOracleRow[ "MESSAGE_GUID"           ] 
                                                + " " + exportToOracleRow[ "REGISTRYNUMBER"         ] 
                                                + " " + exportToOracleRow[ "NAME"                   ]
                                                + " " + exportToOracleRow[ "MODIFIED"               ]
                                                );

            command.ExecuteNonQuery();

            conn.Close();
        }

        #endregion
    }
}