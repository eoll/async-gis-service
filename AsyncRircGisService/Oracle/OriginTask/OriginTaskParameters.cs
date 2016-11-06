using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncRircGisService.TaskUnit;

namespace AsyncRircGisService.Oracle
{
    /// <summary>
    /// Параметры  модели OriginTaskModel, которая обслуживает класс OriginTask, метод ProvideTaskInfo.
    /// </summary>
    public struct OriginTaskParameters
    {
        /// connectSettings - параметры подключения к Oracle
        public string connectSettings;

        public string taskId;

        public int taskStatus;

        public string lastStartDate;

        public selectOperation operationsSelect;

        // Перечисление для выбора, какой вариант метода Select будет использован.
        public enum selectOperation
        {
            Null,
            Check,
            ProvideTaskInfo
        };
    }
}
