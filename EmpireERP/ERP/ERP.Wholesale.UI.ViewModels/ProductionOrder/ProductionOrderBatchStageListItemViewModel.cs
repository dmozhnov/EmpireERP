using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    /// <summary>
    /// Строка списка этапов партии, описывающая один этап
    /// </summary>
    public class ProductionOrderBatchStageListItemViewModel
    {
        /// <summary>
        /// Код
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тип этапа
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Планируемая длительность этапа, дни
        /// </summary>
        public string PlannedDuration { get; set; }

        /// <summary>
        /// Планируемая дата начала этапа
        /// </summary>
        public string PlannedStartDate { get; set; }

        /// <summary>
        /// Порядковый номер этапа в пределах партии
        /// </summary>
        public string OrdinalNumber { get; set; }

        #region Разрешенные действия

        /// <summary>
        /// Разрешено ли редактирование
        /// </summary>
        public bool AllowToEdit { get; set; }

        /// <summary>
        /// Разрешено ли удаление
        /// </summary>
        public bool AllowToDelete { get; set; }

        /// <summary>
        /// Разрешено ли перемещение вверх
        /// </summary>
        public bool AllowToMoveUp { get; set; }

        /// <summary>
        /// Разрешено ли перемещение вниз
        /// </summary>
        public bool AllowToMoveDown { get; set; }

        #endregion
    }
}