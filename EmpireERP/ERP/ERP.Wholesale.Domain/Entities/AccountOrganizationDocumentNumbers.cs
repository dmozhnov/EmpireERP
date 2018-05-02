using ERP.Utils;
using ERP.Infrastructure.Entities;
namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Последние номера документов собственной организации
    /// </summary>
    public class AccountOrganizationDocumentNumbers: Entity<int>
    {
        /// <summary>
        /// Организация - собственник для которой действуют номера
        /// </summary>
        public virtual AccountOrganization AccountOrganization { get; set; }

        /// <summary>
        /// Год, в котором действуют указанные номера
        /// </summary>
        public virtual int Year { get; set; }

        #region Последние использованные номера накладных

        /// <summary>
        /// Последний использованный номер накладной прихода
        /// </summary>
        public virtual decimal ReceiptWaybillLastNumber { get; set; }

        /// <summary>
        /// Последний использованный номер накладной перемещения
        /// </summary>
        public virtual decimal MovementWaybillLastNumber { get; set; }

        /// <summary>
        /// Последний использованный номер накладной смены собственника
        /// </summary>
        public virtual decimal ChangeOwnerWaybillLastNumber { get; set; }

        /// <summary>
        /// Последний использованный номер накладной реализации
        /// </summary>
        public virtual decimal ExpenditureWaybillLastNumber { get; set; }

        /// <summary>
        /// Последний использованный номер накладной списания
        /// </summary>
        public virtual decimal WriteoffWaybillLastNumber { get; set; }

        /// <summary>
        /// Последний использованный номер накладной возврата от клиента
        /// </summary>
        public virtual decimal ReturnFromClientWaybillLastNumber { get; set; }

        #endregion

        #region Конструкторы
        protected AccountOrganizationDocumentNumbers()
        { }

        public AccountOrganizationDocumentNumbers(int year, AccountOrganization accountOrganization)
        {
            ValidationUtils.NotNull(accountOrganization, "Не указана собственная организация.");

            this.AccountOrganization = accountOrganization;
            this.Year = year;
            ReceiptWaybillLastNumber = 0;
            ReturnFromClientWaybillLastNumber = 0;
            MovementWaybillLastNumber = 0;
            ExpenditureWaybillLastNumber = 0;
            ChangeOwnerWaybillLastNumber = 0;
            WriteoffWaybillLastNumber = 0;
        }
        #endregion
    }
}
