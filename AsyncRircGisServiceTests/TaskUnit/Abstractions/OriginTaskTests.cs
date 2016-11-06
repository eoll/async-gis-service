using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncRircGisService.TaskUnit;
using System.Data;
using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;
using System.Collections;

namespace AsyncRircGisServiceTests.TaskUnit.Abstractions
{
    [TestClass]
    public class OriginTaskTests : OriginTask
    {
        protected override void ProvideTaskOracleData()
        {
            throw new NotImplementedException();
        }

        // Для запуска теста необходимо в классе Notificator закоментировать 46 строку.
        [TestMethod]
        public void ProvideTaskInfo__TaskID_5__Fill_TaskDataPack()
        {
            // Arrange.
            DataPack dataPack = new DataPack { TaskId = "5" };
            TaskDataPack = dataPack;
            // Act.
            ProvideTaskInfo();

            Debug.WriteLine("PATH: "               + TaskDataPack.Path                     + "\n " + 
                              "REQUEST_PATH: "     + TaskDataPack.TemplatePath             + "\n " +
                              "REQUEST_ACTION: "   + TaskDataPack.Action                   + "\n " +
                              "RESPONSE_PATH: "    + TaskDataPack.TemplateResponsePath     + "\n " +
                              "RESPONSE_ACTION: "  + TaskDataPack.ActionResponce           + "\n " +
                              "VERSION: "          + TaskDataPack.MethodVersion            + "\n " +
                              "ORG_PPAGUID: "      + TaskDataPack.OrgPPAGUID               + "\n " +
                              "ADD_ORG_PPA_GUID: " + TaskDataPack.AddOrgPpaGuid.ToString() + "\n " +
                              "ADD_SIGNATURE: "    + TaskDataPack.AddSignature.ToString());

            // Assert.
            Assert.IsNotNull( TaskDataPack.Action               );
            Assert.IsNotNull( TaskDataPack.ActionResponce       );
            Assert.IsNotNull( TaskDataPack.AddOrgPpaGuid        );
            Assert.IsNotNull( TaskDataPack.AddSignature         );
            Assert.IsNotNull( TaskDataPack.Attempt              );
            Assert.IsNotNull( TaskDataPack.OrgPPAGUID           );
            Assert.IsNotNull( TaskDataPack.Path                 );
            Assert.IsNotNull( TaskDataPack.TemplatePath         );
            Assert.IsNotNull( TaskDataPack.TemplateResponsePath );
        }

        [TestMethod]
        public void Check__TaskId_6__Get_result_from_DB()
        {
            // Arrange.
            DataPack dataPack = new DataPack { TaskId = "6", LastStartDate = @"04.07.16 15:56:13,000000 +03:00" };
            TaskDataPack = dataPack;
            string expected = "3";

            // Act.
            Check();

            // Assert.
            Debug.WriteLine( "taskStatus = " + taskStatusField.ToString() );
            Assert.AreEqual( expected, taskStatusField.ToString() );
        }

        [TestMethod]
        public void Complete__TaskId_6_TaskStatus_3__Update_in_gis_task__status()
        {
            // Arrange.
            DataPack dataPack = new DataPack { TaskId = "6" };
            TaskDataPack = dataPack;
            taskStatusField = 3;

            // Act.
            Complete();
            ArrayList list = Select(Convert.ToInt32(TaskDataPack.TaskId), taskStatusField);

            // Assert.
            Assert.AreEqual(Convert.ToInt32(list[8]), taskStatusField );
        }

        public static ArrayList Select( int task_id, int taskStatus )
        {

            OracleConnection conn = new OracleConnection("Data Source=brn;User Id=***;Password=***;");
            conn.Open();
            OracleCommand command = conn.CreateCommand();
            command.CommandText = @"SELECT * FROM gis.task
                                    WHERE task_id = :p_task_id
                                    AND   status  = :p_status";

            OracleParameter parametertaskId = new OracleParameter();
            {
                #region SQL Parameter
                parametertaskId.ParameterName = ":p_task_id";
                parametertaskId.Direction = ParameterDirection.Input;
                parametertaskId.OracleDbType = OracleDbType.Int16;
                parametertaskId.Value = task_id;
                #endregion
            }
            OracleParameter parameterStatus = new OracleParameter();
            {
                #region SQL Parameter
                parameterStatus.ParameterName = ":p_status";
                parameterStatus.Direction = ParameterDirection.Input;
                parameterStatus.OracleDbType = OracleDbType.Int16;
                parameterStatus.Value = taskStatus;
                #endregion
            }

            command.Parameters.Add( parametertaskId );
            command.Parameters.Add( parameterStatus );

            OracleDataReader reader = command.ExecuteReader();

            reader.Read();

            ArrayList list = new ArrayList();

            for ( int i = 0; i < reader.FieldCount; i++ )
            {
                list.Add(reader.GetValue(i).ToString());
            }
            reader.Close();
            conn.Close();
            return list;

        }

    }
}
