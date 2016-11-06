using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncRircGisService.XML;
using System.Data;

namespace AsyncRircGisServiceTests
{
    [TestClass]
    public class ExportDataProviderNsiItemBuilderTest
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [DataSource( "Microsoft.VisualStudio.TestTools.DataSource.XML",
        "TaskUnit\\Nsi\\ExportDataProviderNsiItem\\OracleData.xml",
        "OracleData",
        DataAccessMethod.Sequential )]
        [ExpectedException( typeof( ArgumentException ), "Contract exception expected" )]
        [TestMethod]
        public void BuildXML_incorrectDataTable_ContractException()
        {
            // Arrange.

            DataTable dt = new DataTable();

            dt.Columns.Add( "requesterMessageGuid" );
            dt.Columns.Add( "registryNumber" );
            dt.Columns.Add( "modifiedAfter" );

            DataRow dr = dt.NewRow();

            dr["requesterMessageGuid"] = "3d7917f9-df95-3a0d-e050-a8c00501768a";
            dr["registryNumber"]       = TestContext.DataRow["registryNumber"].ToString();
            dr["modifiedAfter"]        = "";
            dt.Rows.Add( dr );

            ExportDataProviderNsiItemBuilder exportDataProviderNsiItemBuilder = new ExportDataProviderNsiItemBuilder();
            exportDataProviderNsiItemBuilder.DataTable = dt;

            // Act.

            exportDataProviderNsiItemBuilder.BuildXML();

            // Assert - Exception expected.
        }
        [TestMethod]
        public void BuildXML_DataTableWithoutModifiedAfter_XML()
        {
            // Arrange.

            DataTable dt = new DataTable();

            dt.Columns.Add( "requesterMessageGuid" );
            dt.Columns.Add( "REGISTRY_NUMBER" );
            dt.Columns.Add( "MODIFIED_AFTER" );

            DataRow dr = dt.NewRow();

            dr["requesterMessageGuid"] = "3d7917f9-df95-3a0d-e050-a8c00501768a";
            dr["REGISTRY_NUMBER"]       = "1";
            dr["MODIFIED_AFTER"]        = "";

            dt.Rows.Add( dr );

            string expected = "<nsi:RegistryNumber>1</nsi:RegistryNumber>";

            ExportDataProviderNsiItemBuilder exportDataProviderNsiItemBuilder = new ExportDataProviderNsiItemBuilder();
            exportDataProviderNsiItemBuilder.DataTable            = dt;

            // Act.

            exportDataProviderNsiItemBuilder.BuildXML();

            string actual = exportDataProviderNsiItemBuilder.BodyXML;

            // Assert.

            Assert.AreEqual( expected, actual );
        }
        [TestMethod]
        public void BuildXML_DataTableWithModifiedAfter_XML()
        {
            // Arrange.

            DataTable dt = new DataTable();

            dt.Columns.Add( "requesterMessageGuid" );
            dt.Columns.Add( "REGISTRY_NUMBER" );
            dt.Columns.Add( "MODIFIED_AFTER" );

            DataRow dr = dt.NewRow();

            dr["requesterMessageGuid"] = "3d7917f9-df95-3a0d-e050-a8c00501768a";
            dr["REGISTRY_NUMBER"]       = "1";
            dr["MODIFIED_AFTER"]        = "01.02.1970 9:44:10";

            dt.Rows.Add( dr );

            string expected = "<nsi:RegistryNumber>1</nsi:RegistryNumber><nsi:ModifiedAfter>01.02.1970 9:44:10</nsi:ModifiedAfter>";

            ExportDataProviderNsiItemBuilder exportDataProviderNsiItemBuilder = new ExportDataProviderNsiItemBuilder();
            exportDataProviderNsiItemBuilder.DataTable            = dt;

            // Act.

            exportDataProviderNsiItemBuilder.BuildXML();

            string actual = exportDataProviderNsiItemBuilder.BodyXML;

            // Assert.

            Assert.AreEqual( expected, actual );
        }
    }
}
