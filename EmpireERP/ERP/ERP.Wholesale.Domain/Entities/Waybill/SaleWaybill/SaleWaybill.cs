using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Накладная реализации
    /// </summary>
    public abstract class SaleWaybill : BaseWaybill
    {
        #region Свойства

        #region Основные свойства

        /// <summary>
        /// Сделка, в рамках которой производится реализация
        /// </summary>
        public virtual Deal Deal { get; set; }

        /// <summary>
        /// Учитываемая при реализации квота
        /// </summary>
        public virtual DealQuota Quota
        {
            get
            {
                return quota;
            }
            set
            {
                ValidationUtils.Assert(Deal.Quotas.Contains(value), String.Format("Квота «{0}» не связана со сделкой «{1}».", value.Name, Deal.Name));

                // Запрещаем смену квоты для проведенной накладной (ее обнуление при отмене проводки происходит после смены состояния)
                if (IsAccepted)
                {
                    throw new Exception("Невозможно сменить квоту у проведенной накладной.");
                }
                else
                {
                    quota = value;
                }
            }
        }
        private DealQuota quota;

        /// <summary>
        /// Организация-отправитель
        /// </summary>
        public virtual AccountOrganization Sender
        {
            get
            {
                ValidationUtils.NotNull(Deal, "Не установлена сделка.");
                ValidationUtils.NotNull(Deal.Contract, "Не установлен договор по сделке.");

                return Deal.Contract.AccountOrganization;
            }
        }

        /// <summary>
        /// Ставка НДС
        /// </summary>
        public virtual ValueAddedTax ValueAddedTax { get; set; }

        /// <summary>
        /// Сумма НДС. Имеет смысл только в случае проведенной накладной (зафиксированы учетные цены)
        /// </summary>
        public virtual decimal ValueAddedTaxSum
        {
            get { return Rows.Sum(x => x.ValueAddedTaxSum); }
        }

        /// <summary>
        /// Находится ли накладная в логическом состоянии "Новая"
        /// </summary>
        public abstract bool IsNew { get; }

        /// <summary>
        /// Проведена ли накладная
        /// </summary>
        public abstract bool IsAccepted { get; }

        /// <summary>
        /// Отгружена ли накладная
        /// </summary>
        public abstract bool IsShipped { get; }

        /// <summary>
        /// Признак того, что реализация полностью оплачена
        /// </summary>
        public virtual bool IsFullyPaid { get; set; }

        /// <summary>
        /// По накладной предусмотрена предоплата
        /// </summary>
        public virtual bool IsPrepayment
        {
            get
            {
                return isPrepayment;
            }
            set
            {
                // Запрещаем смену формы взаиморасчетов для проведенной накладной
                ValidationUtils.Assert(!IsAccepted, "Невозможно сменить форму взаиморасчетов у проведенной накладной.");
                
                isPrepayment = value;
            }
        }
        private bool isPrepayment;

        /// <summary>
        /// Дата удаления
        /// </summary>        
        public virtual DateTime? DeletionDate
        {
            get { return deletionDate; }
            set
            {
                // запрещаем повторную пометку об удалении
                if (deletionDate == null && value != null)
                {
                    deletionDate = value;

                    foreach (var row in rows)
                    {
                        if (row.DeletionDate == null)
                        {
                            row.DeletionDate = deletionDate;
                        }
                    }
                }
            }
        }
        protected DateTime? deletionDate;

        /// <summary>
        /// Адрес поставки
        /// </summary>
        /// <remarks>не более 250 символов, необязательное</remarks>
        public virtual string DeliveryAddress { get; set; }

        /// <summary>
        /// Тип адреса поставки
        /// </summary>
        public virtual DeliveryAddressType DeliveryAddressType { get; set; }

        /// <summary>
        /// Команда, к которой относится реализация
        /// </summary>
        /// <remarks>Может быть null</remarks>
        public virtual Team Team
        {
            get { return team; }
            set
            {
                CheckPossibilityToEditTeam();
                team = value;
            }
        }
        private Team team;

        #endregion

        #region Коллекция разнесений платежных документов

        public virtual IEnumerable<DealPaymentDocumentDistributionToSaleWaybill> Distributions
        {
            get { return new ImmutableSet<DealPaymentDocumentDistributionToSaleWaybill>(distributions); }
        }
        private Iesi.Collections.Generic.ISet<DealPaymentDocumentDistributionToSaleWaybill> distributions = new HashedSet<DealPaymentDocumentDistributionToSaleWaybill>();

        #endregion

        #region Позиции накладной

        /// <summary>
        /// Позиции накладной
        /// </summary>
        /// 
        public new virtual IEnumerable<SaleWaybillRow> Rows
        {
            get { return new ImmutableSet<BaseWaybillRow>(rows).Select(x => x.As<SaleWaybillRow>()); }
        }

        /// <summary>
        /// Количество позиций накладной
        /// </summary>
        public virtual int RowCount
        {
            get { return rows.Count; }
        }
        #endregion

        #region Показатели

        /// <summary>
        /// Процент скидки по квоте
        /// </summary>
        protected internal abstract decimal TotalDiscountPercent { get; }

        #endregion

        #endregion

        #region Конструкторы

        protected SaleWaybill() {}

        protected SaleWaybill(WaybillType waybillType)
            : base(waybillType)
        {
        }

        protected SaleWaybill(WaybillType waybillType, string number, DateTime date, Deal deal, Team team, DealQuota quota, bool isPrepayment, User curator, 
            DeliveryAddressType deliveryAddressType, string deliveryAddress, DateTime creationDate, User createdBy)
            : base(waybillType, number, date, curator, createdBy, creationDate)
        {            
            ValidationUtils.Assert(deliveryAddressType == DeliveryAddressType.CustomAddress ? !String.IsNullOrEmpty(deliveryAddress) : true,
                "Адрес поставки не может быть пустым при ручном указании адреса поставки.");
            ValidationUtils.NotNull(team, "Необходимо указать команду.");
            
            Deal = deal;
            this.team = team;
            Quota = quota;
            IsPrepayment = isPrepayment;
            IsFullyPaid = false;
            DeliveryAddressType = deliveryAddressType;
            DeliveryAddress = deliveryAddress;
        }

        #endregion

        #region Методы

        #region Разнесения платежного документа на данную сущность

        /// <summary>
        /// Добавление разнесения платежного документа на данную сущность
        /// </summary>
        /// <param name="paymentDistribution"></param>
        public virtual void AddDealPaymentDocumentDistribution(DealPaymentDocumentDistributionToSaleWaybill dealPaymentDocumentDistributionToSaleWaybill)
        {
            dealPaymentDocumentDistributionToSaleWaybill.SetOrdinalNumber(Distributions);
            distributions.Add(dealPaymentDocumentDistributionToSaleWaybill);
        }

        /// <summary>
        /// Удаление разнесения платежного документа на данную сущность
        /// </summary>
        /// <param name="paymentDistribution"></param>
        public virtual void RemoveDealPaymentDocumentDistribution(DealPaymentDocumentDistributionToSaleWaybill dealPaymentDocumentDistributionToSaleWaybill)
        {
            ValidationUtils.Assert(distributions.Contains(dealPaymentDocumentDistributionToSaleWaybill),
                "Расшифровка распределения платежного документа не связана с данной накладной реализации.");
            distributions.Remove(dealPaymentDocumentDistributionToSaleWaybill);
        }

        #endregion

        #region Проверка на возможность совершения операций

        /// <summary>
        /// Проверка на возможность изменения команды
        /// </summary>
        public abstract void CheckPossibilityToEditTeam();

        #endregion

        #endregion
    }
}
