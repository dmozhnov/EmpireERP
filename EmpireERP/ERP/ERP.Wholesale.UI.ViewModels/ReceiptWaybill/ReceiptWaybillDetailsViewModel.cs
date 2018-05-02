using System;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ReceiptWaybill
{
    public class ReceiptWaybillDetailsViewModel
    {
        public ReceiptWaybillMainDetailsViewModel MainDetails { get; set; }        

        /// <summary>
        /// Грид накладной ожидания
        /// </summary>
        public GridData ReceiptWaybillRows { get; set; }

        /// <summary>
        /// Состояние грида по группам товаров
        /// </summary>
        public GridState ReceiptArticleGroupsGridState { get; set; }

        #region Гриды накладной с расхождениями

        /// <summary>
        /// Добавленные строки
        /// </summary>
        public GridData ReceiptWaybillAddedRowsGrid { get; set; }

        /// <summary>
        /// Состояние грида групп товаров для добавленных строк
        /// </summary>
        public GridState ReceiptWaybillAddedArticleGroupsGridState { get; set; }

        /// <summary>
        /// Строки с различиями
        /// </summary>
        public GridData ReceiptWaybillDifRowsGrid { get; set; }

        /// <summary>
        /// Состояние грида групп товаров для строк с различиями
        /// </summary>
        public GridState ReceiptWaybillDifArticleGroupsGridState { get; set; }

        /// <summary>
        /// Строки с соответствием
        /// </summary>
        public GridData ReceiptWaybillMatchRowsGrid { get; set; }

        /// <summary>
        /// Состояние грида групп товаров для строк с соответствием
        /// </summary>
        public GridState ReceiptWaybillMatchArticleGroupsGridState { get; set; }

        #endregion

        /// <summary>
        /// Грид окончательно согласованной накладной
        /// </summary>
        public GridData ReceiptWaybillApproveRowsGrid { get; set; }

        /// <summary>
        /// Грид групп товаров окончательно согласованной накладной
        /// </summary>
        public GridState ReceiptWaybillApproveArticleGroupsGridState { get; set; }

        /// <summary>
        /// Грид документов
        /// </summary>
        public GridData DocumentsGrid { get; set; }

        public Guid Id { get; set; }

        public bool IsReceipted { get; set; }
        public bool IsApproved { get; set; }
        public bool AreSumDivergences { get; set; }

        public string BackURL { get; set; }
        
        public bool AllowToEdit { get; set; }
        public bool AllowToEditProviderDocuments { get; set; }
        public bool AllowToDelete { get; set; }
        public bool AllowToCreateAccountingPriceList { get; set; }
        public bool AllowToAccept { get; set; }
        public bool AllowToCancelAcceptance { get; set; }        
        public bool AllowToReceipt { get; set; }
        public bool IsPossibilityToReceipt { get; set; }
        public bool AllowToCancelReceipt { get; set; }
        public bool AllowToApprove { get; set; }
        public bool AllowToCancelApprovement { get; set; }

        /// <summary>
        /// Заголовок кнопки отмены перевода накладной в финальный статус (приемка или согласование)
        /// </summary>
        public string CancelApprovementButtonCaption { get; set; }

        /// <summary>
        /// Разрешена ли проводка накладной задним числом
        /// </summary>
        public bool AllowToAcceptRetroactively { get; set; }
        public bool IsPossibilityToAcceptRetroactively { get; set; }
    }
}