using System.Diagnostics;

namespace AsyncRircGisService
{
    /// <summary>
    /// Предназначен для уведомления администратора о наступившем событии с помошью лога сервиса и e-mail сообщения.
    /// </summary>
    public static class Notificator
    {
        /// <summary>
        /// Ссылка на журнал событий.
        /// </summary>
        private static EventLog sysEventLog;

        /// <summary>
        /// Свойство для доступа к журналу событий.
        /// </summary>
        public static EventLog SysEventLog { get { return sysEventLog; } }

        /// <summary>
        /// Инициализирует журнал событий windows.
        /// </summary>
        public static void Init( ref EventLog eventLog )
        {
            eventLog = new EventLog();

            // Записываем ссылку на системный журнал.
            sysEventLog = eventLog;

            if( !EventLog.SourceExists( "RircGisService" ) )
            {
                EventLog.CreateEventSource(
                    "RircGisService", "RircGisServiceLog" );
            }
            eventLog.Source = "RircGisService";
            eventLog.Log = "RircGisServiceLog";
        }

        /// <summary>
        /// Пишет в журнал событий.
        /// </summary>
        /// <param name="message">Строка для записи в журнал событий.</param>
        public static void Write( string message )
        {

          SysEventLog.WriteEntry( message );

        }

        /// <summary>
        /// Пишет в журнал событий.
        /// </summary>
        /// <param name="message">Строка для записи в журнал событий.</param>
        /// <param name="type">Одно из значений EventLogEntryType.</param>
        public static void Write( string message, EventLogEntryType type )
        {
            // Пишем в журнал событий.
            SysEventLog.WriteEntry( message, type );

#if RELEASE
            if( type == EventLogEntryType.Error ) Email.Sender.SendMail(message);
#endif

        }

    }
}
