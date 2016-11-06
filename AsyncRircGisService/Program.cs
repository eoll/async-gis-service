using System.ServiceProcess;

namespace AsyncRircGisService
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new AsyncRircGisService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
