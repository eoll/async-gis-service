using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncRircGisService.Oracle;
using System.Data;

namespace AsyncRircGisServiceTests
{
    [TestClass]
    public class ExportNsiItemModelTest
    {
        private ExportNsiItemModel exportNsiItemModel;
        private ExportNsiItemParameters exportNsiItemParameters;

        [TestInitialize]
        public void TestInitialize()
        {
            string XMLString = System.IO.File.ReadAllText("TaskUnit\\NsiСommon\\ExportNsiItem\\ResultFromGis.xml");

            exportNsiItemParameters = new ExportNsiItemParameters
            {
                task_id              = "2",
                connectSettings      = string.Empty,
                requesterMessageGuid = "3d78473c-c0e9-61d3-e050-a8c005015fb0",
                insertData           = new[] { new Tuple< string, string > ( "ns13:getStateResult/ns4:MessageGUID", "3d7917f9-df95-3a0d-e050-a8c00501768a" ),
                                       new Tuple< string, string > ( "ns13:getStateResult/ns13:NsiList/ns6:Created", "2016-05-26+03:00" ),
                                       new Tuple< string, string > ( "XMLstring", XMLString )
                                   }
            };

        }

        [TestMethod]
        public void Select_CorrectDataPack_FillResultData()
        {
            // Arrange.
            exportNsiItemModel = new ExportNsiItemModel( exportNsiItemParameters );

            // Act.
            exportNsiItemModel.Select();

            // Assert.
            Assert.IsNotNull( exportNsiItemModel.ResultData.Rows[0].Field<string>( "REQUESTER_MESSAGE_GUID" ) );
            Assert.IsNotNull( exportNsiItemModel.ResultData.Rows[0].Field<string>( "LIST_GROUP"             ) );
            Assert.IsNotNull( exportNsiItemModel.ResultData.Rows[0].Field<string>( "REGISTRY_NUMBER"        ) );
        }

        [TestMethod]
        public void Insert__CorrectTuple__FillDbTable_st_res_export_nsi_Item10()
        {
            // Arrange.
            exportNsiItemModel = new ExportNsiItemModel( exportNsiItemParameters );

            DataTable table = new DataTable();
            table.Columns.Add( "REGUESTERMESSAGEGUID" );
            table.Columns.Add( "lISTGROUP"            );
            table.Columns.Add( "REGISTERNUMBER"       );

            DataRow row = table.NewRow();
            row[ "REGUESTERMESSAGEGUID" ] = "3d78473c-c0e9-61d3-e050-a8c005015fb0";
            row[ "lISTGROUP"            ] = "NSI";
            row[ "REGISTERNUMBER"       ] = "10";

            table.Rows.Add( row );

            exportNsiItemModel.ResultData = table;

            // Act.
            exportNsiItemModel.Insert();

            // Assert - NO EXCEPTION. Заполнение таблицы gis.st_res_export_nsi_item_10.
        }

        [TestMethod]
        public void Insert__CorrectTupleWithErrorCode__FillDbTable_st_res_error()
        {
            // Arrange.
            exportNsiItemParameters.insertData = new[] { new Tuple< string, string > ( "MessageGUID", "8e867011-7e88-4abf-a0b1-4b9c7bfd3742" ),
                                                         new Tuple< string, string > ( "ErrorCode", "INT016001" ),
                                                         new Tuple< string, string > ( "Description", "Справочник с реестровым номером 9999 не найден." ),
                                                       };

            exportNsiItemModel = new ExportNsiItemModel( exportNsiItemParameters );

            DataTable table = new DataTable();
            table.Columns.Add( "REGUESTERMESSAGEGUID" );
            table.Columns.Add( "lISTGROUP"            );
            table.Columns.Add( "REGISTRYNUMBER"       );

            DataRow row = table.NewRow();
            row[ "REGUESTERMESSAGEGUID" ] = "3d78473c-c0e9-61d3-e050-a8c005015fb0";
            row[ "lISTGROUP"            ] = "NSI";
            row[ "REGISTRYNUMBER"       ] = "10";

            table.Rows.Add( row );

            exportNsiItemModel.ResultData = table;

            // Act.
            exportNsiItemModel.Insert();

            // Assert - NO EXCEPTION.
        }
    }
}
