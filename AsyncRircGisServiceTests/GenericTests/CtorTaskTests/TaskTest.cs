using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncRircGisService.TaskUnit;
using System.Diagnostics;
using System.Collections.Generic;

namespace AsyncRircGisServiceTests
{
    [TestClass]
    public class TaskTest
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        /// <summary>
        /// Возвращает объект Task.
        /// </summary>
        /// <param name="classTaskName">Имя задачи, которая будет подвергаться тестам.</param>
        /// <param name="dataPack">Параметры задачи.</param>
        /// <returns></returns>
        public OriginTask GetTaskObject( string classTaskName, DataPack dataPack )
        {
            Debug.WriteLine( classTaskName +
                             " TaskId: "        + dataPack.TaskId +
                             " ServiceId: "     + dataPack.ServiceId +
                             " MethodId: "      + dataPack.MethodId +
                             " LastStartDate: " + dataPack.LastStartDate );

            if ( classTaskName == "ExportOrgRegistryTask" ) return new ExportOrgRegistryTask( dataPack );
            else if ( classTaskName == "ExportNsiListTask" ) return new ExportNsiListTask( dataPack );
            else if ( classTaskName == "ExportNsiItemTask" ) return new ExportNsiItemTask( dataPack );
            else throw new Exception( "Класс задача не найден!" );
        }

        [DataSource( "Microsoft.VisualStudio.TestTools.DataSource.XML",
        "TaskUnit\\Generic\\CtorTaskTests\\DataPack.xml",
        "DataPack",
        DataAccessMethod.Sequential )]
        [ExpectedException( typeof( ArgumentException ), "Contract exception expected" )]
        [TestMethod]
        public void Ctor_incorrectDataPack_ContractException()
        {
            // Arrange.
            AsyncRircGisService.TaskUnit.DataPack dataPack = new AsyncRircGisService.TaskUnit.DataPack
            {
                TaskId        = testContextInstance.DataRow[ "TaskId"        ].ToString(),
                MethodId      = testContextInstance.DataRow[ "MethodId"      ].ToString(),
                ServiceId     = testContextInstance.DataRow[ "ServiceId"     ].ToString(),
                LastStartDate = testContextInstance.DataRow[ "LastStartDate" ].ToString()
            };

            // Act.
            GetTaskObject( testContextInstance.DataRow[ "Service" ].ToString(), dataPack );

            // Assert - Exception expected.
        }
    }
}
