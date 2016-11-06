using System.Linq;
using System.Data;
using System.Xml.Linq;
using System.Collections.Generic;

namespace AsyncRircGisService.XML
{
    /// <summary>
    /// Класс разбора XML справочника ГИС ЖКХ - NSI 10.
    /// </summary>
    public class XmlDataNSI1 : Builder, IXMLToDataTable
    {

        /// <summary>
        /// Заполняет строковым значением XML поле класса.
        /// </summary>
        /// <param name="bodyXML">Строковое представление XML</param>
        public XmlDataNSI1( string bodyXML )
        {

            this.bodyXML = bodyXML;
  
        }

        /// <summary>
        /// Преобразует XML в DataTable.
        /// </summary>
        /// <returns>Таблица типа DataTable.</returns>
        public DataTable BuildDataTable()
        {

            DataTable table = new DataTable();

            table.Columns.Add( "MESSAGE_GUID"    );
            table.Columns.Add( "CREATED"         );
            table.Columns.Add( "CODE"            );
            table.Columns.Add( "GUID"            );
            table.Columns.Add( "MODIFIED"        );
            table.Columns.Add( "IS_ACTUAL"       );
            table.Columns.Add( "-793499481"      );            //"ADDITIONAL_SERVICE_NAME"
            table.Columns.Add( "1592027100"      );            //"DIMENSIONS_UNIT_OKEI"
            table.Columns.Add( "-882200887"      );            //"IS_OKEI_UNIT"
            table.Columns.Add( "1432260840"      );            //"DIMENSIONS_UNIT_STRING"

            // Читаем данные.
            XDocument doc = XDocument.Parse( bodyXML );

            // Получаем узел MessageGUID.
            var messageGUID = GetXElements( doc, "MessageGUID" );

            // Получаем узел created.
            var created        = GetXElements( doc, "Created" );

            // Получаем множетсво узлов NsiElement.
            var nsiElements    = GetXElements( doc, "NsiElement" );

            // Берём один nsiElement.
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

                // Получаем последовательность узлов "NsiElementField"
                IEnumerable<XElement> nsiElentField = from el in item.Descendants()
                                                  where  el.Name.LocalName == "NsiElementField"
                                                  select el;

                // Берём один NsiElementField.
                foreach ( XElement elementField in nsiElentField )
                {
                    // Получаем namespace узла.
                    XNamespace xnsField = elementField.Name.Namespace;

                    // Получаем хэш код из подузла Name, это будет имя колонки.
                    string columnName = elementField.Element( xnsField + "Name" ).Value.GetHashCode().ToString();

                    // если DIMENSIONS_UNIT_OKEI в БД, Единица измерения в XML.
                    if ( columnName == "1592027100" )
                    {
                        // Проверяем есть ли узел "Code".
                        bool isCode = elementField.Elements().Any( node => node.Name.LocalName == "Code" );

                        // Если есть, то берём значение в узле и записываем.
                        if ( isCode ) row[columnName] = elementField.Element( xnsField + "Code" ).Value; // Заносим в колонку значение.

                    }
                    else
                    {
                        // Проверяем есть ли узел "Code".
                        bool isValue = elementField.Elements().Any( node => node.Name.LocalName == "Value" );
                        // Если есть, то берём значение в узле и записываем.
                        if ( isValue ) row[columnName] = elementField.Element( xnsField + "Value" ).Value; // Заносим в колонку значение.
                    }
                }
                table.Rows.Add( row );
            }

            return table;

        }
    }
}
