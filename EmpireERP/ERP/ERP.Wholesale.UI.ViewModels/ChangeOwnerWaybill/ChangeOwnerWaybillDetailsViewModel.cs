using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ChangeOwnerWaybill
{
    public class ChangeOwnerWaybillDetailsViewModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Главные детали
        /// </summary>
        public ChangeOwnerWaybillMainDetailsViewModel MainDetails { get; set; }

        /// <summary>
        /// Грид строк накладной
        /// </summary>
        public GridData ChangeOwnerWaybillRows { get; set; }

        /// <summary>
        /// Состояние грида с группами товаров накладной
        /// </summary>
        public GridState ChangeOwnerWaybillArticleGroupsGridState { get; set; }

        /// <summary>
        /// Грид документов
        /// </summary>
        public GridData DocGrid { get; set; }

        /// <summary>
        /// Обратный адресс
        /// </summary>
        public string BackURL { get; set; }

        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// название накладной
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Признак отгрузки
        /// </summary>
        public bool IsShipped { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToDelete { get; set; }
        public bool AllowToPrepareToAccept { get; set; }
        public bool IsPossibilityToPrepareToAccept { get; set; }
        public bool AllowToCancelReadinessToAccept { get; set; }
        public bool AllowToAccept { get; set; }
        public bool IsPossibilityToAccept { get; set; }
        public bool AllowToCancelAcceptance { get; set; }
        public bool AllowToPrintForms { get; set; }
    }
}
