using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Таможенный лист
    /// </summary>
    public class ProductionOrderCustomsDeclaration : Entity<Guid>
    {
        #region Свойства

        #region Основные свойства

        /// <summary>
        /// Заказ, к которому относится данный таможенный лист
        /// </summary>
        public virtual ProductionOrder ProductionOrder { get; protected internal set; }

        /// <summary>
        /// Номер ГТД
        /// </summary>
        /// <remarks>не более 33 символов</remarks>
        public virtual string CustomsDeclarationNumber { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string Name { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public virtual DateTime Date
        {
            get { return date; }
            set { date = value.Date; } // время пока не храним
        }
        protected DateTime date;

        /// <summary>
        /// Сумма ввозных таможенных пошлин
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public virtual decimal ImportCustomsDutiesSum { get; set; }

        /// <summary>
        /// Сумма вывозных таможенных пошлин
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public virtual decimal ExportCustomsDutiesSum { get; set; }

        /// <summary>
        /// Сумма НДС
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public virtual decimal ValueAddedTaxSum { get; set; }

        /// <summary>
        /// Акциз
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public virtual decimal ExciseSum { get; set; }

        /// <summary>
        /// Сумма таможенных сборов
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public virtual decimal CustomsFeesSum { get; set; }

        /// <summary>
        /// Сумма КТС (корректировка таможенной стоимости)
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public virtual decimal CustomsValueCorrection { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Дата удаления
        /// </summary>
        public virtual DateTime? DeletionDate
        {
            get { return deletionDate; }
            set
            {
                if (deletionDate == null && value != null) { deletionDate = value; } // запрещаем повторную пометку об удалении
            }
        }
        protected DateTime? deletionDate;

        /// <summary>
        /// Комментарий
        /// </summary>
        /// <remarks>не более 4000 символов</remarks>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Оплаты
        /// </summary>
        public virtual IEnumerable<ProductionOrderCustomsDeclarationPayment> Payments
        {
            get { return new ImmutableSet<ProductionOrderCustomsDeclarationPayment>(payments); }
        }
        private Iesi.Collections.Generic.ISet<ProductionOrderCustomsDeclarationPayment> payments;

        #endregion

        #region Показатели и вычисляемые поля

        /// <summary>
        /// Название и номер ГТД
        /// </summary>
        public virtual string NameAndCustomsDeclarationNumber
        {
            get { return Name + " / " + (!String.IsNullOrEmpty(CustomsDeclarationNumber) ? CustomsDeclarationNumber : "---"); }
        }

        /// <summary>
        /// Общая сумма всех сумм таможенных листов
        /// </summary>
        public virtual decimal Sum
        {
            get { return ImportCustomsDutiesSum + ExportCustomsDutiesSum + ValueAddedTaxSum + ExciseSum + CustomsFeesSum + CustomsValueCorrection; }
        }

        /// <summary>
        /// Всего оплачено
        /// </summary>
        public virtual decimal PaymentSum { get { return payments.Sum(x => x.SumInCurrency); } }

        /// <summary>
        /// Неоплаченный остаток
        /// </summary>
        public virtual decimal DebtRemainder { get { return Sum - PaymentSum; } }

        /// <summary>
        /// Всего оплачено в процентах
        /// </summary>
        public virtual decimal PaymentPercent { get { return Sum != 0M ? Math.Round(PaymentSum * 100 / Sum, 2) : 0; } }

        #endregion

        // TOOD: перенести в методы
        #region Проверки на возможность выполнения операций

        #region Редактирование

        public virtual void CheckPossibilityToEdit()
        {
            // Если заказ закрыт (т.е. все его партии закрыты), ничего редактировать нельзя
            ValidationUtils.Assert(!ProductionOrder.IsClosed, "Невозможно редактировать таможенный лист по закрытому заказу.");
        }

        #endregion

        #region Удаление

        public virtual void CheckPossibilityToDelete()
        {
            // Если заказ закрыт (т.е. все его партии закрыты), ничего удалять нельзя
            ValidationUtils.Assert(!ProductionOrder.IsClosed, "Невозможно удалить таможенный лист по закрытому заказу.");
            ValidationUtils.Assert(!payments.Any(), "Невозможно удалить таможенный лист, по которому есть оплаты.");
        }

        #endregion

        #endregion

        #endregion

        #region Конструкторы

        protected ProductionOrderCustomsDeclaration()
        {
        }

        public ProductionOrderCustomsDeclaration(string name, DateTime date, decimal importCustomsDutiesSum, decimal exportCustomsDutiesSum, decimal valueAddedTaxSum,
            decimal exciseSum, decimal customsFeesSum, decimal customsValueCorrection)
        {
            CreationDate = DateTime.Now;
            
            ValidationUtils.Assert(!String.IsNullOrEmpty(name), "Название не указано.");
            ValidationUtils.Assert(importCustomsDutiesSum >= 0, "Ввозные таможенные пошлины не могут быть отрицательной величиной.");
            ValidationUtils.Assert(exportCustomsDutiesSum >= 0, "Вывозные таможенные пошлины не могут быть отрицательной величиной.");
            ValidationUtils.Assert(exciseSum >= 0, "Акциз не может быть отрицательной величиной.");
            ValidationUtils.Assert(customsFeesSum >= 0, "Таможенные сборы не могут быть отрицательной величиной.");
            ValidationUtils.Assert(!(importCustomsDutiesSum == 0 && exportCustomsDutiesSum == 0 && valueAddedTaxSum == 0 && exciseSum == 0 && customsFeesSum == 0
                && customsValueCorrection == 0), "Все суммы не могут быть равны 0.");

            Name = name;
            this.date = date;
            ImportCustomsDutiesSum = importCustomsDutiesSum;
            ExportCustomsDutiesSum = exportCustomsDutiesSum;
            ValueAddedTaxSum = valueAddedTaxSum;
            ExciseSum = exciseSum;
            CustomsFeesSum = customsFeesSum;
            CustomsValueCorrection = customsValueCorrection;
            Comment = String.Empty;
            CustomsDeclarationNumber = String.Empty;

            payments = new HashedSet<ProductionOrderCustomsDeclarationPayment>();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавление оплаты
        /// </summary>
        /// <param name="payment">Оплата</param>
        protected internal virtual void AddPayment(ProductionOrderCustomsDeclarationPayment payment)
        {
            if (payments.Contains(payment))
            {
                throw new Exception("Данная оплата уже связана с этим таможенным листом.");
            }

            payments.Add(payment);
        }

        /// <summary>
        /// Удаление оплаты (из коллекции). Дата удаления оплаты ставится в соответствующем методе заказа, который вызывает данный метод
        /// </summary>
        /// <param name="payment">Оплата</param>
        public virtual void DeletePayment(ProductionOrderCustomsDeclarationPayment payment, DateTime currentDateTime)
        {
            payments.Remove(payment);
            payment.ProductionOrder.DeletePayment(payment, currentDateTime);
        }

        #endregion
    }
}
