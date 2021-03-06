﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncRircGisService.XML;
using System.Data;
using System.Diagnostics;

namespace AsyncRircGisServiceTests.XML
{
    [TestClass]
    public class XmlDataNSI10Test
    {
        [TestMethod]
        public void BuildDataTable_CorrectXML_DataTable()
        {
            // Arrange.
            string bodyXML = System.IO.File.ReadAllText("TaskUnit\\NsiСommon\\ExportNsiItem\\ResultFromGis.xml");


            // Act.
            XmlDataNSI10 xmlData = new XmlDataNSI10(bodyXML);

            DataTable actual = xmlData.BuildDataTable();

            foreach ( DataRow row in actual.Rows )
            {
                Debug.WriteLine( "Row содержит: " + " " +               
                                    row[ "MESSAGE_GUID" ].ToString() + " " +
                                    row[ "CREATED"      ].ToString() + " " +
                                    row[ "CODE"         ].ToString() + " " +
                                    row[ "GUID"         ].ToString() + " " +
                                    row[ "MODIFIED"     ].ToString() + " " +
                                    row[ "IS_ACTUAL"    ].ToString() + " " +
                                    row[ "NAME"         ].ToString() + " " +
                                    row[ "VALUE"        ].ToString() 
                               );
            }

            // Assert.
            Assert.IsNotNull (actual, "DataTable actual не должен быть пустым");

        }
    }
}
