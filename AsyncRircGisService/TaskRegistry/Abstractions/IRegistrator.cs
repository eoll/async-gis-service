using System.Collections.Generic;
using AsyncRircGisService.TaskUnit;

namespace AsyncRircGisService.TaskRegistry
{
    /// <summary>
    /// Получает и обрабатывает очередной список задач для выполнения с помощью сервисов ГИС ЖКХ.
    /// </summary>
    interface IRegistrator
    {

        /// <summary>
        /// Очередь задач на выполнение метода сервиса ГИС ЖКХ.
        /// </summary>
        Queue<TaskUnit.DataPack> TaskDataQueue { get; }


        /// <summary>
        /// Запрашивает очередной набор исходных задач из Oracle и помещает эти данные в очередь
        /// </summary>
        void ProvideTaskData();

    }

}
