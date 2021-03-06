﻿using Xades.Abstractions;
using Xades.Helpers;
using AsyncRircGisService.Gis.Configurations.Options;
using AsyncRircGisService.Gis.Configurations.Sections;

namespace AsyncRircGisService.Gis.Commands
{
    public class VerifyCommand : XadesCommandBase<VerifyOptions>
    {
        public VerifyCommand(VerifyOptions option, IXadesService xadesService, SigningConfiguration signingConfig) : base(option, xadesService, signingConfig)
        {
        }

        protected override void OnExecute(VerifyOptions option)
        {
            Info($"Выполняется чтение файла {option.InputFileName}...");
            var xmlDocument = XmlDocumentHelper.Load(option.InputFileName);
            var elementId = option.Element;

            Info("Проверка подписи файла...");
            Validate(xmlDocument, elementId);
            Success("Подпись элемента верна");
        }
    }
}