using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Накладная реализации товаров
    /// </summary>
    public class ExpenditureWaybill : SaleWaybill
    {
        #region Свойства

        #region Основные свойства

        /// <summary>
        /// Статус накладной
        /// </summary>
        public virtual ExpenditureWaybillState State { get; protected internal set; }

        /// <summary>
        /// Место хранения-отправитель
        /// </summary>
        public virtual Storage SenderStorage { get; protected set; }

        /// <summary>
        /// Находится ли накладная в логическом состоянии "Новая"
        /// </summary>
        public override bool IsNew
        {
            get { return IsDraft || IsReadyToAccept; }
        }

        /// <summary>
        /// Является ли накладная черновиком
        /// </summary>
        public virtual bool IsDraft
        {
            get { return State == ExpenditureWaybillState.Draft; }
        }

        /// <summary>
        /// Подготовлена ли накладная к проводке
        /// </summary>
        public virtual bool IsReadyToAccept
        {
            get { return State == ExpenditureWaybillState.ReadyToAccept; }
        }

        /// <summary>
        /// Проведена ли накладная
        /// </summary>
        public override bool IsAccepted
        {
            get
            {
                return State.ContainsIn(ExpenditureWaybillState.ReadyToShip, ExpenditureWaybillState.ArticlePending,
                   ExpenditureWaybillState.ConflictsInArticle, ExpenditureWaybillState.ShippedBySender);
            }
        }

        /// <summary>
        /// Отгружена ли накладная
        /// </summary>
        public override bool IsShipped
        {
            get { return (State == ExpenditureWaybillState.ShippedBySender); }
        }

        /// <summary>
        /// Дата завершения срока отсрочки платежа (СОП)
        /// </summary>
        public virtual DateTime? FinalPaymentDate
        {
            get { return IsShipped ? ShippingDate.Value.AddDays(Quota.PostPaymentDays.Value).Date : (DateTime?)null; }
        }

        /// <summary>
        /// Округлять ли отпускную цену товаров до целого
        /// </summary>
        public virtual bool RoundSalePrice { get; set; }

        /// <summary>
        /// Пользователь, отгрузивший накладную
        /// </summary>
        public virtual User ShippedBy { get; protected set; }

        /// <summary>
        /// Дата отгрузки реализованного товара со склада
        /// </summary>
        public virtual DateTime? ShippingDate { get; protected set; }

        #endregion

        #region Показатели

        /// <summary>
        /// Сумма товаров в закупочных ценах
        /// </summary>
        public virtual decimal PurchaseCostSum
        {
            get
            {
                return Rows.Sum(x => Math.Round(x.As<ExpenditureWaybillRow>().ReceiptWaybillRow.PurchaseCost * x.SellingCount, 6));
            }
        }

        /// <summary>
        /// Сумма товаров в учетных ценах места хранения - отправителя
        /// </summary>
        public virtual decimal SenderAccountingPriceSum
        {
            get { return senderAccountingPriceSum ?? 0; }
            set
            {
                if (senderAccountingPriceSum != value)
                {
                    if (IsAccepted && senderAccountingPriceSum != null)
                    {
                        throw new Exception("Невозможно установить значение суммы в учетных ценах, т.к. накладная уже проведена.");
                    }

                    senderAccountingPriceSum = value;
                }
            }
        }
        private decimal? senderAccountingPriceSum;

        /// <summary>
        /// Сумма товаров в отпускных ценах
        /// </summary>
        public virtual decimal SalePriceSum
        {
            get { return salePriceSum ?? 0; }
            set
            {
                if (salePriceSum != value)
                {
                    if (IsAccepted && salePriceSum != null)
                    {
                        throw new Exception("Невозможно установить значение суммы в отпускных ценах, т.к. накладная уже проведена.");
                    }

                    salePriceSum = value;
                }
            }
        }
        private decimal? salePriceSum;

        /// <summary>
        /// Итоговая сумма скидки
        /// </summary>
        protected internal virtual decimal TotalDiscountSum
        {
            get 
            {
                ValidationUtils.Assert(IsAccepted, "Невозможно получить сумму скидки для непроведенной накладной.");
                
                return SenderAccountingPriceSum - SalePriceSum;
            }
        }

        /// <summary>
        /// Итоговый процент скидки
        /// </summary>
        protected internal override decimal TotalDiscountPercent
        {
            get 
            {
                ValidationUtils.Assert(IsAccepted, "Невозможно получить процент скидки для непроведенной накладной.");
                        
                return SenderAccountingPriceSum == 0 ? 0 : Math.Round(((SenderAccountingPriceSum - SalePriceSum) / SenderAccountingPriceSum) * 100, 2); 
            }
        }

        /// <summary>
        /// Просрочка платежа (превышение допустимого срока отсрочки платежа) в днях (возвращает 0, если нет просрочки)
        /// </summary>
        public virtual int PaymentDelay
        {
            get
            {
                DateTime currentDate = DateTimeUtils.GetCurrentDateTime().Date;

                // Полностью оплаченные и не отгруженные накладные не рассматриваем
                if (IsFullyPaid || !IsShipped)
                {
                    return 0;
                }

                // Накладные по квоте с предоплатой не рассматриваем                
                if (Quota.IsPrepayment || !Quota.PostPaymentDays.HasValue)
                {
                    return 0;
                }

                // Если срок отсрочки платежа == 0, это значит, что разрешается неограниченная просрочка
                if (Quota.PostPaymentDays.Value == 0)
                {
                    return 0;
                }

                // Иначе вычисляем дату завершения срока отсрочки платежа и проверяем
                DateTime finalPaymentDate = FinalPaymentDate.Value;
                if (finalPaymentDate >= currentDate)
                {
                    return 0;
                }

                return (int)Math.Ceiling((currentDate - finalPaymentDate).TotalDays);
            }
        }

        #endregion
                
        #endregion

        #region Конструкторы

        protected ExpenditureWaybill()
            : base(WaybillType.ExpenditureWaybill)
        {
        }

        public ExpenditureWaybill(string number, DateTime date, Storage senderStorage, Deal deal, Team team, DealQuota quota, bool isPrepayment, 
            User curator, DeliveryAddressType deliveryAddressType, string deliveryAddress, DateTime creationDate, User createdBy) :
            base(WaybillType.ExpenditureWaybill, number, date, deal, team, quota, isPrepayment, curator, deliveryAddressType, deliveryAddress, creationDate, createdBy)
        {
            ValidationUtils.NotNull(senderStorage, "Не указано место хранения отправителя.");
            ValidationUtils.NotNull(quota, "Не указана квота по сделке.");
            ValidationUtils.NotNull(deal.Contract, "Невозможно создать накладную реализации товаров по сделке, для которой не создан договор.");

            if (quota.IsPrepayment && !isPrepayment)
            {
                throw new Exception("Невозможно создать накладную с отсрочкой платежа по данной квоте. Возможно, действующая квота была изменена.");
            }
            if (!deal.Contract.AccountOrganization.Storages.Contains(senderStorage))
            {
                throw new Exception(String.Format("Место хранения «{0}» не связано с собственной организацией «{1}», указанной в договоре по данной сделке.", 
                    senderStorage.Name,deal.Contract.AccountOrganization.ShortName));
            }

            State = ExpenditureWaybillState.Draft;
            SenderStorage = senderStorage;
        }

        #endregion

        #region Методы

        #region Добавление / редактирование позиций

        /// <summary>
        /// Добавление позиции в накладную
        /// </summary>
        public virtual void AddRow(ExpenditureWaybillRow row)
        {
            if (IsAccepted)
            {
                throw new Exception(String.Format("Невозможно добавить позицию в накладную со статусом «{0}».", State.GetDisplayName()));
            }

            if (Rows.Any(x => ((ExpenditureWaybillRow)x).ReceiptWaybillRow.Id == row.ReceiptWaybillRow.Id))
            {
                throw new Exception("Позиция накладной по данной партии и товару уже добавлена.");
            }

            rows.Add(row);
            row.ExpenditureWaybill = this;
        }

        /// <summary>
        /// Удаление позиции из накладной
        /// </summary>
        public virtual void DeleteRow(ExpenditureWaybillRow row)
        {
            if (IsAccepted)
            {
                throw new Exception(String.Format("Невозможно удалить позицию из накладной со статусом «{0}».", State.GetDisplayName()));
            }

            if (!rows.Contains(row))
            {
                throw new Exception("Позиция накладной не найдена. Возможно, она была удалена.");
            }

            rows.Remove(row);
            row.DeletionDate = DateTime.Now;            
        }

        #endregion

        #region Обновление статуса накладной на основании статусов ее позиций

        /// <summary>
        /// Обновление статуса накладной на основании статусов ее позиций
        /// </summary>
        protected internal virtual void UpdateStateByRowsState()
        {
            ValidationUtils.Assert(!IsShipped, "Невозможно обновить статус отгруженной накладной по ее позициям.");
            
            if (rows.Any(x => ((ExpenditureWaybillRow)x).OutgoingWaybillRowState == OutgoingWaybillRowState.Conflicts))
            {
                State = ExpenditureWaybillState.ConflictsInArticle;
            }
            else if (Rows.Any(x => x.As<ExpenditureWaybillRow>().OutgoingWaybillRowState.ContainsIn(OutgoingWaybillRowState.ArticlePending, OutgoingWaybillRowState.Undefined)))
            {
                State = ExpenditureWaybillState.ArticlePending;
            }
            else
            {
                State = ExpenditureWaybillState.ReadyToShip;
            }
        }

        #endregion

        #region Подготовка / отмена готовности к проводке

        public virtual void PrepareToAccept()
        {
            CheckPossibilityToPrepareToAccept();

            State = ExpenditureWaybillState.ReadyToAccept;
        }

        public virtual void CancelReadinessToAccept()
        {
            CheckPossibilityToCancelReadinessToAccept();

            State = ExpenditureWaybillState.Draft;
        }

        #endregion

        #region Проводка и отмена проводки

        /// <summary>
        /// Проводка накладной
        /// </summary>
        public virtual void Accept(IEnumerable<ArticleAccountingPrice> senderPriceLists, bool useReadyToAcceptState, User acceptedBy, DateTime currentDateTime)
        {
            CheckPossibilityToAccept(useReadyToAcceptState);
            
            decimal _salePriceSum = 0M, _senderAccountingPriceSum = 0M;
            foreach (var row in rows.Select(x => x as ExpenditureWaybillRow))
            {
                // Выставляем реестры, по которым формируется учетная цена отправителя
                var senderPriceList = senderPriceLists.FirstOrDefault(x => x.Article == row.Article);
                ValidationUtils.NotNull(senderPriceList, String.Format("Невозможно выполнить проводку, так как на товар «{0}» не установлена учетная цена.",
                    row.Article.FullName));
                
                row.SenderArticleAccountingPrice = senderPriceList;
                
                _senderAccountingPriceSum += Math.Round(senderPriceList.AccountingPrice * row.SellingCount, 2);
                _salePriceSum += Math.Round(row.SalePrice.Value * row.SellingCount, 2);
            }

            UpdateStateByRowsState();

            this.senderAccountingPriceSum = _senderAccountingPriceSum;
            this.salePriceSum = _salePriceSum;

            AcceptanceDate = currentDateTime;
            AcceptedBy = acceptedBy;
        }

        /// <summary>
        /// Отмена проводки
        /// </summary>
        public virtual void CancelAcceptance(bool useReadyToAcceptState)
        {
            CheckPossibilityToCancelAcceptance();

            State = useReadyToAcceptState ? ExpenditureWaybillState.ReadyToAccept : ExpenditureWaybillState.Draft;

            // Сбрасываем реестры, по которым формируется учетная цена на отправителе
            foreach (var item in rows)
            {
                var row = item.As<ExpenditureWaybillRow>();
                
                row.SenderArticleAccountingPrice = null;

                // если позиция без источника, то сбрасываем статус в «Не определено»
                if (row.IsUsingManualSource == false)
                {
                    row.OutgoingWaybillRowState = OutgoingWaybillRowState.Undefined;
                }
            }
            
            SenderAccountingPriceSum = 0;
            SalePriceSum = 0;

            AcceptanceDate = null;
            AcceptedBy = null;
        }

        #endregion

        #region Отгрузка / отмена отгрузки

        /// <summary>
        /// Отгрузка товара по накладной
        /// </summary>
        public virtual void Ship(User shippedBy, DateTime currentDateTime)
        {
            CheckPossibilityToShip();

            foreach (var row in Rows)
            {
                // все реализованное кол-во делаем доступным для возврата
                row.AvailableToReturnCount = row.SellingCount;
            }

            State = ExpenditureWaybillState.ShippedBySender;
            ShippingDate = currentDateTime;
            ShippedBy = shippedBy;
        }

        /// <summary>
        /// Отмена отгрузки
        /// </summary>
        public virtual void CancelShipping()
        {
            CheckPossibilityToCancelShipping();

            foreach (var row in Rows)
            {
                // обнуляем доступное для возврата кол-во
                row.AvailableToReturnCount = 0;
            }

            // принудительно выставляем статус "Готово к отгрузке" и корректируем его в зависимости от статуса позиций накладной
            State = ExpenditureWaybillState.ReadyToShip;
            ShippingDate = null;
            ShippedBy = null;
            UpdateStateByRowsState();
        }

        #endregion

        #region Проверки на возможность выполнения операций

        /// <summary>
        /// Проверка возможности редактирования накладной
        /// </summary>
        public virtual void CheckPossibilityToEdit()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно изменить накладную со статусом «{0}».", State.GetDisplayName()));
        }

        public virtual void CheckPossibilityToDelete()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно удалить накладную со статусом «{0}».", State.GetDisplayName()));
            ValidationUtils.Assert(Deal.IsActive, "Невозможно удалить накладную реализации по закрытой сделке.");
        }

        public virtual void CheckPossibilityToDeleteRow()
        {
            CheckPossibilityToEdit();
            ValidationUtils.Assert(Deal.IsActive, "Невозможно удалить позицию накладной по закрытой сделке.");
        }

        public virtual void CheckPossibilityToPrepareToAccept()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно подготовить к проводке накладную со статусом «{0}».", State.GetDisplayName()));
            ValidationUtils.Assert(RowCount > 0, "Невозможно подготовить к проводке накладную, не содержащую ни одной позиции.");
        }

        public virtual void CheckPossibilityToCancelReadinessToAccept()
        {
            ValidationUtils.Assert(!IsAccepted, "Невозможно отменить готовность к проводке для проведенной накладной.");
            ValidationUtils.Assert(!IsShipped, "Невозможно отменить готовность к проводке для отгруженной накладной.");
            ValidationUtils.Assert(IsReadyToAccept, String.Format("Невозможно отменить готовность к проводке для накладной со статусом «{0}».", State.GetDisplayName()));
        }

        /// <summary>
        /// Проверка возможности проводки накладной
        /// </summary>
        public virtual void CheckPossibilityToAccept(bool useReadyToAcceptState)
        {
            ValidationUtils.Assert(useReadyToAcceptState ? IsReadyToAccept : IsNew,
                String.Format("Невозможно провести накладную из состояния «{0}».", State.GetDisplayName()));
            ValidationUtils.Assert(RowCount > 0, "Невозможно провести накладную, не содержащую ни одной позиции.");
        }

        /// <summary>
        /// Проверка возможности отмены проводки
        /// </summary>
        public virtual void CheckPossibilityToCancelAcceptance()
        {
            ValidationUtils.Assert(IsAccepted, "Невозможно отменить проводку непроведенной накладной.");  
            ValidationUtils.Assert(!IsShipped, "Невозможно отменить проводку отгруженной накладной.");
            ValidationUtils.Assert(Deal.IsActive, "Невозможно отменить проводку накладной реализации товара по закрытой сделке.");
            
            // TODO Как реализация может быть с отсрочкой по квоте с предоплатой?
            if (!IsPrepayment && Quota.IsPrepayment)
            {
                throw new Exception("Невозможно отменить проводку накладной с отсрочкой платежа, так как квота накладной предусматривает предоплату.");
            }
        }

        /// <summary>
        /// Проверка возможности отгрузки товара по накладной
        /// </summary>
        public virtual void CheckPossibilityToShip()
        {
            ValidationUtils.Assert(State == ExpenditureWaybillState.ReadyToShip,
                String.Format("Невозможно отгрузить товар по накладной из состояния «{0}».", State.GetDisplayName()));
            
            ValidationUtils.Assert(!IsPrepayment || IsFullyPaid, "Невозможно отгрузить неоплаченную накладную с предоплатой.");
        }

        /// <summary>
        /// Проверка возможности отмены отгрузки товара по накладной
        /// </summary>
        public virtual void CheckPossibilityToCancelShipping()
        {
            ValidationUtils.Assert(IsShipped, "Невозможно отменить отгрузку еще неотгруженной накладной.");
            ValidationUtils.Assert(Deal.IsActive, "Невозможно отменить отгрузку по закрытой сделке.");
            ValidationUtils.Assert(!Rows.Any(x => x.ReservedByReturnCount > 0), "Невозможно отменить отгрузку накладной, т.к. по ней есть возвраты.");
        }

        /// <summary>
        /// Проверка на возможность изменить команду
        /// </summary>
        public override void CheckPossibilityToEditTeam()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно изменить команду для накладной в состоянии «{0}».", State.GetDisplayName()));
        }

        /// <summary>
        /// Проверка возможности изменить куратора
        /// </summary>
        public override void CheckPossibilityToChangeCurator()
        {
            ValidationUtils.Assert(IsNew, String.Format("Невозможно изменить куратора для накладной со статусом «{0}».", State.GetDisplayName()));
        }

        /// <summary>
        /// Проверка возможности изменить дату накладной
        /// </summary>
        public virtual void CheckPossibilityToChangeDate()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно изменить дату накладной со статусом «{0}».", State.GetDisplayName()));
        }

        #endregion


        #endregion
    }
}
