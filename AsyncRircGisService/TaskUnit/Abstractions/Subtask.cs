using System;
using System.Data;
using AsyncRircGisService.Gis;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace AsyncRircGisService.TaskUnit
{
    /// <summary>
    /// Подзадача на выполнение метода сервиса ГИС ЖКХ
    /// </summary>
    public abstract class Subtask
    {

        #region PrivateFields

        /// <summary>
        /// Данные входящей задачи от регистратора.
        /// </summary>
        protected TaskUnit.DataPack taskDataPack;



        /// <summary>
        /// Срез данных задачи (кадр), который представляет собой данные подзадачи из Oracle.
        /// </summary>
        protected DataTable oracleData;



        /// <summary>
        /// Данные Oracle в виде узла XML.
        /// </summary>
        protected string oracleDataXMLNode;



        /// <summary>
        /// Набор данных подзадачи для запроса в ГИС ЖКХ.
        /// </summary>
        protected Gis.DataPack gisDataPack;


        /// <summary>
        /// Набор данных подзадачи для запроса "Получение результата запроса в ГИС ЖКХ".
        /// </summary>
        protected Gis.DataPack gisResultDataPack;

        /// <summary>
        /// GUID ответа, по которому можно получить результат запроса.
        /// </summary>
        protected string messageGuid;


        /// <summary>
        /// Хэш сумма набора данных.
        /// </summary>
        protected string hashSum;

        #endregion

        #region PublicProperties

        /// <summary>
        /// messageGuid по которому можно получить результат выполнения запроса.
        /// </summary>
        public string MessageGuid { get { return messageGuid; } }

        /// <summary>
        /// Набор данных подзадачи для запроса в ГИС ЖКХ.
        /// </summary>
        public Gis.DataPack GisDataPack
        {
            get
            {
                return gisDataPack;
            }
            set
            {
                gisDataPack = value;
            }
        }

        public Gis.DataPack GisResultDataPack { get { return gisResultDataPack; } set { gisResultDataPack = value; } }

        /// <summary>
        /// Срез данных задачи, который представляет собой данные подзадачи из Oracle.
        /// </summary>
        public DataTable OracleData { get { return oracleData; } set { oracleData = value; } }

        #endregion

        #region PrivateMethods

        /// <summary>
        /// Вноcит данные задачи в структуру <see cref="gisDataPack"/>
        /// </summary>
        protected void AddTaskDataToGisDataPack( string requesterMessageGuid )
        {
            gisDataPack.TaskId = taskDataPack.TaskId;

            // Дозаполняем недостающими сведениями gisDataPack.
            gisDataPack.Action               = taskDataPack.Action;
            gisDataPack.OrgPPAGUID           = taskDataPack.OrgPPAGUID;
            gisDataPack.AddOrgPPAGUID        = taskDataPack.AddOrgPpaGuid;
            gisDataPack.AddSignature         = taskDataPack.AddSignature;
            gisDataPack.TemplatePath         = taskDataPack.TemplatePath;
            gisDataPack.RequesterMessageGuid = requesterMessageGuid;
        }


        /// <summary>
        /// Вноcит данные задачи в структуру <see cref="gisResultDataPack"/>
        /// </summary>
        protected void AddTaskDataToGisResultDataPack()
        {
            // Дозаполняем недостающими сведениями gisDataPack.
            gisResultDataPack.TaskId               = taskDataPack.TaskId;
            gisResultDataPack.Action               = taskDataPack.ActionResponce;
            gisResultDataPack.OrgPPAGUID           = taskDataPack.OrgPPAGUID;
            gisResultDataPack.AddOrgPPAGUID        = taskDataPack.AddOrgPpaGuid;
            gisResultDataPack.AddSignature         = taskDataPack.AddSignature;
            gisResultDataPack.TemplatePath         = taskDataPack.TemplateResponsePath;
            gisResultDataPack.RequesterMessageGuid = GetNextGuid();
            gisResultDataPack.MessageGuid          = messageGuid;
        }



        /// <summary>
        /// Вносит конфигурационные данные в структуру <see cref="gisDataPack"/>
        /// </summary>
        protected void AddConfigDataToGisDataPack()
        {
            gisDataPack = ConfigForDataPack();
        }

        /// <summary>
        /// Вносит конфигурационные данные в структуру <see cref="gisResultDataPack"/>
        /// </summary>
        protected void AddConfigDataToGisResultDataPack()
        {
            gisResultDataPack = ConfigForDataPack();
        }

        private Gis.DataPack ConfigForDataPack()
        {
            Gis.DataPack dataPackTemplate = new Gis.DataPack();

            dataPackTemplate.SchemaVersion         = taskDataPack.MethodVersion;
            dataPackTemplate.SoapConfig            = ServiceConfig.Conformation.soapConfiguration;
            dataPackTemplate.LoginService          = ServiceConfig.Conformation.login;
            dataPackTemplate.PassService           = ServiceConfig.Conformation.password;
            dataPackTemplate.CertificateThumbprint = ServiceConfig.Conformation.certificateThumbprint;
            dataPackTemplate.SignPasswd            = ServiceConfig.Conformation.certificatePassword;
            dataPackTemplate.Url                   = ServiceConfig.Conformation.baseUrl + taskDataPack.Path;

            return dataPackTemplate;
        }

        /// <summary>
        /// Формирует узел XML из набора данных Oracle.
        /// </summary>
        protected abstract void BuildOracleDataXMLNode();

        /// <summary>
        /// Формирует добавляет строку xPath в gisDataPack.
        /// </summary>
        protected abstract void AddXPathToGisDataPack();

        /// <summary>
        /// Формирует добавляет строку xPath в gisResultDataPack.
        /// </summary>
        protected abstract void AddXPathToGisResultDataPack();

        #endregion

        #region PublicMethods

        /// <summary>
        /// Заполняет значениями структуру <see cref="gisDataPack"/>.
        /// </summary>
        public void InitGisDataPack( string requesterMessageGuid )
        {
            Contract.Requires( Regex.IsMatch( requesterMessageGuid, @"^([0-9a-f]){8}-([0-9a-f]){4}-([0-9a-f]){4}-([0-9a-f]){4}-([0-9a-f]){12}$" ) );
            // Добавляем данные конфига.
            AddConfigDataToGisDataPack();

            // Добавляем данные задачи.
            AddTaskDataToGisDataPack( requesterMessageGuid );

            /// Строим XML узел из данных Oracle и добавляем в <see cref="oracleDataXMLNode"/>.
            BuildOracleDataXMLNode();

            // Добавить xPath строку.
            AddXPathToGisDataPack();

        }

        /// <summary>
        /// Заполняет значениями структуру <see cref="gisResultDataPack"/>.
        /// </summary>
        public void InitGisResultDataPack()
        {
            // Добавляем Данные конфига.
            AddConfigDataToGisResultDataPack();

            // Добавляем информацию о номере задачи.
            AddTaskDataToGisResultDataPack();

            // Добавить xPath строку.
            AddXPathToGisResultDataPack();
        }

        /// <summary>
        /// Выполняет запрос к методам сервиса ГИС ЖКХ.
        /// </summary>
        public void PerformGisRequest()
        {
            // Выполнить запрос на исполнение метода и получить MessageGiud результата
            messageGuid = Sender.RequestMethod( gisDataPack );
#if DEBUG
            Debug.WriteLine( messageGuid );
#endif
        }


        /// <summary>
        /// Выполняет запрос на получение результатов метода сервиса ГИС ЖКХ.
        /// </summary>
        public void PerformGisRequestForResults()
        {
            // Выполнить запрос на исполнение метода и получить результат запроса.
            gisResultDataPack.GisResponseData = Sender.RequestMethodResults( gisResultDataPack );

#if DEBUG
            string response = string.Empty;

            foreach ( var element in gisResultDataPack.GisResponseData )
            {
                Debug.WriteLine( element.Item1 + " = " + element.Item2 );
                response += "/n " + element.Item1 + " " + element.Item2;
            }
#endif
        }

        /// <summary>
        /// Результаты запроса в ГИС ЖКХ внести в базу данных.
        /// </summary>
        public abstract void WriteGisResultsToOracle();

        #endregion

        #region MiscPrivateMethods

        /// <summary>
        /// Расчитывает хэш сумму набора данных.
        /// </summary>
        protected abstract void SetHashSum();



        /// <summary>
        /// Создать очередной GUID.
        /// </summary>
        /// <returns>Очередной глобальный идентификатор GUID.</returns>
        protected string GetNextGuid() { return Guid.NewGuid().ToString( "D" ); }

        #endregion

    }
}

//Contract.Requires( Regex.IsMatch( gisDataPack.TaskId, @"^\d+$" ) );
//Contract.Requires( Regex.IsMatch( gisDataPack.RequesterMessageGuid, @"^\w{32}$" ) );
//Contract.Requires( Regex.IsMatch( gisDataPack.MessageGuid, @"^\w{32}$" ) );
//Contract.Requires( Regex.IsMatch( gisDataPack.LoginService, @"^\w+$" ) );
//Contract.Requires( !string.IsNullOrEmpty( gisDataPack.PassService ) );
//Contract.Requires( !string.IsNullOrEmpty( gisDataPack.TemplatePath ) );
//Contract.Requires( Regex.IsMatch( gisDataPack.SchemaVersion, @"^(\d+\.){3}\d+$" ) );
//Contract.Requires( gisDataPack.Xpath2Values != null );
//Contract.Requires( gisDataPack.SoapConfig != null );
//Contract.Requires( Regex.IsMatch( gisDataPack.CertificateThumbprint, @"^\w+$" ) );
//Contract.Requires( Regex.IsMatch( gisDataPack.SignPasswd, @"^\w+$" ) );
//Contract.Requires( !string.IsNullOrEmpty( gisDataPack.Url ) );
//Contract.Requires( !string.IsNullOrEmpty( gisDataPack.Action ) );
//Contract.Requires( gisDataPack.GisResponseData != null );