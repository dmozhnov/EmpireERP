using System;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.MovementWaybill
{
    public class MovementWaybillDetailsViewModel
    {
        public Guid Id { get; set; }

        public MovementWaybillMainDetailsViewModel MainDetails { get; set; }

        public GridData MovementWaybillRows { get; set; }
        public GridState MovementWaybillArticleGroupsGridState { get; set; } 

        public GridData DocGrid { get; set; }

        public string BackURL { get; set; }

        public bool AllowToEdit { get; set; }

        /// <summary>
        /// Разрешено ли удаление накладной
        /// </summary>
        public bool AllowToDelete { get; set; }

        /// <summary>
        /// Разрешено ли подготавливать накладную к проводке
        /// </summary>
        public bool AllowToPrepareToAccept { get; set; }
        public bool IsPossibilityToPrepareToAccept { get; set; }

        /// <summary>
        /// Разрешено ли отменять подготавку накладной к проводке
        /// </summary>
        public bool AllowToCancelReadinessToAccept { get; set; }

        /// <summary>
        /// Разрешена ли проводка накладной
        /// </summary>
        public bool AllowToAccept { get; set; }
        public bool IsPossibilityToAccept { get; set; }

        /// <summary>
        /// Разрешена ли отмена проводки накладной
        /// </summary>
        public bool AllowToCancelAcceptance { get; set; }
        
        /// <summary>
        /// Разрешена ли отгрузка накладной
        /// </summary>
        public bool AllowToShip { get; set; }
        public bool IsPossibilityToShip { get; set; }

        /// <summary>
        /// Разрешена ли отмена отгрузки накладной
        /// </summary>
        public bool AllowToCancelShipping { get; set; }

        /// <summary>
        /// Разрешена ли приемка накладной
        /// </summary>
        public bool AllowToReceipt { get; set; }

        /// <summary>
        /// Разрешена ли отмена приемки накладной
        /// </summary>
        public bool AllowToCancelReceipt { get; set; }

        /// <summary>
        /// Разрешено ли формировать печатные формы
        /// </summary>
        public bool AllowToPrintForms { get; set; }

        public bool AllowToPrintCashMemoForm { get; set; }
        public bool AllowToPrintInvoiceForm { get; set; }
        public bool AllowToPrintWaybillFormInSenderPrices { get; set; }
        public bool AllowToPrintWaybillFormInRecipientPrices { get; set; }
        public bool AllowToPrintWaybillFormInBothPrices { get; set; }
        public bool AllowToPrintTORG12Form { get; set; }
        public bool AllowToPrintT1Form { get; set; }
    }
}