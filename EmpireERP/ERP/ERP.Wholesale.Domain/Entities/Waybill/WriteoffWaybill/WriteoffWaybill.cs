using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Накладная списания
    /// </summary>
    public class WriteoffWaybill : BaseWaybill
    {
        #region Свойства

        /// <summary>
        /// Место хранения, с которого производится списание
        /// </summary>
        public virtual Storage SenderStorage { get; protected set; }

        /// <summary>
        /// Организация отправителя
        /// </summary>
        public virtual AccountOrganization Sender { get; protected set; }

        /// <summary>
        /// Основание для списания
        /// </summary>
        public virtual WriteoffReason WriteoffReason { get; set; }

        /// <summary>
        /// Статус накладной
        /// </summary>
        public virtual WriteoffWaybillState State { get; protected internal set; }

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
        /// Пользователь, осуществивший списание
        /// </summary>
        public virtual User WrittenoffBy { get; protected set; }

        /// <summary>
        /// Дата списания товаров по накладной
        /// </summary>
        public virtual DateTime? WriteoffDate
        {
            get { return writeoffDate; }
            protected set
            {
                writeoffDate = value;
            }
        }
        private DateTime? writeoffDate;

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
            get { return State == WriteoffWaybillState.Draft; }
        }

        /// <summary>
        /// Подготовлена ли накладная к проводке
        /// </summary>
        public virtual bool IsReadyToAccept
        {
            get { return State == WriteoffWaybillState.ReadyToAccept; }
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
        /// Списан ли товар по накладной
        /// </summary>
        public virtual bool IsWrittenOff
        {
            get
            {
                return State == WriteoffWaybillState.Writtenoff;
            }
        }

        #region Позиции накладной

        /// <summary>
        /// Позиции накладной списания
        /// </summary>
        public new virtual IEnumerable<WriteoffWaybillRow> Rows
        {
            get { return new ImmutableSet<BaseWaybillRow>(rows).Select(x => x.As<WriteoffWaybillRow>()); }
        }

        /// <summary>
        /// Количество позиций накладной списания
        /// </summary>
        public virtual int RowCount
        {
            get { return rows.Count; }
        }

        #endregion

        #region Показатели

        /// <summary>
        /// Сумма товаров в закупочных ценах
        /// </summary>
        public virtual decimal PurchaseCostSum
        {
            get
            {
                return Rows.Sum(x => Math.Round(x.ReceiptWaybillRow.PurchaseCost * x.WritingoffCount, 6));
            }
        }

        /// <summary>
        /// Сумма товаров в учетных ценах места хранения - отправителя
        /// </summary>
        public virtual decimal? SenderAccountingPriceSum
        {
            get { return senderAccountingPriceSum ?? 0; }
            set
            {
                if (senderAccountingPriceSum != value)
                {
                    if (IsWrittenOff && senderAccountingPriceSum != null)
                    {
                        throw new Exception("Невозможно установить значение суммы в учетных ценах, т.к. товар по накладной уже списан.");
                    }

                    senderAccountingPriceSum = value;
                }
            }
        }
        private decimal? senderAccountingPriceSum;

        /// <summary>
        /// Сумма недополученной прибыли
        /// </summary>
        public virtual decimal ReceivelessProfitSum
        {
            get { return SenderAccountingPriceSum.Value - PurchaseCostSum; }
        }

        /// <summary>
        /// Процент недополученной прибыли
        /// </summary>
        public virtual decimal? ReceivelessProfitPercent
        {
            get
            {
                return PurchaseCostSum != 0M ? (decimal?)Math.Round(ReceivelessProfitSum / PurchaseCostSum * 100, 2) : null;
            }
        }

        #endregion

        #endregion

        #region Конструкторы

        protected WriteoffWaybill()
            : base(WaybillType.WriteoffWaybill)
        {
        }

        public WriteoffWaybill(string number, DateTime date, Storage senderStorage, AccountOrganization sender, WriteoffReason writeoffReason, User curator, User createdBy, DateTime creationDate) :
            base(WaybillType.WriteoffWaybill, number, date, curator, createdBy, creationDate)
        {
            ValidationUtils.NotNull(senderStorage, "Не указано место хранения отправителя.");
            ValidationUtils.NotNull(sender, "Не указана организация отправителя.");
            ValidationUtils.NotNull(writeoffReason, "Не указано основание для списания.");

            State = WriteoffWaybillState.Draft;

            Sender = sender;
            SenderStorage = senderStorage;
            WriteoffReason = writeoffReason;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавление позиции в накладную
        /// </summary>
        public virtual void AddRow(WriteoffWaybillRow row)
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно добавить позицию в накладную со статусом «{0}».", State.GetDisplayName()));

            if (Rows.Any(x => x.ReceiptWaybillRow == row.ReceiptWaybillRow))
            {
                throw new Exception("Позиция накладной по данной партии и товару уже добавлена.");
            }

            rows.Add(row);
            row.WriteoffWaybill = this;
        }

        /// <summary>
        /// Удаление позиции из накладной
        /// </summary>
        public virtual void DeleteRow(WriteoffWaybillRow row)
        {
            ValidationUtils.Assert(rows.Contains(row), "Позиция накладной не найдена. Возможно, она была удалена.");

            row.CheckPossibilityToDelete();

            rows.Remove(row);
            row.DeletionDate = DateTime.Now;
        }

        /// <summary>
        /// Обновление статуса накладной на основании статусов ее позиций
        /// </summary>
        protected internal virtual void UpdateStateByRowsState()
        {
            ValidationUtils.Assert(!IsWrittenOff, "Невозможно обновить статус отгруженной накладной по ее позициям.");

            if (Rows.Any(x => x.OutgoingWaybillRowState == OutgoingWaybillRowState.Conflicts))
            {
                State = WriteoffWaybillState.ConflictsInArticle;
            }
            else if (Rows.Any(x => x.OutgoingWaybillRowState.ContainsIn(OutgoingWaybillRowState.ArticlePending, OutgoingWaybillRowState.Undefined)))
            {
                State = WriteoffWaybillState.ArticlePending;
            }
            else
            {
                State = WriteoffWaybillState.ReadyToWriteoff;
            }
        }

        /// <summary>
        /// Подготовка накладной к проводке
        /// </summary>
        public virtual void PrepareToAccept()
        {
            CheckPossibilityToPrepareToAccept();

            State = WriteoffWaybillState.ReadyToAccept;
        }

        /// <summary>
        /// Отменить готовность к проводке
        /// </summary>
        public virtual void CancelReadinessToAccept()
        {
            CheckPossibilityToCancelReadinessToAccept();

            State = WriteoffWaybillState.Draft;
        }

        /// <summary>
        /// Проводка
        /// </summary>
        public virtual void Accept(IEnumerable<ArticleAccountingPrice> senderPriceLists, bool useReadyToAcceptState, User acceptedBy, DateTime currentDateTime)
        {
            CheckPossibilityToAccept(useReadyToAcceptState);

            decimal _senderAccountingPriceSum = 0M;
            foreach (var row in Rows)   //выставляем реестры, по которым формируется учетная цена на отправителе
            {
                var senderPriceList = senderPriceLists.FirstOrDefault(x => x.Article == row.Article);
                ValidationUtils.NotNull(senderPriceList, String.Format("Невозможно выполнить списание, так как на товар «{0}» не установлена учетная цена.",
                    row.Article.FullName));

                row.SenderArticleAccountingPrice = senderPriceList;

                _senderAccountingPriceSum += Math.Round(senderPriceList.AccountingPrice * row.WritingoffCount, 2);
            }

            UpdateStateByRowsState();

            SenderAccountingPriceSum = _senderAccountingPriceSum;

            AcceptanceDate = currentDateTime;
            AcceptedBy = acceptedBy;
        }

        /// <summary>
        /// Отмена проводки
        /// </summary>
        public virtual void CancelAcceptance(bool useReadyToAcceptState)
        {
            CheckPossibilityToCancelAcceptance();

            State = useReadyToAcceptState ? WriteoffWaybillState.ReadyToAccept : WriteoffWaybillState.Draft;

            AcceptanceDate = null;
			AcceptedBy = null;
            SenderAccountingPriceSum = null;

            // Сбрасываем реестры, по которым формируется учетная цена на отправителе
            foreach (var row in Rows)
            {
                row.SenderArticleAccountingPrice = null;

                // если позиция без источника, то сбрасываем статус в «Не определено»
                if (row.IsUsingManualSource == false)
                {
                    row.OutgoingWaybillRowState = OutgoingWaybillRowState.Undefined;
                }
            }
        }

        /// <summary>
        /// Cписание накладной
        /// </summary>
        public virtual void Writeoff(User writtenoffBy, DateTime currentDateTime)
        {
            CheckPossibilityToWriteoff();

            State = WriteoffWaybillState.Writtenoff;

            WriteoffDate = currentDateTime;
            WrittenoffBy = writtenoffBy;
        }

        /// <summary>
        /// Отмена списания
        /// </summary>
        public virtual void CancelWriteoff()
        {
            CheckPossibilityToCancelWriteoff();

            State = WriteoffWaybillState.ReadyToWriteoff;

            WriteoffDate = null;
            WrittenoffBy = null;
        }

        #region Проверки на возможность выполнения операций

        #region Редактирование

        public virtual void CheckPossibilityToEdit()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно изменить накладную со статусом «{0}».", State.GetDisplayName()));
        }

        #endregion

        #region Удаление

        public virtual void CheckPossibilityToDelete()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно удалить накладную со статусом «{0}».", State.GetDisplayName()));
        }
        #endregion

        #region Подготовка / Отменить готовность к проводке

        /// <summary>
        /// Подготовка накладной к проводке
        /// </summary>
        public virtual void CheckPossibilityToPrepareToAccept()
        {
            ValidationUtils.Assert(IsDraft, String.Format("Невозможно подготовить к проводке накладную со статусом «{0}».", State.GetDisplayName()));
            ValidationUtils.Assert(RowCount > 0, "Невозможно подготовить к проводке накладную, не содержащую ни одной позиции.");
        }

        /// <summary>
        /// Отменить готовность к проводке
        /// </summary>
        public virtual void CheckPossibilityToCancelReadinessToAccept()
        {
            ValidationUtils.Assert(!IsWrittenOff, "Невозможно отменить готовность к проводке для отгруженной накладной.");
            ValidationUtils.Assert(IsReadyToAccept, String.Format("Невозможно отменить готовность к проводке для накладной со статусом «{0}».", State.GetDisplayName()));
        }

        #endregion

        #region Проводка / отмена проводки

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
            ValidationUtils.Assert(!IsWrittenOff, "Невозможно отменить проводку отгруженной накладной.");
        }

        #endregion

        #region Списание

        public virtual void CheckPossibilityToWriteoff()
        {
            ValidationUtils.Assert(!IsWrittenOff, "Товары по накладной уже списаны.");
            ValidationUtils.Assert(State == WriteoffWaybillState.ReadyToWriteoff,
                String.Format("Невозможно списать товары по накладной со статусом «{0}».", State.GetDisplayName()));
        }
        #endregion

        #region Отмена списания

        public virtual void CheckPossibilityToCancelWriteoff()
        {
            if (!IsWrittenOff)
            {
                throw new Exception(String.Format("Невозможно отменить списание товаров по накладной со статусом «{0}».", State.GetDisplayName()));
            }
        }
        #endregion

        #region Смена куратора

        public override void CheckPossibilityToChangeCurator()
        {
            ValidationUtils.Assert(State == WriteoffWaybillState.Draft || State == WriteoffWaybillState.ReadyToAccept,
                String.Format("Невозможно изменить куратора для накладной со статусом «{0}».", State.GetDisplayName()));
        }

        #endregion

        #endregion

        #endregion
    }
}