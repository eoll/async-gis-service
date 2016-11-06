using System.Linq;
using System.Data;
using System.Xml.Linq;

namespace AsyncRircGisService.XML
{
    /// <summary>
    /// Класс для разбора XML документов объединённых одной общей структурой.
    /// Является родителем для обработчиков 
    /// </summary>
    public abstract class XMLDataCommonAs10 : Builder, IXMLToDataTable
    {
    
        /// <summary>
        /// Преобразует XML в DataTable.
        /// </summary>
        /// <returns>Таблица типа DataTable.</returns>
        public DataTable BuildDataTable()
        {

            DataTable table = new DataTable();

            table.Columns.Add( "MESSAGE_GUID"  );
            table.Columns.Add( "CREATED"       );
            table.Columns.Add( "CODE"          );
            table.Columns.Add( "GUID"          );
            table.Columns.Add( "MODIFIED"      );
            table.Columns.Add( "IS_ACTUAL"     );
            table.Columns.Add( "NAME"          );
            table.Columns.Add( "VALUE"         );

            // Читаем данные.
            XDocument doc = XDocument.Parse( bodyXML );

            // Получаем узел MessageGUID.
            var messageGUID = GetXElements( doc, "MessageGUID" );

            //// Получаем узел NsiItemRegistryNumber.
            //var registrynumber = GetXElements( doc, "NsiItemRegistryNumber" );

            // Получаем узел created.
            var created        = GetXElements( doc, "Created" );

            // Получаем множетсво узлов NsiElement.
            var nsiElements    = GetXElements( doc, "NsiElement" );

            // Помещаем значения узла в DataTable.
            foreach ( var item in nsiElements )
            {
                XNamespace xns = item.Name.Namespace;

                // В таблице создаём строку.
                DataRow row = table.NewRow();

                // Заполняем колонки в строке данными.
                row[ "MESSAGE_GUID" ] = messageGUID.Last().Value;
                row[ "CREATED"      ] = created.First().Value;
                row[ "CODE"         ] = item.Element( xns + "Code"            ).Value;
                row[ "GUID"         ] = item.Element( xns + "GUID"            ).Value;
                row[ "MODIFIED"     ] = item.Element( xns + "Modified"        ).Value;
                row[ "IS_ACTUAL"    ] = item.Element( xns + "IsActual"        ).Value;
                row[ "NAME"         ] = item.Element( xns + "NsiElementField" ).Element( xns + "Name" ).Value;
                row[ "VALUE"        ] = item.Element( xns + "NsiElementField" ).Element( xns + "Value" ).Value;

                table.Rows.Add( row );
            }

            return table;

        }


    }
}
