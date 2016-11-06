using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Xades.Implementations;
using Xades.Helpers;
using AsyncRircGisService.Gis.Infrastructure;
using AsyncRircGisService.Gis.Helpers;

namespace AsyncRircGisService.Gis
{
    public class Sender
    {
        #region PrivateMethodsAndProps

        // Метод ищет ноды в XML которые необходимо подписать. Возвращает метод, который подписывает ноды. signPasswd - пароль контейнера подписи.
        private static string SignNode( XmlDocument xml, string xpath, string certificateThumbprint, string signPasswd, string guid )
        {
            var manager = xml.CreateNamespaceManager();
            var node    = xml.SelectSingleNode( xpath, manager );
            if ( node == null )
            {
                Email.Sender.SendMail( $"Не удалось найти узел{xpath}" );
                throw new InvalidOperationException( $"Не удалось найти узел{xpath}" );
            }
            var nodeId = node.Attributes[ "id" ];

            if ( nodeId == null )
            {
                nodeId = xml.CreateAttribute( "Id" );
                node.Attributes.Append( nodeId );
            }

            if ( string.IsNullOrEmpty( nodeId.Value ) )
            {
                nodeId.Value = guid;
            }
            return Sign( xml, nodeId.Value, certificateThumbprint, signPasswd, guid );
        }

        // Метод добавления сигнатуры подписи в аттрибуты XML документа.
        private static string Sign( XmlDocument xml, string elementId, string certificateThumbprint, string passwd, string guid )
        {
            if ( string.IsNullOrEmpty( elementId ) )
            {
                var rootNode   = xml.DocumentElement;
                var rootNodeId = GetRootId( rootNode );
                if ( !string.IsNullOrEmpty( rootNodeId ) )
                {
                    //Warning($"Не задан элемент для подписи. Используется корневой элемент {rootNode.Name} с Id {rootNodeId}");
                    elementId = rootNodeId;
                }
                else
                {
                    elementId        = guid;
                    var attribulte   = xml.CreateAttribute( "Id" );
                    attribulte.Value = elementId;
                    rootNode.Attributes.Append( attribulte );
                    //Warning($"Не задан элемент для подписи. Используется корневой элемент {rootNode.Name} с Id {elementId} (атрибут сгенерирован)");
                }
            }
            GostXadesBesService xadesService = new GostXadesBesService();
            return xadesService.Sign( xml.OuterXml, elementId, certificateThumbprint, passwd );
        }

        // Проверка подписи в ответной XML.
        protected static void Validate( XmlDocument xml, string elementId )
        {
            if ( string.IsNullOrEmpty( elementId ) )
            {
                var rootNode   = xml.DocumentElement;
                var rootNodeId = GetRootId( rootNode );
                if ( !string.IsNullOrEmpty( rootNodeId ) )
                {
                    //Warning($"Не задан элемент для проверки подписи. Используется элемент {rootNode.Name} с Id {rootNodeId}");
                    elementId = rootNodeId;
                }
                else
                {
                    throw new ArgumentException( "Не задан Id элемента для проверки подписи и корневой элемент не имеет Id" );
                }
            }
            GostXadesBesService xadesService = new GostXadesBesService();
            xadesService.ValidateSignature( xml.OuterXml, elementId );
        }

        private static readonly string[] _idAttributeNames = { "Id", "id", "ID", "iD", "_Id", "_id", "_ID", "_iD" };

        // Получение рутовой ноды.
        private static string GetRootId( XmlNode rootId )
        {
            var idName = _idAttributeNames.SingleOrDefault( x => rootId.Attributes[x] != null );
            return !string.IsNullOrEmpty( idName ) ? rootId.Attributes[idName]?.Value : null;
        }

        /// <summary>
        /// Формирует XML тело сообщения на основе структуры GisDataPack.
        /// </summary>
        /// <param name="gisRequest">Структура типа GisDataPack</param>
        /// <returns>Строковое представление сформированного XML</returns>
        private static string GetBodyXML( DataPack gisRequest )
        {
            var soapFormatter = new GisSoapFormatter
            {
                Template         = PathHelper.ToAppAbsolutePath( gisRequest.TemplatePath ),
                SchemaVersion    = gisRequest.SchemaVersion,
                AddOrgPPAGuid      = gisRequest.AddOrgPPAGUID,
                ValuesDictionary = gisRequest.Xpath2Values,
                OrgPPAGuid         = gisRequest.OrgPPAGUID,
                Config           = gisRequest.SoapConfig
            };
            var soapString = soapFormatter.GetSoapRequest( gisRequest.RequesterMessageGuid );

#if DEBUG
            Notificator.Write( soapString );
#endif

            if ( gisRequest.AddSignature && gisRequest.Action != "urn:getState" )
            {
                var soapXml = XmlDocumentHelper.Create( soapString );
                soapString  = SignNode( soapXml, Constants.SoapContentXpath, gisRequest.CertificateThumbprint, gisRequest.SignPasswd, gisRequest.RequesterMessageGuid );
#if DEBUG
                Notificator.Write( soapString );
#endif
                return soapString;
            }
            return soapString;
        }

        /// <summary>
        /// Отправка запроса к сервису ГИС ЖКХ
        /// </summary>
        /// <param name="gisRequest">Структура типа GisDataPack</param>
        /// <returns>кортеж IEnumerable<Tuple<string, string>></returns>
        private static IEnumerable<Tuple<string, string>> CallGisService( DataPack gisRequest )
        {
            string body = GetBodyXML( gisRequest );

            var webRequest    = ( HttpWebRequest )WebRequest.Create( gisRequest.Url );
            webRequest.Headers.Add( "SOAPAction", gisRequest.Action );
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            // Console.WriteLine( "\nurl: {0}\n\naction: {1}\n\nbody: {2}", gisRequest.Url, gisRequest.Action, body );
            webRequest.Credentials = new NetworkCredential( gisRequest.LoginService, gisRequest.PassService );

            using ( Stream stream = webRequest.GetRequestStream() )
            {
                var buffer = Encoding.UTF8.GetBytes( body );
                stream.Write( buffer, 0, buffer.Length );
            }

            using ( WebResponse webResponse = webRequest.GetResponse() )
            {
                using ( StreamReader rd = new StreamReader( webResponse.GetResponseStream() ) )
                {
                    return CheckResponse( rd.ReadToEnd() );
                }
            }
        }

        // Проверка ответа от сервера.
        private static IEnumerable<Tuple<string, string>> CheckResponse( string response )
        {
            IEnumerable<Tuple<string, string>> resultValues;
            try
            {
                resultValues = ProcessSoapResponse( response );
                // В кортеж добавлен ключ XMLstring, по которому расположено текстовое представление XML документа
                // это нужно для обработки результата используя не кортеж, а XML.
                resultValues = resultValues.Concat( new[] { new Tuple<string, string>( "XMLstring", response ) } );

                return resultValues;

            }
            //catch( XadesBesValidationException ex )
            //{
            //    /*Error($"Подпись ответа от ГИС ЖКХ не прошла проверку: {ex.Message}", ex)*/
            //    ;
            //    var subj = new Tuple<string, string>($"\n\rStackTrace: ", ex.StackTrace);
            //    var error = subj as IEnumerable<Tuple<string, string>>;
            //    return error;
            //}
            catch ( WebException ex )
            {
                var warning = $"Сервер ответил с ошибкой:" + ex.Message;
                using ( var streamWriter = new StreamReader( ex.Response.GetResponseStream() ) )
                {
                    var Errorresponse = streamWriter.ReadToEnd();
                    resultValues      = ProcessSoapResponse( response );
                    return resultValues;
                }
            }

            //Info($"Сохранение ответа ...");
            //Console.WriteLine(resultValues);
            //Success("Запрос успешно выполнен");
        }

        private static IEnumerable<Tuple<string, string>> ProcessSoapResponse( string response )
        {
            var soapXml      = XmlDocumentHelper.Create( response );

            var manager      = soapXml.CreateNamespaceManager();
            var bodyResponse = soapXml.SelectSingleNode( Constants.SoapContentXpathResponce, manager );

            var idAttribute = bodyResponse.Attributes["Id"]?.Value;

            if ( !string.IsNullOrEmpty( idAttribute ) && bodyResponse.ChildNodes.OfType<XmlNode>().Any( x => x.LocalName == Constants.SignatureName ) )
            {
                //Info("Проверка подписи ответа...");
                Validate( soapXml, idAttribute );
            }
            else
            {
                //Info("Ответ не содержит подписи");
                //var subj = new Tuple<string, string>("Error", "Ответ не содержит подписи");
                //var error = subj as IEnumerable<Tuple<string, string>>;
                //return error;
            }

            return FindXmlDataNode( bodyResponse, $"{bodyResponse.Name}/" );
        }
        private static IEnumerable<Tuple<string, string>> FindXmlDataNode( XmlNode node, string currentPath = "" )
        {
            var childs = node.ChildNodes.OfType<XmlNode>()
                .Where( x => x.NodeType == XmlNodeType.Element && x.LocalName != Constants.SignatureName ).ToList();
            if ( childs.Count == 0 )
            {
                throw new InvalidOperationException( "В ответе на запрос не найден узел с данными" );
            }

            //TODO: временное решение
            return childs.Count > 1
                ? ParseXmlDataNode( childs, currentPath )
                : FindXmlDataNode( childs.First(), $"{ currentPath }{ childs.First().Name }/" );
        }
        private static IEnumerable<Tuple<string, string>> ParseXmlDataNode( IEnumerable<XmlNode> nodeList, string currentPath )
        {
            foreach ( var node in nodeList )
            {
                if ( node.NodeType == XmlNodeType.Text )
                {
                    yield return Tuple.Create( currentPath.Trim( '/' ), node.InnerText );
                }
                else
                {
                    var childs = node.ChildNodes
                        .OfType<XmlNode>()
                        .Where( x => ( x.NodeType == XmlNodeType.Element || x.NodeType == XmlNodeType.Text ) && x.LocalName != Constants.SignatureName );
                    var data = ParseXmlDataNode( childs, $"{ currentPath }{ node.Name }/");
                    foreach ( var item in data )
                    {
                        yield return item;
                    }
                }
            }
        }

        #endregion

        #region PublicMethods

        /// <summary>
        /// Выполняет асинхронный запрос к методу сервиса ГИС ЖКХ.
        /// </summary>
        /// <param name="gisDataPack">Объект типа GisDataPack</param>
        public static string RequestMethod( DataPack gisDataPack )
        {
            Contract.Requires( !string.IsNullOrEmpty( gisDataPack.CertificateThumbprint ) );
            Contract.Requires( !string.IsNullOrEmpty( gisDataPack.SchemaVersion         ) );
            Contract.Requires( !string.IsNullOrEmpty( gisDataPack.LoginService          ) );
            Contract.Requires( !string.IsNullOrEmpty( gisDataPack.PassService           ) );
            Contract.Requires( gisDataPack.SoapConfig != null                             );
            Contract.Requires( !string.IsNullOrEmpty( gisDataPack.TemplatePath          ) );
            Contract.Requires( !string.IsNullOrEmpty( gisDataPack.Url                   ) );
            Contract.Requires( !string.IsNullOrEmpty( gisDataPack.Action                ) );

            // Код на получение результатов
            string messageGuid = "";

            var callGisService = CallGisService( gisDataPack );

            var itemFirst      = callGisService.First();

            messageGuid        = itemFirst.Item2;

            return messageGuid;
        }




        /// <summary>
        /// Выполняет асинхронный запрос на получение результатов метода сервиса ГИС ЖКХ.
        /// </summary>
        /// <param name="gisRequestPack">Объект типа Gis.DataPack</param>
        /// <returns>Кортеж строковых значений - данных ответа</returns>
        public static IEnumerable<Tuple<string, string>> RequestMethodResults( DataPack gisRequestPack )
        {

            Contract.Requires( !string.IsNullOrEmpty( gisRequestPack.CertificateThumbprint ) );
            Contract.Requires( !string.IsNullOrEmpty( gisRequestPack.LoginService          ) );
            Contract.Requires( !string.IsNullOrEmpty( gisRequestPack.PassService           ) );
            Contract.Requires( gisRequestPack.SoapConfig != null                             );
            Contract.Requires( !string.IsNullOrEmpty( gisRequestPack.TemplatePath          ) );
            Contract.Requires( !string.IsNullOrEmpty( gisRequestPack.Url                   ) );
            Contract.Requires( !string.IsNullOrEmpty( gisRequestPack.Action                ) );
            Contract.Requires( gisRequestPack.Xpath2Values.Count() > 0                       );

            // Получить результат запроса к сервису ГИС ЖКХ
            var gisResultData = CallGisService( gisRequestPack );

            return gisResultData;

        }

        #endregion
    }
}
