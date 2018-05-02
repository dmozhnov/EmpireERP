using System;
using System.Linq;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Позиция приходной накладной
    /// </summary>
    public class ReceiptWaybillRow : BaseIncomingWaybillRow
    {
        #region Свойства

        /// <summary>
        /// Приходная накладная, частью которой является позиция
        /// </summary>
        public virtual ReceiptWaybill ReceiptWaybill { get; protected internal set; }

        /// <summary>
        /// Реестр цен, по которому сформирована учетная цена для этой позиции
        /// </summary>
        public virtual ArticleAccountingPrice RecipientArticleAccountingPrice
        {
            get { return recipientArticleAccountingPrice; }

            protected internal set
            {
                ValidationUtils.Assert((!ReceiptWaybill.IsApproved && AreDivergencesAfterReceipt) || (!ReceiptWaybill.IsAccepted && !AreDivergencesAfterReceipt), 
                    "Реестр, по которому рассчитывается учетная цена, не может быть сменен.");                

                recipientArticleAccountingPrice = value;
            }
        }
        private ArticleAccountingPrice recipientArticleAccountingPrice;

        /// <summary>
        /// Товар
        /// </summary>
        public override Article Article
        {
            get { return article; }
            protected set
            {
                ValidationUtils.NotNull(value, "Не указан товар.");
                CheckPossibilityToEdit();

                if (article != value)
                {
                    ValidationUtils.Assert(!AreOutgoingWaybills, "Невозможно изменить товар, т.к. по данной позиции есть зарезервированные товары.");
                    
                    // Проверка на дублирование товара в накладной, когда позиция редактируется, а не создается.
                    // При создании проверка на уникальность товара делается в сущности накладной.
                    ValidationUtils.Assert(IsBeingConstructed || ReceiptWaybill.Rows.Where(x => x.Id != Id && x.Article == value).Count() == 0,
                        "Данный товар уже содержится в накладной.");
                    
                    ArticleMeasureUnitScale = value.MeasureUnit.Scale;
                }

                article = value;
            }
        }

        /// <summary>
        /// Кол-во знаков после запятой (из единицы измерения товара)
        /// </summary>
        public virtual byte ArticleMeasureUnitScale { get; set; }

        /// <summary>
        /// Количество ожидаемое
        /// </summary>
        public virtual decimal PendingCount
        {
            get { return pendingCount; }
            set
            {
                ValidationUtils.Assert(value >= TotallyReservedCount,
                    String.Format("Количество товара не может быть меньше {0}, т.к. товар участвует в исходящих накладных.", TotallyReservedCount));

                ValidationUtils.CheckDecimalScale(value, 12, ArticleMeasureUnitScale,
                    "Ожидаемое количество имеет слишком большое число цифр.",
                    "Ожидаемое количество имеет слишком большое число цифр после запятой.");
                
                CheckPossibilityToEditCount();
                pendingCount = value;
            }
        }
        private decimal pendingCount;

        /// <summary>
        /// Сумма ожидаемая
        /// </summary>
        public virtual decimal PendingSum
        {
            get { return pendingSum; }
            set
            {
                ValidationUtils.CheckDecimalScale(value, 16, 2,
                    "Ожидаемая сумма имеет слишком большое количество цифр.",
                    "Ожидаемая сумма имеет слишком большое количество цифр после запятой.");
                CheckPossibilityToEditSum();
                pendingSum = value;
            }
        }
        private decimal pendingSum;

        /// <summary>
        /// Количество принятое
        /// </summary>
        public virtual decimal? ReceiptedCount
        {
            get { return receiptedCount; }
            set
            {
                ValidationUtils.Assert(value == null || value >= 0M, "Принятое количество не может быть меньше 0.");
                ValidationUtils.CheckDecimalScale(value, 12, ArticleMeasureUnitScale,
                    "Принятое количество имеет слишком большое количество цифр.",
                    "Принятое количество имеет слишком большое количество цифр после запятой.");
                CheckPossibilityToEditCount();
                receiptedCount = value;
            }
        }
        private decimal? receiptedCount;

        /// <summary>
        /// Количество по документу поставщика
        /// </summary>
        public virtual decimal? ProviderCount
        {
            get { return providerCount; }
            set
            {
                ValidationUtils.Assert(value == null || value >= 0M, "Количество по документу поставщика не может быть меньше 0.");
                ValidationUtils.CheckDecimalScale(value, 12, ArticleMeasureUnitScale,
                    "Количество по документу поставщика имеет слишком большое количество цифр.",
                    "Количество по документу поставщика имеет слишком большое количество цифр после запятой.");
                CheckPossibilityToEditCount();
                providerCount = value;
            }
        }
        private decimal? providerCount;

        /// <summary>
        /// Сумма по документу
        /// </summary>
        public virtual decimal? ProviderSum
        {
            get { return providerSum; }
            set
            {
                ValidationUtils.Assert(value == null || value >= 0M, "Сумма по документу не может быть меньше 0.");
                ValidationUtils.CheckDecimalScale(value, 16, 2,
                    "Сумма по документу имеет слишком большое количество цифр.",
                    "Сумма по документу имеет слишком большое количество цифр после запятой.");
                CheckPossibilityToEditCount();
                providerSum = value;
            }
        }
        private decimal? providerSum;

        /// <summary>
        /// Количество согласованное
        /// </summary>
        public virtual decimal? ApprovedCount
        {
            get { return approvedCount; }
            set
            {
                ValidationUtils.CheckDecimalScale(value, 12, ArticleMeasureUnitScale,
                    "Согласованное количество имеет слишком большое количество цифр.",
                    "Согласованное количество имеет слишком большое количество цифр после запятой.");

                ValidationUtils.Assert(value == null || value >= TotallyReservedCount,
                    String.Format("По данной позиции уже зарезервировано {0} единиц товара.", TotallyReservedCount.ForDisplay()));

                ValidationUtils.Assert(!ReceiptWaybill.IsApproved, "Невозможно изменить согласованное количество у окончательно согласованной накладной.");
                ValidationUtils.Assert(value == approvedCount || value == null || approvedCount == null || AreDivergencesAfterReceipt,
                    "Невозможно изменить согласованное количество по позиции, у которой не было расхождений при приемке.");
                approvedCount = value;
            }
        }
        private decimal? approvedCount;

        /// <summary>
        /// Сумма согласованная
        /// </summary>
        public virtual decimal? ApprovedSum
        {
            get { return approvedSum; }
            set
            {
                ValidationUtils.CheckDecimalScale(value, 16, 2,
                    "Согласованная сумма имеет слишком большое количество цифр.",
                    "Согласованная сумма имеет слишком большое количество цифр после запятой.");
                CheckPossibilityToEditSum();
                approvedSum = value;
            }
        }
        private decimal? approvedSum;

        /// <summary>
        /// Текущая сумма.
        /// Исторически хранила 6 знаков, так и сейчас
        /// </summary>
        public virtual decimal CurrentSum
        {
            get
            {
                return Math.Round(ReceiptWaybill.IsApproved ? PurchaseCost * ApprovedCount.Value : PurchaseCost * PendingCount, 6);
            }
        }

        /// <summary>
        /// Текущее количество
        /// </summary>
        public virtual decimal CurrentCount
        {
            get
            {
                if (ReceiptWaybill.IsPending)
                {
                    return PendingCount;
                }
                else if (ReceiptWaybill.IsReceiptedWithDivergences)
                {
                    return (AreDivergencesAfterReceipt ? PendingCount : ApprovedCount.Value); // TODO: зачем? Ведь если нет расхождений, то ApprovedCount и так равен PendingCount
                }
                else
                {
                    return ApprovedCount.Value;
                }
            }
        }

        /// <summary>
        /// Ожидаемая закупочная цена (без учета скидки).
        /// Нужна для того, чтобы при отмене согласования накладной (со скидкой) получить закупочную цену, указанную при создании позиции,
        /// так как вычислить эту цену простым делением PendingSum / PendingCount не всегда точно возможно из-за округления
        /// </summary>
        public virtual decimal InitialPurchaseCost
        {
            get { return initialPurchaseCost; }
            set
            {
                CheckPossibilityToEditSum();
                initialPurchaseCost = Math.Round(value, 6);
            }
        }
        private decimal initialPurchaseCost;

        /// <summary>
        /// Закупочная цена. При создании / редактировании позиции (новая накладная) равна InitialPurchaseCost. При согласовании с ненулевой скидкой
        /// (такое возможно только для накладной без расхождений) меняется с учетом скидки. Лишь эта закупочная цена используется для расчетов сумм
        /// в закупочных ценах везде в проекте и в индикаторах
        /// </summary>
        public virtual decimal PurchaseCost
        {
            get { return purchaseCost; }
            set
            {
                CheckPossibilityToEditSum();
                purchaseCost = Math.Round(value, 6);
            }
        }
        private decimal purchaseCost;

        /// <summary>
        /// Закупочная цена, предлагаемая при попытке согласования. Хранит введенное при этой попытке значение. Пока накладная не согласована,
        /// в общем случае не равна PurchaseCost. После согласования копируется в PurchaseCost. Используется только при согласовании
        /// </summary>
        public virtual decimal? ApprovedPurchaseCost
        {
            get { return approvedPurchaseCost; }
            set
            {
                CheckPossibilityToEditSum();
                ValidationUtils.Assert(value == approvedPurchaseCost || value == null || approvedPurchaseCost == null || AreDivergencesAfterReceipt || ReceiptWaybill.IsCreatedFromProductionOrderBatch,
                    "Невозможно изменить согласованную закупочную цену по позиции, у которой не было расхождений при приемке.");
                approvedPurchaseCost = value.HasValue ? Math.Round(value.Value, 6) : value;
            }
        }
        private decimal? approvedPurchaseCost;

        /// <summary>
        /// Ставка НДС ожидаемая
        /// </summary>
        public virtual ValueAddedTax PendingValueAddedTax { get; set; }

        /// <summary>
        /// Текущая сумма НДС
        /// </summary>
        public virtual decimal ValueAddedTaxSum { get { return VatUtils.CalculateVatSum(Math.Round(CurrentSum, 2), CurrentValueAddedTax.Value); } }

        /// <summary>
        /// Сумма НДС получателя. Имеет смысл только в случае проведенной накладной (зафиксированы учетные цены)
        /// </summary>
        public virtual decimal RecipientValueAddedTaxSum
        {
            get
            {
                ValidationUtils.Assert(ReceiptWaybill.IsAccepted, "Зафиксированной учетной цены не существует, так как накладная еще не проведена.");

                return VatUtils.CalculateVatSum(recipientArticleAccountingPrice.AccountingPrice * CurrentCount, CurrentValueAddedTax.Value);
            }
        }

        /// <summary>
        /// Ставка НДС согласованная
        /// </summary>
        public virtual ValueAddedTax ApprovedValueAddedTax { get; set; }

        /// <summary>
        /// Текущая ставка НДС
        /// </summary>
        public virtual ValueAddedTax CurrentValueAddedTax { get { return ReceiptWaybill.IsApproved ? ApprovedValueAddedTax : PendingValueAddedTax; } }

        /// <summary>
        /// Номер ГТД (грузовая таможенная декларация)
        /// </summary>
        public virtual string CustomsDeclarationNumber
        {
            get { return customsDeclarationNumber; }
            set
            {
                CheckPossibilityToEdit();
                customsDeclarationNumber = value;
            }
        }
        private string customsDeclarationNumber;

        /// <summary>
        /// Страна производства
        /// </summary>
        public virtual Country ProductionCountry { get; set; }

        /// <summary>
        /// Производитель
        /// </summary>
        public virtual Manufacturer Manufacturer { get; set; }

        /// <summary>
        /// Название партии
        /// </summary>
        public virtual string BatchName { get { return ReceiptWaybill.Name; } }

        /// <summary>
        /// Одновременная установка количеств исходящего товара (проведенного, отгруженного и окончательно перемещенного).
        /// Если они некорректно заданы, происходит исключение.
        /// </summary>
        public virtual void SetOutgoingArticleCount(decimal acceptedCount, decimal shippedCount, decimal finallyMovedCount)
        {
            ValidationUtils.Assert(acceptedCount >= 0M && shippedCount >= 0M && finallyMovedCount >= 0M,
                "Количество проведенного, отгруженного или окончательно перемещенного товара не может быть меньше 0.");
            ValidationUtils.Assert(shippedCount + finallyMovedCount + acceptedCount <= CurrentCount,
                "Сумма проведенного, отгруженного и окончательно перемещенного товара не может быть больше текущего количества товара (ожидаемого или принятого).");
            
            ValidationUtils.CheckDecimalScale(acceptedCount, 12, ArticleMeasureUnitScale,
                "Количество проведенного товара должно иметь соответствующее число знаков до запятой.",
                "Количество проведенного товара должно иметь соответствующее число знаков после запятой.");
            ValidationUtils.CheckDecimalScale(shippedCount, 12, ArticleMeasureUnitScale,
                "Количество отгруженного товара должно иметь соответствующее число знаков до запятой.",
                "Количество отгруженного товара должно иметь соответствующее число знаков после запятой.");
            ValidationUtils.CheckDecimalScale(finallyMovedCount, 12, ArticleMeasureUnitScale,
                "Количество окончательно перемещенного товара должно иметь соответствующее число знаков до запятой.",
                "Количество окончательно перемещенного товара должно иметь соответствующее число знаков после запятой.");

            this.acceptedCount = acceptedCount;
            this.shippedCount = shippedCount;
            this.finallyMovedCount = finallyMovedCount;

            this.AvailableToReserveCount = CurrentCount - acceptedCount - shippedCount - finallyMovedCount;
        }

        /// <summary>
        /// Есть ли любые расхождения при приемке (по количеству или по сумме). Зависит от ReceiptedCount, ProviderCount и ProviderSum,
        /// которые должны быть установлены в нужные значения перед вызовом
        /// </summary>
        public virtual bool AreDivergencesAfterReceipt
        {
            get
            {
                return AreCountDivergencesAfterReceipt || AreSumDivergencesAfterReceipt;
            }
        }

        /// <summary>
        /// Есть ли расхождения по количеству. Зависит от ReceiptedCount и ProviderCount, которые должны быть установлены в нужные значения перед вызовом
        /// </summary>
        public virtual bool AreCountDivergencesAfterReceipt
        {
            get { return areCountDivergencesAfterReceipt; }
            protected internal set { areCountDivergencesAfterReceipt = value; }
        }
        private bool areCountDivergencesAfterReceipt;

        /// <summary>
        /// Есть ли расхождения по сумме. Зависит от ProviderSum, которая должна быть установлена в нужное значение перед вызовом
        /// </summary>
        public virtual bool AreSumDivergencesAfterReceipt
        {
            get { return areSumDivergencesAfterReceipt; }
            protected internal set { areSumDivergencesAfterReceipt = value; }
        }
        private bool areSumDivergencesAfterReceipt;
        
        private bool IsBeingConstructed { get { return ReceiptWaybill == null; } }

        /// <summary>
        /// Порядковый номер для сортировки
        /// </summary>
        public virtual int OrdinalNumber { get; protected set; }

        /// <summary>
        /// Количество товара по позиции
        /// </summary>
        protected override decimal ArticleCount
        {
            get 
            {
                // Здесь не используется CurrentCount, т.к. добавленные при приемке товары учитываются только после согласования.
                // Из-за этого после приемки и до согласования кол-во таких товаров считается равным 0 и объем с весом так же = 0. 
                // Но в ПФ кол-во берется принятое, т.е. != 0. Итог - несогласованность данных.

                if (ReceiptWaybill.IsPending)
                {
                    return PendingCount;
                }
                else if (ReceiptWaybill.IsReceiptedWithDivergences)
                {
                    return receiptedCount.Value;    //т.к. после приемки это значение не может быть null 
                }
                else
                {
                    return ApprovedCount.Value;
                }
            }
        }

        /// <summary>
        /// Дата перевода позиции накладной в финальный статус (окончательного прихода товара по позиции на склад)
        /// </summary>
        public virtual DateTime? FinalizationDate
        {
            get
            {
                // если накладная не принята - возвращаем null
                if (!this.ReceiptWaybill.IsReceipted) return null;

                // если были расхождения после приемки
                if (this.AreDivergencesAfterReceipt)
                {
                    // если накладная была согласована после расхождений - возвращаем дату согласования
                    if (this.ReceiptWaybill.IsApproved)
                    {
                        return this.ReceiptWaybill.ApprovementDate;
                    }

                    // если накладная не была согласована после расхождений - возвращаем null 
                    return null;
                }
                // если расхождений при приемке не было - возвращаем дата приемки
                else
                {
                    return this.ReceiptWaybill.ReceiptDate;
                }
            }
        }

        /// <summary>
        /// Признак того, что позиции добавлена при приемке
        /// </summary>
        public virtual bool IsAddedOnReceipt
        {
            get
            {
                return PendingCount == 0;
            }
        }

        #endregion

        #region Конструкторы

        protected ReceiptWaybillRow()
            : base(WaybillType.ReceiptWaybill)
        {
        }

        public ReceiptWaybillRow(Article article, decimal pendingCount, decimal pendingSum, ValueAddedTax valueAddedTax, int ordinalNumber = int.MaxValue)
            : this(article, pendingCount, valueAddedTax, ordinalNumber)
        {
            ValidationUtils.Assert(pendingSum >= 0M, "Невозможно создать партию с отрицательной ожидаемой суммой.");
            PendingSum = pendingSum;
            RecalculateInitialPurchaseCost();
            PurchaseCost = initialPurchaseCost;
            RecalculatePendingSum();
        }

        public ReceiptWaybillRow(Article article, decimal pendingCount, ValueAddedTax valueAddedTax, decimal purchaseCost)
            : this(article, pendingCount, valueAddedTax)
        {
            ValidationUtils.Assert(pendingCount > 0, "Количество товара должно быть больше 0.");
            ValidationUtils.Assert(purchaseCost >= 0M, "Невозможно создать партию с отрицательной закупочной стоимостью.");
            InitialPurchaseCost = purchaseCost;
            PurchaseCost = purchaseCost;
            RecalculatePendingSum();
        }

        private ReceiptWaybillRow(Article article, decimal pendingCount, ValueAddedTax valueAddedTax, int ordinalNumber = int.MaxValue)
            : this()
        {
            ValidationUtils.Assert(pendingCount >= 0M, "Невозможно создать партию с отрицательным ожидаемым количеством.");
            Article = article; // Должно идти перед присваиванием количества, т.к. задает допустимое число его знаков после запятой
            PendingValueAddedTax = valueAddedTax;
            PendingCount = pendingCount;
            OrdinalNumber = ordinalNumber;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Рассчитать ожидаемую закупочную цену по ожидаемой сумме
        /// </summary>
        public virtual void RecalculateInitialPurchaseCost()
        {
            InitialPurchaseCost = pendingCount != 0M ? Math.Round(pendingSum / pendingCount, 6) : 0M;
        }

        // TODO: избавиться от pendingSumIsChangedLast при сохранении позиции, от PendingSum и от ApprovedSum (вычислять их только по зак.цене и к-ву, геттером)

        /// <summary>
        /// Рассчитать ожидаемую сумму по ожидаемой закупочной стоимости. Вызывается повсюду после изменений в закупочной стоимости
        /// </summary>
        public virtual void RecalculatePendingSum()
        {
            PendingSum = Math.Round(initialPurchaseCost * pendingCount, 2);
        }

        /// <summary>
        /// Рассчитать согласованную сумму по согласованной закупочной стоимости. Вызывается повсюду после изменений в согласованных свойствах
        /// </summary>
        public virtual void RecalculateApprovedSum()
        {
            ApprovedSum = Math.Round(approvedPurchaseCost.Value * approvedCount.Value, 2);
        }

        /// <summary>
        /// Проверка корректности выставленных количеств (ожидаемое, принятое на склад и по документам поставщика не могут все три равняться 0).
        /// </summary>
        public virtual void CheckCounts()
        {
            ValidationUtils.Assert(pendingCount > 0 || receiptedCount > 0 || providerCount > 0, "Все три количества по позиции не могут быть одновременно равны 0.");
        }

        /// <summary>
        /// Рассчитать сумму скидки по позиции
        /// </summary>
        public virtual decimal CalculateDiscountSum()
        {
            ValidationUtils.Assert(ReceiptWaybill.State == ReceiptWaybillState.ApprovedWithoutDivergences,
                String.Format("Расчет суммы скидки может быть произведен только для состояния накладной «{0}».",
                ReceiptWaybillState.ApprovedWithoutDivergences.GetDisplayName()));

            return Math.Round(InitialPurchaseCost * PendingCount, 6) - Math.Round(PurchaseCost * ApprovedCount.Value, 6);
        }

        /// <summary>
        /// Смена товара по позиции накладной
        /// </summary>
        /// <param name="article"></param>
        public virtual void SetArticle(Article article)
        {
            Article = article;
        }

        #region Проверки на возможность совершения операций
        
        public virtual void CheckPossibilityToEdit()
        {
            // Если позиция только что создана и еще не добавлена в накладную, редактировать можно
            ValidationUtils.Assert(IsBeingConstructed || !ReceiptWaybill.IsApproved, "Невозможно изменить позицию окончательно согласованной накладной.");
            ValidationUtils.Assert(IsBeingConstructed || !ReceiptWaybill.IsReceipted, "Невозможно изменить позицию принятой накладной.");
        }

        // TODO: разбить на 2. PendingCount отдельно, при приемке отдельно. К тому же при приемке не все Count, а есть одна Sum (ProviderSum) - назв. неверное
        public virtual void CheckPossibilityToEditCount()
        {
            // Если позиция только что создана и еще не добавлена в накладную, редактировать можно
            ValidationUtils.Assert(IsBeingConstructed || ReceiptWaybill.IsCreatedFromProductionOrderBatch || !ReceiptWaybill.IsApproved,
                "Невозможно изменить количество в позиции окончательно согласованной накладной.");
            ValidationUtils.Assert(IsBeingConstructed || ReceiptWaybill.IsCreatedFromProductionOrderBatch || !ReceiptWaybill.IsReceipted || AreDivergencesAfterReceipt,
                "Невозможно изменить количество в позиции принятой накладной, если по позиции нет расхождений.");
        }

        // TODO: тоже разбить на 2, почему оно вызывается при изменениях PendingSum и (ApprovedSum + ApprovedPurchaseCost)? Это разные этапы и разные ограничения
        public virtual void CheckPossibilityToEditSum()
        {
            // Если позиция только что создана и еще не добавлена в накладную, редактировать можно
            ValidationUtils.Assert(IsBeingConstructed || ReceiptWaybill.IsCreatedFromProductionOrderBatch || !ReceiptWaybill.IsApproved,
                "Невозможно изменить сумму позиции окончательно согласованной накладной.");
        }

        public virtual void CheckPossibilityToDelete()
        {
            ValidationUtils.Assert(!(ReceiptWaybill.IsAccepted && PendingCount > 0), "Невозможно удалить позицию из проведенной накладной.");

            ValidationUtils.Assert(!AreOutgoingWaybills, "Невозможно удалить позицию, так как по ней уже создана позиция другой исходящей накладной.");
        }

        #endregion

        #endregion
    }
}
