using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncRircGisService.XML;
using System.Diagnostics;
using System.Data;

namespace AsyncRircGisServiceTests.XML
{
    [TestClass]
    public class ExportNsiItemBuilderTest
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [DataSource( "Microsoft.VisualStudio.TestTools.DataSource.XML",
        "TaskUnit\\XML\\NsiCommon\\ExportNsiList\\OracleData.xml",
        "OracleData",
        DataAccessMethod.Sequential )]
        [ExpectedException( typeof( ArgumentException ), "Contract exception expected" )]
        [TestMethod]
        public void BuildXML_incorrectDataTable_ContractException()
        {
            // Arrange.

            DataTable dt = new DataTable();

            dt.Columns.Add( "requesterMessageGuid" );
            dt.Columns.Add( "LIST_GROUP"           );
            dt.Columns.Add( "REGISTRY_NUMBER"      );
            dt.Columns.Add( "MODIFIED_AFTER"       );

            DataRow dr = dt.NewRow();

            dr[ "requesterMessageGuid" ] = "3d7917f9-df95-3a0d-e050-a8c00501768a";
            dr[ "LIST_GROUP"           ] = TestContext.DataRow["listGroup"].ToString();
            dr[ "REGISTRY_NUMBER"      ] = TestContext.DataRow["registryNumber"].ToString();
            dr[ "MODIFIED_AFTER"       ] = "";

            dt.Rows.Add( dr );

            ExportNsiItemBuilder exportNsiItemBuilder = new ExportNsiItemBuilder();
            exportNsiItemBuilder.DataTable            = dt;

            // Act.

            exportNsiItemBuilder.BuildXML();

            // Assert - Exception expected.
        }
        [TestMethod]
        public void BuildXML_DataTableWithoutModifiedAfter_XML()
        {
            // Arrange.

            DataTable dt = new DataTable();

            dt.Columns.Add( "requesterMessageGuid" );
            dt.Columns.Add( "LIST_GROUP"           );
            dt.Columns.Add( "REGISTRY_NUMBER"      );
            dt.Columns.Add( "MODIFIED_AFTER"       );

            DataRow dr = dt.NewRow();

            dr[ "requesterMessageGuid" ] = "3d7917f9-df95-3a0d-e050-a8c00501768a";
            dr[ "REGISTRY_NUMBER"      ] = "10";
            dr[ "LIST_GROUP"           ] = "NSI";
            dr[ "MODIFIED_AFTER"       ] = null;

            dt.Rows.Add( dr );

            string expected = "<nsi:RegistryNumber>10</nsi:RegistryNumber><nsi1:ListGroup>NSI</nsi1:ListGroup>";

            ExportNsiItemBuilder exportNsiItemBuilder = new ExportNsiItemBuilder();
            exportNsiItemBuilder.DataTable            = dt;

            // Act.

            exportNsiItemBuilder.BuildXML();

            string actual = exportNsiItemBuilder.BodyXML;

            // Assert.

            Assert.AreEqual( expected, actual );
        }
        [TestMethod]
        public void BuildXML_DataTableWithModifiedAfter_XML()
        {
            // Arrange.

            DataTable dt = new DataTable();

            dt.Columns.Add( "requesterMessageGuid" );
            dt.Columns.Add( "LIST_GROUP"           );
            dt.Columns.Add( "REGISTRY_NUMBER"      );
            dt.Columns.Add( "MODIFIED_AFTER"       );

            DataRow dr = dt.NewRow();

            dr["requesterMessageGuid"] = "3d7917f9-df95-3a0d-e050-a8c00501768a";
            dr[ "REGISTRY_NUMBER"      ] = "10";
            dr[ "LIST_GROUP"           ] = "NSI";
            dr[ "MODIFIED_AFTER"       ] = "01.02.1970 9:44:10";

            dt.Rows.Add( dr );

            string expected = "<nsi:RegistryNumber>10</nsi:RegistryNumber><nsi1:ListGroup>NSI</nsi1:ListGroup><nsi:ModifiedAfter>01.02.1970 9:44:10</nsi:ModifiedAfter>";

            ExportNsiItemBuilder exportNsiItemBuilder = new ExportNsiItemBuilder();
            exportNsiItemBuilder.DataTable            = dt;

            // Act.

            exportNsiItemBuilder.BuildXML();

            string actual = exportNsiItemBuilder.BodyXML;

            // Assert.

            Assert.AreEqual( expected, actual );
        }
    }
}
