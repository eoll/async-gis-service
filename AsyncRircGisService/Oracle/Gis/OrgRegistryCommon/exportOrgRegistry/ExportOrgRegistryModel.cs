using System;
using System.Data;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;
using System.Diagnostics;
using AsyncRircGisService.Exceptions;

namespace AsyncRircGisService.Oracle

{
    public class ExportOrgRegistryModel : OracleBase<ExportOrgRegistryParameters, DataTable>
    {
        private ExportOrgRegistryTemplate exportOrgRegistryTemplate;

        #region Constructor

        /// <summary>
        /// В конструкторе передаются параметры для подключения к oracle и получения данных для задачи.
        /// </summary>
        /// <param name="parameters"><see cref="ExportOrgRegistryParameters"/>Список параметров для запроса.</param>
        public ExportOrgRegistryModel( ExportOrgRegistryParameters parameters )
        {

            // Записываются параметры соединения с БД.
            parameters.connectSettings = GetConnectSettings( );
            /// Параметры добавляются в публичное свойство.
            Parameters = parameters;
        }

        #endregion

        #region PublicMethods

        /// <summary>
        /// Выборка из БД ОГРН организации и запись в <see cref="ResultData"/>/.
        /// </summary>
        /// <param name="parameters"><see cref="ExportOrgRegistryParameters"/>Список параметров для запроса.</param>
        public override void Select()
        {
            // Контрактами устанавливаются предусловия и постусловия входных значений.
            Contract.Requires( Regex.IsMatch( Parameters.task_id, @"^\d+$" ) );
            Contract.Requires( Regex.IsMatch( Parameters.connectSettings, @"^Data\sSource=\w+;User\sId=\w+;Password=\w+;$" ) );

            /// Устанавливается соединение с базой и заполняется <see cref="ResultData"/>.
            DataTable resultDataTable = GetOracleData();
            ResultData = resultDataTable;

#if DEBUG
            Debug.WriteLine( "ResultData = {0} и {1}", ResultData.Rows[0][0], ResultData.Rows[0][1] );
#endif
        }

        /// <summary>
        /// Запись в таблицу st_res_export_org_registry результата из ГИС ЖКХ.
        /// </summary>
        public override void Insert()
        {
            Contract.Requires( Regex.IsMatch( Parameters.connectSettings, @"^Data\sSource=\w+;User\sId=\w+;Password=\w+;$" ) );

            // Передаём кортеж в поле базового класса для работы метода ConvertTupleErrorToDataTable.
            TupleValue = Parameters.insertData;

            // Получаем ошибку из кортежа.
            string error = TupleElementValue( "ErrorCode" );

            // Если переменная error пустая, то вызываем метод который из кортежа дастаёт xml и преобразует в DataTable.
            // Иначе получаем из кортежа ошибку, описание и без разбора XML формируем DataTable.
            if ( string.IsNullOrEmpty( error ) )
            {
                // Заполняем шаблон данными из ГИС ЖКХ.
                SetTemplate();

                // Вносит данные из шаблона < see cref = "exportOrgRegistryTemplate" /> в базу данных.
                InsertGisResult();
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
        /// <param name="parameters"><see cref="ExportOrgRegistryParameters"/>Список параметров для запроса.</param>
        /// <returns>DataTable представление данных БД.</returns>
        private DataTable GetOracleData()
        {
            // Контракты кода.
            Contract.Requires( Regex.IsMatch( Parameters.task_id, @"^\d+$" ) );
            Contract.Requires( Regex.IsMatch( Parameters.connectSettings, @"^Data\sSource=\w+;User\sId=\w+;Password=\w+;$" ) );

            OracleConnection conn = new OracleConnection(Parameters.connectSettings);
            conn.Open();
            OracleCommand command = conn.CreateCommand();
            command.CommandText = "gis.export_org_registry.provide_task_oracle_data";
            command.CommandType = CommandType.StoredProcedure;

            OracleParameter parameterV_ref_cursor = new OracleParameter();
            {
                parameterV_ref_cursor.ParameterName = "p_ref_cursor";
                parameterV_ref_cursor.Direction = ParameterDirection.Output;
                parameterV_ref_cursor.OracleDbType = OracleDbType.RefCursor;
            }
            OracleParameter parameterTask_id = new OracleParameter();
            {
                parameterTask_id.ParameterName = "p_task_id";
                parameterTask_id.Direction = ParameterDirection.Input;
                parameterTask_id.OracleDbType = OracleDbType.Int32;
                parameterTask_id.Value = Convert.ToInt32( Parameters.task_id );
            }
            command.BindByName = true;
            command.Parameters.Add( parameterV_ref_cursor );
            command.Parameters.Add( parameterTask_id );

            OracleDataAdapter oraAdapt = new OracleDataAdapter(command);

            DataTable resultDataTable = new DataTable();

            oraAdapt.Fill( resultDataTable );

            conn.Close();

            return resultDataTable;
        }

        /// <summary>
        /// Заполняет объект <see cref="ExportOrgRegistryTemplate"/> данными кордежа.
        /// </summary>
        /// <param name="insertData">Кортеж с данными из ГИС ЖКХ.</param>
        /// <returns></returns>
        private void SetTemplate()
        {
            exportOrgRegistryTemplate = new ExportOrgRegistryTemplate();

            exportOrgRegistryTemplate.requesterMessageGuid = Parameters.requesterMessageGuid;

            foreach ( var element in Parameters.insertData )
            {
                // Получаем из Xpath строки имя ноды.
                var NodeName = element.Item1.Substring( element.Item1.LastIndexOf( ':' ) + 1 );

                switch ( NodeName )
                {
                    case "RequestState":
                        exportOrgRegistryTemplate.requestState = element.Item2;
                        break;
                    case "MessageGUID":
                        exportOrgRegistryTemplate.messageGuid = element.Item2;
                        break;
                    case "orgRootEntityGUID":
                        exportOrgRegistryTemplate.orgRootEntityGuid = element.Item2;
                        break;
                    case "orgVersionGUID":
                        exportOrgRegistryTemplate.orgVersionGuid = element.Item2;
                        break;
                    case "lastEditingDate":
                        exportOrgRegistryTemplate.lastEditingDate = element.Item2.Remove( element.Item2.IndexOf( '+' ) );
                        break;
                    case "IsActual":
                        exportOrgRegistryTemplate.isActual = element.Item2 == "true" ? 1 : 0;
                        break;
                    case "ShortName":
                        exportOrgRegistryTemplate.shortName = element.Item2;
                        break;
                    case "FullName":
                        exportOrgRegistryTemplate.fullName = element.Item2;
                        break;
                    case "OGRN":
                        exportOrgRegistryTemplate.ogrn = Convert.ToInt64( element.Item2 );
                        break;
                    case "StateRegistrationDate":
                        exportOrgRegistryTemplate.stateRegistrationDate = element.Item2.Remove( element.Item2.IndexOf( '+' ) );
                        break;
                    case "INN":
                        exportOrgRegistryTemplate.inn = Convert.ToInt64( element.Item2 );
                        break;
                    case "KPP":
                        exportOrgRegistryTemplate.kpp = Convert.ToInt64( element.Item2 );
                        break;
                    case "OKOPF":
                        exportOrgRegistryTemplate.okopf = Convert.ToInt64( element.Item2 );
                        break;
                    case "registryOrganizationStatus":
                        exportOrgRegistryTemplate.organizationStatus = element.Item2;
                        break;
                    case "orgPPAGUID":
                        exportOrgRegistryTemplate.orgPpaguid = element.Item2;
                        break;
                    case "Code":
                        exportOrgRegistryTemplate.code = element.Item2;
                        break;
                    case "GUID":
                        exportOrgRegistryTemplate.guid = element.Item2;
                        break;
                    case "Name":
                        exportOrgRegistryTemplate.name = element.Item2;
                        break;
                    case "isRegistered":
                        exportOrgRegistryTemplate.isRegistered = element.Item2 == "true" ? 1 : 0;
                        break;
                    case "ErrorCode":
                        exportOrgRegistryTemplate.errorCode = element.Item2;
                        break;
                    case "Description":
                        exportOrgRegistryTemplate.errorMessage = element.Item2;
                        break;
                        // Тут хранится XML тело ответа, его не обрабатываем в данном методе.
                    case "XMLstring":
                        // Ни делать ничего.
                        ;
                        break;
                    default:
                        throw new NotFoundException( element.Item1 + " в операторе case не найден." );
                }
            }
        }

        /// <summary>
        /// Вносит данные из шаблона <see cref="exportOrgRegistryTemplate"/> в базу данных.
        /// </summary>
        private void InsertGisResult()
        {
            OracleConnection conn = new OracleConnection(Parameters.connectSettings);
            OracleCommand command = conn.CreateCommand();

            conn.Open();

            command.CommandText = @"
                                    DECLARE
                                      v_results gis.st_res_export_org_registry%ROWTYPE;
                                    BEGIN
  
                                      v_results.REQUESTER_MESSAGE_GUID := :P_REQUESTER_MESSAGE_GUID;
                                      v_results.MESSAGE_GUID           := :P_MESSAGE_GUID;
                                      v_results.ORG_ROOT_ENTITY_GUID   := :P_ORG_ROOT_ENTITY_GUID;
                                      v_results.ORG_VERSION_GUID       := :P_ORG_VERSION_GUID;
                                      v_results.LAST_EDITING_DATE      := :P_LAST_EDITING_DATE;
                                      v_results.IS_ACTUAL              := :P_IS_ACTUAL;
                                      v_results.SHORT_NAME             := :P_SHORT_NAME;
                                      v_results.FULL_NAME              := :P_FULL_NAME;
                                      v_results.OGRN                   := :P_OGRN;
                                      v_results.STATE_REGISTRATION_DATE:= :P_STATE_REGISTRATION_DATE;
                                      v_results.INN                    := :P_INN;
                                      v_results.KPP                    := :P_KPP;
                                      v_results.OKOPF                  := :P_OKOPF;
                                      v_results.ORGANIZATION_STATUS    := :P_ORGANIZATION_STATUS;
                                      v_results.ORG_PPAGUID            := :P_ORG_PPAGUID;
                                      v_results.CODE                   := :P_CODE;
                                      v_results.GUID                   := :P_GUID;
                                      v_results.NAME                   := :P_NAME;
                                      v_results.IS_REGISTERED          := :P_IS_REGISTERED;          
    
                                      gis.export_org_registry.register_results( p_results => v_results );
      
                                    END;
                                    ";

            command.BindByName = true;

            command.Parameters.Add( ":P_REQUESTER_MESSAGE_GUID", OracleDbType.Varchar2 ).Value = exportOrgRegistryTemplate.requesterMessageGuid;
            command.Parameters.Add( ":P_MESSAGE_GUID", OracleDbType.Varchar2 ).Value           = exportOrgRegistryTemplate.messageGuid;
            command.Parameters.Add( ":P_ORG_ROOT_ENTITY_GUID", OracleDbType.Varchar2 ).Value   = exportOrgRegistryTemplate.orgRootEntityGuid;
            command.Parameters.Add( ":P_ORG_VERSION_GUID", OracleDbType.Varchar2 ).Value       = exportOrgRegistryTemplate.orgVersionGuid;
            command.Parameters.Add( ":P_LAST_EDITING_DATE", OracleDbType.Date ).Value          = Convert.ToDateTime( exportOrgRegistryTemplate.lastEditingDate );
            command.Parameters.Add( ":P_IS_ACTUAL", OracleDbType.Int32 ).Value                 = exportOrgRegistryTemplate.isActual;
            command.Parameters.Add( ":P_SHORT_NAME", OracleDbType.Varchar2 ).Value             = exportOrgRegistryTemplate.shortName;
            command.Parameters.Add( ":P_FULL_NAME", OracleDbType.Varchar2 ).Value              = exportOrgRegistryTemplate.fullName;
            command.Parameters.Add( ":P_OGRN", OracleDbType.Int32 ).Value                      = exportOrgRegistryTemplate.ogrn;
            command.Parameters.Add( ":P_STATE_REGISTRATION_DATE", OracleDbType.Date ).Value    = Convert.ToDateTime( exportOrgRegistryTemplate.stateRegistrationDate );
            command.Parameters.Add( ":P_INN", OracleDbType.Int32 ).Value                       = exportOrgRegistryTemplate.inn;
            command.Parameters.Add( ":P_KPP", OracleDbType.Int32 ).Value                       = exportOrgRegistryTemplate.kpp;
            command.Parameters.Add( ":P_OKOPF", OracleDbType.Int32 ).Value                     = exportOrgRegistryTemplate.okopf;
            command.Parameters.Add( ":P_ORGANIZATION_STATUS", OracleDbType.Varchar2 ).Value    = exportOrgRegistryTemplate.organizationStatus;
            command.Parameters.Add( ":P_ORG_PPAGUID", OracleDbType.Varchar2 ).Value            = exportOrgRegistryTemplate.orgPpaguid;
            command.Parameters.Add( ":P_CODE", OracleDbType.Varchar2 ).Value                   = exportOrgRegistryTemplate.code;
            command.Parameters.Add( ":P_GUID", OracleDbType.Varchar2 ).Value                   = exportOrgRegistryTemplate.guid;
            command.Parameters.Add( ":P_NAME", OracleDbType.Varchar2 ).Value                   = exportOrgRegistryTemplate.name;
            command.Parameters.Add( ":P_IS_REGISTERED", OracleDbType.Int32 ).Value             = exportOrgRegistryTemplate.isRegistered;

            command.ExecuteNonQuery();

            conn.Close();
        }

        #endregion
    }
}