using System;
using System.Data;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;
using System.Diagnostics;

namespace AsyncRircGisService.Oracle
{
    public class ExportDataProviderNsiItemModel : OracleBase<ExportDataProviderNsiItemParameters, DataTable>
    {

        #region Constructor

        /// <summary>
        /// В конструкторе передаются параметры для подключения к oracle и получения данных для задачи.
        /// </summary>
        /// <param name="parameters"><see cref="ExportDataProviderNsiItemParameters"/>Список параметров для запроса.</param>
        public ExportDataProviderNsiItemModel( ExportDataProviderNsiItemParameters parameters )
        {

            // Записываются параметры соединения с БД.
            parameters.connectSettings = GetConnectSettings();
            /// Параметры добавляются в публичное свойство.
            Parameters = parameters;
        }

        #endregion

        #region PublicMethods

        /// <summary>
        /// Выборка из БД значения параметра LISTGROUP <see cref="ResultData"/>.
        /// </summary>
        /// <param name="parameters"><see cref="ExportDataProviderNsiItemParameters"/>Список параметров для запроса.</param>
        public override void Select()
        {
            // Контрактами устанавливаются предусловия и постусловия входных значений.
            Contract.Requires( Regex.IsMatch( Parameters.task_id        , @"^\d+$"                                         ) );
            Contract.Requires( Regex.IsMatch( Parameters.connectSettings, @"^Data\sSource=\w+;User\sId=\w+;Password=\w+;$" ) );

            /// Устанавливается соединение с базой и заполняется ResultData.
            DataTable resultDataTable = GetOracleData();
            ResultData = resultDataTable;

#if DEBUG
            Debug.WriteLine( "ResultData = {0} и {1} и {2} ", ResultData.Rows[0].Field<string>( "REQUESTER_MESSAGE_GUID" ),
                                                              ResultData.Rows[0].Field<string>( "REGISTRY_NUMBER"        ),
                                                              ResultData.Rows[0].Field<string>( "MODIFIED_AFTER"         )
                                                              );

#endif
        }

        /// <summary>
        /// Запись в таблицу ST_RES_EXPORT_NSI_ITEM результата из ГИС ЖКХ.
        /// </summary>
        public override void Insert()
        {
            Contract.Requires<ArgumentException>( Regex.IsMatch( Parameters.connectSettings,      @"^Data\sSource=\w+;User\sId=\w+;Password=\w+;$" ) );
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
                // Получаем ошибку, значение из картежа и записываем в DataTable.
                ConvertTupleErrorToDataTable();
                // Вносим в БД.
                InsertGisError( exportToOracle.Rows[0], Parameters.requesterMessageGuid );
            }


        }

        #endregion

        #region PrivateMethods

        /// <summary>
        /// Возвращает таблицу данных DataTable со значениями из БД Oracle.
        /// </summary>
        /// <returns>DataTable представление данных БД.</returns>
        private DataTable GetOracleData()
        {

            Contract.Requires( Regex.IsMatch( Parameters.task_id        , @"^\d+$"                                         ) );
            Contract.Requires( Regex.IsMatch( Parameters.connectSettings, @"^Data\sSource=\w+;User\sId=\w+;Password=\w+;$" ) );



            OracleConnection conn = new OracleConnection(Parameters.connectSettings);
            conn.Open();

            OracleCommand command = conn.CreateCommand();
            command.CommandText   = "gis.expdp_nsi_item.provide_task_oracle_data";
            command.CommandType   = CommandType.StoredProcedure;
            command.BindByName    = true;

            OracleParameter parameterV_ref_cursor = new OracleParameter();
            parameterV_ref_cursor.ParameterName   = "p_ref_cursor";
            parameterV_ref_cursor.Direction       = ParameterDirection.Output;
            parameterV_ref_cursor.OracleDbType    = OracleDbType.RefCursor;

            OracleParameter parameterTask_id      = new OracleParameter();
            parameterTask_id.ParameterName        = "p_task_id";
            parameterTask_id.Direction            = ParameterDirection.Input;
            parameterTask_id.OracleDbType         = OracleDbType.Int32;
            parameterTask_id.Value                = Convert.ToInt32( Parameters.task_id );

            command.Parameters.Add( parameterV_ref_cursor );
            command.Parameters.Add( parameterTask_id      );

            OracleDataAdapter oraAdapt = new OracleDataAdapter( command );

            DataTable resultDataTable = new DataTable();

            oraAdapt.Fill( resultDataTable );

            conn.Close();

            return resultDataTable;

        }

        /// <summary>
        /// Заполняет поле <see cref="exportToOracle"/> данными кортежа.
        /// </summary>
        private void ConvertTupleToDataTable()
        {

#if DEBUG
                Notificator.Write("ExportDataProviderNsiItemModel XMLBody: " + TupleElementValue( "XMLstring" ));  
#endif
            // Получаем строковое представление XML от ГИС ЖКХ.
            var bodyXML = TupleElementValue( "XMLstring" );

            // Создаём объект класса-обработчика XML документа.
            XML.XmlDataNSI1 xmlDataNSI1 = new XML.XmlDataNSI1( bodyXML );

            // Результат обработки XML файла заносим в поле класса-модели.
            exportToOracle = xmlDataNSI1.BuildDataTable();
        }

        /// <summary>
        /// Вносит данные из шаблона <see cref="ExportNsiItemTemplate"/> в базу данных.
        /// </summary>
        private void InsertGisResult( DataRow exportToOracleRow )
        {
            OracleConnection conn = new OracleConnection( Parameters.connectSettings );
            OracleCommand command = conn.CreateCommand();

            conn.Open();

            command.CommandText = @"
                                    DECLARE
                                    v_results gis.t_str_dictionary;
                                    BEGIN
  
                                    v_results := gis.t_str_dictionary(
                                        gis.t_str_dict_item( 'requester_message_guid', :P_REQUESTER_MESSAGE_GUID ),       
                                        gis.t_str_dict_item( 'message_guid'          , :P_MESSAGE_GUID           ), 
                                        gis.t_str_dict_item( 'registry_number'       , :P_REGISTRY_NUMBER        ),
                                        gis.t_str_dict_item( 'created'               , :P_CREATED                ), 
                                        gis.t_str_dict_item( 'code'                  , :P_CODE                   ), 
                                        gis.t_str_dict_item( 'guid'                  , :P_GUID                   ),  
                                        gis.t_str_dict_item( 'modified'              , :P_MODIFIED               ), 
                                        gis.t_str_dict_item( 'is_actual'             , :P_IS_ACTUAL              ), 
                                        gis.t_str_dict_item( '-793499481'            , :P_793499481             ), 
                                        gis.t_str_dict_item( '1592027100'            , :P_1592027100             ),
                                        gis.t_str_dict_item( '-882200887'            , :P_882200887             ),    
                                        gis.t_str_dict_item( '1432260840'            , :P_1432260840             )
                                    );  

                                    gis.expdp_nsi_item.register_results( v_results );

                                    END;
                                ";

            command.BindByName = true;

            command.Parameters.Add( ":P_REQUESTER_MESSAGE_GUID", OracleDbType.Varchar2 ).Value = Parameters.requesterMessageGuid;
            command.Parameters.Add( ":P_REGISTRY_NUMBER"       , OracleDbType.Varchar2 ).Value = ResultData.Rows[0].Field<string>( "REGISTRY_NUMBER" );

            command.Parameters.Add( ":P_MESSAGE_GUID"          , OracleDbType.Varchar2 ).Value = exportToOracleRow[ "MESSAGE_GUID" ].ToString();
            command.Parameters.Add( ":P_CREATED"               , OracleDbType.Varchar2 ).Value = exportToOracleRow[ "CREATED"      ].ToString();
            command.Parameters.Add( ":P_CODE"                  , OracleDbType.Varchar2 ).Value = exportToOracleRow[ "CODE"         ].ToString();
            command.Parameters.Add( ":P_GUID"                  , OracleDbType.Varchar2 ).Value = exportToOracleRow[ "GUID"         ].ToString();
            command.Parameters.Add( ":P_MODIFIED"              , OracleDbType.Varchar2 ).Value = exportToOracleRow[ "MODIFIED"     ].ToString();
            command.Parameters.Add( ":P_IS_ACTUAL"             , OracleDbType.Varchar2 ).Value = exportToOracleRow[ "IS_ACTUAL"    ].ToString();
            command.Parameters.Add( ":P_793499481"             , OracleDbType.Varchar2 ).Value = exportToOracleRow[ "-793499481"   ].ToString();
            command.Parameters.Add( ":P_1592027100"            , OracleDbType.Varchar2 ).Value = exportToOracleRow[ "1592027100"   ].ToString();
            command.Parameters.Add( ":P_882200887"             , OracleDbType.Varchar2 ).Value = exportToOracleRow[ "-882200887"   ].ToString();
            command.Parameters.Add( ":P_1432260840"            , OracleDbType.Varchar2 ).Value = exportToOracleRow[ "1432260840"   ].ToString();

            Debug.WriteLine( "В таблицу внесено: " +
                                Parameters.requesterMessageGuid                        + " " +

                                exportToOracleRow[ "MESSAGE_GUID" ].ToString()         + " " +

                                ResultData.Rows[0].Field<string>( "REGISTRY_NUMBER" )  + " " +

                                exportToOracleRow[ "CREATED"      ].ToString()         + " " +
                                exportToOracleRow[ "CODE"         ].ToString()         + " " +
                                exportToOracleRow[ "GUID"         ].ToString()         + " " +
                                exportToOracleRow[ "MODIFIED"     ].ToString()         + " " +
                                exportToOracleRow[ "IS_ACTUAL"    ].ToString()         + " " +
                                exportToOracleRow[ "-793499481"   ].ToString()         + " " +
                                exportToOracleRow[ "1592027100"   ].ToString()         + " " +
                                exportToOracleRow[ "-882200887"   ].ToString()         + " " +
                                exportToOracleRow[ "1432260840"   ].ToString()
            );

            command.ExecuteNonQuery();

            conn.Close();
        }
#endregion
    }
}