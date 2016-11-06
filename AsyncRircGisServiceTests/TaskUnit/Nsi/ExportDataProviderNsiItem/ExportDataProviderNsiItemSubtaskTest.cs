using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using AsyncRircGisService.TaskUnit;
using AsyncRircGisService.Gis;
using AsyncRircGisService;

namespace AsyncRircGisServiceTests.TaskUnit.Nsi.ExportDataProviderNsiItem
{
    [TestClass]
    public class ExportDataProviderNsiItemSubtaskTest
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
                TaskId        = "5",
                ServiceId     = "3",
                MethodId      = "1",
                LastStartDate = "20.09.16 01:00:00,000000 +03:00",
                MethodVersion = "10.0.1.2"
            };

            AsyncRircGisService.Gis.DataPack gisDataPack = new AsyncRircGisService.Gis.DataPack
            {
                TemplatePath          = "Templates\\nsi\\exportDataProviderNsiItem-template.xml",
                SchemaVersion         = ServiceConfig.Conformation.schemaVersion,
                AddOrgPPAGUID         = true,
                OrgPPAGUID            = "0b1c0cee-398e-48b8-90f2-7db954ea91a9",
                Xpath2Values          = new[] { new Tuple<string, string>( "./base:getStateRequest/base:MessageGUID", "8e867011-7e88-4abf-a0b1-4b9c7bfd3742" ) },
                SoapConfig            = ServiceConfig.Conformation.soapConfiguration,
                CertificateThumbprint = ServiceConfig.Conformation.certificateThumbprint,
                SignPasswd            = ServiceConfig.Conformation.certificatePassword,
                AddSignature          = true,
                RequesterMessageGuid  = "3d7920f9-df95-3a0d-e050-a8c00501768a",

                Url                   = @"http://127.0.0.1:8080/ext-bus-nsi-service/services/NsiAsync",
                Action                = "exportDataProviderNsiItem",
                LoginService          = ServiceConfig.Conformation.login,
                PassService           = ServiceConfig.Conformation.password
            };

            string oracleDataXMLNode = @"<nsi:RegistryNumber>1</nsi:RegistryNumber>";

            gisDataPack.Xpath2Values = new[] { new Tuple<string, string>( "nsi:exportDataProviderNsiItemRequest/@base:version" , dataPack.MethodVersion ),
                                               new Tuple<string, string>( "nsi:exportDataProviderNsiItemRequest", oracleDataXMLNode ) };

            ExportDataProviderNsiItemSubtask exportDataProviderNsiItemSubtask = new ExportDataProviderNsiItemSubtask(dataPack);
            exportDataProviderNsiItemSubtask.GisDataPack = gisDataPack;


            // Act.
            exportDataProviderNsiItemSubtask.PerformGisRequest();

            // Assert.
            Debug.WriteLine( "MessageGuid = {0}", exportDataProviderNsiItemSubtask.MessageGuid);

            Assert.IsNotNull(exportDataProviderNsiItemSubtask.MessageGuid);
        }

        [TestMethod]
        public void PerformGisRequestForResults_CorrectGisResultDataPack_FillTuple()
        {
            // Arrange.
            AsyncRircGisService.TaskUnit.DataPack dataPack = new AsyncRircGisService.TaskUnit.DataPack
            {
                TaskId        = "5",
                ServiceId     = "3",
                MethodId      = "1",
                LastStartDate = "20.09.16 01:00:00,000000 +03:00"
            };

            ServiceConfig.Conformation = new Сonformation
            {
                login                 = "lanit",
                password              = "tv,n8!Ya",
                certificateThumbprint = "0FC21AD8F0CE100EF455185A12C5DF811057D652",
                certificatePassword   = "",
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
                TemplatePath          = "Templates\\nsi\\getState-template.xml",
                AddOrgPPAGUID         = true,
                OrgPPAGUID            = "0b1c0cee-398e-48b8-90f2-7db954ea91a9",
                Xpath2Values          = new[] { new Tuple<string, string>( "./base:getStateRequest/base:MessageGUID", "c829bdb0-bfa0-497e-a27c-3bf98e0e99bd" ) },
                SoapConfig            = ServiceConfig.Conformation.soapConfiguration,
                CertificateThumbprint = ServiceConfig.Conformation.certificateThumbprint,
                SignPasswd            = ServiceConfig.Conformation.certificatePassword,
                AddSignature          = false,
                RequesterMessageGuid  = "3c654236-79f6-ff2a-e050-b8c005015c76",

                Url                   = @"http://127.0.0.1:8080/ext-bus-nsi-service/services/NsiAsync",
                Action                = "getState",
                LoginService          = ServiceConfig.Conformation.login,
                PassService           = ServiceConfig.Conformation.password
            };

            // Act.
            ExportDataProviderNsiItemSubtask exportDataProviderNsiItemSubtask = new ExportDataProviderNsiItemSubtask(dataPack);
            exportDataProviderNsiItemSubtask.GisResultDataPack = gisDataPack;


            // Act.
            exportDataProviderNsiItemSubtask.PerformGisRequestForResults();

            // Assert.
            Assert.IsNotNull( exportDataProviderNsiItemSubtask.GisResultDataPack.GisResponseData );
        }
    }
}
