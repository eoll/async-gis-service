using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using Xades.Abstractions;
using Xades.Exceptions;
using Xades.Helpers;
using AsyncRircGisService.Gis.Configurations.Options;
using AsyncRircGisService.Gis.Configurations.Sections;
using AsyncRircGisService.Gis.Infrastructure;
using AsyncRircGisService.Gis.Helpers;
using AsyncRircGisService.Gis;

namespace AsyncRircGisService.Gis.Commands
{
    public abstract class GisCommandBase<TOption> : XadesCommandBase<TOption> where TOption : XadesOptionBase
    {
        private readonly GisServiceConfiguration _serviceConfig;
        protected abstract bool IsSignatureRequired { get; }

        protected GisCommandBase(TOption option, IXadesService xadesService, SigningConfiguration signingConfig, GisServiceConfiguration serviceConfig) 
            : base(option, xadesService, signingConfig)
        {
            _serviceConfig = serviceConfig;
        }

        protected void SendRequest(string serviceName, string methodName, IEnumerable<Tuple<string, string>> xpath2Values)
        {
            var service = _serviceConfig.Services[serviceName];
            if (service == null)
            {
                throw new ConfigurationErrorsException($"Конфигурация для сервиса {serviceName} не задана");
            }

            var method = service.Methods[methodName];
            if (method == null)
            {
                throw new ConfigurationErrorsException($"Конфигурация для метода {methodName} не задана");
            }

            if (method.RequiredBody && !xpath2Values.Any())
            {
                throw new ConfigurationErrorsException($"Метод {methodName} имеет обязательные параметры для запроса (-с файл)");
            }

            Info("Создание запроса...");
            var soapFormatter = new GisSoapFormatter
            {
                Template = PathHelper.ToAppAbsolutePath(method.Template),
                //SchemaVersion = _serviceConfig.SchemaVersion, TODO  Удалить если всё норм
                AddOrgPPAGuid = service.AddSenderId,
                ValuesDictionary = xpath2Values,
                // TODO обратить внимание на эту строчку, нужно ли из конфига брать? или из базы guid организации.
                OrgPPAGuid = _serviceConfig.RIRCorgPPAGUID,
                Config = _serviceConfig.SoapConfiguration,
                MessageGuid = ""
                
            };

            var soapString = soapFormatter.GetSoapRequest(soapFormatter.MessageGuid);
            System.IO.File.WriteAllText(@"soapString.txt", soapString);
            if (IsSignatureRequired && service.AddSignature)
            {
                Info("Добавление подписи в запрос...");
                var soapXml = XmlDocumentHelper.Create(soapString);
                soapString = SignNode(soapXml, Constants.SoapContentXpath);
                System.IO.File.WriteAllText(@"soapStringSign.txt", soapString);
            }



            IEnumerable<Tuple<string, string>> resultValues;
            try
            {
                Info($"Отправка запроса по адресу: {_serviceConfig.BaseUrl}{service.Path}/{method.Action}");

                Console.WriteLine($"Отправка запроса по адресу: {_serviceConfig.BaseUrl}{service.Path}/{method.Action}");

                var response = CallGisService($"{_serviceConfig.BaseUrl}{service.Path}", method.Action, soapString, _serviceConfig.Login, _serviceConfig.Password);

                Console.WriteLine(response);

                Info("Обработка ответа на запрос...");
                resultValues = ProcessSoapResponse(response);
                Console.WriteLine(resultValues);
            }
            catch (XadesBesValidationException ex)
            {
                Error($"Подпись ответа от ГИС ЖКХ не прошла проверку: {ex.Message}", ex);
                return;
            }
            catch (WebException ex)
            {
                Warning($"Сервер ответил с ошибкой: {ex.Message}");
                using (var streamWriter = new StreamReader(ex.Response.GetResponseStream()))
                {
                    var response = streamWriter.ReadToEnd();
                    resultValues = ProcessSoapResponse(response);
                }
            }

            Info($"Сохранение ответа ...");
            Console.WriteLine(resultValues);
            Success("Запрос успешно выполнен");
        }

        private IEnumerable<Tuple<string, string>> ProcessSoapResponse(string response)
        {
            var soapXml = XmlDocumentHelper.Create(response);

            var manager = soapXml.CreateNamespaceManager();
            var bodyResponse = soapXml.SelectSingleNode(Constants.SoapContentXpath, manager);

            var idAttribute = bodyResponse.Attributes["Id"]?.Value;

            if (!string.IsNullOrEmpty(idAttribute) && bodyResponse.ChildNodes.OfType<XmlNode>().Any(x => x.LocalName == Constants.SignatureName))
            {
                Info("Проверка подписи ответа...");
                Validate(soapXml, idAttribute);
            }
            else
            {
                Info("Ответ не содержит подписи");
            }

            return FindXmlDataNode(bodyResponse, $"{bodyResponse.Name}/");
        }

        private static IEnumerable<Tuple<string, string>> FindXmlDataNode(XmlNode node, string currentPath = "")
        {
            var childs = node.ChildNodes.OfType<XmlNode>()
                .Where( x => x.NodeType == XmlNodeType.Element && x.LocalName != Constants.SignatureName).ToList();
            if (childs.Count == 0)
            {
                throw new InvalidOperationException("В ответе на запрос не найден узел с данными");
            }

            //TODO: временное решение
            return childs.Count > 1 
                ? ParseXmlDataNode(childs, currentPath)
                : FindXmlDataNode(childs.First(), $"{currentPath}{childs.First().Name}/");
        }

        private static IEnumerable<Tuple<string, string>> ParseXmlDataNode(IEnumerable<XmlNode> nodeList, string currentPath)
        {
            foreach(var node in nodeList)
            {
                if (node.NodeType == XmlNodeType.Text)
                {
                    yield return Tuple.Create(currentPath.Trim('/'), node.InnerText);
                }
                else
                {
                    var childs = node.ChildNodes
                        .OfType<XmlNode>()
                        .Where(x => (x.NodeType == XmlNodeType.Element || x.NodeType == XmlNodeType.Text) && x.LocalName != Constants.SignatureName);
                    var data = ParseXmlDataNode(childs, $"{currentPath}{node.Name}/");
                    foreach (var item in data)
                    {
                        yield return item;
                    }
                }
            }
        }

        private static string CallGisService(string url, string action, string body, string login, string password)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            Console.WriteLine("\nurl: {0}\n\naction: {1}\n\nbody: {2}", url, action, body);
            webRequest.Credentials = new NetworkCredential(login, password);

            using (Stream stream = webRequest.GetRequestStream())
            {
                var buffer = Encoding.UTF8.GetBytes(body);
                stream.Write(buffer, 0, buffer.Length);
            }

            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    Console.WriteLine(rd);
                    return rd.ReadToEnd();
                }
            }
        }
    }
}