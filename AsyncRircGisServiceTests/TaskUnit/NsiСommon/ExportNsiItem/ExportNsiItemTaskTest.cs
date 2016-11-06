using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncRircGisService.TaskUnit;
using System.Diagnostics;

namespace AsyncRircGisServiceTests
{
    [TestClass]
    public class ExportNsiItemTaskTest
    {
        private AsyncRircGisService.TaskUnit.DataPack dataPack;

        [TestInitialize]
        public void TestInitialize()
        {
            dataPack = new AsyncRircGisService.TaskUnit.DataPack
            {
                TaskId        = "2",
                MethodId      = "2",
                ServiceId     = "2",
                LastStartDate = "04.07.16 15:56:13,000000 +03:00",
                Attempt       = 0
            };
        }
        // Перед запуском данного теста необходимо выставить в БД gis.task для необходимого task_id поле status в 0. После выполнения 
        [TestMethod]
        public void Prepare__Correct_DataPack__PropertyTaskOracleData_isNotEmpty()
        {
            // Arrange.
            ExportNsiItemTask exportNsiItemTask = new ExportNsiItemTask( dataPack );

            // Act.
            exportNsiItemTask.Prepare();

            // Assert.
            Debug.WriteLine( "TaskOracleData[requester_message_guid] = {0}, TaskOracleData[list_group] = {1}, TaskOracleData[registry_number] = {2}, TaskOracleData[MODIFIED_AFTER] = {3}",
                                exportNsiItemTask.TaskOracleData.Rows[0][0].ToString(),
                                exportNsiItemTask.TaskOracleData.Rows[0][1].ToString(), 
                                exportNsiItemTask.TaskOracleData.Rows[0][2].ToString(),
                                exportNsiItemTask.TaskOracleData.Rows[0][3].ToString() );

            Assert.IsNotNull( exportNsiItemTask.TaskOracleData.Rows[0][0], "When correctly filled dataPaсk, the property can not be empty." );
            Assert.IsNotNull( exportNsiItemTask.TaskOracleData.Rows[0][1], "When correctly filled dataPaсk, the property can not be empty." );
        }
    }
}
