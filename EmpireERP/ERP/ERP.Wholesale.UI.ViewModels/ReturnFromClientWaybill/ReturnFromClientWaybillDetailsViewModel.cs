using System;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ReturnFromClientWaybill
{
    public class ReturnFromClientWaybillDetailsViewModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        public ReturnFromClientWaybillMainDetailsViewModel MainDetails { get; set; }

        /// <summary>
        /// Заголовок страницы (№123 от 01.01.2011)
        /// </summary>
        public string Name { get; set; }

        public GridData RowGrid { get; set; }

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

        /// <summary>
        /// Разрешение подготовить накладную к проводке
        /// </summary>
        public bool AllowToPrepareToAccept { get; set; }
        public bool IsPossibilityToPrepareToAccept { get; set; }

        /// <summary>
        /// Разрешение отменить готовность накладной к проводке
        /// </summary>
        public bool AllowToCancelReadinessToAccept { get; set; }

        /// <summary>
        /// Разрешена ли проводка накладной
        /// </summary>
        public bool AllowToAccept { get; set; }
        public bool IsPossibilityToAccept { get; set; }

        /// <summary>
        /// Разрешена ли отмена проводки
        /// </summary>
        public bool AllowToCancelAcceptance { get; set; }

        /// <summary>
        /// Разрешена ли отгрузка товара по накладной
        /// </summary>
        public bool AllowToReceipt { get; set; }

        /// <summary>
        /// Разрешена ли отмена отгрузки товара по накладной
        /// </summary>
        public bool AllowToCancelReceipt { get; set; }

        /// <summary>
        /// Разрешено ли формировать печатные формы
        /// </summary>
        public bool AllowToPrintForms { get; set; }
    }
}