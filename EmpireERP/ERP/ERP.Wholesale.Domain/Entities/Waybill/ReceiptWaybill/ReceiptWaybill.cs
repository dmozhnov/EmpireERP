using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Приходная накладная
    /// </summary>
    public class ReceiptWaybill : BaseWaybill
    {
        #region Свойства

        /// <summary>
        /// Место хранения - приемщик
        /// </summary>
        public virtual Storage ReceiptStorage { get; protected set; }

        /// <summary>
        /// Собственная организация
        /// </summary>
        public virtual AccountOrganization AccountOrganization { get; protected set; }

        public virtual Contractor Contractor
        {
            get
            {
                return IsCreatedFromProductionOrderBatch ? (Contractor)ProductionOrderBatch.ProductionOrder.Producer : (Contractor)Provider;
            }
        }

        /// <summary>
        /// Название поставщика или производителя (если накладная создана по партии заказа)
        /// </summary>
        public virtual string ContractorName
        {
            get
            {
                return IsCreatedFromProductionOrderBatch ? ProductionOrderBatch.ProductionOrder.Producer.Name : Provider.Name;
            }
        }

        /// <summary>
        /// Поставщик
        /// </summary>
        public virtual Provider Provider { get; protected set; }

        /// <summary>
        /// Номер накладной поставщика
        /// </summary>
        public virtual string ProviderNumber { get; set; }

        /// <summary>
        /// Дата накладной поставщика
        /// </summary>
        public virtual DateTime? ProviderDate { get; set; }

        /// <summary>
        /// Номер и дата накладной поставщика
        /// </summary>
        public virtual string ProviderWaybillName
        {
            get { return (!String.IsNullOrEmpty(ProviderNumber) ? "№ " + ProviderNumber : "") + (ProviderDate != null ? " от " + ProviderDate.Value.ToShortDateString() : ""); }
        }

        /// <summary>
        /// Номер счет-фактуры поставщика
        /// </summary>
        public virtual string ProviderInvoiceNumber { get; set; }

        /// <summary>
        /// Дата счет-фактуры поставщика
        /// </summary>
        public virtual DateTime? ProviderInvoiceDate { get; set; }

        /// <summary>
        /// Номер и дата счет-фактуры поставщика
        /// </summary>
        public virtual string ProviderInvoice
        {
            get { return (!String.IsNullOrEmpty(ProviderInvoiceNumber) ? "№ " + ProviderInvoiceNumber : "") + (ProviderInvoiceDate != null ? " от " + ProviderInvoiceDate.Value.ToShortDateString() : ""); }
        }

        /// <summary>
        /// Организация контрагента
        /// </summary>
        public virtual ContractorOrganization ContractorOrganization
        {
            get
            {
                return IsCreatedFromProductionOrderBatch ? ProductionOrderBatch.ProductionOrder.Producer.Organization : ProviderContract.ContractorOrganization;
            }
        }

        /// <summary>
        /// Название организации поставщика или производителя (если накладная создана по партии заказа)
        /// </summary>
        public virtual string ContractorOrganizationName
        {
            get
            {
                return IsCreatedFromProductionOrderBatch ? ProductionOrderBatch.ProductionOrder.Producer.OrganizationName : ProviderContract.ContractorOrganization.ShortName;
            }
        }

        /// <summary>
        /// Договор с поставщиком
        /// </summary>
        public virtual ProviderContract ProviderContract { get; set; }

        /// <summary>
        /// Договор с контрагентом
        /// </summary>
        public virtual Contract Contract 
        { 
            get 
            {
                return IsCreatedFromProductionOrderBatch ? (Contract)ProductionOrderBatch.ProductionOrder.Contract : ProviderContract;
            } 
        }

        /// <summary>
        /// Связанная с приходом партия заказа
        /// </summary>
        public virtual ProductionOrderBatch ProductionOrderBatch { get; protected set; }

        /// <summary>
        /// Создана ли накладная по партии заказа
        /// </summary>
        public virtual bool IsCreatedFromProductionOrderBatch { get { return ProductionOrderBatch != null; } }

        /// <summary>
        /// Номер ГТД (грузовая таможенная декларация)
        /// </summary>
        public virtual string CustomsDeclarationNumber { get; set; }

        /// <summary>
        /// Признак того, откуда берется номер ГТД
        /// true - из приходной накладной, false - последний номер по позиции
        /// </summary>
        public virtual bool IsCustomsDeclarationNumberFromReceiptWaybill { get; set; }

        /// <summary>
        /// Ожидаемая сумма. Для установки значения желательно пользоваться функцией SetPendingSum из ReceiptWaybillService.
        /// </summary>
        public virtual decimal PendingSum
        {
            get { return pendingSum; }
            set
            {
                ValidationUtils.Assert(value >= 0M, "Общая сумма по накладной не может быть меньше 0.");
                ValidationUtils.CheckDecimalScale(value, 2, "Общая сумма по накладной должна иметь не более 2 знаков после запятой.");
                pendingSum = value;
            }
        }
        private decimal pendingSum;

        /// <summary>
        /// Ожидаемая сумма по строкам
        /// </summary>
        public virtual decimal PendingSumByRows
        {
            get
            {
                return Math.Round(Rows.Sum(x => Math.Round(x.InitialPurchaseCost * x.PendingCount, 6)), 2);
            }
        }

        /// <summary>
        /// Согласованная сумма по строкам (имеет смысл, когда накладная согласована)
        /// </summary>
        public virtual decimal ApprovedSumByRows
        {
            get
            {
                return Math.Round(Rows.Sum(x => Math.Round(x.ApprovedPurchaseCost.Value * x.ApprovedCount.Value, 6)), 2);
            }
        }

        /// <summary>
        /// Ожидаемая ставка НДС
        /// </summary>
        public virtual ValueAddedTax PendingValueAddedTax { get; set; }

        /// <summary>
        /// Сумма по позициям по документу (указывается при приемке на склад)
        /// </summary>
        public virtual decimal ReceiptedSum { get { return Rows.Sum(x => x.ProviderSum.GetValueOrDefault()); } }

        /// <summary>
        /// Согласованная сумма
        /// </summary>
        public virtual decimal? ApprovedSum { get; set; } // TODO: переделать на верблюжий горб и поставить проверку на 2 знака после запятой, как в PendingSum

        /// <summary>
        /// Текущая сумма накладной
        /// </summary>
        public virtual decimal CurrentSum { get { return IsApproved ? ApprovedSum.Value : PendingSum; } }

        /// <summary>
        /// Текущая сумма НДС
        /// </summary>
        public virtual decimal ValueAddedTaxSum { get { return Rows.Sum(x => x.ValueAddedTaxSum); } }

        /// <summary>
        /// Процент скидки
        /// </summary>
        public virtual decimal DiscountPercent { get { return CurrentSum != 0M ? Math.Round(DiscountSum / CurrentSum * 100M, 2) : 100M; } }

        /// <summary>
        /// Ожидаемая сумма скидки
        /// </summary>
        public virtual decimal PendingDiscountSum
        {
            get { return pendingDiscountSum; }
            set
            {
                ValidationUtils.Assert(value >= 0M, "Сумма скидки по накладной не может быть меньше 0.");
                ValidationUtils.CheckDecimalScale(value, 2, "Сумма скидки по накладной должна иметь не более 2 знаков после запятой.");
                pendingDiscountSum = value;
            }
        }
        private decimal pendingDiscountSum;

        public virtual decimal DiscountSum
        {
            get
            {
                return (State == ReceiptWaybillState.ApprovedWithoutDivergences ? CalculateDiscountSum() :
                    !State.ContainsIn(
                    ReceiptWaybillState.ReceiptedWithCountDivergences, ReceiptWaybillState.ReceiptedWithSumAndCountDivergences,
                    ReceiptWaybillState.ReceiptedWithSumDivergences, ReceiptWaybillState.ApprovedFinallyAfterDivergences) ?
                    PendingDiscountSum : 0M);
            }
        }

        /// <summary>
        /// Дата удаления
        /// </summary>
        public virtual DateTime? DeletionDate
        {
            get { return deletionDate; }
            set
            {
                // Запрещаем повторную пометку об удалении
                if (deletionDate == null && value != null)
                {
                    ValidationUtils.Assert(!AreOutgoingWaybills, "Невозможно удалить приходную накладную, так как товар из нее используется в других документах.");
                    deletionDate = value;

                    foreach (var row in Rows)
                    {
                        if (row.DeletionDate == null)
                        {
                            row.DeletionDate = deletionDate;
                        }
                    }

                    if (IsCreatedFromProductionOrderBatch)
                    {
                        foreach (var productionOrderBatchRow in ProductionOrderBatch.Rows)
                        {
                            productionOrderBatchRow.ReceiptWaybillRow = null;
                        }

                        ProductionOrderBatch.ReceiptWaybill = null;
                    }
                }
            }
        }
        protected DateTime? deletionDate;

        /// <summary>
        /// Статус приходной накладной
        /// </summary>
        public virtual ReceiptWaybillState State { get; protected set; }

        /// <summary>
        /// Строки (состав) накладной
        /// </summary>
        public new virtual IEnumerable<ReceiptWaybillRow> Rows
        {
            get { return new ImmutableSet<BaseWaybillRow>(rows).Select(x => x.As<ReceiptWaybillRow>()); }
        }

        /// <summary>
        /// Есть ли исходящие документы
        /// </summary>
        public virtual bool AreOutgoingWaybills { get { return Rows.Any(x => x.AreOutgoingWaybills); } }

        /// <summary>
        /// Находится ли накладная в статусе «Новая»
        /// </summary>
        public virtual bool IsNew
        {
            get { return State == ReceiptWaybillState.New; }
        }

        /// <summary>
        /// Принята ли накладная на склад
        /// </summary>
        public virtual bool IsReceipted
        {
            get
            {
                return (State.ContainsIn(ReceiptWaybillState.ReceiptedWithCountDivergences, ReceiptWaybillState.ApprovedFinallyAfterDivergences,
                    ReceiptWaybillState.ReceiptedWithSumAndCountDivergences, ReceiptWaybillState.ReceiptedWithSumDivergences,
                    ReceiptWaybillState.ApprovedWithoutDivergences));
            }
        }

        /// <summary>
        /// True, если накладная принята на склад с какими-либо расхождениями, но еще не согласована
        /// </summary>
        public virtual bool IsReceiptedWithDivergences
        {
            get
            {
                return (State.ContainsIn(ReceiptWaybillState.ReceiptedWithCountDivergences, ReceiptWaybillState.ReceiptedWithSumAndCountDivergences,
                    ReceiptWaybillState.ReceiptedWithSumDivergences));
            }
        }

        /// <summary>
        /// Согласована ли накладная
        /// </summary>
        public virtual bool IsApproved
        {
            get { return (State.ContainsIn(ReceiptWaybillState.ApprovedFinallyAfterDivergences, ReceiptWaybillState.ApprovedWithoutDivergences)); }
        }

        /// <summary>
        /// Проведена ли накладная
        /// </summary>
        public virtual bool IsAccepted
        {
            get
            {
                return (State.ContainsIn(
                    ReceiptWaybillState.AcceptedDeliveryPending, ReceiptWaybillState.ReceiptedWithCountDivergences,
                    ReceiptWaybillState.ReceiptedWithSumAndCountDivergences, ReceiptWaybillState.ReceiptedWithSumDivergences,
                    ReceiptWaybillState.ApprovedFinallyAfterDivergences, ReceiptWaybillState.ApprovedWithoutDivergences));
            }
        }

        /// <summary>
        /// Ожидает ли накладная
        /// </summary>
        public virtual bool IsPending
        {
            get
            {
                return (State.ContainsIn(ReceiptWaybillState.New, ReceiptWaybillState.AcceptedDeliveryPending));
            }
        }

        /// <summary>
        /// Сходится ли ожидаемая сумма накладной с ожидаемой суммой по позициям
        /// </summary>
        public virtual bool AreSumDivergences
        {
            get
            {
                return (PendingSum != PendingSumByRows);
            }
        }

        /// <summary>
        /// Определяем, было ли расхождение по сумме при приемке на склад
        /// </summary>
        public virtual bool AreSumDivergencesAfterReceipt
        {
            get
            {
                return Rows.Any(x => x.AreSumDivergencesAfterReceipt);
            }
        }

        /// <summary>
        /// Есть ли в накладной позиции, по которым были расхождения при приемке
        /// </summary>
        public virtual bool AreDivergencesAfterReceipt
        {
            get
            {
                return Rows.Any(x => x.AreDivergencesAfterReceipt);
            }
        }

        /// <summary>
        /// Есть ли в накладной внутренние расхождения по количеству
        /// </summary>
        public virtual bool AreCountDivergencesAfterReceipt
        {
            get 
            {
                return Rows.Any(x => x.AreCountDivergencesAfterReceipt && x.PendingCount > 0M || x.PendingCount == 0M);
            }
        }

        /// <summary>
        /// Процент расхождений по количеству
        /// </summary>
        public virtual decimal CountDivergencePercent
        {
            get
            {
                decimal divergenceRowCount = Rows.Count(x => x.AreCountDivergencesAfterReceipt && x.PendingCount > 0M);
                decimal addedFromReceiptRowCount = Rows.Count(x => x.PendingCount == 0M);

                return PendingRowCount != 0 ? (divergenceRowCount + addedFromReceiptRowCount) / (decimal)PendingRowCount * 100M : 0M;
            }
        }

        /// <summary>
        /// Пользователь, принявший накладную
        /// </summary>
        public virtual User ReceiptedBy { get; protected set; }

        /// <summary>
        /// Дата приемки
        /// </summary>
        public virtual DateTime? ReceiptDate { get; protected set; }

        /// <summary>
        /// Пользователь, согласовавший накладную
        /// </summary>
        public virtual User ApprovedBy { get; protected set; }

        /// <summary>
        /// Дата согласования
        /// </summary>
        public virtual DateTime? ApprovementDate { get; protected set; }

        /// <summary>
        /// Количество принятых позиций (исключаются те, у которых принятое количество = 0)
        /// </summary>
        public virtual int ReceiptedRowCount { get { return Rows.Count(r => r.ReceiptedCount > 0M); } }

        /// <summary>
        /// Количество позиций из ожидания
        /// </summary>
        public virtual int PendingRowCount { get { return Rows.Count(x => x.PendingCount > 0M); } }

        /// <summary>
        /// Проверка на резервирование товаров
        /// </summary>
        public virtual bool HasReservedArticles
        {
            get 
            {
                return Rows.Any(row => row.AreOutgoingWaybills);
            }
        }

        #endregion

        #region Конструкторы

        protected ReceiptWaybill()
            : base(WaybillType.ReceiptWaybill)
        {
        }

        /// <summary>
        /// Конструктор для обычной приходной накладной (не связанной с партией заказа)
        /// </summary>
        /// <param name="number">Номер</param>
        /// <param name="date">Дата</param>
        /// <param name="receiptStorage">Место хранения</param>
        /// <param name="accountOrganization">Собственная организация</param>
        /// <param name="provider">Поставщик</param>
        /// <param name="pendingSum">Ожидаемая сумма накладной</param>
        /// <param name="pendingDiscountSum">Ожидаемая сумма скидки</param>
        /// <param name="pendingValueAddedTax">Ожидаемая НДС</param>
        /// <param name="providerContract">Договор с поставщиком</param>
        /// <param name="curator">Куратор</param>
        public ReceiptWaybill(string number, DateTime date, Storage receiptStorage, AccountOrganization accountOrganization, Provider provider, decimal pendingSum, 
            decimal pendingDiscountSum, ValueAddedTax pendingValueAddedTax, ProviderContract providerContract, string customsDeclarationNumber, User curator, User createdBy, DateTime creationDate)
            : this(number, date, receiptStorage, accountOrganization, pendingSum, pendingDiscountSum, pendingValueAddedTax, customsDeclarationNumber, curator, createdBy, creationDate)
        {
            ValidationUtils.NotNull(provider, "Необходимо указать поставщика.");
            Provider = provider;
            ValidationUtils.NotNull(providerContract, "Необходимо указать договор.");
            ChangeProvider(provider, providerContract);
        }

        /// <summary>
        /// Конструктор для создания приходной накладной, связанной с партией заказа
        /// </summary>
        /// <param name="productionOrderBatch">Партия заказа</param>
        /// <param name="number">Номер</param>
        /// <param name="date">Дата</param>
        /// <param name="pendingValueAddedTax">Ожидаемая НДС</param>
        /// <param name="curator">Куратор</param>
        public ReceiptWaybill(ProductionOrderBatch productionOrderBatch, string number, DateTime date, ValueAddedTax pendingValueAddedTax, string customsDeclarationNumber, 
            User curator, User createdBy, DateTime creationDate)
            : this(number, date, productionOrderBatch.ProductionOrder.Storage, productionOrderBatch.ProductionOrder.Contract.AccountOrganization, 0, 0, pendingValueAddedTax, 
            customsDeclarationNumber, curator, createdBy, creationDate)
        {
            ValidationUtils.NotNull(productionOrderBatch, "Необходимо указать партию заказа.");
            ValidationUtils.Assert(productionOrderBatch.ReceiptWaybill == null, "Указанная партия заказа уже связана с приходной накладной.");
            ProductionOrderBatch = productionOrderBatch;
            ProductionOrderBatch.ReceiptWaybill = this;
        }

        /// <summary>
        /// Базовый конструктор для создания приходной накладной
        /// </summary>
        /// <param name="number">Номер</param>
        /// <param name="date">Дата</param>
        /// <param name="receiptStorage">Место хранения</param>
        /// <param name="accountOrganization">Собственная организация</param>
        /// <param name="pendingSum">Ожидаемая сумма накладной</param>
        /// <param name="pendingDiscountSum">Ожидаемая сумма скидки</param>
        /// <param name="pendingValueAddedTax">Ожидаемая НДС</param>
        /// <param name="curator">Куратор</param>
        private ReceiptWaybill(string number, DateTime date, Storage receiptStorage, AccountOrganization accountOrganization, decimal pendingSum, decimal pendingDiscountSum, 
            ValueAddedTax pendingValueAddedTax, string customsDeclarationNumber, User curator, User createdBy, DateTime creationDate)
            : base(WaybillType.ReceiptWaybill, number, date, curator, createdBy, creationDate)
        {
            ValidationUtils.NotNull(receiptStorage, "Необходимо указать место хранения.");
            ReceiptStorage = receiptStorage;
            ValidationUtils.NotNull(accountOrganization, "Необходимо указать организацию.");
            AccountOrganization = accountOrganization;
            ValidationUtils.NotNull(pendingValueAddedTax, "Необходимо указать ставку НДС.");
            PendingValueAddedTax = pendingValueAddedTax;
            ValidationUtils.CheckDecimalScale(pendingSum, 2, "Общая сумма по накладной должна иметь не более 2 знаков после запятой.");
            PendingSum = pendingSum;
            PendingDiscountSum = pendingDiscountSum;
            State = ReceiptWaybillState.New;


            if (String.IsNullOrEmpty(customsDeclarationNumber))
            {
                CustomsDeclarationNumber = String.Empty;
                IsCustomsDeclarationNumberFromReceiptWaybill = false;
            }
            else
            {
                CustomsDeclarationNumber = customsDeclarationNumber;
                IsCustomsDeclarationNumberFromReceiptWaybill = true;
            }
            
            ProviderNumber = String.Empty;
            ProviderInvoiceNumber = String.Empty;
            CheckPendingDiscountSum();
        }

        #endregion

        #region События

        //internal static event EventHandler ReceiptWaybillRowAdded;

        #endregion

        #region Методы

        #region Позиции накладной

        /// <summary>
        /// Добавление позиции в накладную
        /// </summary>
        public virtual void AddRow(ReceiptWaybillRow row)
        {
            if (row.PendingCount > 0M)
            {
                ValidationUtils.Assert(!IsAccepted, "Невозможно добавить позицию к проведенной накладной.");
            }
            else
            {
                CheckPossibilityToEditRowFromReceipt();
            }

            ValidationUtils.Assert(!Rows.Any(x => x.Article == row.Article), "Данный товар уже содержится в накладной.");

            rows.Add(row);
            row.ReceiptWaybill = this;
        }

        /// <summary>
        /// Удаление позиции из накладной
        /// </summary>
        public virtual void DeleteRow(ReceiptWaybillRow row)
        {
            ValidationUtils.Assert(rows.Contains(row), "Позиция не содержится в накладной. Возможно, она была удалена.");
            row.CheckPossibilityToDelete();

            rows.Remove(row);
            row.DeletionDate = DateTime.Now;
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Проверка корректности установки ожидаемой суммы скидки по отношению к ожидаемой сумме всей накладной
        /// </summary>
        public virtual void CheckPendingDiscountSum()
        {
            ValidationUtils.Assert(pendingDiscountSum <= pendingSum, "Сумма скидки не может быть больше суммы по накладной.");
        }

        /// <summary>
        /// Записать в ожидаемую сумму по накладной сумму ожиданий по позициям
        /// </summary>
        public virtual void RecalculatePendingSum()
        {
            PendingSum = PendingSumByRows;
        }

        #endregion

        #region Изменение закупочных цен в накладной

        public virtual void ResetPurchaseCosts()
        {
            foreach (var row in Rows)
            {
                row.InitialPurchaseCost = row.PurchaseCost = 0M;
                row.RecalculatePendingSum();
                row.ProviderSum = row.ProviderSum.HasValue ? row.PendingSum : (decimal?)null;
                row.ApprovedPurchaseCost = row.ApprovedPurchaseCost.HasValue ? row.PurchaseCost : (decimal?)null;
                if (row.ApprovedSum.HasValue)
                {
                    row.RecalculateApprovedSum();
                }
            }

            PendingSum = PendingSumByRows;
            ApprovedSum = ApprovedSum.HasValue ? PendingSum : (decimal?)null;
        }

        #endregion

        #region Смена места хранения и поставщика

        /// <summary>
        /// Смена места хранения
        /// </summary>
        /// <param name="receiptStorage">Новое место хранения</param>
        /// <param name="accountOrganization">Новая организация</param>
        public virtual void ChangeReceiptStorage(Storage receiptStorage, AccountOrganization accountOrganization)
        {
            ValidationUtils.Assert(!IsCreatedFromProductionOrderBatch, "Данный метод не подходит для накладной, созданной по партии заказа.");

            ValidationUtils.NotNull(receiptStorage, "Место хранения не указано.");
            ValidationUtils.NotNull(accountOrganization, "Организация не указана.");
            ValidationUtils.Assert(accountOrganization.Storages.Contains(receiptStorage),
                String.Format("Место хранения «{0}» не связано с организацией «{1}».", receiptStorage.Name, accountOrganization.ShortName));
            ValidationUtils.Assert(ProviderContract == null || ProviderContract.AccountOrganization == accountOrganization,
                "Невозможно изменить место хранения. Собственная организация не соответствует организации, с которой заключен договор.");
            ValidationUtils.Assert(!HasReservedArticles, "Невозможно изменить место хранения и организацию для накладной, по которой имеется резервирование товара.");

            ReceiptStorage = receiptStorage;
            AccountOrganization = accountOrganization;
        }

        /// <summary>
        /// Смена поставщика
        /// </summary>
        /// <param name="provider">Новый поставщик</param>
        /// <param name="contract">Новый договор</param>
        public virtual void ChangeProvider(Provider provider, ProviderContract contract)
        {
            ValidationUtils.Assert(!IsCreatedFromProductionOrderBatch, "Данный метод не подходит для накладной, созданной по партии заказа.");

            ValidationUtils.NotNull(provider, "Поставщик не указан.");
            ValidationUtils.Assert(contract == null || provider.Contracts.Contains(contract), "Указан договор, не связанный с поставщиком.");

            Provider = provider;
            ProviderContract = contract;
        }

        #endregion

        #region Проводка / отмена проводки

        /// <summary>
        /// Проводка накладной
        /// </summary>        
        /// <param name="articleAccountingPriceList">Список строк реестров, по которым назначается учетная цена для товаров накладной</param>
        public virtual void Accept(IEnumerable<ArticleAccountingPrice> articleAccountingPriceList, User acceptedBy, DateTime acceptanceDate)
        {
            CheckPossibilityToAccept();

            foreach (var row in Rows)
            {
                // Выставляем реестры, по которым формируется учетная цена
                var articleAccountingPrice = articleAccountingPriceList.FirstOrDefault(x => x.Article == row.Article);
                ValidationUtils.NotNull(articleAccountingPrice, String.Format("Невозможно выполнить проводку, так как на товар «{0}» не установлена учетная цена.",
                    row.Article.FullName));
                row.RecipientArticleAccountingPrice = articleAccountingPrice;

                // переводим кол-во по позиции в доступное для резервирования
                row.AvailableToReserveCount = row.PendingCount;
            }

            AcceptanceDate = acceptanceDate;
            AcceptedBy = acceptedBy;
            State = ReceiptWaybillState.AcceptedDeliveryPending;
        }

        /// <summary>
        /// Отмена проводки
        /// </summary>
        public virtual void CancelAcceptance(bool checkIfCreatedFromProductionOrderBatch)
        {
            CheckPossibilityToCancelAcceptance(checkIfCreatedFromProductionOrderBatch);

            AcceptanceDate = null;
            AcceptedBy = null;
            State = ReceiptWaybillState.New;

            var rowsToDelete = new List<ReceiptWaybillRow>();

            // Очищаем позиции накладной от данных проводки и приемки
            foreach (var row in Rows)
            {
                row.RecipientArticleAccountingPrice = null;
                
                // обнуляем доступное для резервирования кол-во товара
                row.AvailableToReserveCount = 0;

                // удаляем позиции, добавленные при приемке
                if (row.PendingCount == 0)
                {
                    rowsToDelete.Add(row);
                }
                else
                {
                    row.ReceiptedCount = null;
                    row.ProviderCount = null;
                    row.ProviderSum = null;                    
                }
            }

            // удаляем позиции, добавленные при приемке
            foreach (var row in rowsToDelete)
            {
                DeleteRow(row);
            }
        }

        #endregion

        #region Приемка и отмена приемки накладной на склад

        /// <summary>
        /// Приемка обычной (не связанной с партией заказа) накладной на склад
        /// </summary>
        /// <param name="receiptedSum">Общая сумма по накладной</param>
        public virtual void Receipt(decimal receiptedSum, User receiptedBy, DateTime currentDateTime)
        {
            ValidationUtils.Assert(!IsCreatedFromProductionOrderBatch, "Этот метод не подходит для данного типа накладной.");

            ValidationUtils.Assert(receiptedSum >= 0, "Общая сумма по накладной не может быть меньше 0.");
            ValidationUtils.CheckDecimalScale(receiptedSum, 2, "Общая сумма по накладной должна иметь не более 2 знаков после запятой.");
            ValidationUtils.Assert(receiptedSum == Rows.Sum(x => x.ProviderSum), "Общая сумма по накладной не сходится с суммой по позициям.");

            PerformReceipt(receiptedBy, currentDateTime);
        }

        /// <summary>
        /// Приемка накладной, связанной с партией заказа, на склад
        /// </summary>
        public virtual void Receipt(User receiptedBy, DateTime currentDateTime)
        {
            ValidationUtils.Assert(IsCreatedFromProductionOrderBatch, "Этот метод не подходит для данного типа накладной.");

            PerformReceipt(receiptedBy, currentDateTime);
        }

        private void PerformReceipt(User receiptedBy, DateTime receiptDate)
        {
            CheckPossibilityToReceipt();

            // Определяем, есть ли расхождения по количеству и сумме, и автоматически согласуем позиции без расхождений
            bool areReceiptedCountDivergences = false, areReceiptedSumDivergences = false;
            foreach (var row in Rows)
            {
                // условие для определения наличия расхождения по кол-ву
                row.AreCountDivergencesAfterReceipt = row.ReceiptedCount.HasValue && row.ProviderCount.HasValue && 
                    (row.PendingCount != row.ReceiptedCount.Value || row.PendingCount != row.ProviderCount.Value);
                                
                // условие для определения наличия расхождения по сумме
                row.AreSumDivergencesAfterReceipt = (row.ProviderSum.HasValue && Math.Round(row.InitialPurchaseCost * row.PendingCount, 2) != row.ProviderSum.Value);
                
                // если при приемке есть расхождения
                if (row.AreDivergencesAfterReceipt)
                {
                    if (row.AreCountDivergencesAfterReceipt)
                    {
                        areReceiptedCountDivergences = true;
                    }
                    if (row.AreSumDivergencesAfterReceipt)
                    {
                        areReceiptedSumDivergences = true;
                    }

                    // обнуляем доступное для резервирования кол-во товара
                    row.AvailableToReserveCount = 0;
                }
                else
                {
                    // Если расхождений в позиции нет, то сразу выставляем значения (подготовка к Approve)
                    row.ApprovedSum = row.PendingSum;
                    row.ApprovedCount = row.PendingCount;
                    row.ApprovedPurchaseCost = row.PurchaseCost;
                }
            }

            ReceiptDate = receiptDate;
            ReceiptedBy = receiptedBy;

            // Выставляем статус накладной
            if (areReceiptedCountDivergences && areReceiptedSumDivergences)
            {
                State = ReceiptWaybillState.ReceiptedWithSumAndCountDivergences;
            }
            else
            {
                // Проверяем наличие расхождений
                if (!areReceiptedCountDivergences && !areReceiptedSumDivergences) // TODO: желательно завести специальный статус для согласования без расхождений, т.к. сейчас другой статус исп.не по назначению!
                {
                    State = ReceiptWaybillState.ReceiptedWithSumAndCountDivergences;    // Для окончательного согласования необходимо выставить какой-нибудь статус приемки
                }
                if (areReceiptedCountDivergences)
                {
                    State = ReceiptWaybillState.ReceiptedWithCountDivergences;
                }
                if (areReceiptedSumDivergences)
                {
                    State = ReceiptWaybillState.ReceiptedWithSumDivergences;
                }
            }

            // Если расхождений в накладной никаких нет - автоматически согласовываем накладную
            if (!areReceiptedCountDivergences && !areReceiptedSumDivergences)
            {
                // Пересчитываем закупочную цену с учетом процента скидки через сервис
                var receiptWaybillRowInfoList = Rows
                    .ToDictionary(x => x.Id, x => new ReceiptWaybillRowInfo(x.Id, x.PendingSum, x.PendingCount, x.InitialPurchaseCost));

                DistributeDiscountSum(receiptWaybillRowInfoList, PendingDiscountSum);

                foreach (var row in Rows)
                {
                    row.PurchaseCost = receiptWaybillRowInfoList[row.Id].PurchaseCost;
                    row.ApprovedValueAddedTax = row.PendingValueAddedTax;
                }

                Approve(ApprovedSumByRows, receiptedBy, receiptDate);
                State = ReceiptWaybillState.ApprovedWithoutDivergences; // TODO: Странно, почему состояние выставляется здесь? Надо перенести в Approve и дописать логику
            }
        }

        /// <summary>
        /// Отмена приемки накладной на склад
        /// </summary>
        public virtual void CancelReceipt()
        {
            CheckPossibilityToCancelReceipt();

            PerformReceiptCanceling();
        }

        private void PerformReceiptCanceling()
        {
            ReceiptDate = null;
            ReceiptedBy = null;

            State = ReceiptWaybillState.AcceptedDeliveryPending;

            // удаляем строки, добавленные при приемке
            var rowsToDelete = new List<ReceiptWaybillRow>(Rows.Where(x => x.PendingCount == 0));
            foreach (var row in rowsToDelete)
            {
                DeleteRow(row);
            }

            foreach (var row in Rows)
            {
                if (row.AreDivergencesAfterReceipt)
                {
                    // переводим кол-во по позиции в доступное для резервирования
                    row.AvailableToReserveCount = row.PendingCount;
                }
                
                row.ReceiptedCount = null;
                row.ProviderCount = null;
                row.ProviderSum = null;
                row.ApprovedCount = null;
                row.ApprovedSum = null;
                row.ApprovedPurchaseCost = null;
                row.ApprovedValueAddedTax = null;
                row.AreCountDivergencesAfterReceipt = false;
                row.AreSumDivergencesAfterReceipt = false;
            }
        }

        #endregion

        #region Окончательное согласование и его отмена
        
        /// <summary>
        /// Окончательное согласование накладной. Принимает (при необходимости переустановить учетные цены для строчек расхождений) коллекцию учетных цен.
        /// </summary>
        /// <param name="totalApproveSum">Согласованная сумма накладной</param>
        /// <param name="articleAccountingPriceList">Коллекция учетных цен. Необязательный параметр. При передаче - установи данные цены на строчки с расхождениями.</param>
        /// <param name="approvementDate">Дата согласования. Необязательный параметр. При передаче - установит дату согласования накладной.</param>
        public virtual void Approve(decimal totalApproveSum, User approvedBy, DateTime approvementDate, IEnumerable<ArticleAccountingPrice> accountingPrices = null)
        {
            CheckPossibilityToApprove();
            ValidationUtils.Assert(totalApproveSum >= 0, "Согласованная сумма накладной не может быть меньше 0.");
            ValidationUtils.CheckDecimalScale(totalApproveSum, 2, "Согласованная сумма накладной должна иметь не более 2 знаков после запятой.");
            ValidationUtils.Assert(totalApproveSum == ApprovedSumByRows, "Сумма накладной не сходится с суммой по позициям.");

            ApprovedSum = totalApproveSum;
            ApprovementDate = approvementDate;
            ApprovedBy = approvedBy;

            foreach (var row in Rows.Where(x => x.AreDivergencesAfterReceipt))
            {
                // копируем закупочные цены, указанные при согласовании, в текущие
                row.PurchaseCost = row.ApprovedPurchaseCost.Value;
                
                if (row.PendingCount == 0)
                {   // Доустановим учетные цены для товаров, добавленных при согласовании
                    ValidationUtils.NotNull(accountingPrices, "Невозможно выполнить согласование, так как не удалось получить учетную цену.");
                    var articleAccountingPrice = accountingPrices.FirstOrDefault(x => x.Article == row.Article);
                    ValidationUtils.NotNull(articleAccountingPrice, String.Format("Невозможно выполнить согласование, так как на товар «{0}» не установлена учетная цена.",
                        row.Article.FullName));
                    row.RecipientArticleAccountingPrice = articleAccountingPrice;
                }

                // переводим кол-во по позиции в доступное для резервирования
                row.AvailableToReserveCount = row.ApprovedCount.Value;
            }

            State = ReceiptWaybillState.ApprovedFinallyAfterDivergences; // TODO почему так? Надо как-то передавать сюда обстоятельства и выставлять правильный State
        }

        /// <summary>
        /// Отмена согласования
        /// </summary>
        public virtual void CancelApprovement()
        {
            CheckPossibilityToCancelApprovement();

            ApprovedSum = null;

            // Выставляем статус накладной
            if (AreCountDivergencesAfterReceipt && AreSumDivergencesAfterReceipt)
            {
                State = ReceiptWaybillState.ReceiptedWithSumAndCountDivergences;
            }
            else
            {
                if (AreCountDivergencesAfterReceipt)
                {
                    State = ReceiptWaybillState.ReceiptedWithCountDivergences;
                }
                if (AreSumDivergencesAfterReceipt)
                {
                    State = ReceiptWaybillState.ReceiptedWithSumDivergences;
                }
                if (!AreCountDivergencesAfterReceipt && !AreSumDivergencesAfterReceipt)
                {
                    State = ReceiptWaybillState.ApprovedWithoutDivergences; // TODO: смысл? В остальных случаях статус "принято", а тут "согласовано" и к тому же оно должно было стоять и так
                    PerformReceiptCanceling();
                }
            }

            // Очищаем строки накладной от данных согласования
            foreach (var row in Rows)
            {
                if (row.AreDivergencesAfterReceipt)
                {
                    row.ApprovedSum = null;
                    row.ApprovedCount = null;
                    row.ApprovedPurchaseCost = null;

                    // обнуляем доступное для резервирования кол-во товара
                    row.AvailableToReserveCount = 0;
                }
                
                // если позиция была добавлена при приемке
                if (row.PendingCount == 0)
                {   
                    // затираем УЦ
                    row.RecipientArticleAccountingPrice = null;
                }

                row.ApprovedValueAddedTax = null;
                row.PurchaseCost = row.InitialPurchaseCost;
            }

            ApprovementDate = null;
            ApprovedBy = null;
        }

        #endregion

        #region Точное разнесение суммы скидки по позициям

        /// <summary>
        /// Точное разнесение суммы скидки
        /// </summary>
        /// <param name="pendingSumList">Список ожидаемых сумм (для расчета скидки)</param>
        /// <param name="countList">Список количеств товаров</param>
        /// <param name="purchaseCostList">Список начальных значений закупочных цен (без учета скидки)</param>
        /// <param name="discountSum">Требуемая сумма скидки</param>
        private void DistributeDiscountSum(IDictionary<Guid, ReceiptWaybillRowInfo> receiptWaybillRowInfoList, decimal discountSum)
        {
            TryDiscountSumDistribute(receiptWaybillRowInfoList, discountSum);
        }

        /// <summary>
        /// Попытаться распределить сумму скидки по позициям
        /// </summary>
        /// <param name="discountSum">Требуемая сумма скидки</param>
        private void TryDiscountSumDistribute(IDictionary<Guid, ReceiptWaybillRowInfo> receiptWaybillRowInfoList, decimal discountSum)
        {
            // Сумма ожидаемая по накладной
            decimal pendingSum = receiptWaybillRowInfoList.Sum(x => x.Value.PendingSum);

            // Коэффициент, на который надо умножать закупочную цену без скидки, чтобы получить закупочную цену со скидкой
            double factor = pendingSum != 0M ? 1.0 - (double)(discountSum) / (double)pendingSum : 0.0;

            var keyList = receiptWaybillRowInfoList.Select(x => x.Key).ToList();

            foreach (var key in keyList)
            {
                ReceiptWaybillRowInfo item = receiptWaybillRowInfoList[key];
                item.PurchaseCost = Math.Round((decimal)((double)item.PurchaseCost * factor), 6);
            }
        }

        /// <summary>
        /// Расчет суммы скидки (имеет смысл при окончательном согласовании без расхождений)
        /// </summary>
        private decimal CalculateDiscountSum()
        {
            // Сумма по позиции округляется до 6-го знака, суммируется, сумма по накладной округляется до 2-го знака
            return Math.Round(Rows.Sum(x => x.CalculateDiscountSum()), 2);
        }

        #endregion

        #region Проверки на возможность выполнения операций

        public virtual void CheckPossibilityToEdit()
        {
            ValidationUtils.Assert(!IsCreatedFromProductionOrderBatch, "Невозможно изменить накладную, созданную по партии заказа.");
            ValidationUtils.Assert(!IsApproved, "Невозможно изменить окончательно согласованную накладную.");
            ValidationUtils.Assert(!IsReceipted, "Невозможно изменить принятую на склад накладную.");
            ValidationUtils.Assert(!IsAccepted, "Невозможно изменить проведенную накладную.");
        }

        public virtual void CheckPossibilityToEditProviderDocuments()
        {
            ValidationUtils.Assert(!IsCreatedFromProductionOrderBatch, "Невозможно редактировать документы поставщика в накладной, созданной по партии заказа.");
        }

        /// <summary>
        /// Возможность удаления накладной (непосредственная, путем нажатия кнопки "Удалить" в деталях прихода)
        /// </summary>
        public virtual void CheckPossibilityToDelete()
        {
            if (State != ReceiptWaybillState.New)
            {
                throw new Exception(String.Format("Невозможно удалить накладную со статусом «{0}».", State.GetDisplayName()));
            }

            if (AreOutgoingWaybills)
            {
                throw new Exception("Невозможно удалить накладную, так как товар из нее используется в других документах.");
            }
        }

        /// <summary>
        /// Возможность удаления накладной, связанной с партией заказа (автоматического, с предварительной отменой проведения, по ссылке "Удалить" в деталях партии заказа
        /// или при возврате партии заказа на предыдущий этап)
        /// </summary>
        public virtual void CheckPossibilityToDeleteFromProductionOrderBatch()
        {
            if (!State.ContainsIn(ReceiptWaybillState.New, ReceiptWaybillState.AcceptedDeliveryPending))
            {
                throw new Exception(String.Format("Невозможно удалить накладную со статусом «{0}».", State.GetDisplayName()));
            }

            if (AreOutgoingWaybills)
            {
                throw new Exception("Невозможно удалить накладную, так как товар из нее используется в других документах.");
            }
        }

        public virtual void CheckPossibilityToEditRowFromReceipt()
        {
            ValidationUtils.Assert(!IsReceipted, "Накладная уже принята.");
            ValidationUtils.Assert(IsAccepted, "Накладная еще не проведена.");
        }

        public virtual void CheckPossibilityToReceipt()
        {
            ValidationUtils.Assert(!IsReceipted, "Накладная уже принята.");
            ValidationUtils.Assert(IsAccepted, "Накладная еще не проведена.");
            ValidationUtils.Assert(!AreSumDivergences, "Сумма накладной не совпадает с суммой по позициям.");
        }

        public virtual void CheckPossibilityToCancelReceipt()
        {
            ValidationUtils.Assert(!IsApproved, "Невозможно отменить приемку окончательно согласованной накладной.");
            ValidationUtils.Assert(IsReceipted, "Невозможно отменить приемку непринятой накладной.");
            
            // Отменить приемку можно, только если ни по одной из позиций накладной не было сформировано других документов
            // со статусом "Отгружено" или "Принято". На момент отмены они могут быть сформированы только для позиций без расхождений.
            foreach (var row in Rows)
            {
                if (!row.AreDivergencesAfterReceipt && row.ShippedCount + row.FinallyMovedCount > 0)
                {
                    throw new Exception("Невозможно отменить приемку накладной на склад, так как товар из нее используется в других документах.");
                }
            }
        }

        public virtual void CheckPossibilityToApprove()
        {
            ValidationUtils.Assert(!IsApproved, "Накладная уже согласована.");
            ValidationUtils.Assert(IsReceipted, "Накладная не может быть окончательно согласована, так как она еще не принята на склад.");            
        }

        public virtual void CheckPossibilityToCancelApprovement()
        {
            ValidationUtils.Assert(IsApproved, "Невозможно отменить согласование несогласованной накладной.");            

            // Отменить окончательное согласование можно, только если ни по одной из позиций накладной не было сформировано других документов
            // со статусом "Отгружено" или "Принято".
            ValidationUtils.Assert(!Rows.Any(x => x.ShippedCount + x.FinallyMovedCount > 0), 
                "Невозможно отменить согласование накладной, так как товар из нее используется в других документах.");            
        }

        public virtual void CheckPossibilityToAccept()
        {
            ValidationUtils.Assert(!IsAccepted, "Накладная уже проведена.");
            ValidationUtils.Assert(Rows.Any(), "Невозможно провести пустую накладную.");
            ValidationUtils.Assert(!AreSumDivergences, "Сумма накладной не совпадает с суммой по позициям.");
        }

        /// <summary>
        /// Есть ли возможность отменить проведение прихода
        /// </summary>
        /// <param name="checkIfCreatedFromProductionOrderBatch">true - операция запрещается, если приход создан по партии заказа</param>
        public virtual void CheckPossibilityToCancelAcceptance(bool checkIfCreatedFromProductionOrderBatch)
        {
            ValidationUtils.Assert(!IsReceipted, "Невозможно отменить проводку принятой накладной.");
            ValidationUtils.Assert(IsAccepted, "Невозможно отменить проводку непроведенной накладной.");
            ValidationUtils.Assert(!Rows.Any(x => x.AreOutgoingWaybills), "Невозможно отменить проводку накладной, т.к. ее позиции участвуют в дальнейшем товародвижении.");

            if (checkIfCreatedFromProductionOrderBatch)
            {
                ValidationUtils.Assert(!IsCreatedFromProductionOrderBatch,
                    "Невозможно отменить проводку накладной, созданной по партии заказа, возможна только автоматическая отмена при удалении прихода из деталей партии заказа.");
            }
        }

        public override void CheckPossibilityToChangeCurator()
        {
            ValidationUtils.Assert(State == ReceiptWaybillState.New,
                String.Format("Невозможно изменить куратора для накладной со статусом «{0}».", State.GetDisplayName()));
        }

        /// <summary>
        /// Проверка возможности изменить дату накладной
        /// </summary>
        public virtual void CheckPossibilityToChangeDate()
        {
            ValidationUtils.Assert(IsNew, String.Format("Невозможно изменить дату накладной со статусом «{0}».", State.GetDisplayName()));
        }

        #endregion

        #endregion
    }
}
