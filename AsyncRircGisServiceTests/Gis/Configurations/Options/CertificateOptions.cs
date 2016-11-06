using CommandLine;

namespace AsyncRircGisServiceTests.Gis.Configurations.Options
{
    [Verb("list-certs", HelpText = "Отобразить список сертификатов, установленных в локальное хранилище пользователя")]
    public class CertificateOptions : OptionBase
    {
    }
}
