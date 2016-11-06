using System;
using System.Data;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using System.Diagnostics;
using AsyncRircGisService.Exceptions;

namespace AsyncRircGisService.XML
{
    /// <summary>
    /// Класс построитель XML документа.
    /// Обслуживает метод ExportDataProviderNsiItem в ГИС ЖКХ.
    /// </summary>
    public class ExportDataProviderNsiItemBuilder : Builder , IDataTableToXML 
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
            Contract.Requires< ArgumentException >( Regex.IsMatch( DataTable.Rows[0].Field<string>( "REGISTRY_NUMBER" ), @"^\d+$" ), "Параметр должен быть числом" );

            string registryNumber = DataTable.Rows[0].Field<string>( "REGISTRY_NUMBER" );
            string modifiedAfter  = DataTable.Rows[0].Field<string>( "MODIFIED_AFTER"  );

            Debug.WriteLine( "registryNumber = {0}, modifiedAfter = {1}", registryNumber, modifiedAfter );

            // Берём данные из Oracledata и по ним строим XML.
            if ( modifiedAfter == null )
            {

                BodyXML = @"<nsi:RegistryNumber>" + registryNumber + "</nsi:RegistryNumber>";
            }
            else
            {
                BodyXML = @"<nsi:RegistryNumber>" + registryNumber + "</nsi:RegistryNumber>" +
                            "<nsi:ModifiedAfter>" + modifiedAfter  + "</nsi:ModifiedAfter>";
            }
        }

        #endregion
    }
}
