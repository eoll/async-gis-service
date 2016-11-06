using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncRircGisService;
using System.Diagnostics;

namespace AsyncRircGisServiceTests
{
    [TestClass]
    public class ServiceConfigTests
    {
        public void Init_Fill_Conformation()
        {
            // Act.

            // Arrange.
            ServiceConfig.Init();

            // Asssert.
            Debug.WriteLine(ServiceConfig.Conformation.baseUrl);

        }
    }
}
