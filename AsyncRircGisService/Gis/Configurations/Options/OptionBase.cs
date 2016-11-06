using CommandLine;

namespace AsyncRircGisService.Gis.Configurations.Options
{
    public abstract class OptionBase
    {
        [Option('v', "verbose", Default = false, HelpText = "Включить отладочный вывод")]
        public bool Verbose { get; set; }
    }
}
