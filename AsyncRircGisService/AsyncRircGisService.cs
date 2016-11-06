using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace AsyncRircGisService
{
    // TODO для инсталляции службы Visual Studio 2015 - Командная строка разработчика для VS2015(от имени администратора)
    // идём в папку со сборкой и выполняем installutil.exe AsyncRircGisService.exe, для удаления installutil.exe /u AsyncRircGisService.exe.
    public partial class AsyncRircGisService : ServiceBase
    {
        // Количество задач на выполнении. Данное поле хранит значение о количестве задач, которые находятся на выполнении.
        // по OnTimer увеличиваем, если задача выполнена или ошибка при выполнении уменьшаем.
        // по OnStop проверяем чтобы данное поле было <= 0, иначе не перестаём выполнять данный метод.
        int taskNumbers = 0;

        // Поле показывает запущена ли сейчас процедура остановки сервиса.
        // по OnTimer проверяем чтобы данное поле было false.
        // Иначе не делаем ничего.
        bool serviceStoped = false;

        public AsyncRircGisService()
        {
            InitializeComponent();
        }

        protected override void OnStart( string[] args )
        {
            // Инициализируем объект notificator.  
            Notificator.Init( ref SrvcEventLog );

            // Инициализируем объект конфига.
            ServiceConfig.Init();

            Notificator.Write( "Service on start" );

            // Устанавливаем интервал таймера значением из конфига.
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = ServiceConfig.Conformation.timeInterval;
            timer.Elapsed += new System.Timers.ElapsedEventHandler( this.OnTimer );
            timer.Start();
        }

        public async void OnTimer( object sender, System.Timers.ElapsedEventArgs args )
        {

            Notificator.Write( "Метод OnTimer() serviceStoped = " + serviceStoped.ToString() );

            // Проверяем не запущен ли процесс остановки сервиса.
            if ( !serviceStoped )
            {

                // Увеличиваем количество задач на единицу.
                taskNumbers++;

                Notificator.Write( "Метод OnTimer() taskNumbers увеличился и = " + taskNumbers );

                try
                {
                    await ExecuteTasksAsync();
                }
                catch ( System.Exception ex )
                {
                    Notificator.Write( "AsyncRircGisService.Ontimer() " + ex.Message + "Stack Trace: " + ex.StackTrace, EventLogEntryType.Warning );
                }
                finally
                {
                    // При любом исходе уменьшаем на единицу количество задач.
                    taskNumbers--;
                    Notificator.Write( "Метод OnTimer() taskNumbers уменьшился и = " + taskNumbers );
                }

            }
            else { ; } // не делаем ничего.
        }

        protected override void OnStop()
        {
            // Запущена процедура остановки сервиса.
            serviceStoped = true;

            Notificator.Write( "Метод OnStop() serviceStoped = " + serviceStoped.ToString() );

            // Проверяем количество задач на выполнении.
            // Выход из цикла только если количество задач <= 0.
            while ( taskNumbers > 0 )
            {
                Notificator.Write( "Метод Onstop() taskNumbers = " + taskNumbers );
                // Задержка 3 минуты.
                System.Threading.Thread.Sleep(180000); // 1 минута = 60000 мс.
            }

            Notificator.Write( "In onStop." );

        }

        protected override void OnContinue()
        {
            Notificator.Write( "In OnContinue." );
        }

        public async Task ExecuteTasksAsync()
        {
            // Создаём регистратор.
            var registrator = new TaskRegistry.Registrator();

            // Заполняем очередь задач.
            registrator.ProvideTaskData();

            // Если задачи в очереди есть...
            if ( registrator.HasTasks )
            {
                // Создаём список асинхронных задач.
                List<Task> taskList = new List<Task>();

                // Пробегаемся по очереди.
                while ( registrator.TaskDataQueue.Count > 0 )
                {
                    // Получаем очередной набор данных задачи ГИС ЖКХ.
                    var nextTaskDataPack = registrator.TaskDataQueue.Dequeue();

#if DEBUG
                    Notificator.Write( "AsyncRircGisService.ExecuteOriginTaskAsync() Выполняется задача Task_id = " + nextTaskDataPack.TaskId + " Service_id = " + nextTaskDataPack.ServiceId + " Method_id = " + nextTaskDataPack.MethodId );
#endif

                    Task task =  ExecuteOriginTaskAsync( nextTaskDataPack );

                    taskList.Add( task );
                }
                var processingTasks = taskList.Select(  async t => { await t; }  ).ToArray();

                // Ожидаем выполнения всех задач.
                await Task.WhenAll( processingTasks );
            }
            else { ; }
        }

        /// <summary>
        /// Выполняет задачу по взаимодействию с ГИС ЖКХ.
        /// </summary>
        /// <param name="taskDataPack"></param>
        /// <returns></returns>
        public async Task ExecuteOriginTaskAsync( TaskUnit.DataPack taskDataPack )
        {
            // Отлавливаем исключения, которые относятся к задаче в целом.
            try
            {
                // Получаем объект-обработчик метода сервиса ГИС ЖКХ.
                var nextOriginTask = TaskUnit.Manager.MethodProcessor(taskDataPack);

                // Выполняет подготовительные работы перед отправкой запроса в ГИС ЖКХ.
                nextOriginTask.Prepare();

                // Выполняет запрос в ГИС ЖКХ и разбор ответа.
                await nextOriginTask.Perform();

                // Проверяет результаты работы задачи.
                nextOriginTask.Check();

                // Устанавливает статус задачи.
                nextOriginTask.Complete();
            }
            catch ( System.Exception ex )
            {
                Notificator.Write( "AsyncRircGisService.ExecuteOriginTaskAsync() Задача Task_id = " + taskDataPack.TaskId + " не выполнена. Сообщение об ошибке: " + ex.Message + "Stack Trace: " + ex.StackTrace, EventLogEntryType.Error );
            }

        }
    }
}
