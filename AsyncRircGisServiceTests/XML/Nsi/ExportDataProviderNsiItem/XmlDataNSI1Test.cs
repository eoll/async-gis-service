using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using AsyncRircGisService.XML;
using System.Diagnostics;


namespace AsyncRircGisServiceTests
{
    [TestClass]
    public class XmlDataNSI1Test
    {
        [TestMethod]
        public void BuildDataTable_CorrectXML_DataTable()
        {
            // Arrange.
            string bodyXML = System.IO.File.ReadAllText("TaskUnit\\Nsi\\ExportDataProviderNsiItem\\ResultFromGis_NSI1.xml");


            // Act.
            XmlDataNSI1 xmlData = new XmlDataNSI1(bodyXML);

            DataTable actual = xmlData.BuildDataTable();

            foreach ( DataRow row in actual.Rows )
            {
                Debug.WriteLine( "Row содержит: "              
                                     + "MESSAGE_GUID: "                                       + row.Field<string>( "MESSAGE_GUID" )
                                     + "| CREATED: "                                          + row.Field<string>( "CREATED"      )
                                     + "| CODE: "                                             + row.Field<string>( "CODE"         )
                                     + "| GUID: "                                             + row.Field<string>( "GUID"         )
                                     + "| MODIFIED: "                                         + row.Field<string>( "MODIFIED"     )
                                     + "| IS_ACTUAL: "                                        + row.Field<string>( "IS_ACTUAL"    )
                                     + "| Вид дополнительной услуги (-793499481): "           + row.Field<string>( "-793499481"   )
                                     + "| Единица измерения (1592027100): "                   + row.Field<string>( "1592027100"   )
                                     + "| Единица измерения (принадлежит ОКЕИ)(-882200887): " + row.Field<string>( "-882200887"   )
                                     + "| Единица измерения (текстовое поле)(1432260840): "   + row.Field<string>( "1432260840"   )
                               );
            }


            // Assert.
            Assert.IsNotNull (actual, "DataTable actual не должен быть пустым");

        }
    }
}
