using System.Configuration;

namespace AsyncRircGisServiceTests.Gis.Configurations.Sections
{
    public class GisServiceConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("Services")]
        public ServiceCollection Services => (ServiceCollection)(base["Services"]);

        [ConfigurationProperty( "RIRCorgPPAGUID", IsRequired = true)]
        public string RIRCorgPPAGUID
        {
            get { return (string)base["RIRCorgPPAGUID"]; }
            set { base["RIRCorgPPAGUID"] = value; }
        }

        [ConfigurationProperty("SchemaVersion", IsRequired = true)]
        public string SchemaVersion
        {
            get { return (string)base["SchemaVersion"]; }
            set { base["SchemaVersion"] = value; }
        }

        [ConfigurationProperty("BaseUrl", IsRequired = true)]
        public string BaseUrl
        {
            get { return (string)base["BaseUrl"]; }
            set { base["BaseUrl"] = value; }
        }

        [ConfigurationProperty("SoapConfiguration", IsRequired = true)]
        public SoapConfiguration SoapConfiguration
        {
            get { return (SoapConfiguration)base["SoapConfiguration"]; }
            set { base["SoapConfiguration"] = value; }
        }

        [ConfigurationProperty("Login", IsRequired = true)]
        public string Login
        {
            get { return (string)base["Login"]; }
            set { base["Login"] = value; }
        }

        [ConfigurationProperty("Password", IsRequired = true)]
        public string Password
        {
            get { return (string)base["Password"]; }
            set { base["Password"] = value; }
        }

        [ConfigurationProperty( "IsTest", IsRequired = true )]
        public string IsTest
        {
            get { return ( string )base["IsTest"]; }
            set { base["IsTest"] = value; }
        }
    }
}