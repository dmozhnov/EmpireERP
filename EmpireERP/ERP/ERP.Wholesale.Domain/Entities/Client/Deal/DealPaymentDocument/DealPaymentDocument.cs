using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Платежный документ по сделке
    /// </summary>
    public abstract class DealPaymentDocument : Entity<Guid>
    {
        #region Свойства

        #region Основные свойства

        /// <summary>
        /// Команда, к которой относится платежный документ
        /// </summary>
        public virtual Team Team { get; protected internal set; }

        /// <summary>
        /// Пользователь, принявший платежный документ
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Сделка, к которой относится платежный документ
        /// </summary>
        public virtual Deal Deal { get; protected internal set; }

        /// <summary>
        /// Сумма платежного документа
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public virtual decimal Sum { get; set; }

        /// <summary>
        /// Разнесенная сумма платежного документа
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public virtual decimal DistributedSum { get; set; }

        /// <summary>
        /// Тип платежного документа
        /// </summary>
        public virtual DealPaymentDocumentType Type
        {
            get
            {
                return type;
            }
            set
            {
                ValidationUtils.Assert(Enum.IsDefined(typeof(DealPaymentDocumentType), value), "Невозможно присвоить полю «Тип платежного документа» недопустимое значение.");
                type = value;
            }
        }
        private DealPaymentDocumentType type;

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Дата удаления
        /// </summary>
        public virtual DateTime? DeletionDate
        {
            get
            {
                return deletionDate;
            }
            set
            {
                // Запрещаем повторную пометку об удалении
                if (deletionDate == null && value != null)
                {
                    deletionDate = value;

                    foreach (var distribution in Distributions.ToList())
                    {
                        RemoveDealPaymentDocumentDistribution(distribution, true);
                    }
                }
            }
        }
        protected DateTime? deletionDate;

        /// <summary>
        /// Дата платежного документа
        /// </summary>
        public virtual DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value.Date; // Время не храним
            }
        }
        protected DateTime date;

        /// <summary>
        /// Разнесение платежного документа на другие сущности
        /// </summary>
        public abstract IEnumerable<DealPaymentDocumentDistribution> Distributions { get; }

        #endregion

        #region Вычисляемые свойства

        /// <summary>
        /// Сумма разнесений на накладные продажи (с учетом возвратов)
        /// </summary>
        public virtual decimal DistributedToSaleWaybillSum
        {
            // DistributedToReturnFromClientWaybillSum всегда имеет знак "-"
            get { return Distributions.Where(x => x.Is<DealPaymentDocumentDistributionToSaleWaybill>()).Sum(x => x.Sum) + DistributedToReturnFromClientWaybillSum; }
        }
        
        /// <summary>
        /// Процент оплаты
        /// </summary>
        public virtual decimal? PaymentPercent
        {
            get { return Sum != 0M ? (DistributedSum * 100M / Sum) : (decimal?)null; }
        }

        /// <summary>
        /// Неразнесенная сумма платежного документа
        /// </summary>
        public virtual decimal UndistributedSum
        {
            get { return Sum - DistributedSum; }
        }

        /// <summary>
        /// Сумма, разнесенная на накладные возврата от клиента
        /// </summary>
        public virtual decimal DistributedToReturnFromClientWaybillSum
        {
            get { return Distributions.Where(x => x.Is<DealPaymentDocumentDistributionToReturnFromClientWaybill>()).Sum(x => x.Sum); }
        }

        /// <summary>
        /// Сумма, разнесенная на возвраты оплат клиенту из данного платежного документа
        /// </summary>
        public virtual decimal PaymentToClientSum
        {
            get
            {
                ValidationUtils.Assert(Type.ContainsIn(DealPaymentDocumentType.DealPaymentFromClient, DealPaymentDocumentType.DealCreditInitialBalanceCorrection),
                    "Неверный тип платежного документа");

                return Distributions.Where(x => x.Is<DealPaymentDocumentDistributionToDealPaymentDocument>())
                    .Where(x => x.As<DealPaymentDocumentDistributionToDealPaymentDocument>().DestinationDealPaymentDocument.Type == DealPaymentDocumentType.DealPaymentToClient)
                    .Sum(x => x.Sum);
            }
        }

        /// <summary>
        /// Сумма, разнесенная на корректировки сальдо (в коллекции не могут быть корректировки разных типов, вычитающихся друг из друга)
        /// </summary>
        public virtual decimal InitialBalancePaymentSum
        {
            get
            {
                // В зависимости от типа данного документа  связанные с ним документы будут находиться в поле SourceDealPaymentDocument или DestinationDealPaymentDocument
                return Type.ContainsIn(DealPaymentDocumentType.DealPaymentFromClient, DealPaymentDocumentType.DealCreditInitialBalanceCorrection) ?

                    Distributions.Where(x => x.Is<DealPaymentDocumentDistributionToDealPaymentDocument>())
                    .Where(x => x.As<DealPaymentDocumentDistributionToDealPaymentDocument>().DestinationDealPaymentDocument.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection)
                    .Sum(x => x.Sum) :

                    Distributions.Where(x => x.SourceDealPaymentDocument.Type == DealPaymentDocumentType.DealCreditInitialBalanceCorrection)
                    .Sum(x => x.Sum);
            }
        }

        /// <summary>
        /// Признак того, что платежный документ полностью разнесен
        /// </summary>
        public virtual bool IsFullyDistributed { get; set; }

        #endregion

        #endregion

        #region Конструкторы

        protected DealPaymentDocument() {}

        protected DealPaymentDocument(Team team, User user, DealPaymentDocumentType type, DateTime date, decimal sum, DateTime currentDate)
        {
            CreationDate = currentDate;
            Date = date;
            Team = team;
            User = user;
            CheckDate(currentDate);

            Sum = sum;
            CheckSum();

            Type = type;
        }

        #endregion

        #region Методы

        #region Разнесения платежного документа на другие сущности

        /// <summary>
        /// Добавление разнесения платежного документа к данной сущности. Переопределяется в потомках
        /// </summary>
        /// <param name="dealPaymentDocumentDistribution">Добавляемое разнесение платежного документа</param>
        /// <param name="updateSecondEntity">Добавлять ли разнесение в коллекцию второй сущности, с которой данная сущность связана этим разнесением</param>
        public abstract void AddDealPaymentDocumentDistribution(DealPaymentDocumentDistribution dealPaymentDocumentDistribution, bool updateSecondEntity = true);
        
        /// <summary>
        /// Удаление разнесения платежного документа из данной сущности. Переопределяется в потомках
        /// </summary>
        /// <param name="paymentDistribution">Удаляемое разнесение платежного документа</param>
        /// <param name="updateSecondEntity">Удалять ли разнесение из коллекции второй сущности, с которой данная сущность связана этим разнесением</param>
        public abstract void RemoveDealPaymentDocumentDistribution(DealPaymentDocumentDistribution dealPaymentDocumentDistribution, bool updateSecondEntity = true);
        
        #endregion

        #region Внутренние проверки

        /// <summary>
        /// Проверка суммы на 0 и отрицательные значения. Переопределяется в потомках
        /// </summary>
        public abstract void CheckSum();
        
        /// <summary>
        /// Проверка даты. Переопределяется в потомках
        /// </summary>
        public abstract void CheckDate(DateTime currentDate);
        
        #endregion

        #region Проверки на возможность выполнения операций

        public virtual void CheckPossibilityToDelete()
        {
            foreach (var dealPaymentDocumentDistribution in Distributions)
            {
                // Платеж от клиента и кредитовую корректировку сальдо нельзя удалить, если они разнесены на отгруженную накладную реализации с предоплатой,
                // либо на возврат оплаты клиенту
                if (Type.ContainsIn(DealPaymentDocumentType.DealCreditInitialBalanceCorrection, DealPaymentDocumentType.DealPaymentFromClient))
                {
                    // Если разнесение на накладную реализации
                    if (dealPaymentDocumentDistribution.Is<DealPaymentDocumentDistributionToSaleWaybill>())
                    {
                        var dealPaymentDocumentDistributionToSaleWaybill = dealPaymentDocumentDistribution.As<DealPaymentDocumentDistributionToSaleWaybill>();
                        ValidationUtils.Assert(!(dealPaymentDocumentDistributionToSaleWaybill.SaleWaybill.IsPrepayment && dealPaymentDocumentDistributionToSaleWaybill.SaleWaybill.IsShipped),
                            String.Format("Невозможно удалить платежный документ, разнесенный на отгруженную накладную с предоплатой: {0}.", dealPaymentDocumentDistributionToSaleWaybill.SaleWaybill.Name));
                    }
                    // Если разнесение на возврат оплаты клиенту
                    else if (dealPaymentDocumentDistribution.Is<DealPaymentDocumentDistributionToDealPaymentDocument>())
                    {
                        ValidationUtils.Assert(!dealPaymentDocumentDistribution.As<DealPaymentDocumentDistributionToDealPaymentDocument>().DestinationDealPaymentDocument
                            .Is<DealPaymentToClient>(), "Невозможно удалить платежный документ, средства по которому возвращены клиенту.");
                    }
                }
            }
        }

        public virtual void CheckPossibilityToRedistribute()
        {
            ValidationUtils.Assert(!IsFullyDistributed, "Переразносить можно только те платежные документы, которые не разнесены полностью.");
            ValidationUtils.Assert(this.Is<DealCreditInitialBalanceCorrection>() || this.Is<DealPaymentFromClient>(), "Невозможно совершить переразнесение по данному типу оплаты.");
        }

        #endregion

        #endregion
    }
}
