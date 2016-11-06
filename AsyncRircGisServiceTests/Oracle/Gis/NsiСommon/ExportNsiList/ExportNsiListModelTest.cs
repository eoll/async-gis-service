using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncRircGisService.Oracle;
using System.Data;

namespace AsyncRircGisServiceTests.Oracle.Gis.NsiСommon.ExportNsiList
{
    [TestClass]
    public class ExportNsiListModelTest
    {
        private ExportNsiListModel exportNsiListModel;
        private ExportNsiListParameters exportNsiListParameters;

        [TestInitialize]
        public void TestInitialize()
        {
            string XMLString = System.IO.File.ReadAllText("ExportNsiListResult.xml");

            exportNsiListParameters = new ExportNsiListParameters
            {
                task_id              = "1",
                connectSettings      = string.Empty,
                requesterMessageGuid = "3c609fbc-a7c8-c56d-e050-a8c005015074",
                insertData           = new[] { new Tuple< string, string > ( "ns13:getStateResult/ns4:MessageGUID", "db11fa0b-a379-4d5e-8cb5-2f1905f3468a" ),
                                       new Tuple< string, string > ( "ns13:getStateResult/ns13:NsiList/ns6:Created", "2016-05-26+03:00" ),
                                       new Tuple< string, string > ( "XMLstring", XMLString )
                                     }
            };

        }

        [TestMethod]
        public void Select_CorrectDataPack_FillResultData()
        {
            // Arrange.
            exportNsiListModel = new ExportNsiListModel( exportNsiListParameters );

            // Act.
            exportNsiListModel.Select();

            // Assert.
            Assert.IsNotNull( exportNsiListModel.ResultData.Rows[0][0] );
            Assert.IsNotNull( exportNsiListModel.ResultData.Rows[0][1] );
        }

        [TestMethod]
        public void Insert__XMLWithGuide__FillDbTable_st_res_export_nsi_list()
        {
            // Arrange.
            exportNsiListModel = new ExportNsiListModel( exportNsiListParameters );

            // Act.
            exportNsiListModel.Insert();

            // Assert - NO EXCEPTION.
        }

        [TestMethod]
        public void Insert__XMLWithError__FillDbTable_st_res_error()
        {
            // Arrange.
            string XMLErrString = System.IO.File.ReadAllText("ExportNsiListResultWithError.xml");

            exportNsiListParameters.insertData = new[] { new Tuple< string, string > ( "MessageGUID", "8e867011-7e88-4abf-a0b1-4b9c7bfd3742" ),
                                                         new Tuple< string, string > ( "ErrorCode", "INT016001" ),
                                                         new Tuple< string, string > ( "Description", "Справочник с реестровым номером 9999 не найден." )
            };

            exportNsiListModel = new ExportNsiListModel( exportNsiListParameters );

            DataTable table = new DataTable();
            table.Columns.Add( "REGUESTERMESSAGEGUID" );
            table.Columns.Add( "lISTGROUP"            );
            table.Columns.Add( "REGISTRYNUMBER"       );

            DataRow row = table.NewRow();
            row[ "REGUESTERMESSAGEGUID" ] = "3d78473c-c0e9-61d3-e050-a8c005015fb0";
            row[ "lISTGROUP"            ] = "NSI";
            row[ "REGISTRYNUMBER"       ] = "10";

            table.Rows.Add( row );

            exportNsiListModel.ResultData = table;

            // Act.
            exportNsiListModel.Insert();

            // Assert.
        }
    }
}
