using System;
using System.Linq;
using Xades.Abstractions;
using AsyncRircGisService.Gis.Configurations.Options;
using AsyncRircGisService.Gis.Configurations.Sections;
using AsyncRircGisService.Gis.Helpers;

namespace AsyncRircGisService.Gis.Commands
{
    public class SendCommand : GisCommandBase<SendOptions>
    {
        public SendCommand(SendOptions option, IXadesService xadesService,  SigningConfiguration signingConfig, GisServiceConfiguration serviceConfig) 
            : base(option, xadesService, signingConfig, serviceConfig)
        {
        }

        protected override bool IsSignatureRequired => true;

        protected override void OnExecute(SendOptions option)
        {
            var valueDictionary = Enumerable.Empty<Tuple<string, string>>();
            if (!string.IsNullOrEmpty(option.ParametersFileName))
            {
              //  Info($"Чтение данных из {option.ParametersFileName}...");
              //  Console.WriteLine("Введите путь к файлу с входными параметрами для запроса");
                option.ParametersFileName = Console.ReadLine();
                //valueDictionary = Helpers.CsvHelper.ReadCsv(option.ParametersFileName);
            }

            SendRequest(option.ServiceName, option.MethodName, valueDictionary);
        }
    }
}