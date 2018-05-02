using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Накладная возврата товара от клиента
    /// </summary>
    public class ReturnFromClientWaybill : BaseWaybill
    {
        #region Свойства

        #region Основные свойства

        /// <summary>
        /// Команда, по которой сделан возврат
        /// </summary>
        public virtual Team Team
        {
            get { return team; }
            set
            {
                CheckPossibilityToEditTeam();
                ValidationUtils.NotNull(value, "Укажите команду.");
                team = value;
            }
        }
        private Team team;

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
        /// Статус накладной
        /// </summary>
        public virtual ReturnFromClientWaybillState State { get; protected internal set; }

        /// <summary>
        /// Место хранения-получатель
        /// </summary>
        public virtual Storage RecipientStorage
        {
            get { return recipientStorage; }
            set
            {
                ValidationUtils.NotNull(value, "Место хранения-получатель не указано.");

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
        /// Сделка
        /// </summary>
        public virtual Deal Deal
        {
            get { return deal; }
            set
            {
                ValidationUtils.NotNull(value, "Сделка не указана.");

                deal = value;
            }
        }
        private Deal deal;

        /// <summary>
        /// Клиент, от которого совершен возврат
        /// </summary>
        public virtual Client Client
        {
            get
            {
                ValidationUtils.NotNull(deal, "Не установлена сделка.");

                return deal.Client;
            }
        }

        /// <summary>
        /// Находится ли накладная в логическом состоянии "Новая"
        /// </summary>
        public virtual bool IsNew
        {
            get { return IsDraft || IsReadyToAccept; }
        }

        /// <summary>
        /// Является ли накладная черновиком
        /// </summary>
        public virtual bool IsDraft
        {
            get { return State == ReturnFromClientWaybillState.Draft; }
        }

        /// <summary>
        /// Подготовлена ли накладная к проводке
        /// </summary>
        public virtual bool IsReadyToAccept
        {
            get { return State == ReturnFromClientWaybillState.ReadyToAccept; }
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
        /// Принята ли накладная на склад
        /// </summary>
        public virtual bool IsReceipted
        {
            get { return State == ReturnFromClientWaybillState.Receipted; }
        }

        /// <summary>
        /// Дата приемки
        /// </summary>
        public virtual DateTime? ReceiptDate { get; protected set; }

        /// <summary>
        /// Пользователь, принявший накладную
        /// </summary>
        public virtual User ReceiptedBy { get; protected set; }

        /// <summary>
        /// Основание для возврата товара от клиента
        /// </summary>
        public virtual ReturnFromClientReason ReturnFromClientReason { get; set; }

        #endregion

        #region Коллекция разнесений платежных документов

        public virtual IEnumerable<DealPaymentDocumentDistributionToReturnFromClientWaybill> Distributions
        {
            get { return new ImmutableSet<DealPaymentDocumentDistributionToReturnFromClientWaybill>(distributions); }
        }
        private Iesi.Collections.Generic.ISet<DealPaymentDocumentDistributionToReturnFromClientWaybill> distributions;

        #endregion

        #region Позиции накладной

        /// <summary>
        /// Позиции накладной
        /// </summary>
        public new virtual IEnumerable<ReturnFromClientWaybillRow> Rows
        {
            get { return new ImmutableSet<BaseWaybillRow>(rows).Select(x => x.As<ReturnFromClientWaybillRow>()); }
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
        /// Есть ли исходящие документы
        /// </summary>
        public virtual bool AreOutgoingWaybills
        {
            get { return Rows.Any(x => x.AreOutgoingWaybills); }
        }

        /// <summary>
        /// Сумма товаров в закупочных ценах
        /// </summary>
        public virtual decimal PurchaseCostSum
        {
            get
            {
                return Rows.Sum(x => Math.Round(x.SaleWaybillRow.PurchaseCost * x.ReturnCount, 6));
            }
        }

        /// <summary>
        /// Сумма товаров в учетных ценах места хранения - получателя
        /// </summary>
        public virtual decimal RecipientAccountingPriceSum
        {
            get
            {
                ValidationUtils.Assert(IsAccepted, "Невозможно получить значение суммы товаров в учетных ценах для непроведенной накладной.");

                return recipientAccountingPriceSum ?? 0;
            }
            set
            {
                if (recipientAccountingPriceSum != value)
                {
                    if (IsAccepted && recipientAccountingPriceSum != null)
                    {
                        throw new Exception("Невозможно установить значение суммы в учетных ценах, т.к. накладная уже проведена.");
                    }

                    recipientAccountingPriceSum = value;
                }
            }
        }
        private decimal? recipientAccountingPriceSum;

        /// <summary>
        /// Сумма товаров в отпускных ценах
        /// </summary>
        public virtual decimal SalePriceSum
        {
            get
            {
                if (IsAccepted)
                {
                    return salePriceSum.Value;
                }

                return Rows.Sum(x => Math.Round((x.SaleWaybillRow.SalePrice.HasValue ? x.SaleWaybillRow.SalePrice.Value : 0) * x.ReturnCount, 2));
            }
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

        #endregion

        #endregion

        #region  Конструкторы

        protected ReturnFromClientWaybill()
            : base(WaybillType.ReturnFromClientWaybill)
        {
        }

        public ReturnFromClientWaybill(string number, DateTime date, AccountOrganization recipient, Deal deal, Team team, Storage recipientStorage,
            ReturnFromClientReason returnFromClientReason, User curator, User createdBy, DateTime creationDate)
            : base(WaybillType.ReturnFromClientWaybill, number, date, curator, createdBy, creationDate)
        {
            ValidationUtils.NotNull(deal, "Не указана сделка.");
            ValidationUtils.NotNull(recipient, "Не указана организация-приемщик.");
            ValidationUtils.NotNull(recipientStorage, "Не указано место хранения-приемщик.");
            ValidationUtils.NotNull(returnFromClientReason, "Не указана причина возврата.");
            ValidationUtils.NotNull(team, "Не указана команда.");

            State = ReturnFromClientWaybillState.Draft;
            distributions = new HashedSet<DealPaymentDocumentDistributionToReturnFromClientWaybill>();

            Recipient = recipient;
            RecipientStorage = recipientStorage;
            ReturnFromClientReason = returnFromClientReason;
            Deal = deal;
            Team = team;

            // Автоматически переводим сделку на этап "Закрытие договора", если она была закрыта
            deal.MoveToContractClosingStage();
        }

        #endregion

        #region  Методы

        #region Добавление / удаление позиций

        /// <summary>
        /// Добавление позиции в накладную
        /// </summary>
        public virtual void AddRow(ReturnFromClientWaybillRow row)
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно добавить позицию в накладную со статусом «{0}».", State.GetDisplayName()));
            ValidationUtils.NotNull(row.SaleWaybillRow.SaleWaybill, "Невозможно добавить позицию со ссылкой на позицию накладной реализации без указанной реализации.");
            ValidationUtils.Assert(row.SaleWaybillRow.SaleWaybill.Sender == Recipient, "Организация-приемщик и организация-продавец товара должны совпадать.");
            ValidationUtils.Assert(!Rows.Any(x => x.SaleWaybillRow.Id == row.SaleWaybillRow.Id), "Позиция накладной по данной позиции накладной реализации уже добавлена.");
            ValidationUtils.Assert(row.SaleWaybillRow.SaleWaybill.Team == Team, "Команда накладной возврата должна совпадать с командой накладной реализации.");

            rows.Add(row);
            row.ReturnFromClientWaybill = this;
        }

        /// <summary>
        /// Удаление позиции из накладной
        /// </summary>
        public virtual void DeleteRow(ReturnFromClientWaybillRow row)
        {
            row.CheckPossibilityToDelete();

            rows.Remove(row);
            row.DeletionDate = DateTime.Now;
        }
        #endregion

        #region Разнесения платежного документа на данную сущность

        /// <summary>
        /// Добавление разнесения платежного документа на данную сущность
        /// </summary>
        /// <param name="paymentDistribution"></param>
        public virtual void AddDealPaymentDocumentDistribution(DealPaymentDocumentDistributionToReturnFromClientWaybill dealPaymentDocumentDistributionToReturnFromClientWaybill)
        {
            dealPaymentDocumentDistributionToReturnFromClientWaybill.SetOrdinalNumber(Distributions);
            distributions.Add(dealPaymentDocumentDistributionToReturnFromClientWaybill);
        }

        /// <summary>
        /// Удаление разнесения платежного документа на данную сущность
        /// </summary>
        /// <param name="paymentDistribution"></param>
        public virtual void RemoveDealPaymentDocumentDistribution(DealPaymentDocumentDistributionToReturnFromClientWaybill dealPaymentDocumentDistributionToReturnFromClientWaybill)
        {
            ValidationUtils.Assert(distributions.Contains(dealPaymentDocumentDistributionToReturnFromClientWaybill),
                "Расшифровка распределения платежного документа не связана с данной накладной возврата от клиента.");
            distributions.Remove(dealPaymentDocumentDistributionToReturnFromClientWaybill);
        }

        #endregion

        #region Подготовка / Отменить готовность к проводке

        /// <summary>
        /// Подготовка наклданой к проводке
        /// </summary>
        public virtual void PrepareToAccept()
        {
            CheckPossibilityToPrepareToAccept();

            State = ReturnFromClientWaybillState.ReadyToAccept;
        }

        /// <summary>
        /// Отмена готовности накладной к проводке
        /// </summary>
        public virtual void CancelReadinessToAccept()
        {
            CheckPossibilityToCancelReadinessToAccept();

            State = ReturnFromClientWaybillState.Draft;
        }

        #endregion

        #region Проводка / отмена проводки

        /// <summary>
        /// Проводка накладной
        /// </summary>
        /// <param name="receiptPriceLists">Позиции реестра цен</param>
        /// <param name="useReadyToAcceptState">Разрешение использовать подготовку накладной к проводке</param>
        /// <param name="acceptedBy">Пользователь, проводящий накладную</param>
        /// <param name="currentDateTime">Текущее время</param>
        public virtual void Accept(IEnumerable<ArticleAccountingPrice> receiptPriceLists, bool useReadyToAcceptState, User acceptedBy, DateTime currentDateTime)
        {
            CheckPossibilityToAccept(useReadyToAcceptState);

            var _accountingPriceSum = 0M;
            foreach (var row in Rows) //выставляем реестры, по которым формируется учетная цена на получателе
            {
                var receiptPriceList = receiptPriceLists.FirstOrDefault(x => x.Article == row.Article);
                ValidationUtils.NotNull(receiptPriceList,
                    String.Format("Не установлена учетная цена на товар «{0}».", row.Article.FullName));

                row.ArticleAccountingPrice = receiptPriceList;
                _accountingPriceSum += Math.Round(receiptPriceList.AccountingPrice * row.ReturnCount, 2);

                // переводим кол-во по позиции в доступное для резервирования
                row.AvailableToReserveCount = row.ReturnCount;
            }

            salePriceSum = Rows.Sum(x => Math.Round(x.SalePrice.Value * x.ReturnCount, 2));

            State = ReturnFromClientWaybillState.Accepted;
            recipientAccountingPriceSum = _accountingPriceSum;
            AcceptanceDate = currentDateTime;
            AcceptedBy = acceptedBy;
        }

        /// <summary>
        /// Отмена проводки
        /// </summary>
        /// <param name="useReadyToAcceptState">Разрешение использовать подготовку накладной к проводке</param>
        public virtual void CancelAcceptance(bool useReadyToAcceptState)
        {
            CheckPossibilityToCancelAcceptance();

            State = useReadyToAcceptState ? ReturnFromClientWaybillState.ReadyToAccept : ReturnFromClientWaybillState.Draft;

            foreach (var row in Rows)
            {
                // сбрасываем позиции реестра цен
                row.ArticleAccountingPrice = null;

                // обнуляем доступное для резервирования кол-во товара
                row.AvailableToReserveCount = 0;
            }

            AcceptanceDate = null;
            recipientAccountingPriceSum = null;
            salePriceSum = null;
            AcceptedBy = null;
        }

        #endregion

        #region Приемка / отмена приемки

        /// <summary>
        /// Приемка товара по накладной
        /// </summary>
        public virtual void Receipt(User receiptedBy, DateTime currentDateTime)
        {
            CheckPossibilityToReceipt();

            State = ReturnFromClientWaybillState.Receipted;
            ReceiptedBy = receiptedBy;
            ReceiptDate = currentDateTime;
        }

        /// <summary>
        /// Отмена приемки
        /// </summary>
        public virtual void CancelReceipt()
        {
            CheckPossibilityToCancelReceipt();

            State = ReturnFromClientWaybillState.Accepted;
            ReceiptedBy = null;
            ReceiptDate = null;

            // Автоматически переводим сделку на этап "Закрытие договора", если она была закрыта
            deal.MoveToContractClosingStage();
        }

        #endregion
        
        #region Проверка возможности совершения операций

        /// <summary>
        /// Проверка возможности редактирования шапки накладной
        /// </summary>
        public virtual void CheckPossibilityToEdit()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно изменить накладную со статусом «{0}».", State.GetDisplayName()));
        }

        /// <summary>
        /// Проверка возможности удаления накладной
        /// </summary>
        public virtual void CheckPossibilityToDelete()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно удалить накладную со статусом «{0}».", State.GetDisplayName()));
            ValidationUtils.Assert(!AreOutgoingWaybills, "Невозможно удалить накладную, так как товар из нее используется в других документах.");
        }

        /// <summary>
        /// Проверка возможности подготовить накладную к проводке
        /// </summary>
        public virtual void CheckPossibilityToPrepareToAccept()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно подготовить к проводке накладную со статусом «{0}».", State.GetDisplayName()));
            ValidationUtils.Assert(RowCount > 0, "Невозможно подготовить к проводке накладную, не содержащую ни одной позиции.");
        }

        /// <summary>
        /// Проверка возможности отмены готовности к проводке
        /// </summary>
        public virtual void CheckPossibilityToCancelReadinessToAccept()
        {
            ValidationUtils.Assert(IsReadyToAccept, String.Format("Невозможно отменить готовность к проводке накладной со статусом «{0}».", State.GetDisplayName()));
        }

        /// <summary>
        /// Проверка возможности проводки накладной
        /// </summary>
        public virtual void CheckPossibilityToAccept(bool useReadyToAcceptState)
        {
            ValidationUtils.Assert(useReadyToAcceptState ? IsReadyToAccept : IsNew,
                String.Format("Невозможно провести накладную со статусом «{0}».", State.GetDisplayName()));
            ValidationUtils.Assert(RowCount > 0, "Невозможно провести накладную, не содержащую ни одной позиции.");
        }

        /// <summary>
        /// Проверка возможности отмены проводки
        /// </summary>
        public virtual void CheckPossibilityToCancelAcceptance()
        {
            ValidationUtils.Assert(IsAccepted && !IsReceipted, String.Format("Невозможно отменить проводку накладной со статусом «{0}».", State.GetDisplayName()));

            // Отменить проводку можно, только если ни по одной из позиций накладной не было сформировано других документов
            // со статусом "Проведено", "Отгружено" или "Принято".
            ValidationUtils.Assert(!Rows.Any(x => x.AreOutgoingWaybills), "Невозможно отменить проводку накладной, т.к. ее позиции участвуют в дальнейшем товародвижении.");
        }

        /// <summary>
        /// Проверка возможности приемки товара по накладной
        /// </summary>
        public virtual void CheckPossibilityToReceipt()
        {
            ValidationUtils.Assert(IsAccepted && !IsReceipted, String.Format("Невозможно принять товар по накладной со статусом «{0}».", State.GetDisplayName()));
        }

        /// <summary>
        /// Проверка возможности отмены приемки товара по накладной
        /// </summary>
        public virtual void CheckPossibilityToCancelReceipt()
        {
            ValidationUtils.Assert(IsReceipted, String.Format("Невозможно отменить приемку товара по накладной со статусом «{0}».", State.GetDisplayName()));
            ValidationUtils.Assert(!Rows.Any(x => x.ShippedCount + x.FinallyMovedCount > 0), "Невозможно отменить приемку накладной, т.к. ее позиции участвуют в дальнейшем товародвижении.");
        }

        /// <summary>
        /// Проверка на возможность редактирования команды
        /// </summary>
        public virtual void CheckPossibilityToEditTeam()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно изменить команду по накладной со статусом «{0}».", State.GetDisplayName()));
            ValidationUtils.Assert(RowCount == 0, "Невозможно изменить команду, т.к. в накладную уже добавлены позиции.");
        }

        public override void CheckPossibilityToChangeCurator()
        {
            ValidationUtils.Assert(State == ReturnFromClientWaybillState.Draft || State == ReturnFromClientWaybillState.ReadyToAccept,
                String.Format("Невозможно изменить куратора для накладной со статусом «{0}».", State.GetDisplayName()));
        }
        #endregion

        #endregion
    }
}