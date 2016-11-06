using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncRircGisService.TaskUnit;
using System.Data;
using System.Diagnostics;

namespace AsyncRircGisServiceTests
{
    [TestClass]
    public class ExportOrgRegistrySubtaskTest
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }
        ExportOrgRegistrySubtask exportOrgRegistrySubtask;

        [DataSource( "Microsoft.VisualStudio.TestTools.DataSource.XML",
            "TaskUnit\\OrgRegistryCommon\\exportOrgRegistry\\ExportOrgRegisrtySubtaskDataPackTest.xml",
            "Pack",
            DataAccessMethod.Sequential )]
        [TestMethod]
        [ExpectedException( typeof( ArgumentException ), "Contract exception expected" )]
        public void Ctor__Incorrect_TaskDataPack__ContractException()
        {
            // Arrange.
            DataPack dataPack = new DataPack
            {
                TaskId        = System.Convert.ToString(testContextInstance.DataRow[ "TaskId"       ] ),
                ServiceId     = System.Convert.ToString(testContextInstance.DataRow[ "ServiceId"    ] ),
                MethodId      = System.Convert.ToString(testContextInstance.DataRow[ "MethodId"     ] ),
                LastStartDate = System.Convert.ToString(testContextInstance.DataRow[ "LastStartDate"] )
            };

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add( new DataColumn( "REQUESTER_MESSAGE_GUID", typeof( string ) ) );
            dataTable.Columns.Add( new DataColumn( "ORGKPP", typeof( string ) ) );

            DataRow dataRow = dataTable.NewRow();
            dataRow["REQUESTER_MESSAGE_GUID"] = "389EB271C619EF74E050A8C005011E2C";
            dataRow["ORGKPP"                ] = "325701001";


            // Act.
            Debug.WriteLine( "DataPack: TaskId = {0}, ServiceId = {1}, MethodId = {2}, MugkxId = {3}, DuId = {4}, LastStartDate = {5}", testContextInstance.DataRow["TaskId"].ToString(), testContextInstance.DataRow["ServiceId"].ToString(),
                                                     testContextInstance.DataRow["MethodId"].ToString(), testContextInstance.DataRow["MugkxId"].ToString(), testContextInstance.DataRow["DuId"].ToString(), testContextInstance.DataRow["LastStartDate"].ToString() );
            exportOrgRegistrySubtask = new ExportOrgRegistrySubtask( dataPack/*, ref dataTable */);

            // Assert - Expect exception.
        }
    }
}
