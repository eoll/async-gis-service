using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncRircGisService.TaskUnit;
using AsyncRircGisService;
using System.Diagnostics;

namespace AsyncRircGisServiceTests
{
    [TestClass]
    public class ExportOrgRegistryTaskTests
    {
        private AsyncRircGisService.TaskUnit.DataPack dataPack;
        private ExportOrgRegistryTask exportOrgRegistryTask;

        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            dataPack = new AsyncRircGisService.TaskUnit.DataPack
            {
                TaskId        = "6",
                MethodId      = "1",
                ServiceId     = "1",
                LastStartDate = "04.07.16 15:56:13,000000 +03:00",
                Attempt       = -1
            };
        }

        #region ConstructorTests

        [DataSource( "Microsoft.VisualStudio.TestTools.DataSource.XML",
    "TaskUnit\\OrgRegistryCommon\\exportOrgRegistry\\ExportOrgRegistryTaskDataPack.xml",
    "DataPack",
    DataAccessMethod.Sequential )]
        [TestMethod]
        [ExpectedException( typeof( ArgumentException ), "Contract exception expected" )]
        public void Ctor__Incorrect_DataPack__ContractException()
        {
            // Arrange.
            dataPack = new AsyncRircGisService.TaskUnit.DataPack
            {
                TaskId        = testContextInstance.DataRow[ "TaskId"       ].ToString(),
                MethodId      = testContextInstance.DataRow[ "MethodId"     ].ToString(),
                ServiceId     = testContextInstance.DataRow[ "ServiceId"    ].ToString(),
                LastStartDate = testContextInstance.DataRow[ "LastStartDate"].ToString()
            };
            // Act.
            Debug.WriteLine( "TaskId = "        + testContextInstance.DataRow["TaskId"       ].ToString() );
            Debug.WriteLine( "MethodId = "      + testContextInstance.DataRow["MethodId"     ].ToString() );
            Debug.WriteLine( "ServiceId = "     + testContextInstance.DataRow["ServiceId"    ].ToString() );
            Debug.WriteLine( "MugkxId = "       + testContextInstance.DataRow["MugkxId"      ].ToString() );
            Debug.WriteLine( "DuId = "          + testContextInstance.DataRow["DuId"         ].ToString() );
            Debug.WriteLine( "LastStartDate = " + testContextInstance.DataRow["LastStartDate"].ToString() );

            exportOrgRegistryTask = new ExportOrgRegistryTask( dataPack );

            //Assert - Expect exception.
        }
        #endregion

        [TestMethod]
        public void Prepare__Correct_DataPack__PropertyTaskOracleData_isNotEmpty()
        {
            // Arrange.
            exportOrgRegistryTask = new ExportOrgRegistryTask( dataPack );

            // Act.
            exportOrgRegistryTask.Prepare();

            // Assert.
            Debug.WriteLine( "TaskOracleData[requester_message_guid] = {0}, TaskOracleData[orgkpp] = {1}", exportOrgRegistryTask.TaskOracleData.Rows[0][0].ToString(), exportOrgRegistryTask.TaskOracleData.Rows[0][1].ToString() );
            Assert.IsNotNull( exportOrgRegistryTask.TaskOracleData.Rows[0][0], "When correctly filled dataPaсk, the property can not be empty." );
            Assert.IsNotNull( exportOrgRegistryTask.TaskOracleData.Rows[0][1], "When correctly filled dataPaсk, the property can not be empty." );
        }

        [TestMethod]
        public void Prepare__CorrectData__Fill_TaskDataPack()
        {
            // Arrange.
            exportOrgRegistryTask = new ExportOrgRegistryTask( dataPack );

            // Act.
            exportOrgRegistryTask.Prepare();

            // Assert.
            Debug.WriteLine( "TaskDataPack[Path] = " + exportOrgRegistryTask.TaskDataPack.Path );
            Debug.WriteLine( "TaskDataPack[AddOrgPpaGuid] = " + exportOrgRegistryTask.TaskDataPack.AddOrgPpaGuid );
            Debug.WriteLine( "TaskDataPack[AddSignature] = " + exportOrgRegistryTask.TaskDataPack.AddSignature );
            Debug.WriteLine( "TaskDataPack[TemplatePath] = " + exportOrgRegistryTask.TaskDataPack.TemplatePath );
            Debug.WriteLine( "TaskDataPack[Action] = " + exportOrgRegistryTask.TaskDataPack.Action );

            Assert.IsNotNull( exportOrgRegistryTask.TaskDataPack.Path, "exportOrgRegistryTask.TaskDataPack.Path is null" );
            Assert.IsNotNull( exportOrgRegistryTask.TaskDataPack.AddOrgPpaGuid, "exportOrgRegistryTask.TaskDataPack.AddOrgPpaGuid" );
            Assert.IsNotNull( exportOrgRegistryTask.TaskDataPack.AddSignature, "exportOrgRegistryTask.TaskDataPack.AddSignature" );
            Assert.IsNotNull( exportOrgRegistryTask.TaskDataPack.TemplatePath, "exportOrgRegistryTask.TaskDataPack.TemplatePath" );
            Assert.IsNotNull( exportOrgRegistryTask.TaskDataPack.Action, "exportOrgRegistryTask.TaskDataPack.Action" );
        }

        [TestMethod]
        public void Perform__CorrectData__Fill_DataPack_MessageGuid()
        {
            // Arrange.
            ServiceConfig.Conformation = new Сonformation
            {
                login                   = "lanit",
                password                = "tv,n8!Ya",
                certificateThumbprint   = "0FC21AD8F0CE100EF455185A12C5DF811057D652",
                certificatePassword     = "",
                schemaVersion           = "10.0.0.6",
                soapConfiguration       = new AsyncRircGisService.Gis.Configurations.Sections.SoapConfiguration
                {
                    SoapTemplatePath            = @"Templates\soap-template.xml",
                    RequestHeaderTemplatePath   = @"Templates\request-header-template.xml",
                    ISRequestHeaderTemplatePath = @"Templates\is-request-header-template.xml"
                },
               baseUrl = "http://127.0.0.1:8080/"

            };

            exportOrgRegistryTask = new ExportOrgRegistryTask( dataPack );

            // Act.
            exportOrgRegistryTask.Prepare();

            exportOrgRegistryTask.Perform();

            // Assert.
        }

        [TestMethod]
        [ExpectedException( typeof( Exception ), "Exception expected" )]
        public void Prepare__TaskId_6_TaskStatus_minus_1__Exception_and_update_in_gis_task_status()
        {
            // Arrange.
            exportOrgRegistryTask = new ExportOrgRegistryTask( dataPack );

            // Act.
            exportOrgRegistryTask.Prepare();

            // Assert - exception.
        }

    }
}