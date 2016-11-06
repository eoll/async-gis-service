using System.Data;
using System.Collections.Generic;
using AsyncRircGisService.TaskUnit;
using AsyncRircGisService.Oracle;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using System;

namespace AsyncRircGisService.TaskRegistry
{
    /// <summary>
    /// Получает и обрабатывает очередной список задач для выполнения с помощью сервисов ГИС ЖКХ.
    /// </summary>
    public class Registrator : IRegistrator
    {

        #region PrivateFields

        // Очередь задач на выполнение метода сервиса ГИС ЖКХ.
        private Queue<TaskUnit.DataPack> taskDataQueue = new Queue<TaskUnit.DataPack>();

        /// <summary>
        /// Флаг показывает наличие задач на выполнение.
        /// </summary>
        private bool hasTasks;

        #endregion

        #region PrivateMethods

        /// <summary>
        /// Делает запрос к Oracle на получение очередного набор задач на регистрации.
        /// </summary>
        /// <returns>Таблица задач на регистрации.</returns>
        private DataTable GetTaskDataFromOracle()
        {
            // Параметры для модели.
            TaskRegistryParameters taskRegistryParameters = new TaskRegistryParameters
            {
                connectSettings = string.Empty,
                isTest = ServiceConfig.Conformation.IsTest

        };

            // Параметры передаем в конструкторе.
            TaskRegistryModel registryModel = new TaskRegistryModel(taskRegistryParameters);

            // Делаем выборку из БД.
            registryModel.Select();

            // Возвращаем набор данных из БД.
            return registryModel.ResultData;
        }


        /// <summary>
        /// Создает очередь задач из массива данных задач из Oracle.
        /// </summary>
        /// <param name="TaskDataTable">Очередной набор задач на регистрации.</param>
        private void CreateQueueDataPack( ref DataTable TaskDataTable )
        {
            TaskUnit.DataPack taskDataPack;

            if( TaskDataTable.Rows.Count > 0 )
            {
                hasTasks = true;

                for( int i = 0; i < TaskDataTable.Rows.Count; i++ )
                {
                    // Выбираем очередную строку из массива данных.
                    DataRow dr = TaskDataTable.Rows[i];

                    // Формируем DataPack.
                    taskDataPack = new DataPack
                    {
                        TaskId        = dr.Field<System.Int32>( "TASK_ID"                   ).ToString(),
                        OrgId         = dr.Field<System.Int32>( "ORG_ID"                    ).ToString(),
                        ServiceId     = dr.Field<System.Int16>( "SERVICE_ID"                ).ToString(),
                        MethodId      = dr.Field<System.Int16>( "METHOD_ID"                 ).ToString(),
                        LastStartDate = dr.Field<string>      ( "TO_CHAR(LAST_START_DATE)"  ),
                        Attempt       = dr.Field<System.Int16>( "ATTEMPT"                   ),
                        Priority      = dr.Field<System.Int16>( "PRIORITY"                  )
                    };
                    // Заносим DataPack в очередь задач.
                    taskDataQueue.Enqueue( taskDataPack );
                }
            }
            else { hasTasks = false; }

        }
        #endregion


        #region PublicProperties

        /// <summary>
        /// Очередь задач на выполнение метода сервиса ГИС ЖКХ.
        /// </summary>
        public Queue<TaskUnit.DataPack> TaskDataQueue { get { return taskDataQueue; } }

        /// <summary>
        /// Флаг показывает наличие задач на выполнение.
        /// </summary>
        public bool HasTasks { get { return hasTasks; } }

        #endregion


        #region PublicMethods

        /// <summary>
        /// Запрашивает очередной набор исходных задач из Oracle и помещает эти данные в очередь.
        /// </summary>
        public void ProvideTaskData()
        {

            // 1. Создать объект-модель регистратора.
            // 2. Выполнить запрос на получение задач из Oracle в виде DataTable.
            DataTable taskData = GetTaskDataFromOracle();

            // 3. Внести данные очередной задачи в структуру AsyncRircGisService.TaskUnit.DataPack.
            // 4. Добавить сформированную структуру задачи в очередь.
            CreateQueueDataPack( ref taskData );

        }

#endregion

    }
}
