using System;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace AsyncRircGisService.TaskUnit
{
    public class ExportOrgRegistrySubtask : Subtask
    {
        #region Ctor
        /// <summary>
        /// Конструктор заполняет данными входящей задачи от регистратора и данными подзадачи из Oracle свойства: <see cref="taskDataPack"/> и <see cref="OracleData"/>.
        /// </summary>
        /// <param name="dataPack">Данные задачи от регистратора.</param>
        /// <param name="dataTable">Срез данных подзадачи из Oracle.</param>
        public ExportOrgRegistrySubtask( DataPack dataPack)
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


        #region ProtectedMethods
        /// <summary>
        /// Добавляет строку xPath в gisDataPack.
        /// </summary>
        protected override void AddXPathToGisDataPack()
        {
            gisDataPack.Xpath2Values = new[] { new Tuple<string, string>( "org:exportOrgRegistryRequest/@base:version", taskDataPack.MethodVersion       ),
                                               new Tuple<string, string>( "org:exportOrgRegistryRequest/org:SearchCriteria/org1:OGRN", oracleDataXMLNode ) };
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
            oracleDataXMLNode = OracleData.Rows[0][1].ToString();
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
            Oracle.ExportOrgRegistryParameters exportOrgRegistryParameters = new Oracle.ExportOrgRegistryParameters
            {

                connectSettings = "",

                task_id         = gisResultDataPack.TaskId,
                requesterMessageGuid = GisDataPack.RequesterMessageGuid,

                insertData      = gisResultDataPack.GisResponseData
            };

            // Создаём объект модель.
            Oracle.ExportOrgRegistryModel exportOrgRegistryModel = new Oracle.ExportOrgRegistryModel(exportOrgRegistryParameters);

            // Заполняем БД данными ГИС ЖКХ.
            exportOrgRegistryModel.Insert();
        }

        #endregion
    }
}