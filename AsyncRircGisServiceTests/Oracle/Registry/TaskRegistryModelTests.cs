using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using AsyncRircGisService.Oracle;

namespace AsyncRircGisServiceTests.Oracle.Registry
{
    [TestClass]
    public class TaskRegistryModelTests
    {
        TaskRegistryParameters taskRegistryParameters;
        TaskRegistryModel taskRegistryModel;

        [TestInitialize]
        public void TestInitialize()
        {
            taskRegistryParameters = new TaskRegistryParameters { connectSettings = "" };
        }

        [TestMethod]
        public void Select__incorrect_taskRegistryParameters__contract_exception()
        {

            // Arrange.
            bool itsOK = false;

            taskRegistryModel = new TaskRegistryModel( taskRegistryParameters );
            taskRegistryParameters.isTest = true;
            taskRegistryModel.Parameters = taskRegistryParameters;

            try
            {
                // Act.
                taskRegistryModel.Select();
            }
            catch( ArgumentException ex )
            {
                // Assert.
                itsOK = true;
                Debug.WriteLine( ex.Message );
                Assert.IsTrue( true );
            }

            Assert.IsTrue( itsOK, "Contract exception expected" );

        }

        [TestMethod]
        public void Select__correct_taskRegistryParameters__ResultData_in_not_null()
        {
            // Arrange.
            taskRegistryModel = new TaskRegistryModel( taskRegistryParameters );
            
            taskRegistryParameters.connectSettings = @"Data Source=brn;User Id=gis;Password=gis;";
            taskRegistryParameters.isTest = true;

            taskRegistryModel.Parameters = taskRegistryParameters;

            // Act.
            taskRegistryModel.Select();

            // Assert.
            Debug.WriteLine( "TASK_ID = " + taskRegistryModel.ResultData.Rows[0][0].ToString() );
            Debug.WriteLine( "MUGKX_ID = " + taskRegistryModel.ResultData.Rows[0][1].ToString() );
            Debug.WriteLine( "DU_ID = " + taskRegistryModel.ResultData.Rows[0][2].ToString() );
            Debug.WriteLine( "SERVICE_ID = " + taskRegistryModel.ResultData.Rows[0][3].ToString() );
            Debug.WriteLine( "METHOD_ID = " + taskRegistryModel.ResultData.Rows[0][4].ToString() );
            Debug.WriteLine( "LAST_START_DATE = " + taskRegistryModel.ResultData.Rows[0][5].ToString() );
            Debug.WriteLine( "PRIORITY = " + taskRegistryModel.ResultData.Rows[0][6].ToString() );
            Assert.IsNotNull( taskRegistryModel.ResultData.Rows[0][0].ToString() );
            Assert.IsNotNull( taskRegistryModel.ResultData.Rows[0][1].ToString() );
            Assert.IsNotNull( taskRegistryModel.ResultData.Rows[0][2].ToString() );
            Assert.IsNotNull( taskRegistryModel.ResultData.Rows[0][3].ToString() );
            Assert.IsNotNull( taskRegistryModel.ResultData.Rows[0][4].ToString() );
            Assert.IsNotNull( taskRegistryModel.ResultData.Rows[0][5].ToString() );
            Assert.IsNotNull( taskRegistryModel.ResultData.Rows[0][6].ToString() );
        }


    }
}
