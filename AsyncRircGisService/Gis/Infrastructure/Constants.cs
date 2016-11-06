namespace AsyncRircGisService.Gis.Infrastructure
{
    internal static class Constants
    {
        internal const string GisServicesConfigSectionName = "GisServicesConfig";
        internal const string XadesConfigSectionName       = "signingConfig";

        internal const string SignElementName              = "sign-element";
        // Путь к телу запроса, который отослан в ГИС.
        internal const string SoapContentXpath             = "soapenv:Envelope/soapenv:Body/*";
        // Путь к телу ответа, который пришёл из ГИС.
        internal const string SoapContentXpathResponce     = "soap:Envelope/soap:Body/*";
        internal const string SoapBodyXpath                = "soapenv:Envelope/soapenv:Body";
        internal const string SoapHeaderXpath              = "soapenv:Envelope/soapenv:Header";
        internal const string SignatureName                = "Signature";
    
        internal const string GetStateMethodName           = "getState";
        internal const string MessageGuidXpath             = "./base:getStateRequest/base:MessageGUID";
    }
}
