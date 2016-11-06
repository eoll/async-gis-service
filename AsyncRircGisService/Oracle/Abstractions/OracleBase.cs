using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;
using System;

namespace AsyncRircGisService.Oracle
{

    /// <summary>
    /// Базовые операции с базой данных Oracle
    /// </summary>
    /// <typeparam name="T">Параметры операции</typeparam>
    /// <typeparam name="U">Результат работы операции - таблица, список или одиночное значение запроса.</typeparam>
    public class OracleBase<T, U> where T : struct
    {

        #region PublicFields

        /// <summary>
        /// Возвращаемые данные
        /// </summary>
        public U ResultData { get; set; }

        #endregion

        #region PrivateFields

        /// <summary>
        /// Параметры для выполнения запроса к базе данных Oracle.
        /// </summary>
        protected T parameters;

        /// <summary>
        /// В данное поле помещается кортеж из ГИС ЖКХ.
        /// </summary>
        private IEnumerable< Tuple< string, string > > tupleValue;


        /// <summary>
        /// Свойство для записи 
        /// </summary>
        protected IEnumerable<Tuple<string, string>> TupleValue { set { tupleValue = value; } }

        protected Dictionary<int, string> serverSidCode = new Dictionary<int, string>();

        // В данное поле помещаются преобразованные данные из XML - ответе из ГИС ЖКХ.
        protected DataTable exportToOracle;

        #endregion

        #region PublicProperties

        /// <summary>
        /// Параметры для выполнения запроса к базе данных Oracle.
        /// </summary>
        public T Parameters { get; set; }

        #endregion

        #region PublicVirtualMethods

        public virtual void Select() { }
        public virtual void Update() { }
        public virtual void Insert() { }
        public virtual void Delete() { }
        public virtual void RunProgram() { }

        #endregion

        #region PrivateMethods

        /// <summary>
        /// Формирует настройки подключения к БД.
        /// </summary>
        /// <returns>Строка с настройками подключения к Oracle.</returns>
        protected string GetConnectSettings( string mugkx_id, string du_id )
        {
            Contract.Requires( Regex.IsMatch( mugkx_id, @"^\d+$" ) );
            Contract.Requires( Regex.IsMatch( du_id, @"^\d+$" ) );
            Contract.Ensures( Regex.IsMatch( Contract.Result<string>(), @"^Data\sSource=\w+;User\sId=\w+;Password=\w+;$" ) );

            /// Заполняем словарь значениями <see cref="InitServerSidCodes"/>.
            InitServerSidCodes();

            // Определяем код сервера ДУ.
            int sid = GetSIDCode( mugkx_id, du_id );

            // Расшифровываем код, получаем sid.
            string dataSource = serverSidCode[sid];

            // Формируем строку connectSettings.
            string connectSettings = @"Data Source=" + dataSource + ";User Id=***;Password=***;";

            // Возврат строки connectSettings.
            return connectSettings;
        }



        /// <summary>
        /// Формирует настройку подключения к БД brn.
        /// </summary>
        /// <returns>Строка с настройками подключения к Oracle.</returns>
        protected string GetConnectSettings()
        {
            Contract.Ensures( Regex.IsMatch( Contract.Result<string>(), @"^Data\sSource=\w+;User\sId=\w+;Password=\w+;$" ) );

            // Формируем строку connectSettings.
            string connectSettings = @"Data Source=brn;User Id=***;Password=***;";

            // Возврат строки connectSettings.
            return connectSettings;
        }



        /// <summary>
        /// Возвращает sid базы данных.
        /// </summary>
        /// <exception cref="Exceptions.ServerCodeNotFoundException">Server code not found</exception>
        /// <returns>Числовое представление кода сервера</returns>
        private static int GetSIDCode( string mugkx_id, string du_id )
        {
            Contract.Requires( Regex.IsMatch( mugkx_id, @"^\d+$" ) );
            Contract.Requires( Regex.IsMatch( du_id, @"^\d+$" ) );
            Contract.Ensures( Contract.Result<int>() > 0 && Contract.Result<int>() < 15 );

            OracleConnection conn = new OracleConnection("Data Source=brn;User Id=***;Password=***;");
            conn.Open();
            OracleCommand command = conn.CreateCommand();
            command.CommandText = "gis.task_manager.server_id";
            command.CommandType = CommandType.StoredProcedure;

            OracleParameter parameterV_server_id = new OracleParameter();
            {
                parameterV_server_id.ParameterName = "v_server_id";
                parameterV_server_id.Direction = ParameterDirection.ReturnValue;
                parameterV_server_id.OracleDbType = OracleDbType.Int32;
            }
            OracleParameter parameterMugkx_id = new OracleParameter();
            {
                parameterMugkx_id.ParameterName = "p_mugkx_id";
                parameterMugkx_id.Direction = ParameterDirection.Input;
                parameterMugkx_id.OracleDbType = OracleDbType.Int32;
                parameterMugkx_id.Value = Convert.ToInt32( mugkx_id );
            }
            OracleParameter parameterDu_id = new OracleParameter();
            {
                parameterDu_id.ParameterName = "p_du_id";
                parameterDu_id.Direction = ParameterDirection.Input;
                parameterDu_id.OracleDbType = OracleDbType.Int32;
                parameterDu_id.Value = Convert.ToInt32( du_id );
            }
            command.BindByName = true;
            command.Parameters.Add( parameterV_server_id );
            command.Parameters.Add( parameterMugkx_id );
            command.Parameters.Add( parameterDu_id );

            command.ExecuteNonQuery();

            int sid = Convert.ToInt32(command.Parameters["v_server_id"].Value.ToString());

            conn.Close();

            if( sid == 0 )
            {
                throw new Exceptions.ServerCodeNotFoundException( "Server code not found" );
            }
            else
            {
                return sid;
            }
        }



        /// <summary>
        /// Заполняет словарь значениями типа {код, sid}.
        /// </summary>
        private void InitServerSidCodes()
        {
            serverSidCode.Add( 1, "brn" );
            serverSidCode.Add( 2, "pch" );
            serverSidCode.Add( 3, "nvz" );
            serverSidCode.Add( 4, "bra" );
            serverSidCode.Add( 5, "unc" );
            serverSidCode.Add( 6, "pgr" );
            serverSidCode.Add( 7, "dub" );
            serverSidCode.Add( 8, "tru" );
            serverSidCode.Add( 9, "dtk" );
            serverSidCode.Add( 10, "sev" );
            serverSidCode.Add( 11, "kar" );
            serverSidCode.Add( 15, "str" );
        }

        /// <summary>
        /// Выбирает из кортежа записи со значениями ошибок, и заполняет поле <see cref="exportToOracle"/>.
        /// </summary>
        protected void ConvertTupleErrorToDataTable()
        {
            string error          = TupleElementValue( "ErrorCode"   );
            string errDescription = TupleElementValue( "Description" );

            DataTable dt = new DataTable();

            dt.Columns.Add( "ERROR_CODE"    );
            dt.Columns.Add( "ERROR_MESSAGE" );

            DataRow dr = dt.NewRow();

            dr[ "ERROR_CODE"    ] = error;
            dr[ "ERROR_MESSAGE" ] = errDescription;

            dt.Rows.Add( dr );

            exportToOracle = dt;
        }

        /// <summary>
        /// Получает значение ключа кортежа
        /// </summary>
        /// <param name="key">Ключ в кортеже.</param>
        protected string TupleElementValue( string key )
        {

            string value = "";

            // Перебираем кортеж для поиска XML.
            foreach ( var element in tupleValue )
            {
                var NodeName = element.Item1.Substring( element.Item1.LastIndexOf( ':' ) + 1 );
                if ( NodeName == key ) { value = element.Item2; break; }
            }

            return value;

        }

        /// <summary>
        /// Вносит код ошибки из ГИС и её описание в таблицу БД.
        /// </summary>
        /// <param name="dataRow">Строка, содержащая код ошибки и её описание.</param>
        /// <param name="requesterMessageGuid">Идентификатор сообщения, присвоенный РИРЦ.</param>
        protected void InsertGisError( DataRow dataRow, string requesterMessageGuid )
        {
            OracleConnection conn = new OracleConnection( "Data Source=brn;User Id=***;Password=***;" );
            OracleCommand command = conn.CreateCommand();

            conn.Open();

            command.CommandText = @"
                                    DECLARE
                                      v_error gis.st_res_error%ROWTYPE;
                                    BEGIN
  
                                      v_error.REQUESTER_MESSAGE_GUID := :P_REQUESTER_MESSAGE_GUID;
                                      v_error.ERROR_CODE             := :P_ERROR_CODE;
                                      v_error.ERROR_MESSAGE          := :P_ERROR_MESSAGE;              
    
                                      gis.err_manager.insert_gis_error( p_error => v_error );
      
                                    END;
                                ";

            command.BindByName = true;

            command.Parameters.Add( ":P_REQUESTER_MESSAGE_GUID", OracleDbType.Varchar2 ).Value = requesterMessageGuid;
            command.Parameters.Add( ":P_ERROR_CODE"            , OracleDbType.Varchar2 ).Value = dataRow[ "ERROR_CODE"    ].ToString();
            command.Parameters.Add( ":P_ERROR_MESSAGE"         , OracleDbType.Varchar2 ).Value = dataRow[ "ERROR_MESSAGE" ].ToString();

            Debug.WriteLine( "В таблицу внесено: " +
                                requesterMessageGuid                 + " " +
                                dataRow["ERROR_CODE"    ].ToString() + " " +
                                dataRow["ERROR_MESSAGE" ].ToString()
            );

            command.ExecuteNonQuery();

            conn.Close();
        }


        #endregion

    }

}
