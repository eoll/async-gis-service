using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using AsyncRircGisService.Oracle;
using Oracle.ManagedDataAccess.Client;

namespace AsyncRircGisServiceTests
{
    [TestClass]
    public class ExportOrgRegistryModelTests
    {
        private ExportOrgRegistryModel exportOrgRegistryModel;
        private ExportOrgRegistryParameters exportOrgRegistryParameters;
        [TestInitialize]
        public void TestInitialize()
        {
            exportOrgRegistryParameters = new ExportOrgRegistryParameters
            {
                task_id              = "6",
                connectSettings      = string.Empty,
                requesterMessageGuid = "3c609fbc-a7c8-c56d-e050-a8c005015074",
                insertData           = new[] { new Tuple< string, string > ( "ns5:getStateResult/ns4:MessageGUID", "52e5ad83-d9b9-42f1-b6ad-1a51dcd3e429" ),
                                               new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:lastEditingDate", "2016-05-26+03:00" ),
                                               new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:Legal/ns4:ShortName", "МУП 'Жилкомсервис' Беж. р-на г. Брянска" ),
                                               new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:Legal/ns4:orgRootEntityGUID", "88454500-c0da-4615-b71f-ab2cdfc43498" ),
                                               new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:Legal/ns4:orgVersionGUID", "88454500-c0da-4615-b71f-ab2cdfc43498" ),
                                               new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:Legal/ns4:IsActual", "true" ),
                                               new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:Legal/ns4:FullName", "МУП Жилкомсервис Беж. р-на г. Брянска" ),
                                               new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:Legal/ns4:OGRN", "1053266091889" ),
                                               new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:Legal/ns4:StateRegistrationDate", "2016-05-26+03:00" ),
                                               new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:Legal/ns4:INN", "3255048205" ),
                                               new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:Legal/ns4:KPP", "325701001" ),
                                               new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:Legal/ns4:OKOPF", "65243" ),
                                               new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:Legal/ns4:registryOrganizationStatus", "P" ),
                                               new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:Legal/ns4:orgPPAGUID", "0b1c0cee-398e-48b8-90f2-7db954ea91a9" ),
                                               new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:Legal/ns4:Code", "1" ),
                                               new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:Legal/ns4:GUID", "9875cc2e-73f9-41d6-bceb-47b48ed23395" ),
                                               new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:Legal/ns4:Name", "Управляющая организация" ),
                                               new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:Legal/ns4:isRegistered", "true" )
                                   }
            };

        }

        [TestMethod]
        public void Select__MugkxId_1_DuID_19__OGRN_1053266091889_Returned()
        {
            // Arrange.
            exportOrgRegistryModel = new ExportOrgRegistryModel( exportOrgRegistryParameters );
            exportOrgRegistryModel.Select();
            long expect = 1053266091889;

            // Act.
            long actual = Convert.ToInt64(exportOrgRegistryModel.ResultData.Rows[0][1]);

            // Assert.
            Debug.WriteLine( "Table return: Column['REQUESTER_MESSAGE_GUID'] = {0}, Column['OGRN'] = {1}", exportOrgRegistryModel.ResultData.Rows[0][0], exportOrgRegistryModel.ResultData.Rows[0][1] );
            Assert.AreEqual( expect, actual, "Actual OGRN {0} does not meet expected {1}", actual, expect );
        }

        [TestMethod]
        public void Select__TaskId_6__ResultData_is_not_empty()
        {
            // Arrange.
            exportOrgRegistryModel = new ExportOrgRegistryModel( exportOrgRegistryParameters );
            exportOrgRegistryParameters = new ExportOrgRegistryParameters
            {
                task_id = "6",
            };

            // Act.
            exportOrgRegistryModel.Select();

            // Assert.
            Assert.IsNotNull( exportOrgRegistryModel.ResultData.Rows[0][0] );
            Assert.IsNotNull( exportOrgRegistryModel.ResultData.Rows[0][1] );
        }

        [TestMethod]
        public void Insert__Correct_Insert_Data__Fill_ST_RES_EXPORT_ORG_REGISTRY()
        {
            // Arrange.
            exportOrgRegistryModel = new ExportOrgRegistryModel( exportOrgRegistryParameters );

            // Act.
            exportOrgRegistryModel.Insert();

            // Assert - Insert без ошибок.
        }

        [TestMethod]
        public void Insert__Correct_Insert_DataWithError__Fill_ST_RES_ERROR()
        {
            // Arrange.
            exportOrgRegistryParameters.insertData = new[] { new Tuple< string, string > ( "ns5:getStateResult/ns4:MessageGUID", "52e5ad83-d9b9-42f1-b6ad-1a51dcd3e429" ),
                                     new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:ErrorCode", "INT016001" ),
                                     new Tuple< string, string > ( "ns5:getStateResult/ns5:exportOrgRegistryResult/ns5:OrgVersion/ns5:Legal/ns4:Description", "Справочник с реестровым номером 888 не найден." ),
                                   };

            exportOrgRegistryModel = new ExportOrgRegistryModel( exportOrgRegistryParameters );

            // Act.
            exportOrgRegistryModel.Insert();

            // Assert - Insert без ошибок.
        }
    }
}
