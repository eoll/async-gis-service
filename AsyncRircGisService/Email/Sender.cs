using System;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;

namespace AsyncRircGisService.Email
{
    /// <summary>
    /// Отправка сообщений на Email.
    /// </summary>
    public class Sender
    {
        /// <summary>
        /// Отправка письма на почтовый ящик C# mail send.
        /// </summary>
        /// <param name="smtpServer">Имя SMTP-сервера.</param>
        /// <param name="from">Адрес отправителя.</param>
        /// <param name="password">пароль к почтовому ящику отправителя.</param>
        /// <param name="mailto">Адрес получателя.</param>
        /// <param name="caption">Тема письма.</param>
        /// <param name="message">Сообщение.</param>
        /// <param name="attachFile">Присоединенный файл.</param>
        public static void SendMail( string smtpServer, string from, string password, string mailto, string caption, string message, string attachFile = null )
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress( from );
                mail.To.Add( new MailAddress( mailto ) );
                mail.Subject = caption;
                mail.Body = message;
                if( !string.IsNullOrEmpty( attachFile ) )
                    mail.Attachments.Add( new Attachment( attachFile ) );
                SmtpClient client = new SmtpClient();
                client.Host = smtpServer;
                client.Port = 25;
                client.EnableSsl = false;
                client.Credentials = new NetworkCredential( from.Split( '@' )[0], password );
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send( mail );
                mail.Dispose();
            }
            catch( Exception e )
            {
                throw new Exception( "Mail.Send: " + e.Message );
            }
        }
        /// <summary>
        /// Отправка письма на почтовый ящик C# mail send.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        /// <param name="attachFile">Присоединенный файл(не обязательно).</param>
        public static void SendMail( string message, string attachFile = null )
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress( @"test@mail.ru" );

            mail.To.Add( new MailAddress( ServiceConfig.Conformation.sendTo ) );

            mail.Subject = @"Ошибка работы службы AsyncRircGisService";

            mail.Body = message;

            if( !string.IsNullOrEmpty( attachFile ) )
                mail.Attachments.Add( new Attachment( attachFile ) );

            SmtpClient client = new SmtpClient();
            client.Host = "test@mail.ru";
            client.Port = 25;
            client.EnableSsl = false;
            // Учётные данные почтового ящика.
            client.Credentials = new NetworkCredential( @"test@mail.ru", @"test" );
            client.Send( mail );
            mail.Dispose();
        }
    }
}
