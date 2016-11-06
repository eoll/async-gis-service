using System.Data;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
namespace AsyncRircGisService.XML
{
    /// <summary>
    /// Конструирует XML структуру тела SOAP запроса на основе данных Oracle.
    /// </summary>
    public abstract class Builder
    {

        /// <summary>
        /// XML структура в строковом представлении. 
        /// </summary>
        protected string bodyXML;

        /// <summary>
        /// XML структура в строковом представлении. 
        /// </summary>
        public string BodyXML
        {
            get { return bodyXML; }
            set { bodyXML = value; }
        }

        /// <summary>
        /// Преобразует переменные XML путем замены знаков подчеркивания на двоеточие.
        /// </summary>
        protected void ProcessXML()
        {
            BodyXML = BodyXML.Replace("___", ":");
        }


        /// <summary>
        /// Возвращает последовательность XElements в документе.
        /// </summary>
        /// <param name="doc">XML документ типа XDocument в котором производится поиск.</param>
        /// <param name="childNode">Нода/ноды, которую/которые необходимо искать.</param>
        /// <returns>IEnumerable<XElement></returns>
        protected IEnumerable<XElement> GetXElements( XDocument doc, string childNode )
        {
            IEnumerable<XElement> xElements = from el in doc.Root.Descendants() where  el.Name.LocalName == childNode select el;
            return xElements;
        }

    }
}


