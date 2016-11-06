using System;
using System.Data;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using System.Diagnostics;
using AsyncRircGisService.Exceptions;

namespace AsyncRircGisService.XML
{
    public class ExportNsiItemBuilder : Builder , IDataTableToXML 
    {
        #region PublicProperties

        /// <summary>
        /// Данные подзадачи из Oracle.
        /// </summary>
        public DataTable DataTable{ get; set; }

        #endregion

        #region PublicFields
        /// <summary>
        /// Выстраивает XML структуру тела запроса на основе данных Oracle по спецификации wsdl.
        /// </summary>
        public void BuildXML()
        {
            Contract.Requires<ArgumentException>( Regex.IsMatch( DataTable.Rows[0].Field<string>( "LIST_GROUP"      ), @"^(NSI|NSIRAO)" ), "Параметр должен быть NSI или NSIRAO" );
            Contract.Requires<ArgumentException>( Regex.IsMatch( DataTable.Rows[0].Field<string>( "REGISTRY_NUMBER" ), @"^\d+$"         ), "Параметр должен быть числом"         );

            string listGroup      = DataTable.Rows[0].Field<string>( "LIST_GROUP"      );
            string registryNumber = DataTable.Rows[0].Field<string>( "REGISTRY_NUMBER" );
            string modifiedAfter  = DataTable.Rows[0].Field<string>( "MODIFIED_AFTER"  );

            Debug.WriteLine( "listGroup = {0}, registryNumber = {1}, modifiedAfter = {2}", listGroup, registryNumber, modifiedAfter );

            // Берём данные из Oracledata и по ним строим XML.
            if ( modifiedAfter == null )
            {

                BodyXML = @"<nsi:RegistryNumber>" + registryNumber + "</nsi:RegistryNumber>" +
                            "<nsi1:ListGroup>"    + listGroup      + "</nsi1:ListGroup>";
            }
            else
            {
                BodyXML = @"<nsi:RegistryNumber>" + registryNumber + "</nsi:RegistryNumber>" +
                            "<nsi1:ListGroup>"    + listGroup      + "</nsi1:ListGroup>" +
                            "<nsi:ModifiedAfter>" + modifiedAfter  + "</nsi:ModifiedAfter>";
            }
        }

        #endregion
    }
}
