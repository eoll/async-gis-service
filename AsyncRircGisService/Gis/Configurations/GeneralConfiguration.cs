using System.Configuration;

namespace AsyncRircGisService.Gis.Configurations.Sections
{
    class GeneralConfiguration : ConfigurationSection
    {
        [ConfigurationProperty( "TimeInterval", IsRequired = true )]
        public string TimeInterval
        {
            get { return ( string )base["TimeInterval"]; }
            set { base["TimeInterval"] = value; }
        }

        [ConfigurationProperty( "AmountAttempt", IsRequired = true )]
        public string AmountAttempt
        {
            get { return ( string )base["AmountAttempt"]; }
            set { base["AmountAttempt"] = value; }
        }
    }
}
