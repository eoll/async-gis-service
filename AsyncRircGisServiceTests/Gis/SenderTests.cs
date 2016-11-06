using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using AsyncRircGisService.Gis;
using AsyncRircGisService;
using AsyncRircGisService.Gis.Configurations.Sections;
using System.Collections.Generic;

namespace AsyncRircGisServiceTests
{
    [TestClass]
    public class SenderTests
    {
        [TestMethod]
        public void RequestMethod_CorrectDataPack_FillTuple()
        {
            // Arrange.

            ServiceConfig.Conformation = new Сonformation
            {
                login = "lanit",
                password = "tv,n8!Ya",
                certificateThumbprint = "‎‎0FC21AD8F0CE100EF455185A12C5DF811057D652",
                certificatePassword = "",
                schemaVersion = "10.0.2.3",
                soapConfiguration = new AsyncRircGisService.Gis.Configurations.Sections.SoapConfiguration
                {
                    SoapTemplatePath = @"Templates\soap-template.xml",
                    RequestHeaderTemplatePath = @"Templates\request-header-template.xml",
                    ISRequestHeaderTemplatePath = @"Templates\is-request-header-template.xml"
                },
                baseUrl = "http://127.0.0.1:8080/"

            };

            AsyncRircGisService.Gis.DataPack dataPack = new AsyncRircGisService.Gis.DataPack
            {
                TemplatePath = @"d:/MyCode/signature-demo-net-master/AsyncRircGisService/bin/Debug/Templates/nsi-common/exportNsiList-template.xml",
                SchemaVersion = ServiceConfig.Conformation.schemaVersion,
                AddOrgPPAGUID = false,
                OrgPPAGUID = "",
                Xpath2Values = new[] { new Tuple<string, string>( "nsi:exportNsiListRequest/@base:version" , "10.0.1.2" ),
                                       new Tuple<string, string>( "nsi:exportNsiListRequest/nsi1:ListGroup", "NSI"      ) },
                SoapConfig = ServiceConfig.Conformation.soapConfiguration,
                CertificateThumbprint = ServiceConfig.Conformation.certificateThumbprint,
                SignPasswd = ServiceConfig.Conformation.certificatePassword,
                AddSignature = true,
                MessageGuid = "392C0F1675D2C91AE050A8C005012A98",
                RequesterMessageGuid = "555afdde-f9d9-475b-9fc7-e7c4573f015c",

                Url = @"http://127.0.0.1:8080/ext-bus-nsi-common-service/services/NsiCommonAsync",
                Action = "urn:exportNsiList",
                LoginService = ServiceConfig.Conformation.login,
                PassService = ServiceConfig.Conformation.password
            };

            // Act.
            var result = Sender.RequestMethod(dataPack);

            // Assert.
            Assert.IsNotNull( string.IsNullOrEmpty( result ) );

        }

        [TestMethod]
        public void RequestMethodResults_CorrectDataPack_FillTuple()
        {
            // Arrange.

            ServiceConfig.Conformation = new Сonformation
            {
                login = "lanit",
                password = "tv,n8!Ya",
                certificateThumbprint = "0FC21AD8F0CE100EF455185A12C5DF811057D652",
                certificatePassword = "",
                schemaVersion = "10.0.0.6",
                soapConfiguration = new AsyncRircGisService.Gis.Configurations.Sections.SoapConfiguration
                {
                    SoapTemplatePath = @"Templates\soap-template.xml",
                    RequestHeaderTemplatePath = @"Templates\request-header-template.xml",
                    ISRequestHeaderTemplatePath = @"Templates\is-request-header-template.xml"
                },
                baseUrl = "http://127.0.0.1:8080/"

            };

            AsyncRircGisService.Gis.DataPack dataPack = new AsyncRircGisService.Gis.DataPack
            {
                TemplatePath = @"d:/MyCode/signature-demo-net-master/AsyncRircGisService/bin/Debug/Templates/nsi-common/getState-template.xml",
                SchemaVersion = ServiceConfig.Conformation.schemaVersion,
                AddOrgPPAGUID = false,
                OrgPPAGUID = "",
                Xpath2Values = new[] { new Tuple<string, string>( "./base:getStateRequest/base:MessageGUID", "e2f11700-f33d-4522-98e5-35d7168bd0b3" ) },
                SoapConfig = ServiceConfig.Conformation.soapConfiguration,
                CertificateThumbprint = ServiceConfig.Conformation.certificateThumbprint,
                SignPasswd = ServiceConfig.Conformation.certificatePassword,
                AddSignature = false,
                MessageGuid = "e2f11700-f33d-4522-98e5-35d7168bd0b3",
                RequesterMessageGuid = "555afdde-f9d9-475b-9fc7-e7c4573f015c",

                Url = @"http://127.0.0.1:8080/ext-bus-nsi-common-service/services/NsiCommonAsync",
                Action = "getState",
                LoginService = ServiceConfig.Conformation.login,
                PassService = ServiceConfig.Conformation.password
            };

            // Act.
            var result = Sender.RequestMethodResults(dataPack);

            foreach ( var element in result )
            {
                Debug.WriteLine(element.Item1 + " = " + element.Item2);
            }

            // Assert.
            Assert.IsNotNull(result);

        }
    }
}
