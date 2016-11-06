using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncRircGisService.TaskUnit;
using System.Data;
using System.Diagnostics;

namespace AsyncRircGisServiceTests
{
    [TestClass]
    public class ExportNsiListSubtaskTest
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }
        ExportNsiListSubtask exportNsiListSubtask;

        [DataSource( "Microsoft.VisualStudio.TestTools.DataSource.XML",
            "TaskUnit\\NsiСommon\\ExportNsiList\\ExportNsiListSubtaskDataPackTest.xml",
            "Pack",
            DataAccessMethod.Sequential )]
        [TestMethod]
        [ExpectedException( typeof( ArgumentException ), "Contract exception expected" )]
        public void Ctor__Incorrect_TaskDataPack__ContractException()
        {
            // Arrange.
            DataPack dataPack = new DataPack
            {
                TaskId        = System.Convert.ToString( testContextInstance.DataRow[ "TaskId"        ] ),
                ServiceId     = System.Convert.ToString( testContextInstance.DataRow[ "ServiceId"     ] ),
                MethodId      = System.Convert.ToString( testContextInstance.DataRow[ "MethodId"      ] ),
                LastStartDate = System.Convert.ToString( testContextInstance.DataRow[ "LastStartDate" ] )
            };

            // Act.
            Debug.WriteLine( "DataPack: TaskId = {0}, ServiceId = {1}, MethodId = {2}, MugkxId = {3}, DuId = {4}, LastStartDate = {5}", testContextInstance.DataRow["TaskId"].ToString(), testContextInstance.DataRow["ServiceId"].ToString(),
                                                     testContextInstance.DataRow["MethodId"].ToString(), testContextInstance.DataRow["MugkxId"].ToString(), testContextInstance.DataRow["DuId"].ToString(), testContextInstance.DataRow["LastStartDate"].ToString() );
            exportNsiListSubtask = new ExportNsiListSubtask( dataPack );

            // Assert - Expect exception.
        }
    }
}
