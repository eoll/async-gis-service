using System.IO;
using Xades.Abstractions;
using Xades.Helpers;
using AsyncRircGisService.Gis.Configurations.Options;
using AsyncRircGisService.Gis.Configurations.Sections;

namespace AsyncRircGisService.Gis.Commands
{
    public class SignCommand : XadesCommandBase<SignOptions>
    {
        public SignCommand(SignOptions option, IXadesService xadesService, SigningConfiguration signingConfig) : base(option, xadesService, signingConfig)
        {
        }

        protected override void OnExecute(SignOptions option)
        {
            Info($"Выпоняется чтение файла {option.InputFileName}...");
            var xmlDocument = XmlDocumentHelper.Load(option.InputFileName);
            var elementId = Option.Element;

            Info("Выполняется подпись файла...");
            var resultXmlText = Sign(xmlDocument, elementId);
            File.WriteAllText(option.OutputFileName, resultXmlText);
            Success("Файл успешно подписан");
        }
    }
}