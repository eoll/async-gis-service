using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncRircGisService.Oracle;
using System.Data;

namespace AsyncRircGisServiceTests
{
    [TestClass]
    public class ExportDataProviderNsiItemModelTest
    {
        private ExportDataProviderNsiItemModel exportDataProviderNsiItemModel;
        private ExportDataProviderNsiItemParameters exportDataProviderNsiItemParameters;

        [TestInitialize]
        public void TestInitialize()
        {
            string XMLString = System.IO.File.ReadAllText("TaskUnit\\Nsi\\ExportDataProviderNsiItem\\ResultFromGis_NSI1.xml");

            exportDataProviderNsiItemParameters = new ExportDataProviderNsiItemParameters
            {
                task_id              = "5",
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
            exportDataProviderNsiItemModel = new ExportDataProviderNsiItemModel( exportDataProviderNsiItemParameters );

            // Act.
            exportDataProviderNsiItemModel.Select();

            // Assert.
            Assert.IsNotNull( exportDataProviderNsiItemModel.ResultData.Rows[0].Field<string>( "REQUESTER_MESSAGE_GUID" ) );
            Assert.IsNotNull( exportDataProviderNsiItemModel.ResultData.Rows[0].Field<string>( "REGISTRY_NUMBER"        ) );
        }

        [TestMethod]
        public void Insert__CorrectTuple__FillDbTable_st_res_expdp_nsi_item_1()
        {
            // Arrange.
            exportDataProviderNsiItemModel = new ExportDataProviderNsiItemModel( exportDataProviderNsiItemParameters );

            DataTable table = new DataTable();
            table.Columns.Add( "REQUESTER_MESSAGE_GUID" );
            table.Columns.Add( "REGISTRY_NUMBER"       );

            DataRow row = table.NewRow();
            row[ "REQUESTER_MESSAGE_GUID" ] = "3d78473c-c0e9-61d3-e050-a8c005015fb0";
            row[ "REGISTRY_NUMBER"      ] = "1";

            table.Rows.Add( row );

            exportDataProviderNsiItemModel.ResultData = table;

            // Act.
            exportDataProviderNsiItemModel.Insert();

            // Assert - NO EXCEPTION. Заполнение таблицы gis.st_res_export_nsi_item_10.
        }

        [TestMethod]
        public void Insert__CorrectTupleWithErrorCode__FillDbTable_st_res_error()
        {
            // Arrange.
            exportDataProviderNsiItemParameters.insertData = new[] { new Tuple< string, string > ( "MessageGUID", "8e867011-7e88-4abf-a0b1-4b9c7bfd3742" ),
                                                         new Tuple< string, string > ( "ErrorCode", "INT016001" ),
                                                         new Tuple< string, string > ( "Description", "Справочник с реестровым номером 9999 не найден." ),
                                                       };

            exportDataProviderNsiItemModel = new ExportDataProviderNsiItemModel( exportDataProviderNsiItemParameters );

            DataTable table = new DataTable();
            table.Columns.Add( "REGUESTERMESSAGEGUID" );
            table.Columns.Add( "lISTGROUP"            );
            table.Columns.Add( "REGISTRYNUMBER"       );

            DataRow row = table.NewRow();
            row[ "REGUESTERMESSAGEGUID" ] = "3d78473c-c0e9-61d3-e050-a8c005015fb0";
            row[ "lISTGROUP"            ] = "NSI";
            row[ "REGISTRYNUMBER"       ] = "10";

            table.Rows.Add( row );

            exportDataProviderNsiItemModel.ResultData = table;

            // Act.
            exportDataProviderNsiItemModel.Insert();

            // Assert - NO EXCEPTION.
        }
    }
}
