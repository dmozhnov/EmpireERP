using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Накладная перемещения
    /// </summary>
    public class MovementWaybill : BaseWaybill
    {
        #region Свойства

        #region Основные свойства

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
                    ValidationUtils.Assert(!AreOutgoingWaybills, "Невозможно удалить накладную перемещения, так как товар из нее используется в других документах.");

                    deletionDate = value;

                    foreach (var row in Rows)
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
        /// Статус накладной перемещения
        /// </summary>
        public virtual MovementWaybillState State { get; protected internal set; }

        /// <summary>
        /// Ставка НДС
        /// </summary>
        public virtual ValueAddedTax ValueAddedTax { get; set; }

        /// <summary>
        /// Сумма НДС отправителя. Имеет смысл только в случае проведенной накладной (зафиксированы учетные цены)
        /// </summary>
        public virtual decimal SenderValueAddedTaxSum
        {
            get { return Rows.Sum(x => x.SenderValueAddedTaxSum); }
        }

        /// <summary>
        /// Сумма НДС получателя. Имеет смысл только в случае проведенной накладной (зафиксированы учетные цены)
        /// </summary>
        public virtual decimal RecipientValueAddedTaxSum
        {
            get { return Rows.Sum(x => x.RecipientValueAddedTaxSum); }
        }

        /// <summary>
        /// Место хранение - отправитель
        /// </summary>
        public virtual Storage SenderStorage
        {
            get { return senderStorage; }
            protected set
            {
                if (senderStorage != null && senderStorage != value)
                {
                    throw new Exception("Для накладной перемещения невозможно сменить место хранения-отправителя.");
                }
                
                ValidationUtils.NotNull(value, "Место хранения-отправитель не указано.");
                
                senderStorage = value;
            }
        }
        private Storage senderStorage;

        /// <summary>
        /// Организация-отправитель
        /// </summary>
        public virtual AccountOrganization Sender
        {
            get { return sender; }
            protected set
            {
                if (sender != null && sender != value)
                {
                    throw new Exception("Для накладных перемещения невозможно сменить организацию-отправителя.");
                }
                
                ValidationUtils.NotNull(value, "Организация-отправитель не указана.");
                
                sender = value;
            }
        }
        private AccountOrganization sender;

        /// <summary>
        /// Место хранения - получатель
        /// </summary>
        public virtual Storage RecipientStorage
        {
            get { return recipientStorage; }
            set
            {
                ValidationUtils.NotNull(value, "Место хранения-получатель не указано.");
                
                ValidationUtils.Assert(senderStorage != value, "Места хранения отправителя и получателя не могут совпадать.");
                
                recipientStorage = value;
            }
        }
        private Storage recipientStorage;

        /// <summary>
        /// Организация-получатель
        /// </summary>
        public virtual AccountOrganization Recipient
        {
            get { return recipient; }
            set
            {
                ValidationUtils.NotNull(value, "Организация-получатель не указана.");
                recipient = value;
            }
        }
        private AccountOrganization recipient;

        /// <summary>
        /// Пользователь, отгрузивший накладную
        /// </summary>
        public virtual User ShippedBy { get; protected set; }

        /// <summary>
        /// Дата отгрузки
        /// </summary>
        public virtual DateTime? ShippingDate { get; protected set; }

        /// <summary>
        /// Пользователь, принявший накладную
        /// </summary>
        public virtual User ReceiptedBy { get; protected set; }

        /// <summary>
        /// Дата приемки
        /// </summary>
        public virtual DateTime? ReceiptDate { get; protected set; }

        /// <summary>
        /// Признак отгрузки товара
        /// </summary>
        public virtual bool IsShipped
        {
            get
            {
                return State.ContainsIn(MovementWaybillState.ShippedBySender, MovementWaybillState.ReceiptedAfterDivergences,
                    MovementWaybillState.ReceiptedWithDivergences, MovementWaybillState.ReceiptedWithoutDivergences);
            }
        }

        /// <summary>
        /// Находится ли накладная в логическом состоянии "Новая"
        /// </summary>
        public virtual bool IsNew
        {
            get
            {
                return IsDraft || IsReadyToAccept;
            }
        }

        /// <summary>
        /// Является ли накладная черновиком
        /// </summary>
        public virtual bool IsDraft
        {
            get
            {
                return State == MovementWaybillState.Draft;
            }
        }

        /// <summary>
        /// Подготовлена ли накладная к проводке
        /// </summary>
        public virtual bool IsReadyToAccept
        {
            get
            {
                return State == MovementWaybillState.ReadyToAccept;
            }
        }

        /// <summary>
        /// Признак проводки накладной
        /// </summary>
        public virtual bool IsAccepted
        {
            get
            {
                return !IsNew;
            }
        }

        /// <summary>
        /// Признак приемки товара
        /// </summary>
        public virtual bool IsReceipted
        {
            get
            {
                return State.ContainsIn(MovementWaybillState.ReceiptedAfterDivergences,
                    MovementWaybillState.ReceiptedWithDivergences, MovementWaybillState.ReceiptedWithoutDivergences);
            }
        }

        #endregion

        #region Показатели

        /// <summary>
        /// Сумма товаров в закупочных ценах – рассчитывается сумма в закупочных ценах по каждой партии
        /// каждого наименования товара, учитывая перемещаемое количество товара по этой партии.
        /// </summary>
        public virtual decimal PurchaseCostSum
        {
            get
            {
                return Rows.Sum(x => Math.Round(x.ReceiptWaybillRow.PurchaseCost * x.MovingCount, 6));
            }
        }

        /// <summary>
        /// Сумма товаров в отпускных учетных ценах – рассчитывается сумма товара в учетных ценах
        /// места хранения - отправителя
        /// </summary>
        public virtual decimal? SenderAccountingPriceSum
        {
            get { return senderAccountingPriceSum ?? 0; }
            protected set { senderAccountingPriceSum = value; }
        }
        private decimal? senderAccountingPriceSum;

        /// <summary>
        /// Сумма товаров в приходных учетных ценах – рассчитывается сумма товара в учетных ценах
        /// места хранения – получателя
        /// </summary>
        public virtual decimal? RecipientAccountingPriceSum
        {
            get { return recipientAccountingPriceSum ?? 0; }
            protected set { recipientAccountingPriceSum = value; }
        }
        private decimal? recipientAccountingPriceSum;

        /// <summary>
        /// Сумма наценки при перемещении
        /// </summary>
        public virtual decimal MovementMarkupSum
        {
            get
            {
                return RecipientAccountingPriceSum.Value - SenderAccountingPriceSum.Value;
            }
        }

        /// <summary>
        /// Процент наценки при перемещении
        /// </summary>
        public virtual decimal MovementMarkupPercent
        {
            get
            {
                if (SenderAccountingPriceSum == 0)
                {
                    return 0;
                }

                return Math.Round((RecipientAccountingPriceSum.Value - SenderAccountingPriceSum.Value) / SenderAccountingPriceSum.Value * 100, 2);
            }
        }


        #endregion

        #region Список позиций накладной

        /// <summary>
        /// Позиции накладной
        /// </summary>
        public new virtual IEnumerable<MovementWaybillRow> Rows
        {
            get { return new ImmutableSet<BaseWaybillRow>(rows).Select(x => x.As<MovementWaybillRow>()); }
        }        

        /// <summary>
        /// Количество позиций накладной перемещения
        /// </summary>
        public virtual int RowCount
        {
            get { return rows.Count; }
        }

        /// <summary>
        /// Есть ли исходящие документы
        /// </summary>
        public virtual bool AreOutgoingWaybills
        {
            get { return Rows.Any(x => x.AreOutgoingWaybills); }
        }

        #endregion

        #endregion

        #region Конструкторы

        protected MovementWaybill()
            : base(WaybillType.MovementWaybill)
        {
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public MovementWaybill(string number, DateTime date, Storage senderStorage, AccountOrganization sender,
            Storage recipientStorage, AccountOrganization recipient, ValueAddedTax valueAddedTax, User curator, User createdBy, DateTime creationDate)
            : base(WaybillType.MovementWaybill, number, date, curator, createdBy, creationDate)
        {
            ValidationUtils.Assert(senderStorage != recipientStorage, "Места хранения отправителя и получателя не могут совпадать.");

            ValidationUtils.Assert(valueAddedTax.Value == 0M || sender != recipient,
                "Накладная, в которой организации-отправитель и получатель совпадают, не может иметь ненулевой НДС.");

            SenderStorage = senderStorage;
            Sender = sender;
            RecipientStorage = recipientStorage;
            Recipient = recipient;
            ValueAddedTax = valueAddedTax;

            State = MovementWaybillState.Draft;
        }

        #endregion

        #region Методы

        #region Добавление/удаление позиций накладной 

        /// <summary>
        /// Добавление позиции в накладную
        /// </summary>
        public virtual void AddRow(MovementWaybillRow row)
        {
            ValidationUtils.Assert(!IsAccepted, String.Format("Невозможно добавить позицию в накладную со статусом «{0}».", State.GetDisplayName()));
            
            ValidationUtils.Assert(!Rows.Any(x => x.ReceiptWaybillRow.Id == row.ReceiptWaybillRow.Id), "Позиция накладной по данной партии и товару уже добавлена.");
            
            ValidationUtils.Assert(row.ValueAddedTax.Value == 0M || Sender != Recipient,
                "Организации-отправитель и получатель совпадают. Невозможно установить ненулевой НДС для позиции.");

            rows.Add(row);
            row.MovementWaybill = this;
        }

        /// <summary>
        /// Удаление позиции из накладной
        /// </summary>
        public virtual void DeleteRow(MovementWaybillRow row)
        {
            ValidationUtils.Assert(!IsAccepted, String.Format("Невозможно удалить позицию из накладной со статусом «{0}».", State.GetDisplayName()));
            
            ValidationUtils.Assert(!row.AreOutgoingWaybills, "Невозможно удалить позицию, так как по ней уже есть исходящие накладные.");            

            rows.Remove(row);
            row.DeletionDate = DateTime.Now;
        }

        #endregion

        #region Обновление статуса накладной на основании статусов ее позиций
        
        protected internal virtual void UpdateStateByRowsState()
        {
            ValidationUtils.Assert(!IsShipped, "Невозможно обновить статус отгруженной накладной по ее позициям.");
            
            if (Rows.Any(x => x.OutgoingWaybillRowState == OutgoingWaybillRowState.Conflicts))
            {
                State = MovementWaybillState.ConflictsInArticle;
            }
            else if (Rows.Any(x => x.OutgoingWaybillRowState.ContainsIn(OutgoingWaybillRowState.ArticlePending, OutgoingWaybillRowState.Undefined)))
            {
                State = MovementWaybillState.ArticlePending;
            }
            else
            {
                State = MovementWaybillState.ReadyToShip;
            }
        }

        #endregion

        /// <summary>
        /// Подготовка к проводке накладной
        /// </summary>
        public virtual void PrepareToAccept()
        {
            CheckPossibilityToPrepareToAccept();

            State = MovementWaybillState.ReadyToAccept;
        }

        /// <summary>
        /// Отмена подготовки к проводке
        /// </summary>
        public virtual void CancelReadinessToAccept()
        {
            CheckPossibilityToCancelReadinessToAccept();

            State = MovementWaybillState.Draft;
        }

        /// <summary>
        /// Проводка накладной (обновляются статус, фиксируются учетные цены отправителя и получателя)
        /// </summary>
        /// <param name="senderArticleAccountingPriceLists">Список реестров по позициям накладной на складе-отправителе</param>
        /// <param name="recipientArticleAccountingPriceLists">Список реестров по позициям накладной на складе-получателе</param>
        public virtual void Accept(IEnumerable<ArticleAccountingPrice> senderArticleAccountingPriceLists, IEnumerable<ArticleAccountingPrice> recipientArticleAccountingPriceLists, 
            bool allowUsingPreparingToAcceptance, User acceptedBy, DateTime currentDateTime)
        {
            CheckPossibilityToAccept(allowUsingPreparingToAcceptance);

            decimal senderAccountingPriceSum = 0M, recipientAccountingPriceSum = 0M;
            foreach (var row in Rows)
            {
                // Выставляем реестры, по которым формируется учетная цена
                var senderArticleAccountingPrice = senderArticleAccountingPriceLists.FirstOrDefault(x => x.Article == row.Article);
                ValidationUtils.NotNull(senderArticleAccountingPrice,
                    String.Format("Невозможно выполнить проводку, так как на товар «{0}» не установлена учетная цена отправителя.", row.Article.FullName));

                var recipientArticleAccountingPrice = recipientArticleAccountingPriceLists.FirstOrDefault(x => x.Article == row.Article);
                ValidationUtils.NotNull(recipientArticleAccountingPrice,
                    String.Format("Невозможно выполнить проводку, так как на товар «{0}» не установлена учетная цена получателя.", row.Article.FullName));

                senderAccountingPriceSum += Math.Round(senderArticleAccountingPrice.AccountingPrice * row.MovingCount, 2);
                row.SenderArticleAccountingPrice = senderArticleAccountingPrice;

                recipientAccountingPriceSum += Math.Round(recipientArticleAccountingPrice.AccountingPrice * row.MovingCount, 2);
                row.RecipientArticleAccountingPrice = recipientArticleAccountingPrice;

                // переводим кол-во по позиции в доступное для резервирования
                row.AvailableToReserveCount = row.MovingCount;
            }

            // обновляем статус накладной по статусам позиций
            UpdateStateByRowsState();

            this.senderAccountingPriceSum = senderAccountingPriceSum;
            this.recipientAccountingPriceSum = recipientAccountingPriceSum;

            AcceptanceDate = currentDateTime;
            AcceptedBy = acceptedBy;
        }

        /// <summary>
        /// Отмена проводки накладной (обновляется статус, сбрасываются зафиксированные значения учетных цен)
        /// </summary>
        public virtual void CancelAcceptance(bool allowUsingPreparingToAcceptance)
        {
            CheckPossibilityToCancelAcceptance();

            State = allowUsingPreparingToAcceptance ? MovementWaybillState.ReadyToAccept : MovementWaybillState.Draft;
            AcceptanceDate = null;
            AcceptedBy = null;

            senderAccountingPriceSum = null;
            recipientAccountingPriceSum = null;

            foreach(var row in Rows)
            {
                row.RecipientArticleAccountingPrice = row.SenderArticleAccountingPrice = null;
                
                // обнуляем доступное для резервирования кол-во товара
                row.AvailableToReserveCount = 0;

                // если позиция без источника, то сбрасываем статус в «Не определено»
                if (row.IsUsingManualSource == false)
                {
                    row.OutgoingWaybillRowState = OutgoingWaybillRowState.Undefined;
                }
            }
        }

        /// <summary>
        /// Отгрузка
        /// </summary>
        public virtual void Ship(User shippedBy, DateTime currentDateTime)
        {
            CheckPossibilityToShip();

            State = MovementWaybillState.ShippedBySender;
            ShippingDate = currentDateTime;
            ShippedBy = shippedBy;
        }

        /// <summary>
        /// Отменить отгрузку
        /// </summary>
        public virtual void CancelShipping()
        {
            CheckPossibilityToCancelShipping();

            State = MovementWaybillState.ReadyToShip;
            ShippingDate = null;
            ShippedBy = null;
        }

        /// <summary>
        /// Принять
        /// </summary>
        public virtual void Receipt(User receiptedBy, DateTime currentDateTime)
        {
            CheckPossibilityToReceipt();

            State = MovementWaybillState.ReceiptedWithoutDivergences; //ЗАГЛУШКА, пока считаем, что всегда принимается без расхождений
            ReceiptDate = currentDateTime;
            ReceiptedBy = receiptedBy;
        }

        /// <summary>
        /// Отмена приемки
        /// </summary>
        public virtual void CancelReceipt()
        {
            CheckPossibilityToCancelReceipt();

            State = MovementWaybillState.ShippedBySender;
            ReceiptDate = null;
            ReceiptedBy = null;
        }

        #region Проверка возможности совершения операций

        public virtual void CheckPossibilityToEdit()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно отредактировать накладную со статусом «{0}».", State.GetDisplayName()));
        }

        public virtual void CheckPossibilityToEditRecipientAndRecipientStorage()
        {
            CheckPossibilityToEdit();

            ValidationUtils.Assert(!AreOutgoingWaybills, "Невозможно сменить получателя накладной, т.к. товар из нее используется в других накладных.");
        }

        public virtual void CheckPossibilityToDelete()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно удалить накладную со статусом «{0}».", State.GetDisplayName()));
            ValidationUtils.Assert(!AreOutgoingWaybills, "Невозможно удалить накладную, т.к. по ней уже сформированы исходящие накладные.");
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
            ValidationUtils.Assert(!IsShipped, "Накладная уже отгружена.");
        }

        public virtual void CheckPossibilityToAccept(bool useReadyToAcceptState)
        {
            ValidationUtils.Assert(IsNew, "Накладная уже проведена.");
            if (useReadyToAcceptState)
            {
                ValidationUtils.Assert(IsReadyToAccept, String.Format("Невозможно провести накладную из состояния «{0}».",State.GetDisplayName()));
            }
            ValidationUtils.Assert(RowCount > 0, "Невозможно провести накладную, не содержащую ни одной позиции.");
        }

        public virtual void CheckPossibilityToCancelAcceptance()
        {
            ValidationUtils.Assert(!IsNew, "Накладная еще не проведена.");
            ValidationUtils.Assert(!IsShipped, "Накладная уже отгружена.");

            ValidationUtils.Assert(!Rows.Any(x => x.AreOutgoingWaybills), "Невозможно отменить проводку накладной, т.к. ее позиции участвуют в дальнейшем товародвижении.");
        }

        public virtual void CheckPossibilityToShip()
        {
            ValidationUtils.Assert(State == MovementWaybillState.ReadyToShip, 
                String.Format("Отгрузить товар можно только для накладной со статусом «{0}».", MovementWaybillState.ReadyToShip.GetDisplayName()));
        }

        public virtual void CheckPossibilityToCancelShipping()
        {
            if (State != MovementWaybillState.ShippedBySender)
            {
                throw new Exception(String.Format("Отменить отгрузку можно только для накладной со статусом «{0}».", MovementWaybillState.ShippedBySender.GetDisplayName()));
            }
        }

        public virtual void CheckPossibilityToReceipt()
        {
            if (!IsShipped || IsReceipted)
            {
                throw new Exception(String.Format("Принять товар можно только для накладной со статусом «{0}».",
                    MovementWaybillState.ShippedBySender.GetDisplayName()));
            }
        }

        public virtual void CheckPossibilityToCancelReceipt()
        {
            if (!IsReceipted)
            {
                throw new Exception(String.Format("Отменить приемку можно только для накладной со статусами «{0}», «{1}» или «{2}».",
                    MovementWaybillState.ReceiptedWithoutDivergences.GetDisplayName(), MovementWaybillState.ReceiptedWithDivergences.GetDisplayName(), MovementWaybillState.ReceiptedAfterDivergences.GetDisplayName()));
            }

            if (Rows.Any(x => x.ShippedCount + x.FinallyMovedCount > 0))
            {
                throw new Exception("Невозможно отменить приемку, так как товар по накладной уже был отгружен.");
            }
        }

        public override void CheckPossibilityToChangeCurator()
        {
            ValidationUtils.Assert(State == MovementWaybillState.Draft || State == MovementWaybillState.ReadyToAccept,
                String.Format("Невозможно изменить куратора для накладной со статусом «{0}».", State.GetDisplayName()));
        }

        #endregion

        #endregion
    }
}