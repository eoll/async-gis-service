using System.Configuration;

namespace AsyncRircGisService.Gis.Configurations.Sections
{
    public class SenderConfiguration : ConfigurationSection
    {
        [ConfigurationProperty( "SendTo", IsRequired = true )]
        public string SendTo
        {
            get { return ( string )base["SendTo"]; }
            set { base["SendTo"] = value; }
        }
    }
}
