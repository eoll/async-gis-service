using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncRircGisService.Oracle;
using System.Diagnostics;
using System.Data;

namespace AsyncRircGisServiceTests
{
    [TestClass]
    public class OriginTaskModelTest
    {
        OriginTaskParameters parameters;
        OriginTaskModel originTaskModel;

        [TestInitialize]
        public void TestInitialized()
        {
            parameters = new OriginTaskParameters
            {
                taskId = "5",
                operationsSelect = OriginTaskParameters.selectOperation.ProvideTaskInfo
            };
        }
        [TestMethod]
        public void Ctor__Correct_OriginTaskParameters__Fill_Parameters()
        {
            // Arrange.
            // Act.
            originTaskModel = new OriginTaskModel( parameters );

            // Assert.
            Debug.WriteLine( "The string 'connect setting' = {0}, task_id = {1}", originTaskModel.Parameters.connectSettings, originTaskModel.Parameters.taskId );
            Assert.IsNotNull( originTaskModel.Parameters.connectSettings, "Parameter 'connect setting' is null" );
            Assert.IsNotNull( originTaskModel.Parameters.taskId, "Parameter 'taskId' is null" );
        }

        [TestMethod]
        public void Select__Correct_OriginTaskParameters__Fill_ResultData()
        {
            // Arrange.
            originTaskModel = new OriginTaskModel( parameters );


            // Act.
            originTaskModel.Select();

            var addOrgPPAGuid = originTaskModel.ResultData.Rows[0].Field<System.Int16>( "ADD_ORG_PPA_GUID" );
            var addSignature = originTaskModel.ResultData.Rows[0].Field<System.Int16>( "ADD_SIGNATURE" );

            // Assert.
            Debug.WriteLine( "originTaskModel.ResultData : " );
            Debug.WriteLine( "SERVICE_NAME = "     + originTaskModel.ResultData.Rows[0].Field<string>( "SERVICE_NAME"     ) );
            Debug.WriteLine( "PATH = "             + originTaskModel.ResultData.Rows[0].Field<string>( "PATH"             ) );
            Debug.WriteLine( "METHOD_NAME = "      + originTaskModel.ResultData.Rows[0].Field<string>( "METHOD_NAME"      ) );
            Debug.WriteLine( "TMPL_PATH = "        + originTaskModel.ResultData.Rows[0].Field<string>( "REQUEST_PATH"     ) );
            Debug.WriteLine( "ACTION = "           + originTaskModel.ResultData.Rows[0].Field<string>( "REQUEST_ACTION"   ) );
            Debug.WriteLine( "VERSION = "          + originTaskModel.ResultData.Rows[0].Field<string>( "VERSION"          ) );
            Debug.WriteLine( "REQUEST_ACTION = "   + originTaskModel.ResultData.Rows[0].Field<string>( "REQUEST_ACTION"   ) );
            Debug.WriteLine( "RESPONSE_PATH = "    + originTaskModel.ResultData.Rows[0].Field<string>( "RESPONSE_PATH"    ) );
            Debug.WriteLine( "ORG_PPAGUID = "      + originTaskModel.ResultData.Rows[0].Field<string>( "ORG_PPAGUID"      ) );

            Debug.WriteLine( "ADD_ORG_PPA_GUID = " + addOrgPPAGuid.ToString() );
            Debug.WriteLine( "ADD_SIGNATURE = "    + addSignature.ToString()  );

            Assert.IsNotNull(originTaskModel.ResultData, "originTaskModel.ResultData is null" );
        }
    }
}
