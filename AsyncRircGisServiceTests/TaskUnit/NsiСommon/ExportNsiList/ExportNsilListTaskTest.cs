using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncRircGisService.TaskUnit;
using System.Diagnostics;

namespace AsyncRircGisServiceTests.TaskUnit.NsiСommon.ExportNsiList
{
    [TestClass]
    public class ExportNsilListTaskTest
    {
        private AsyncRircGisService.TaskUnit.DataPack dataPack;

        [TestInitialize]
        public void TestInitialize()
        {
            dataPack = new AsyncRircGisService.TaskUnit.DataPack
            {
                TaskId        = "1",
                MethodId      = "1",
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
            ExportNsiListTask exportNsiListTask = new ExportNsiListTask( dataPack );

            // Act.
            exportNsiListTask.Prepare();

            // Assert.
            Debug.WriteLine( "TaskOracleData[requester_message_guid] = {0}, TaskOracleData[choise] = {1}", exportNsiListTask.TaskOracleData.Rows[0][0].ToString(), exportNsiListTask.TaskOracleData.Rows[0][1].ToString() );
            Assert.IsNotNull( exportNsiListTask.TaskOracleData.Rows[0][0], "When correctly filled dataPaсk, the property can not be empty." );
            Assert.IsNotNull( exportNsiListTask.TaskOracleData.Rows[0][1], "When correctly filled dataPaсk, the property can not be empty." );
        }
    }
}
