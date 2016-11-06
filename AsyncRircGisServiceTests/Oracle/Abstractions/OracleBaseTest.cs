using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using AsyncRircGisService.Oracle;

namespace AsyncRircGisServiceTests
{
    public struct Parameters
    {
        public string mugkx_id;
        public string du_id;
        public string connectionstring;
    };

    [TestClass]
    public class OracleBaseTest: OracleBase<Parameters, string>
    {
        [TestMethod]
        public void GetConnectSettings__Mugkx_id_Incorrect_Du_id_19__ContractExceptionReturned()
        {
            // Arrange.
            bool itsOK = false;
            Parameters param = new Parameters
            {
                mugkx_id = "!!!",
                du_id = "19"
            };
            string actual;

            try
            {
                // Act.
                actual = GetConnectSettings( param.mugkx_id, param.du_id );
            }
            catch ( Exception )
            {
                // Assert.
                itsOK = true;
                Assert.IsTrue( itsOK );
            }

            Assert.IsTrue( itsOK, "Exception is not catch, mugkx_id = {0}, du_id = {1}", param.mugkx_id, param.du_id );
        }
        [TestMethod]
        [ExpectedException( typeof( AsyncRircGisService.Oracle.Exceptions.ServerCodeNotFoundException ), "Server code not found" )]
        public void GetSIDCode__Mugkx_id_1_Du_id_Incorrect__ServerCodeNotFoundExceptionReturned() {
            // Arrange.
            Parameters param = new Parameters
            {
                mugkx_id = "1",
                du_id = "399"
            };

            // Act.
            string actual = GetConnectSettings(param.mugkx_id, param.du_id);

            // Assert - Expects exception.
        }

        [TestMethod]
        public void GetConnectSettings__Mugkx_id_1_Du_id_19__CorrectConnectSettingsReturned()
        {
            // Arrange.
            Parameters param = new Parameters
            {
                mugkx_id = "1",
                du_id = "19"
            };
            string expect = @"Data Source=brn;User Id=gis;Password=gis;";

            // Act.
            string actual = GetConnectSettings(param.mugkx_id, param.du_id);

            // Assert.
            StringAssert.Contains(expect, actual, "Expect string \"{0}\" does not contain the actual \"{1}\"", expect, actual );
        }
    }
}
