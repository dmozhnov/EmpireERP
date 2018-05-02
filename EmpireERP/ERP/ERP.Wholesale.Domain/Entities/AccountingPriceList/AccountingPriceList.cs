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
    /// Реестр цен
    /// </summary>
    public class AccountingPriceList : Entity<Guid>
    {
        #region Основные свойства

        /// <summary>
        /// Номер
        /// </summary>
        public virtual string Number { get; set; }

        /// <summary>
        /// Дата начала действия
        /// </summary>
        public virtual DateTime StartDate { get; set; }

        /// <summary>
        /// Дата завершения действия
        /// </summary>
        public virtual DateTime? EndDate { get; set; }

        /// <summary>
        /// Дата создания документа
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
                // запрещаем повторную пометку об удалении
                if (deletionDate == null && value != null)
                {
                    deletionDate = value;

                    // ставим позициям реестра цен текущую дату удаления
                    foreach (var articlePrice in articlePrices)
                    {
                        if (articlePrice.DeletionDate == null)
                        {
                            articlePrice.DeletionDate = deletionDate;
                        }
                    }

                    // уничтожаем список мест хранения (связь ManyToMany)
                    storages.Clear();
                }
            }
        }
        protected DateTime? deletionDate;

        /// <summary>
        /// Дата проводки
        /// </summary>
        public virtual DateTime? AcceptanceDate { get; protected set; }

        /// <summary>
        /// Проведен ли реестр цен
        /// </summary>
        public virtual bool IsAccepted
        {
            get { return AcceptanceDate != null; }
        }

        /// <summary>
        /// Наименование реестра цен (№ "Номер" от "дата")
        /// </summary>
        public virtual string Name
        {
            get { return "№ " + Number + " от " + this.StartDate.ToShortDateString(); }
        }

        /// <summary>
        /// Куратор реестра цен
        /// </summary>
        public virtual User Curator { get; set; }

        /// <summary>
        /// Статус реестра цен
        /// </summary>
        public virtual AccountingPriceListState State
        {
            get { return state; }
            protected set
            {
                if (!Enum.IsDefined(typeof(AccountingPriceListState), value))
                {
                    throw new Exception("Невозможно присвоить полю «Статус» реестра цен недопустимое значение.");
                }
                state = value;
            }
        }
        private AccountingPriceListState state;

        /// <summary>
        /// Правило расчета учетной цены по умолчанию
        /// </summary>
        public virtual AccountingPriceCalcRule AccountingPriceCalcRule { get; set; }

        /// <summary>
        /// Правило формирования последней цифры в учетной цене
        /// </summary>
        public virtual LastDigitCalcRule LastDigitCalcRule { get; set; }

        /// <summary>
        /// Признак: осуществлена ли переоценка на начало периода действия РЦ
        /// </summary>
        public virtual bool IsRevaluationOnStartCalculated { get; set; }

        /// <summary>
        /// Признак: осуществлена ли переоценка на конец периода действия РЦ
        /// </summary>
        public virtual bool IsRevaluationOnEndCalculated { get; set; }

        #endregion

        #region Работа со списком мест хранения
       
        /// <summary>
        /// Список мест хранения
        /// </summary>
        public virtual IEnumerable<Storage> Storages
        {
            get { return new ImmutableSet<Storage>(storages); }
        }
        private Iesi.Collections.Generic.ISet<Storage> storages;

        /// <summary>
        /// Установка нового вида распространения и нового списка мест хранения
        /// </summary>
        public virtual void SetStorageList(IEnumerable<Storage> newStorageList)
        {
            if (newStorageList == null)
            {
                throw new Exception("Список мест хранения не указан.");
            }
            if (State == AccountingPriceListState.Accepted)
            {
                throw new Exception("Невозможно отредактировать проведенный документ.");
            }
            if (Reason == AccountingPriceListReason.Storage)
            {
                throw new Exception("Невозможно изменить список мест хранения в реестре с основанием «По месту хранения».");
            }
            if (newStorageList.Count() != newStorageList.Distinct().Count())
            {
                throw new Exception("Список мест хранения содержит повторяющиеся элементы.");
            }

            storages.Clear();
            storages.AddAll(newStorageList as ICollection<Storage>);
        }

        /// <summary>
        /// Установка нового вида распространения и добавление в список одного места хранения
        /// </summary>
        public virtual void AddStorage(Storage storage)
        {
            ValidationUtils.NotNull(storage, "Место хранения не указано.");

            CheckPossibilityToAddStorage();

            if (storages.Contains(storage))
            {
                throw new Exception("Место хранения уже имеется в списке.");
            }
            
            storages.Add(storage);
        }

        /// <summary>
        /// Установка нового вида распространения и удаление из списка одного места хранения
        /// </summary>
        public virtual void RemoveStorage(Storage storage)
        {
            ValidationUtils.NotNull(storage, "Место хранения не указано.");

            CheckPossibilityToRemoveStorage();

            if (!storages.Contains(storage))
            {
                throw new Exception("Место хранения не содержится в списке.");
            }

            if (storages.Count() == 1)
            {
                throw new Exception("Невозможно удалить последнее место хранения из распространения.");
            }
            
            storages.Remove(storage);
        }

        #endregion

        #region Основание

        /// <summary>
        /// Основание для создания реестра цен
        /// </summary>
        public virtual AccountingPriceListReason Reason
        {
            get { return reason; }
            protected set
            {
                if (!Enum.IsDefined(typeof(AccountingPriceListReason), value))
                {
                    throw new Exception("Невозможно присвоить полю «Основание» реестра недопустимое значение.");
                }
                reason = value;
            }
        }
        private AccountingPriceListReason reason;

        /// <summary>
        /// Код приходной накладной, на основании которой сформирован реестр цен
        /// </summary>
        public virtual Guid? ReasonReceiptWaybillId { get; protected set; }

        public virtual string ReasonReceiptWaybillNumber { get; protected set; }
        public virtual DateTime? ReasonReceiptWaybillDate { get; protected set; }

        /// <summary>
        /// Строка, описывающая основание для создания реестра цен (для прихода содержит дату и номер)
        /// </summary>
        public virtual string ReasonDescription
        {
            get
            {
                string format = Reason.GetDisplayName();

                return Reason == AccountingPriceListReason.ReceiptWaybill ?
                    String.Format(format, ReasonReceiptWaybillNumber, ((DateTime)ReasonReceiptWaybillDate).ToShortDateString()) : format;
            }
        }

        #endregion

        #region Работа со списком товаров

        /// <summary>
        /// Список товаров реестра цен
        /// </summary>
        public virtual IEnumerable<ArticleAccountingPrice> ArticlePrices
        {
            get { return new ImmutableSet<ArticleAccountingPrice>(articlePrices); }
        }
        private Iesi.Collections.Generic.ISet<ArticleAccountingPrice> articlePrices;

        protected void SetArticleAccountingPriceList(IEnumerable<ArticleAccountingPrice> newArticleAccountingPriceList)
        {
            articlePrices.Clear();
            articlePrices.AddAll(newArticleAccountingPriceList as ICollection<ArticleAccountingPrice>);
        }

        protected virtual void CheckArticleListForUniqueness(IEnumerable<ArticleAccountingPrice> articleAccountingPriceList)
        {
            var array = articleAccountingPriceList.ToArray<ArticleAccountingPrice>();

            foreach (ArticleAccountingPrice item in articleAccountingPriceList)
            {
                if (articleAccountingPriceList.Where(x => areArticlePricesEqual(x, item)).Count() > 1)
                {
                    throw new Exception("Список товаров содержит повторяющиеся элементы.");
                }
            }
        }

        /// <summary>
        /// Установить владельца для списка товаров
        /// </summary>
        protected virtual void SetArticleAccountingPriceOwner(IEnumerable<ArticleAccountingPrice> articleAccountingPriceList, AccountingPriceList newOwner)
        {
            foreach (ArticleAccountingPrice price in articleAccountingPriceList)
            {
                price.AccountingPriceList = newOwner;
            }
        }

        /// <summary>
        /// Предикат, определяющий, равны ли два элемента (товара) реестра цен.
        /// Используется в лямбдах и методах работы с коллекциями
        /// </summary>
        private Func<ArticleAccountingPrice, ArticleAccountingPrice, bool> areArticlePricesEqual = ((x, y) => x == y || x.Article == y.Article);

        /// <summary>
        /// Добавление позиции РЦ
        /// </summary>
        public virtual void AddArticleAccountingPrice(ArticleAccountingPrice price)
        {
            if (IsAlreadyAddedArticle(price))
            {
                throw new Exception("Товар уже имеется в списке.");
            }

            articlePrices.Add(price);
            price.AccountingPriceList = this;
        }

        /// <summary>
        /// Проверка налиция позиции товара в РЦ
        /// </summary>
        public virtual bool IsAlreadyAddedArticle(ArticleAccountingPrice price)
        {
            ValidationUtils.NotNull(price, "Позиция реестра цен не указана.");

            if (articlePrices.Where(x => areArticlePricesEqual(x, price)).Any())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Удаление позиции РЦ
        /// </summary>
        public virtual void DeleteArticleAccountingPrice(ArticleAccountingPrice price)
        {
            ValidationUtils.NotNull(price, "Позиция реестра цен не указана.");

            CheckPossibilityToDeleteRow();

            if (!articlePrices.Where(x => areArticlePricesEqual(x, price)).Any())
            {
                throw new Exception("Товар не содержится в списке.");
            }

            price.DeletionDate = DateTime.Now;
            articlePrices.Remove(price);
        }

        /// <summary>
        /// Количество позиций в списке "состав - товары" по реестру цен
        /// </summary>
        public virtual int ArticleAccountingPriceCount
        {
            get { return articlePrices.Count; }
        }

        #endregion

        #region Конструкторы

        protected AccountingPriceList()
        {
        }

        /// <summary>
        /// Конструктор, инициализирующий общие для всех конструкторов данные
        /// </summary>
        private AccountingPriceList(string number, DateTime startDate, DateTime? endDate, AccountingPriceCalcRule priceCalcRule, LastDigitCalcRule lastDigitRule, User curator)            
        {
            if (String.IsNullOrWhiteSpace(number))
            {
                throw new Exception("Номер не указан.");
            }

            CreationDate = DateTimeUtils.GetCurrentDateTime();
            state = AccountingPriceListState.New;
            storages = new HashedSet<Storage>();
            articlePrices = new HashedSet<ArticleAccountingPrice>();
            IsRevaluationOnStartCalculated = false;
            IsRevaluationOnEndCalculated = false;

            CheckAndCorrectDates(ref startDate, ref endDate, CreationDate);

            Number = number;
            StartDate = startDate;
            EndDate = endDate;
            AccountingPriceCalcRule = priceCalcRule ?? AccountingPriceCalcRule.GetDefault();
            LastDigitCalcRule = lastDigitRule ?? LastDigitCalcRule.GetDefault();
            Curator = curator;
        }

        /// <summary>
        /// Конструктор, для распространения на один склад
        /// </summary>
        private AccountingPriceList(string number, DateTime startDate, DateTime? endDate, AccountingPriceCalcRule priceCalcRule, LastDigitCalcRule lastDigitRule,  IEnumerable<Storage> storageList, User curator)
            : this(number, startDate, endDate, priceCalcRule, lastDigitRule, curator)
        {
            if (storageList.Any())
            {
                SetStorageList(storageList);
            }
            else
            {
                throw new Exception("Не выбрано ни одного места хранения.");
            }
        }

        /// <summary>
        /// Конструктор для распространения из нескольких складов
        /// </summary>
        private AccountingPriceList(string number, DateTime startDate, DateTime? endDate, AccountingPriceCalcRule priceCalcRule, LastDigitCalcRule lastDigitRule, Storage storage, User curator)
            : this(number, startDate, endDate, priceCalcRule, lastDigitRule, curator)
        {
            storages.Add(storage);
        }

        /// <summary>
        /// Конструктор для основания "Переоценка"
        /// </summary>
        public AccountingPriceList(string number, DateTime startDate, DateTime? endDate,
            IEnumerable<Storage> storageList, User curator, AccountingPriceCalcRule priceCalcRule = null, LastDigitCalcRule lastDigitRule = null)
            : this(number, startDate, endDate, priceCalcRule, lastDigitRule, storageList, curator)
        {
            Reason = AccountingPriceListReason.Revaluation;            
        }

        /// <summary>
        /// Конструктор для основания "Приход"
        /// </summary>
        public AccountingPriceList(string number, DateTime startDate, DateTime? endDate, ReceiptWaybill receiptWaybill, IEnumerable<Storage> storageList,
            IEnumerable<ArticleAccountingPrice> articleAccountingPriceList, User curator, AccountingPriceCalcRule priceCalcRule = null, LastDigitCalcRule lastDigitRule = null)
            : this(number, startDate, endDate, priceCalcRule, lastDigitRule, storageList, curator)
        {
            Reason = AccountingPriceListReason.ReceiptWaybill;

            ReasonReceiptWaybillId = receiptWaybill.Id;
            ReasonReceiptWaybillDate = receiptWaybill.Date;
            ReasonReceiptWaybillNumber = receiptWaybill.Number;

            CheckArticleListForUniqueness(articleAccountingPriceList);
            SetArticleAccountingPriceOwner(articleAccountingPriceList, this);
            SetArticleAccountingPriceList(articleAccountingPriceList);
        }

        /// <summary>
        /// Конструктор для основания "По месту хранения"
        /// </summary>
        public AccountingPriceList(string number, DateTime startDate, DateTime? endDate, Storage storage,
            IEnumerable<ArticleAccountingPrice> articleAccountingPriceList, User curator, AccountingPriceCalcRule priceCalcRule = null, LastDigitCalcRule lastDigitRule = null)
            : this(number, startDate, endDate, priceCalcRule, lastDigitRule, storage, curator)
        {
            Reason = AccountingPriceListReason.Storage;

            CheckArticleListForUniqueness(articleAccountingPriceList);
            SetArticleAccountingPriceOwner(articleAccountingPriceList, this);
            SetArticleAccountingPriceList(articleAccountingPriceList);
        }

        #endregion

        #region Методы

        #region Проводка и отмена проводки

        /// <summary>
        /// Проводка
        /// </summary>
        /// <returns>true - дата начала действия была автоматически изменена на текущую</returns>
        public virtual bool Accept(DateTime currentDateTime)
        {
            CheckPossibilityToAccept();

            bool dateChanged = CheckAndCorrectDates(currentDateTime);

            State = AccountingPriceListState.Accepted;
            AcceptanceDate = currentDateTime;

            foreach (var articlePrice in ArticlePrices) // "забываем", что не получилось использовать заданные правила расчета учетной цены
            {
                articlePrice.ErrorLastDigitCalculation = false;
                articlePrice.ErrorAccountingPriceCalculation = false;
            }

            // если дата начала действия РЦ меньше даты проводки - устанавливаем дату начала действия равной дате проводке
            if (StartDate < AcceptanceDate)
            {
                StartDate = AcceptanceDate.Value;
            }

            ValidationUtils.Assert(EndDate == null || EndDate > StartDate, "Дата окончания действия реестра цен должна быть больше текущей даты.");

            return dateChanged;
        }

        /// <summary>
        /// Отмена проводки
        /// </summary>
        public virtual void CancelAcceptance()
        {
            CheckPossibilityToCancelAcceptance();

            State = AccountingPriceListState.New;
            AcceptanceDate = null;
        }

        #endregion

        #region Проверки дат

        /// <summary>
        /// Проверка дат действия реестра цен на валидность. В случае ошибки выбрасывается исключение.
        /// Основана на одноименном статическом методе.
        /// </summary>
        /// <returns>true, если день даты начала действия был автоматически изменен</returns>
        public virtual bool CheckAndCorrectDates(DateTime currentDateTime)
        {
            DateTime startDate = StartDate;
            DateTime? endDate = EndDate;

            bool dateChanged = CheckAndCorrectDates(ref startDate, ref endDate, currentDateTime);

            StartDate = startDate;
            EndDate = endDate;

            return dateChanged;
        }

        /// <summary>
        /// Проверка дат действия реестра цен на валидность. В случае ошибки выбрасывается исключение.
        /// </summary>
        /// <param name="startDate">Дата начала действия (для проверки)</param>
        /// <param name="endDate">Дата конца действия (для проверки)</param>
        /// <returns>true, если день даты начала действия был автоматически изменен</returns>
        public static bool CheckAndCorrectDates(ref DateTime startDate, ref DateTime? endDate, DateTime currentDateTime)
        {
            bool startDateCorrected = SetTimeForStartDateAndCheck(ref startDate, currentDateTime);

            if (endDate.HasValue)
            {
                if (endDate.Value <= startDate)
                {
                    throw new Exception("Дата конца действия реестра цен должна быть позже даты начала действия и текущей даты.");
                }
            }

            return startDateCorrected;
        }

        /// <summary>
        /// Установить время для даты начала действия реестра цен и проверить, не устарела ли она.
        /// Если нужно, провести коррекцию (приведение к сегодняшней дате).
        /// </summary>
        /// <param name="startDate">проверяемая дата начала действия</param>
        /// <returns>true, если дата начала действия РЦ была изменена</returns>
        public static bool SetTimeForStartDateAndCheck(ref DateTime startDate, DateTime currentDateTime)
        {
            if (startDate < currentDateTime)
            {
                startDate = currentDateTime;

                return true;
            }

            return false;
        }

        #endregion

        #region Проверки на возможность выполнения операций

        public virtual void CheckPossibilityToEdit()
        {
            ValidationUtils.Assert(State == AccountingPriceListState.New, "Невозможно изменить проведенный реестр цен.");
        }

        public virtual void CheckPossibilityToDelete()
        {
            ValidationUtils.Assert(State == AccountingPriceListState.New, "Невозможно удалить проведенный реестр цен.");
        }

        public virtual void CheckPossibilityToAddRow()
        {
            ValidationUtils.Assert(State == AccountingPriceListState.New, "Невозможно добавить позицию в проведенный реестр цен.");
        }

        public virtual void CheckPossibilityToEditRow()
        {
            ValidationUtils.Assert(State == AccountingPriceListState.New, "Невозможно изменить позицию проведенного реестра цен.");
        }

        public virtual void CheckPossibilityToDeleteRow()
        {
            ValidationUtils.Assert(State == AccountingPriceListState.New, "Невозможно удалить позицию проведенного реестра цен.");            
        }

        public virtual void CheckPossibilityToEditPrice()
        {
            ValidationUtils.Assert(State == AccountingPriceListState.New, "Невозможно изменить позицию проведенного реестра цен.");
        }

        public virtual void CheckPossibilityToAccept()
        {
            ValidationUtils.Assert(State == AccountingPriceListState.New,
                String.Format("Невозможно провести реестр со статусом {0}", State.GetDisplayName()));
            
            ValidationUtils.Assert(Storages.Any(), "Невозможно провести реестр цен без мест хранения.");
            
            ValidationUtils.Assert(ArticlePrices.Any(), "Невозможно провести реестр цен без товаров.");
        }

        public virtual void CheckPossibilityToCancelAcceptance()
        {
            ValidationUtils.Assert(State == AccountingPriceListState.Accepted,
                String.Format("Невозможно отменить проводку реестра со статусом «{0}».", State.GetDisplayName()));

            ValidationUtils.Assert(!IsRevaluationOnStartCalculated && !IsRevaluationOnEndCalculated, 
                "Невозможно отменить вступивший в действие реестр цен.");
        }

        public virtual void CheckPossibilityToAddStorage()
        {
            if (State != AccountingPriceListState.New)
            {
                throw new Exception(String.Format("Невозможно добавить место хранения в распространение реестра со статусом «{0}».", State.GetDisplayName()));
            }
            if (Reason == AccountingPriceListReason.Storage)
            {
                throw new Exception(String.Format("Невозможно добавить место хранения в распространение реестра с основанием «{0}».", Reason.GetDisplayName()));
            }
        }
        
        public virtual void CheckPossibilityToRemoveStorage()
        {
            if (State != AccountingPriceListState.New)
            {
                throw new Exception(String.Format("Невозможно удалить место хранения из распространения реестра со статусом «{0}».", State.GetDisplayName()));
            }
            if (Reason == AccountingPriceListReason.Storage)
            {
                throw new Exception(String.Format("Невозможно удалить место хранения из распространения реестра с основанием «{0}».", Reason.GetDisplayName()));
            }
        }
        
        #endregion

        #endregion
        
    }
}
