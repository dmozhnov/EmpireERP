using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Накладная смены собственника
    /// </summary>
    public class ChangeOwnerWaybill : BaseWaybill
    {
        #region Свойства

        /// <summary>
        /// Место хранения
        /// </summary>
        public virtual Storage Storage
        {
            get { return storage; }
            protected set
            {
                ValidationUtils.NotNull(value, "Место хранения-отправитель не указано.");
                
                if (storage != null && storage != value)
                {
                    throw new Exception("Для накладной смены собственника невозможно сменить место хранения-отправителя.");
                }
                                
                storage = value;
            }
        }
        private Storage storage;
        
        /// <summary>
        /// Организация-отправитель
        /// </summary>
        public virtual AccountOrganization Sender { get; protected set; }

        /// <summary>
        /// Организация-получатель
        /// </summary>
        public virtual AccountOrganization Recipient { get; protected set; }

        /// <summary>
        /// Статус накладной
        /// </summary>
        public virtual ChangeOwnerWaybillState State { get; protected set; }

        /// <summary>
        /// Позиции накладной
        /// </summary>
        public new virtual IEnumerable<ChangeOwnerWaybillRow> Rows
        {
            get
            {
                return new ImmutableSet<BaseWaybillRow>(rows).Select(x => x.As<ChangeOwnerWaybillRow>());
            }
        }

        /// <summary>
        /// Количество позиций в накладной
        /// </summary>
        public virtual int RowCount
        {
            get
            {
                return rows.Count;
            }
        }

        /// <summary>
        /// Находится ли накладная в логическом состоянии "Новая"
        /// </summary>
        public virtual bool IsNew
        {
            get
            {
                return State == ChangeOwnerWaybillState.Draft || State == ChangeOwnerWaybillState.ReadyToAccept;
            }
        }

        /// <summary>
        /// Является ли накладная черновиком
        /// </summary>
        public virtual bool IsDraft
        {
            get { return State == ChangeOwnerWaybillState.Draft; }
        }

        /// <summary>
        /// Готова ли накладная к проводке
        /// </summary>
        public virtual bool IsReadyToAccept
        {
            get { return State == ChangeOwnerWaybillState.ReadyToAccept; }
        }

        /// <summary>
        /// Проведена ли накладная
        /// </summary>
        public virtual bool IsAccepted
        {
            get
            {
                return !IsNew;
            }
        }

        /// <summary>
        /// Сменен ли собственник окончательно
        /// </summary>
        public virtual bool IsOwnerChanged
        {
            get
            {
                return State == ChangeOwnerWaybillState.OwnerChanged;
            }
        }

        /// <summary>
        /// Сумма товаров в закупочных ценах
        /// </summary>
        public virtual decimal PurchaseCostSum
        {
            get
            {
                return Rows.Sum(x => Math.Round(x.ReceiptWaybillRow.PurchaseCost * x.MovingCount, 6));
            }
        }

        /// <summary>
        /// Сумма товаров в учетных ценах
        /// </summary>
        public virtual decimal AccountingPriceSum
        {
            get { return accountingPriceSum ?? 0; }
            set
            {
                if (accountingPriceSum != value)
                {
                    if (IsAccepted && accountingPriceSum != null)
                    {
                        throw new Exception("Невозможно установить значение суммы в учетных ценах, т.к. накладная уже проведена.");
                    }

                    accountingPriceSum = value;
                }
            }
        }
        private decimal? accountingPriceSum;

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
        private DateTime? deletionDate;

        /// <summary>
        /// Пользователь, осуществивший смену собственника
        /// </summary>
        public virtual User ChangedOwnerBy { get; protected set; }

        /// <summary>
        /// Дата смены собственника
        /// </summary>
        public virtual DateTime? ChangeOwnerDate { get; set; }

        #endregion
        
        #region Конструкторы

        protected ChangeOwnerWaybill()
            : base(WaybillType.ChangeOwnerWaybill)
        {            
        }

        public ChangeOwnerWaybill(string number, DateTime date, Storage storage, AccountOrganization sender, AccountOrganization recipient, ValueAddedTax valueAddedTax,
            User curator, User createdBy, DateTime creationDate)
            : base(WaybillType.ChangeOwnerWaybill, number, date, curator, createdBy, creationDate)
        {
            Storage = storage;
            Sender = sender;
            Recipient = recipient;
            ValueAddedTax = valueAddedTax;
            State = ChangeOwnerWaybillState.Draft;
        }

        #endregion

        #region Методы

        #region Обновление статуса накладной на основании статусов ее позиций

        /// <summary>
        /// Обновление статуса накладной на основании статусов ее позиций
        /// </summary>
        public virtual void UpdateStateByRowsState()
        {
            ValidationUtils.Assert(!IsOwnerChanged, "Невозможно обновить статус отгруженной накладной по ее позициям.");

            if (Rows.Any(x => x.OutgoingWaybillRowState == OutgoingWaybillRowState.Conflicts))
            {
                State = ChangeOwnerWaybillState.ConflictsInArticle;
            }
            else if (Rows.Any(x => x.OutgoingWaybillRowState.ContainsIn(OutgoingWaybillRowState.ArticlePending, OutgoingWaybillRowState.Undefined)))            
            {
                State = ChangeOwnerWaybillState.ArticlePending;
            }
            else
            {
                State = ChangeOwnerWaybillState.ReadyToChangeOwner;
            }
        }

        #endregion

        #region Добавление / удаление позиций

        public virtual void AddRow(ChangeOwnerWaybillRow row)
        {
            if (IsAccepted)
            {
                throw new Exception(String.Format("Невозможно добавить позицию в накладную со статусом «{0}».", State.GetDisplayName()));
            }

            if (Rows.Any(x => x.ReceiptWaybillRow.Id == row.ReceiptWaybillRow.Id))
            {
                throw new Exception("Позиция накладной по данной партии товара уже добавлена.");
            }

            rows.Add(row);
            row.ChangeOwnerWaybill = this;
        }

        /// <summary>
        /// Удаление позиции из накладной
        /// </summary>
        public virtual void DeleteRow(ChangeOwnerWaybillRow row)
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

        #region Подготовка / Отменить готовность к проводке

        public virtual void PrepareToAccept()
        {
            CheckPossibilityToPrepareToAccept();

            State = ChangeOwnerWaybillState.ReadyToAccept;
        }

        public virtual void CancelReadinessToAccept()
        {
            CheckPossibilityToCancelReadinessToAccept();

            State = ChangeOwnerWaybillState.Draft;
        }

        #endregion

        #region Проводка/отмена проводки накладной

        public virtual void Accept(IEnumerable<ArticleAccountingPrice> articleAccountingPriceList, bool useReadyToAcceptState, User acceptedBy, DateTime currentDateTime)
        {
            CheckPossibilityToAccept(useReadyToAcceptState);

            decimal accountingPriceSum = 0M, purchaseCostSum = 0M;
            foreach (var row in Rows)
            {
                // Выставляем реестры, по которым формируется учетная цена
                var articleAccountingPrice = articleAccountingPriceList.FirstOrDefault(x => x.Article == row.Article);
                ValidationUtils.NotNull(articleAccountingPrice, String.Format("Невозможно выполнить проводку, так как на товар «{0}» не установлена учетная цена.",
                    row.Article.FullName));
                row.ArticleAccountingPrice = articleAccountingPrice;

                accountingPriceSum += articleAccountingPrice.AccountingPrice * row.MovingCount;
                purchaseCostSum += row.ReceiptWaybillRow.PurchaseCost * row.MovingCount;

                // переводим кол-во по позиции в доступное для резервирования
                row.AvailableToReserveCount = row.MovingCount;
            }

            this.accountingPriceSum = Math.Round(accountingPriceSum, 2);
 
            UpdateStateByRowsState();

            AcceptanceDate = currentDateTime;
            AcceptedBy = acceptedBy;
        }

        public virtual void CancelAcceptance(bool useReadyToAcceptState)
        {
            CheckPossibilityToCancelAcceptance();

            State = useReadyToAcceptState ? ChangeOwnerWaybillState.ReadyToAccept : ChangeOwnerWaybillState.Draft;
            AcceptanceDate = null;
            AcceptedBy = null;
            ChangedOwnerBy = null;
            ChangeOwnerDate = null;
            accountingPriceSum = null;

            foreach (var row in Rows)
            {
                // сбрасываем реестры, по которым формируется учетная цена на отправителе
                row.ArticleAccountingPrice = null;

                // обнуляем доступное для резервирования кол-во товара
                row.AvailableToReserveCount = 0;

                // если позиция без источника, то сбрасываем статус в «Не определено»
                if (row.IsUsingManualSource == false)
                {
                    row.OutgoingWaybillRowState = OutgoingWaybillRowState.Undefined;
                }
            }
        }

        #endregion

        #region Смена собственника

        public virtual void ChangeOwner(DateTime currentDateTime)
        {
            ChangeOwnerDate = currentDateTime;
            ChangedOwnerBy = AcceptedBy;	// считаем, что пользователя сменил тот же,кто и провел

            State = ChangeOwnerWaybillState.OwnerChanged;
        }

        #endregion

        #region Смена получателя

        public virtual void ChangeRecipient(AccountOrganization recipient)
        {
            CheckPossibilityToChangeRecipient();

            if (!Storage.AccountOrganizations.Any(x => x.Id == recipient.Id))
            {
                throw new Exception(String.Format("Собственная организация «{0}» не связана с местом хранения «{1}».", recipient.ShortName, Storage.Name));
            }

            Recipient = recipient;
        }

        #endregion

        #region Проверки на возможность выполнения операций

        public virtual void CheckPossibilityToChangeRecipient()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно сменить получателя для накладной со статусом «{0}».", State.GetDisplayName()));
        }
        
        public virtual void CheckPossibilityToEdit()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно редактировать накладную со статусом «{0}».", State.GetDisplayName()));
        }
        
        public virtual void CheckPossibilityToDelete()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно удалить накладную со статусом «{0}».", State.GetDisplayName()));
            ValidationUtils.Assert(!Rows.Any(x => x.AreOutgoingWaybills), "Невозможно удалить накладную, позиции которой участвуют в дальнейшем товародвижении.");
        }
        
        public virtual void CheckPossibilityToCreateRow()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно добавить позицию в накладную со статусом «{0}».", State.GetDisplayName()));
        }

        public virtual void CheckPossibilityToEditRow()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно редактировать позицию накладной со статусом «{0}».", State.GetDisplayName()));
        }

        public virtual void CheckPossibilityToPrepareToAccept()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно подготовить к проводке накладную со статусом «{0}».", State.GetDisplayName()));
            ValidationUtils.Assert(RowCount > 0, "Невозможно подготовить к проводке накладную, не содержащую ни одной позиции.");
        }

        public virtual void CheckPossibilityToCancelReadinessToAccept()
        {
            ValidationUtils.Assert(!IsDraft, "Накладная еще не подготовлена к проводке.");
            ValidationUtils.Assert(!IsAccepted, "Накладная уже проведена.");
        }

        public virtual void CheckPossibilityToAccept(bool useReadyToAcceptState)
        {
            ValidationUtils.Assert(useReadyToAcceptState ? IsReadyToAccept : IsNew,
                    String.Format("Невозможно провести накладную из состояния «{0}».", State.GetDisplayName()));
            ValidationUtils.Assert(RowCount > 0, "Невозможно провести накладную без позиций.");
        }

        public virtual void CheckPossibilityToCancelAcceptance()
        {
            ValidationUtils.Assert(IsAccepted, String.Format("Невозможно отменить проводку накладной со статусом «{0}».", State.GetDisplayName()));
            ValidationUtils.Assert(!Rows.Any(x => x.AreOutgoingWaybills), "Невозможно отменить проводку накладной, т.к. ее позиции участвуют в дальнейшем товародвижении.");
        }

        public virtual void CheckPossibilityToCancelOwnerChange()
        {
            foreach (var row in Rows)
            {
                if (row.ShippedCount > 0 || row.FinallyMovedCount > 0)
                {
                    throw new Exception("Невозможно отменить отгрузку накладной, т.к. ее позиции участвуют в дальнейшем товародвижении.");
                }
            }
        }

        public override void CheckPossibilityToChangeCurator()
        {
            ValidationUtils.Assert(State == ChangeOwnerWaybillState.Draft || State == ChangeOwnerWaybillState.ReadyToAccept,
                String.Format("Невозможно изменить куратора для накладной со статусом «{0}».", State.GetDisplayName()));
        }

        #endregion

        #endregion
    }
}
