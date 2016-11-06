using System.Data;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System;
using AsyncRircGisService.Oracle;
using System.Linq;
using System.Diagnostics;

namespace AsyncRircGisService.TaskUnit
{
    /// <summary>
    /// Действия для объекта-обработчика задачи, реализуемой методом сервиса ГИС ЖКХ.
    /// </summary>
    [ContractClass( typeof( ContractForOriginTask ) )]
    public abstract class OriginTask
    {
        #region PrivateFields

        /// <summary>
        /// Данные входящей задачи от регистратора.
        /// </summary>
        private DataPack taskDataPack;

        /// <summary>
        /// Данные задачи из базы Oracle.
        /// </summary>
        protected DataTable taskOracleData;

        /// <summary>
        /// Данные подзадачи, извлеченные из общего массива данных исходной задачи <see cref="taskOracleData"/>.
        /// </summary>
        protected List<object> subtaskDataList;

        /// <summary>
        /// Статус задачи.
        /// </summary>
        protected int taskStatusField;

        #endregion

        #region PublicProperties

        /// <summary>
        /// Данные задачи из базы Oracle
        /// </summary>
        public DataTable TaskOracleData
        {
            get
            {
                return taskOracleData;
            }
        }

        /// <summary>
        /// Данные входящей задачи.
        /// </summary>
        public DataPack TaskDataPack
        {
            get
            {
                return taskDataPack;
            }
            set
            {
                taskDataPack = value;
            }
        }

        #endregion

        #region PrivateMethods

        /// <summary>
        /// Поставляет данные задачи из базы Oracle в <see cref="taskOracleData"/>
        /// </summary>
        protected abstract void ProvideTaskOracleData();

        /// <summary>
        /// Возвращает данные о задаче в рамках выполнения запроса к сервисам ГИС ЖКХ.
        /// А именно:
        /// <see cref="DataPack.Path"/>, 
        /// <see cref="DataPack.AddOrgPpaGuid"/>, 
        /// <see cref="DataPack.AddSignature"/>, 
        /// <see cref="DataPack.TemplatePath"/>, 
        /// <see cref="DataPack.Action"/>,
        /// <see cref="DataPack.TemplateResponsePath"/>,
        /// <see cref="DataPack.ActionResponce"/>,
        /// <see cref="DataPack.MethodVersion"/>.
        /// </summary>
        protected void ProvideTaskInfo()
        {
            // Создаем параметры модели.
            OriginTaskParameters originTaskParameters = new OriginTaskParameters
            {
                taskId           = taskDataPack.TaskId,
                connectSettings  = string.Empty,
                operationsSelect = OriginTaskParameters.selectOperation.ProvideTaskInfo
            };

            // Скармливаем конструктору.
            OriginTaskModel originTaskModel = new OriginTaskModel(originTaskParameters);

            // Выполняем выборку данных.
            originTaskModel.Select();

            // Заполняем параметры задачи полученными из БД данными.
            taskDataPack.Path                 = originTaskModel.ResultData.Rows[0].Field<string>( "PATH"            );
            taskDataPack.TemplatePath         = originTaskModel.ResultData.Rows[0].Field<string>( "REQUEST_PATH"    );
            taskDataPack.Action               = originTaskModel.ResultData.Rows[0].Field<string>( "REQUEST_ACTION"  );
            taskDataPack.TemplateResponsePath = originTaskModel.ResultData.Rows[0].Field<string>( "RESPONSE_PATH"   );
            taskDataPack.ActionResponce       = originTaskModel.ResultData.Rows[0].Field<string>( "RESPONSE_ACTION" );
            taskDataPack.MethodVersion        = originTaskModel.ResultData.Rows[0].Field<string>( "VERSION"         );
            taskDataPack.OrgPPAGUID           = originTaskModel.ResultData.Rows[0].Field<string>( "ORG_PPAGUID"     );

            taskDataPack.AddOrgPpaGuid        = Convert.ToBoolean( originTaskModel.ResultData.Rows[0].Field<System.Int16>( "ADD_ORG_PPA_GUID" ) );
            taskDataPack.AddSignature         = Convert.ToBoolean( originTaskModel.ResultData.Rows[0].Field<System.Int16>( "ADD_SIGNATURE"    ) );

#if DEBUG
            Notificator.Write("PATH: "             + taskDataPack.Path                     + " " + 
                              "REQUEST_PATH: "     + taskDataPack.TemplatePath             + " " +
                              "REQUEST_ACTION: "   + taskDataPack.Action                   + " " +
                              "RESPONSE_PATH: "    + taskDataPack.TemplateResponsePath     + " " +
                              "RESPONSE_ACTION: "  + taskDataPack.ActionResponce           + " " +
                              "VERSION: "          + taskDataPack.MethodVersion            + " " +
                              "ORG_PPAGUID: "      + taskDataPack.OrgPPAGUID               + " " +
                              "ADD_ORG_PPA_GUID: " + taskDataPack.AddOrgPpaGuid.ToString() + " " +
                              "ADD_SIGNATURE: "    + taskDataPack.AddSignature.ToString()
                              );
#endif
        }

        /// <summary>
        /// Выборка массива значений у которых значение в первой колонке равно item.
        /// </summary>
        /// <param name="item">Значение по которым выбираются данные.</param>
        /// <returns></returns>
        private DataTable GetSubtaskData( string item )
        {
            var chunkOfData = from c in taskOracleData.AsEnumerable()
                              where c["REQUESTER_MESSAGE_GUID"].ToString() == item
                              select c;

            return chunkOfData.CopyToDataTable();
        }

        /// <summary>
        /// Выполняет преобразования данных задачи <see cref="ProvideTaskOracleData"/> в картриджи,
        /// каждый из которых представляет собой, в свою очередь, данные подзадачи.
        /// </summary>
        protected void ProcessTaskOracleData()
        {

            // Извлекаем уникальные значения в колонке REQUESTER_MESSAGE_GUID.
            var distinctMessageGuid = ( from i in taskOracleData.AsEnumerable()
                                        select i["REQUESTER_MESSAGE_GUID"] ).Distinct().ToList();

            subtaskDataList = distinctMessageGuid;

        }


        /// <summary>
        /// Выполняет асинхронно очередную подзадачу на запрос или ответ с помощью менеджера взаимодействия с ГИС ЖКХ.
        /// </summary>
        /// <param name="subtaskDataTable">Порция данных Oracle в форме <see cref="DataTable"/>, которая соответсвует подзадаче.</param>
        /// <returns>Ожидаемая задача</returns>
        protected async Task PerformSubtaskAsync( List<object> subtaskDataList )
        {
            List<Task> taskList = new List<Task>();

            foreach ( var item in subtaskDataList )
            {
                Task task = Task.Run( ()=> { ExecuteSubtask(item); } );

                taskList.Add( task );
            }

            var processingTasks = taskList.Select(  async t => { await t; }  ).ToArray();

            // Ожидаем выполнения всех задач.
            await Task.WhenAll( processingTasks );

        }

        private void ExecuteSubtask( object item )
        {
            try
            {
                // Извлекаем кадр - подзадачу.
                DataTable oracleData = GetSubtaskData( item.ToString() );

                // Просим фабрику сгенерить нам нужный объект
                var creator = new SubtaskCreator( TaskDataPack );

                // Создаем подзадачу на запрос в ГИС.
                var subtask = creator.GetSubtask();

                // Скармливаем данные подзадачи
                subtask.OracleData = oracleData;

                // Формируем набор данных для выполнения запроса в ГИС.
                subtask.InitGisDataPack( item.ToString() );

                // Выполняем запрос к сервису ГИС ЖКХ.
                subtask.PerformGisRequest();

                // Формируем набор данных для получения результатов запроса в ГИС ЖКХ.
                subtask.InitGisResultDataPack();

                // Получаем результат работы сервиса ГИС ЖКХ.
                subtask.PerformGisRequestForResults();

                // Записываем результаты работы сервиса в Oracle.
                subtask.WriteGisResultsToOracle();
            }
            catch ( Exception ex )
            {
                Notificator.Write( "Ошибка. Task_id = " + TaskDataPack.TaskId + ", REQUESTER_MESSAGE_GUID = " + item.ToString() + ". OriginTask. ExecuteSubtask().  Сообщение об ошибке: " + ex.Message + "Stack Trace: " + ex.StackTrace, EventLogEntryType.Error );
            }
        }

        /// <summary>
        /// Проверяет количество повторых запусков задачи. Если больше чем в конфиге, то Exception.
        /// </summary>
        private void CheckAttempts()
        {
            if ( taskDataPack.Attempt > ServiceConfig.Conformation.amountAttempt )
            {
                // Прерываем выполнение задачи.
                AbortTask();

                throw new Exception( "Количество попыток выполнения задачи" + "Task_id = " + taskDataPack.TaskId + "превышено. Допустимо: " + ServiceConfig.Conformation.amountAttempt );
            }
        }

        /// <summary>
        /// Устанавливает статус задачи в БД на "прервана".
        /// </summary>
        private void AbortTask()
        {
            OriginTaskParameters originParameters = new OriginTaskParameters
            {
                taskId = taskDataPack.TaskId,
                taskStatus = -1
            };

            OriginTaskModel originTaskModel = new OriginTaskModel( originParameters );

            originTaskModel.Update();
        }

        #endregion

        #region PublicMethods

        /// <summary>
        /// Разбирает параметры задачи <see cref="DataPack"/>.
        /// При необходимости догружает сведения для выполнения задачи из базы данных Oracle.
        /// </summary>
        public void Prepare()
        {
            // Проверяем сколько раз выполнялась задача.
            CheckAttempts();

            // Дозаполнить параметры задачи дополнительными данными, необходимыми для совершения запроса к сервису ГИС ЖКХ.
            ProvideTaskInfo();

            // На основании данных задачи догружаем фактические данные из базы.
            ProvideTaskOracleData();

            // Полученные рабочие данные мы разбиваем на кадры - подзадачи с ключом requester id типа DataTable.
            // Вносим эти кадры в очередь.
            ProcessTaskOracleData();

        }

        /// <summary>
        /// Ставит на выполнение все подзадачи очереди задач <see cref="subtaskDataList"/>.
        /// Этот метод работает со списком подзадач.
        /// </summary>
        public async Task Perform()
        {
            await PerformSubtaskAsync( subtaskDataList );
        }


        /// <summary>
        /// Проверяет полноту выполнения всех подзадач центральной задачи.
        /// </summary>
        public void Check()
        {

            // Заполняем параметры запроса.
            OriginTaskParameters originParameters = new OriginTaskParameters
            {
                taskId           = taskDataPack.TaskId,
                lastStartDate    = taskDataPack.LastStartDate,
                connectSettings  = string.Empty,
                operationsSelect = OriginTaskParameters.selectOperation.Check
            };

            OriginTaskModel originTaskModel = new OriginTaskModel( originParameters );

            originTaskModel.Select();

            taskStatusField = Convert.ToInt32( originTaskModel.ResultData.Rows[0][0] );

        }



        /// <summary>
        /// Регистрирует статус результата выполнения задачи в Oracle.
        /// </summary>
        public void Complete()
        {

            OriginTaskParameters originParameters = new OriginTaskParameters
            {
                taskId     = taskDataPack.TaskId,
                taskStatus = taskStatusField
            };

            OriginTaskModel originTaskModel = new OriginTaskModel(originParameters);

            originTaskModel.Update();

        }

        #endregion

    }

    #region Contract

    [ContractClassFor( typeof( OriginTask ) )]
    abstract class ContractForOriginTask : OriginTask
    {

        ///// <summary>
        ///// Разбирает параметры задачи <see cref="DataPack"/>.
        ///// При необходимости догружает сведения для выполнения задачи из базы данных Oracle.
        ///// </summary>
        //public override void Prepare()
        //{
        //    Contract.Requires( Regex.IsMatch( TaskDataPack.TaskId, @"^\d+$" ) );
        //    Contract.Requires( Regex.IsMatch( TaskDataPack.MugkxId, @"^\d+$" ) );
        //    Contract.Requires( Regex.IsMatch( TaskDataPack.DuId, @"^\d+$" ) );
        //    Contract.Requires( Regex.IsMatch( TaskDataPack.ServiceId, @"^\d+$" ) );
        //    Contract.Requires( Regex.IsMatch( TaskDataPack.MethodId, @"^\d+$" ) );
        //    Contract.Requires( Regex.IsMatch( TaskDataPack.LastStartDate, @"^(\d+\.){2}\d+\s(\d+\:){2}\d+.+$" ) );
        //}


        /// <summary>
        /// Поставляет данные задачи из базы Oracle.
        /// </summary>
        protected override void ProvideTaskOracleData()
        {
            Contract.Ensures( taskOracleData.IsInitialized );
            Contract.Ensures( !taskOracleData.HasErrors );
        }

    }

    #endregion

}
