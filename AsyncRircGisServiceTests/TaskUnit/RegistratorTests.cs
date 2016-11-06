using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using AsyncRircGisService.TaskRegistry;

namespace AsyncRircGisServiceTests.TaskUnit
{
    [TestClass]
    public class RegistratorTests
    {
        [TestMethod]
        public void ProvideTaskData_Fill_TaskDataQueue()
        {
            // Arrange.

            // Act.
            Registrator registrator = new Registrator();
            registrator.ProvideTaskData();

            // Assert.
            Debug.WriteLine("Количество строк на выполнение равно {0}", registrator.TaskDataQueue.Count);
            Assert.IsTrue(registrator.TaskDataQueue.Count > 0, "TaskDataQueue.Count не должна быть пустой." );

        }
    }
}
