using System;
using Xades.Abstractions;
using AsyncRircGisService.Gis.Configurations.Options;
using AsyncRircGisService.Gis.Configurations.Sections;
using AsyncRircGisService.Gis.Infrastructure;

namespace AsyncRircGisService.Gis.Commands
{
    public class GetStateCommand : GisCommandBase<GetStateOptions>
    {
        public GetStateCommand(GetStateOptions option, IXadesService xadesService, SigningConfiguration signingConfig, GisServiceConfiguration serviceConfig) 
            : base(option, xadesService, signingConfig, serviceConfig)
        {
        }

        protected override bool IsSignatureRequired => false;

        protected override void OnExecute(GetStateOptions option)
        {
            var valuesDictionary = new [] { new Tuple<string, string>(Constants.MessageGuidXpath, option.MessageGuid) };
            SendRequest(option.ServiceName, Constants.GetStateMethodName, valuesDictionary);
        }
    }
}