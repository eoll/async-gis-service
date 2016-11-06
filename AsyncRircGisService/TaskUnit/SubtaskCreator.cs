using System;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using AsyncRircGisService.Exceptions;

namespace AsyncRircGisService.TaskUnit
{
    /// <summary>
    /// Класс создатель объекта.
    /// Создаёт объект определённого типа ориентируясь на передаваемый <see cref="DataPack"/>.
    /// </summary>
    public class SubtaskCreator
    {
        private DataPack taskDataPack;
        /// <summary>
        /// Заполняет поле класса из параметров задачи.
        /// </summary>
        /// <param name="dataPack">Входящие параметры задачи.</param>
        public SubtaskCreator( DataPack dataPack )
        {
            Contract.Requires( Regex.IsMatch( dataPack.ServiceId, @"^\d+$" ) );
            Contract.Requires( Regex.IsMatch( dataPack.MethodId, @"^\d+$" ) );

            // Заполняем поле класса данными из задачи.
            taskDataPack = dataPack;
        }

        /// <summary>
        /// Возвращает объект типа-подзадачи на основе переданных в конструкторе данных задачи.
        /// </summary>
        /// <returns>Объект - подзадача.</returns>
        public Subtask GetSubtask()
        {

            if      ( taskDataPack.ServiceId == "1" && taskDataPack.MethodId == "1" ) return new ExportOrgRegistrySubtask         ( taskDataPack );
            else if ( taskDataPack.ServiceId == "2" && taskDataPack.MethodId == "1" ) return new ExportNsiListSubtask             ( taskDataPack );
            else if ( taskDataPack.ServiceId == "2" && taskDataPack.MethodId == "2" ) return new ExportNsiItemSubtask             ( taskDataPack );
            else if ( taskDataPack.ServiceId == "3" && taskDataPack.MethodId == "1" ) return new ExportDataProviderNsiItemSubtask ( taskDataPack );

            else throw new NotFoundException( "По данным ServiceId = " + taskDataPack.ServiceId + " и MethodId = " + taskDataPack.MethodId + " класс подзадача не найден" );

        }
    }
}
