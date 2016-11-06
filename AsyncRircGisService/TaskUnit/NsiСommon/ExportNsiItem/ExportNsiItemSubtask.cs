using System;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using AsyncRircGisService.XML;
using System.Diagnostics;

namespace AsyncRircGisService.TaskUnit
{
    public class ExportNsiItemSubtask : Subtask
    {
        #region Ctor

        /// <summary>
        /// Конструктор заполняет данными входящей задачи от регистратора и данными подзадачи из Oracle свойства: <see cref="taskDataPack"/> и <see cref="OracleData"/>.
        /// </summary>
        /// <param name="dataPack">Данные задачи от регистратора.</param>
        /// <param name="dataTable">Срез данных подзадачи из Oracle.</param>
        public ExportNsiItemSubtask( DataPack dataPack )
        {
            #region ContractsDataPack

            Contract.Requires< ArgumentException >( Regex.IsMatch( dataPack.TaskId,    @"^\d+$" ), "TaskId in dataPack is not correct"    );
            Contract.Requires< ArgumentException >( Regex.IsMatch( dataPack.ServiceId, @"^\d+$" ), "ServiceId in dataPack is not correct" );
            Contract.Requires< ArgumentException >( Regex.IsMatch( dataPack.MethodId,  @"^\d+$" ), "MethodId in dataPack is not correct"  );

            Contract.Requires< ArgumentException >( Regex.IsMatch( dataPack.LastStartDate, @"^(\d+\.){2}\d+\s(\d+\:){2}\d+.+$" ), "LastStartDate in dataPack is not correct" );

            #endregion

            taskDataPack = dataPack;
        }

        #endregion


        #region PrivateMethods

        /// <summary>
        /// Добавляет строку xPath в gisDataPack.
        /// </summary>
        protected override void AddXPathToGisDataPack()
        {
            gisDataPack.Xpath2Values = new[] { new Tuple<string, string>( "nsi:exportNsiItemRequest/@base:version" , taskDataPack.MethodVersion ),
                                               new Tuple<string, string>( "nsi:exportNsiItemRequest",                oracleDataXMLNode          ) };
        }

        /// <summary>
        /// Добавляет строку xPath в gisResultDataPack.
        /// </summary>
        protected override void AddXPathToGisResultDataPack()
        {
            gisResultDataPack.Xpath2Values = new[] { new Tuple<string, string>( "./base:getStateRequest/base:MessageGUID", gisResultDataPack.MessageGuid ) };
        }

        /// <summary>
        /// Формирует узел XML из набора данных Oracle.
        /// </summary>
        protected override void BuildOracleDataXMLNode()
        {
            // Создание объекта для работы с XML.
            ExportNsiItemBuilder exportNsiItemBuilder = new ExportNsiItemBuilder();

            // Заносим данные подзадачи из Oracle в свойство для работы с ними в XML билдере.
            exportNsiItemBuilder.DataTable            = OracleData;

            // Формируем XML строку.
            exportNsiItemBuilder.BuildXML();

            // Полученное строковое представление XML, заносим в свойство.
            oracleDataXMLNode                         = exportNsiItemBuilder.BodyXML;
        }

        /// <summary>
        /// Расчитывает хэш сумму набора данных.
        /// </summary>
        protected override void SetHashSum()
        {
            hashSum = Convert.ToString( gisDataPack.GetHashCode() );
        }

        #endregion


        #region PublicMethods

        /// <summary>
        /// Записывает результаты работы сервиса в Oracle.
        /// </summary>
        public override void WriteGisResultsToOracle()
        {

            // Создаём объект параметров.
            Oracle.ExportNsiItemParameters exportNsiItemParameters = new Oracle.ExportNsiItemParameters
            {

                task_id              = gisResultDataPack.TaskId,
                requesterMessageGuid = GisDataPack.RequesterMessageGuid,

                insertData           = gisResultDataPack.GisResponseData,

                connectSettings      = "",
            };

            // Создаём объект модель.
            Oracle.ExportNsiItemModel exportNsiItemModel = new Oracle.ExportNsiItemModel(exportNsiItemParameters);

            // Передаём данные подзадачи в новый объект модели.
            exportNsiItemModel.ResultData = OracleData;

            // Заполняем БД данными ГИС ЖКХ.
            exportNsiItemModel.Insert();
        }

        #endregion
    }
}
