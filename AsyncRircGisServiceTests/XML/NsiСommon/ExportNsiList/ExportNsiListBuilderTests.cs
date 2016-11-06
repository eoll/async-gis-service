using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncRircGisService.XML;
using System.Diagnostics;
using System.Data;

namespace AsyncRircGisServiceTests.XML
{
    [TestClass]
    public class ExportNsiListBuilderTests
    {
        [TestMethod]
        public void GetXMLData_CorrectXML_FillDataTable()
        {
            // Arrange.
            string xml = System.IO.File.ReadAllText(@"ExportNsiListResult.xml");

            // Act.
            ExportNsiListBuilder builder = new ExportNsiListBuilder();
            builder.XMLResponse = xml;
            builder.BuildDataTable();

            // Assert.
            foreach ( DataRow item in builder.exportToOracle.Rows )
            {
                Assert.IsTrue( !string.IsNullOrEmpty( item["REGISTRYNUMBER"].ToString() ) );
                Assert.IsTrue( !string.IsNullOrEmpty( item["NAME"].ToString() ) );
                Assert.IsTrue( !string.IsNullOrEmpty( item["MODIFIED"].ToString() ) );

                Debug.WriteLine( item["REGISTRYNUMBER"].ToString() + " " + item["NAME"].ToString() + " " + item["MODIFIED"].ToString() );
            }
        }
        [TestMethod]
        public void GetXMLData_CorrectXMLWithError_FillDataTable()
        {
            // Arrange.
            string xml = System.IO.File.ReadAllText(@"ExportNsiListResultWithError.xml");

            // Act.
            ExportNsiListBuilder builder = new ExportNsiListBuilder();
            builder.XMLResponse = xml;
            builder.BuildDataTable();

            // Assert.
            foreach ( DataRow item in builder.exportToOracle.Rows )
            {
                Assert.IsTrue( !string.IsNullOrEmpty( item["ERROR_CODE"].ToString() ) );
                Assert.IsTrue( !string.IsNullOrEmpty( item["ERROR_MESSAGE"].ToString() ) );

                Debug.WriteLine( item["ERROR_CODE"].ToString() + " " + item["ERROR_MESSAGE"].ToString() );
            }
        }
    }
}
