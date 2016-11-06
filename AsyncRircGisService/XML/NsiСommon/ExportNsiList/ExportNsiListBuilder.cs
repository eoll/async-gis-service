using System;
using System.Data;
using System.Xml.Linq;
using System.Linq;
using System.Diagnostics.Contracts;

namespace AsyncRircGisService.XML
{
    public class ExportNsiListBuilder : Builder, IXMLToDataTable
    {

        /// <summary>
        /// Ответ сервиса ГИС ЖКХ в виде XML.
        /// </summary>
        public string XMLResponse { get; set; }

        /// <summary>
        /// Выборка данных из XML.
        /// </summary>
        public DataTable exportToOracle { get; set; }


        /// <summary>
        /// Разбирает данные из <see cref="XMLResponse"/> и значения узлов записывает в <see cref="exportToOracle"/>
        /// </summary>
        public DataTable BuildDataTable()
        {

            Contract.Requires<ArgumentException>( !string.IsNullOrEmpty( XMLResponse ), "ExportNsiListBuilder.GetXMLData() XMLResponse is NULL" );

            DataTable table = new DataTable();

            table.Columns.Add( "MESSAGE_GUID"           );
            table.Columns.Add( "REGISTRYNUMBER"         );
            table.Columns.Add( "NAME"                   );
            table.Columns.Add( "MODIFIED"               );

            // Читаем данные.
            XDocument doc = XDocument.Parse( XMLResponse );

            // Получаем узел MessageGUID.
            var messageGUID = GetXElements( doc, "MessageGUID" );

            // Получаем множетсво узлов NsiItemInfo.
            var elements = GetXElements(doc, "NsiItemInfo");

            // Помещаем значения узла в DataTable.
            foreach ( var item in elements )
            {

                XNamespace xns = item.Name.Namespace;

                // В таблице создаём строку.
                DataRow row = table.NewRow();

                // Заполняем колонки в строке данными.
                row[ "MESSAGE_GUID"          ] = messageGUID.Last().Value                    ;
                row[ "REGISTRYNUMBER"        ] = item.Element( xns + "RegistryNumber" ).Value;
                row[ "NAME"                  ] = item.Element( xns + "Name"           ).Value;
                row[ "MODIFIED"              ] = item.Element( xns + "Modified"       ).Value;
                table.Rows.Add( row );
            }

            return table;
        }


    }
}
