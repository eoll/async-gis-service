using System.Data;
using AsyncRircGisService.Exceptions;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using System;

namespace AsyncRircGisService.XML
{
    /// <summary>
    /// Класс создатель объекта - IDataTableToXML
    /// Создаёт объект определённого типа ориентируясь на передаваемые параметры из OracleBase.ResultData.
    /// </summary>
    public class BuilderCreator
    {
        // Поле для хранения данных - подзадачи из oracle.
        private DataTable dataOracle;

        private string xmlBody;

        /// <summary>
        /// Передаёт данные подзадачи из Oracle в своё внутреннее поле для дальнейшей обработки.
        /// </summary>
        /// <param name="oracleData">Данные подзадачи из Oracle.</param>
        public BuilderCreator( DataTable oracleData, string xmlBody )
        {
            Contract.Requires<ArgumentException>( Regex.IsMatch( oracleData.Rows[0][2].ToString(), @"^\d+$"         ), "Некорректный номер справочника."                );
            Contract.Requires<ArgumentException>( Regex.IsMatch( oracleData.Rows[0][1].ToString(), @"^(NSI|NSIRAO)" ), "Группа справочников должна быть NSI или NSIRAO" );
            Contract.Requires<ArgumentException>(  !string.IsNullOrEmpty( xmlBody                                   ), "Отсутствует XML документ."                      );

            dataOracle = oracleData;

            this.xmlBody = xmlBody;
        }

        /// <summary>
        /// Возвращает объект обработчик кортежа на основе переданных в конструкторе данных - подзадачи.
        /// </summary>
        /// <returns>Объект - обработчик кортежа.</returns>
        public IXMLToDataTable GetBuilder()
        {

            string registryNumber =  dataOracle.Rows[0][2].ToString();
            string listGroup      =  dataOracle.Rows[0][1].ToString();

            // По передаваемому значению номера счётчика и типа группы возвращается объект - обработчик XML документа.
            if      ( registryNumber == "10" && listGroup == "NSI" ) return new XmlDataNSI10( xmlBody );
            else if ( registryNumber == "95" && listGroup == "NSI" ) return new XmlDataNSI95( xmlBody );
            else if ( registryNumber == "22" && listGroup == "NSI" ) return new XmlDataNSI22( xmlBody );
            else throw new NotFoundException( "По данным registryNumber = " + registryNumber + "и listGroup = " + listGroup + "класс подзадача не найден" );

        }

    }
}
