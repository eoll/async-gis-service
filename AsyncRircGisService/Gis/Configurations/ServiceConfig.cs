using System;
using System.Configuration;
using AsyncRircGisService.Gis.Configurations.Sections;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace AsyncRircGisService
{
    /// <summary>
    /// Класс для доступа к конфигурационному файлу приложения.
    /// </summary>
    public class ServiceConfig
    {
        /// <summary>
        /// Поле для записи данных из конфигурационного файла.
        /// </summary>
        private static Сonformation conformation;

        /// <summary>
        /// Свойство для получения данных файла конфигурации.
        /// </summary>
        public static Сonformation Conformation
        {
            get
            {
                Contract.EnsuresOnThrow<ArgumentNullException>( !string.IsNullOrEmpty( conformation.login                   ) );
                Contract.EnsuresOnThrow<ArgumentNullException>( !string.IsNullOrEmpty( conformation.password                ) );
                Contract.EnsuresOnThrow<ArgumentNullException>( Regex.IsMatch( conformation.RIRCorgPPAGUID, @"^\w{32}$"     ) );
                Contract.EnsuresOnThrow<ArgumentNullException>( Regex.IsMatch( conformation.certificateThumbprint, @"^\w+$" ) );
                Contract.EnsuresOnThrow<ArgumentNullException>( !string.IsNullOrEmpty( conformation.baseUrl                 ) );
                Contract.EnsuresOnThrow<ArgumentNullException>( conformation.soapConfiguration != null                        );

                return conformation;
            }
            set
            {
                conformation = value;
            }
        }

        /// <summary>
        /// Считывает конфигурацию из app.config и записывает в <see cref="Conformation"/>.
        /// </summary>
        public static void Init()
        {
            // Подготовка объекта содержащего в себе ноду из файла app.config "GisServicesConfig".
            GisServiceConfiguration section = (GisServiceConfiguration)ConfigurationManager.GetSection("GisServicesConfig");

            // Подготовка объекта содержащего в себе ноду из файла app.config "signingConfig".
            SigningConfiguration sign = (SigningConfiguration)ConfigurationManager.GetSection("signingConfig");

            // Подготовка объекта содержащего в себе ноду из файла app.config "Sender".
            SenderConfiguration sender = (SenderConfiguration)ConfigurationManager.GetSection("Sender");

            // Подготовка объекта содержащего в себе ноду из файла app.config "General".
            GeneralConfiguration general = (GeneralConfiguration)ConfigurationManager.GetSection("General");

            // Заполнение структуры сведениями из конфиг файла.
            Сonformation conf = new Сonformation
            {
                login                 = section.Login,
                password              = section.Password,
                certificateThumbprint = sign.CertificateThumbprint,
                certificatePassword   = sign.CertificatePassword,

                RIRCorgPPAGUID        = section.RIRCorgPPAGUID,
                baseUrl               = section.BaseUrl,
                //schemaVersion         = section.SchemaVersion, TODO удалить если всё норм. ТЕперь берём из базы.
                soapConfiguration     = section.SoapConfiguration,
                IsTest                = Convert.ToBoolean(section.IsTest),
                sendTo                = sender.SendTo,
                timeInterval          = Convert.ToInt32(general.TimeInterval) * 60000,
                amountAttempt         = Convert.ToInt32(general.AmountAttempt)
            };

            conformation = conf;
        }
    }
}
