using System;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.WriteoffWaybill
{
    public class WriteoffWaybillDetailsViewModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        public WriteoffWaybillMainDetailsViewModel MainDetails { get; set; }

        /// <summary>
        /// Заголовок страницы (№123 от 01.01.2011)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Грид позиций накладной
        /// </summary>
        public GridData RowGrid { get; set; }

        /// <summary>
        /// Состояние грида с группами товара 
        /// </summary>
        public GridState ArticleGroupGridState { get; set; }

        /// <summary>
        /// Грид документов накладной
        /// </summary>
        public GridData DocumentGrid { get; set; }

        /// <summary>
        /// Адрес возврата
        /// </summary>
        public string BackURL { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToDelete { get; set; }
        public bool AllowToWriteoff { get; set; }
        public bool IsPossibilityToWriteoff { get; set; }
        public bool AllowToCancelWriteoff { get; set; }
        public bool AllowToPrintForms { get; set; }

        public bool AllowToPrepareToAccept{ get; set; }
        public bool IsPossibilityToPrepareToAccept { get; set; }
        public bool AllowToCancelReadinessToAccept { get; set; }

        public bool AllowToAccept { get; set; }
        public bool IsPossibilityToAccept { get; set; }
        public bool AllowToCancelAcceptance { get; set; }
    }
}