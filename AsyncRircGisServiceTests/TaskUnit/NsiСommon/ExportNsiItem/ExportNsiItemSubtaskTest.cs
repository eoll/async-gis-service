using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using AsyncRircGisService.TaskUnit;
using AsyncRircGisService.Gis;
using AsyncRircGisService;

namespace AsyncRircGisServiceTests
{
    [TestClass]
    public class ExportNsiItemSubtaskTest
    {
        [TestMethod]
        public void PerformGisRequest_CorrectGisDataPack_ReturnMessageGuid()
        {
            // Arrange.
            ServiceConfig.Conformation = new Сonformation
            {
                login                 = "lanit",
                password              = "tv,n8!Ya",
                certificateThumbprint = "0FC21AD8F0CE100EF455185A12C5DF811057D652",
                certificatePassword   = "",
                schemaVersion         = "10.0.1.2",
                soapConfiguration     = new AsyncRircGisService.Gis.Configurations.Sections.SoapConfiguration
                {
                    SoapTemplatePath            = @"Templates\soap-template.xml",
                    RequestHeaderTemplatePath   = @"Templates\request-header-template.xml",
                    ISRequestHeaderTemplatePath = @"Templates\is-request-header-template.xml"
                },
                baseUrl = "http://127.0.0.1:8080/"

            };

            AsyncRircGisService.TaskUnit.DataPack dataPack = new AsyncRircGisService.TaskUnit.DataPack
            {
                TaskId        = "2",
                ServiceId     = "2",
                MethodId      = "2",
                LastStartDate = "20.09.16 01:00:00,000000 +03:00"
            };

            AsyncRircGisService.Gis.DataPack gisDataPack = new AsyncRircGisService.Gis.DataPack
            {
                TemplatePath          = "Templates\\nsi-common\\exportNsiItem-template.xml",
                SchemaVersion         = ServiceConfig.Conformation.schemaVersion,
                AddOrgPPAGUID         = false,
                OrgPPAGUID            = "",
                Xpath2Values          = new[] { new Tuple<string, string>( "./base:getStateRequest/base:MessageGUID", "db11fa0b-a379-4d5e-8cb5-2f1905f3468a" ) },
                SoapConfig            = ServiceConfig.Conformation.soapConfiguration,
                CertificateThumbprint = ServiceConfig.Conformation.certificateThumbprint,
                SignPasswd            = ServiceConfig.Conformation.certificatePassword,
                AddSignature          = true,
                RequesterMessageGuid  = "5d7917f9-df95-3a0d-e050-a8c00501768a",

                Url                   = @"http://127.0.0.1:8080/ext-bus-nsi-common-service/services/NsiCommonAsync",
                Action                = "exportNsiItem",
                LoginService          = ServiceConfig.Conformation.login,
                PassService           = ServiceConfig.Conformation.password
            };

            string oracleDataXMLNode = @"<nsi:RegistryNumber>10</nsi:RegistryNumber><nsi1:ListGroup>NSI</nsi1:ListGroup>";

            gisDataPack.Xpath2Values = new[] { new Tuple<string, string>( "nsi:exportNsiItemRequest/@base:version" , ServiceConfig.Conformation.schemaVersion ),
                                               new Tuple<string, string>( "nsi:exportNsiItemRequest", oracleDataXMLNode ) };

            ExportNsiItemSubtask exportNsiItemSubtask = new ExportNsiItemSubtask(dataPack);
            exportNsiItemSubtask.GisDataPack = gisDataPack;


            // Act.
            exportNsiItemSubtask.PerformGisRequest();

            // Assert.
            Debug.WriteLine( "MessageGuid = {0}", exportNsiItemSubtask.MessageGuid);

            Assert.IsNotNull(exportNsiItemSubtask.MessageGuid);
        }

        [TestMethod]
        public void PerformGisRequestForResults_CorrectGisResultDataPack_FillTuple()
        {
            // Arrange.
            AsyncRircGisService.TaskUnit.DataPack dataPack = new AsyncRircGisService.TaskUnit.DataPack
            {
                TaskId        = "2",
                ServiceId     = "2",
                MethodId      = "2",
                LastStartDate = "20.09.16 01:00:00,000000 +03:00"
            };

            ServiceConfig.Conformation = new Сonformation
            {
                login                 = "lanit",
                password              = "tv,n8!Ya",
                certificateThumbprint = "0FC21AD8F0CE100EF455185A12C5DF811057D652",
                certificatePassword   = "",
                schemaVersion         = "10.0.1.2",
                soapConfiguration     = new AsyncRircGisService.Gis.Configurations.Sections.SoapConfiguration
                {
                    SoapTemplatePath            = @"Templates\soap-template.xml",
                    RequestHeaderTemplatePath   = @"Templates\request-header-template.xml",
                    ISRequestHeaderTemplatePath = @"Templates\is-request-header-template.xml"
                },
                baseUrl = "http://127.0.0.1:8080/"

            };

            AsyncRircGisService.Gis.DataPack gisDataPack = new AsyncRircGisService.Gis.DataPack
            {
                TemplatePath          = "Templates\\nsi-common\\getState-template.xml",
                SchemaVersion         = ServiceConfig.Conformation.schemaVersion,
                AddOrgPPAGUID         = false,
                OrgPPAGUID            = "",
                Xpath2Values          = new[] { new Tuple<string, string>( "./base:getStateRequest/base:MessageGUID", "8e867011-7e88-4abf-a0b1-4b9c7bfd3742" ) },
                SoapConfig            = ServiceConfig.Conformation.soapConfiguration,
                CertificateThumbprint = ServiceConfig.Conformation.certificateThumbprint,
                SignPasswd            = ServiceConfig.Conformation.certificatePassword,
                AddSignature          = false,
                RequesterMessageGuid  = "3c654236-79f6-ff2a-e050-a8c005015c76",

                Url                   = @"http://127.0.0.1:8080/ext-bus-nsi-common-service/services/NsiCommonAsync",
                Action                = "getState",
                LoginService          = ServiceConfig.Conformation.login,
                PassService           = ServiceConfig.Conformation.password
            };

            // Act.
            ExportNsiItemSubtask exportNsiItemSubtask = new ExportNsiItemSubtask(dataPack);
            exportNsiItemSubtask.GisResultDataPack = gisDataPack;


            // Act.
            exportNsiItemSubtask.PerformGisRequestForResults();

            // Assert.
            Assert.IsNotNull( exportNsiItemSubtask.GisResultDataPack.GisResponseData );
        }
    }
}
