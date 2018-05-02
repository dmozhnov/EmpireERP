using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Заказ на производство
    /// </summary>
    public class ProductionOrder : Entity<Guid>
    {
        #region Свойства

        #region Основные свойства

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Дата начала
        /// </summary>
        public virtual DateTime Date { get; protected set; }

        /// <summary>
        /// Создатель
        /// </summary>
        public virtual User CreatedBy { get; protected set; }

        /// <summary>
        /// Куратор
        /// </summary>
        public virtual User Curator { get; protected set; }

        /// <summary>
        /// Название
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string Name { get; set; }

        /// <summary>
        /// Место хранения
        /// </summary>
        public virtual Storage Storage { get; set; }

        /// <summary>
        /// Валюта
        /// </summary>
        public virtual Currency Currency
        {
            get { return currency; }
            set
            {
                if (currency != value)
                {
                    CurrencyRate = null;
                }

                currency = value;
            }
        }
        private Currency currency;

        /// <summary>
        /// Курс валюты
        /// </summary>
        public virtual CurrencyRate CurrencyRate
        {
            get { return currencyRate; }
            set
            {
                if (value != null && currency != value.Currency)
                {
                    throw new Exception("Валюта курса не совпадает с валютой заказа. Возможно, валюта заказа была изменена.");
                }

                currencyRate = value;
            }
        }
        private CurrencyRate currencyRate;

        /// <summary>
        /// Производитель
        /// </summary>
        public virtual Producer Producer { get; protected set; }

        /// <summary>
        /// Пакеты материалов
        /// </summary>
        public virtual IEnumerable<ProductionOrderMaterialsPackage> MaterialsPackages
        {
            get
            {
                return new ImmutableSet<ProductionOrderMaterialsPackage>(materialsPackages);
            }
        }
        private Iesi.Collections.Generic.ISet<ProductionOrderMaterialsPackage> materialsPackages;

        /// <summary>
        /// Партии
        /// </summary>
        public virtual IEnumerable<ProductionOrderBatch> Batches
        {
            get { return new ImmutableSet<ProductionOrderBatch>(batches); }
        }
        private Iesi.Collections.Generic.ISet<ProductionOrderBatch> batches;

        /// <summary>
        /// Количество партий
        /// </summary>
        public virtual int ProductionOrderBatchCount { get { return batches.Count; } }

        /// <summary>
        /// Начало диапазона
        /// </summary>
        public virtual ProductionOrderBatchStage MinOrderBatchStage
        {
            get
            {
                var minOrdinalNumber = batches.Min(x => x.CurrentStage.OrdinalNumber);

                return batches.First(x => x.CurrentStage.OrdinalNumber == minOrdinalNumber).CurrentStage;
            }
        }

        /// <summary>
        /// Конец диапазона
        /// </summary>
        public virtual ProductionOrderBatchStage MaxOrderBatchStage
        {
            get
            {
                var maxOrdinalNumber = batches.Max(x => x.CurrentStage.OrdinalNumber);

                return batches.First(x => x.CurrentStage.OrdinalNumber == maxOrdinalNumber).CurrentStage;
            }
        }

        /// <summary>
        /// Контракт
        /// </summary>
        public virtual ProducerContract Contract { get; protected set; }

        /// <summary>
        /// Оплаты по заказу (со всеми назначениями)
        /// </summary>
        public virtual IEnumerable<ProductionOrderPayment> Payments
        {
            get { return new ImmutableSet<ProductionOrderPayment>(payments); }
        }
        private Iesi.Collections.Generic.ISet<ProductionOrderPayment> payments;

        /// <summary>
        /// Планируемые оплаты по заказу (план совершения платежей)
        /// </summary>
        public virtual IEnumerable<ProductionOrderPlannedPayment> PlannedPayments
        {
            get { return new ImmutableSet<ProductionOrderPlannedPayment>(plannedPayments); }
        }
        private Iesi.Collections.Generic.ISet<ProductionOrderPlannedPayment> plannedPayments;

        /// <summary>
        /// Закрыт ли заказ
        /// </summary>
        public virtual bool IsClosed { get; set; }

        /// <summary>
        /// Способ расчета закупочных цен (а именно транспортных расходов) в приходах, созданных по данному заказу
        /// </summary>
        public virtual ProductionOrderArticleTransportingPrimeCostCalculationType ArticleTransportingPrimeCostCalculationType
        {
            get { return articleTransportingPrimeCostCalculationType; }
            set
            {
                ValidationUtils.Assert(Enum.IsDefined(typeof(ProductionOrderArticleTransportingPrimeCostCalculationType), value),
                    "Недопустимый способ подсчета себестоимости транспортировки.");

                articleTransportingPrimeCostCalculationType = value;
            }
        }
        private ProductionOrderArticleTransportingPrimeCostCalculationType articleTransportingPrimeCostCalculationType;

        /// <summary>
        /// Задан ли способ расчета закупочных цен (а именно транспортных расходов) в приходах, созданных по данному заказу
        /// </summary>
        public virtual bool IsArticleTransportingPrimeCostCalculationTypeSet
        {
            get
            {
                return ArticleTransportingPrimeCostCalculationType.ContainsIn(ProductionOrderArticleTransportingPrimeCostCalculationType.Volume,
                    ProductionOrderArticleTransportingPrimeCostCalculationType.Weight);
            }
        }

        /// <summary>
        /// График рабочих дней для расчета продолжительности этапов заказа (упакованный в целое число)
        /// </summary>
        public virtual byte WorkDaysPlanScheme { get; protected set; }

        /// <summary>
        /// Является ли понедельник рабочим днем
        /// </summary>
        public virtual bool MondayIsWorkDay
        {
            get
            {
                return (WorkDaysPlanScheme & (1 << 6)) != 0;
            }
            set
            {
                CheckPossibilityToEditWorkDaysPlan();

                WorkDaysPlanScheme = (byte)(WorkDaysPlanScheme & ~(1 << 6) | (value ? 1 << 6 : 0));
            }
        }

        /// <summary>
        /// Является ли вторник рабочим днем
        /// </summary>
        public virtual bool TuesdayIsWorkDay
        {
            get
            {
                return (WorkDaysPlanScheme & (1 << 5)) != 0;
            }
            set
            {
                CheckPossibilityToEditWorkDaysPlan();

                WorkDaysPlanScheme = (byte)(WorkDaysPlanScheme & ~(1 << 5) | (value ? 1 << 5 : 0));
            }
        }

        /// <summary>
        /// Является ли среда рабочим днем
        /// </summary>
        public virtual bool WednesdayIsWorkDay
        {
            get
            {
                return (WorkDaysPlanScheme & (1 << 4)) != 0;
            }
            set
            {
                CheckPossibilityToEditWorkDaysPlan();

                WorkDaysPlanScheme = (byte)(WorkDaysPlanScheme & ~(1 << 4) | (value ? 1 << 4 : 0));
            }
        }

        /// <summary>
        /// Является ли четверг рабочим днем
        /// </summary>
        public virtual bool ThursdayIsWorkDay
        {
            get
            {
                return (WorkDaysPlanScheme & (1 << 3)) != 0;
            }
            set
            {
                CheckPossibilityToEditWorkDaysPlan();

                WorkDaysPlanScheme = (byte)(WorkDaysPlanScheme & ~(1 << 3) | (value ? 1 << 3 : 0));
            }
        }

        /// <summary>
        /// Является ли пятница рабочим днем
        /// </summary>
        public virtual bool FridayIsWorkDay
        {
            get
            {
                return (WorkDaysPlanScheme & (1 << 2)) != 0;
            }
            set
            {
                CheckPossibilityToEditWorkDaysPlan();

                WorkDaysPlanScheme = (byte)(WorkDaysPlanScheme & ~(1 << 2) | (value ? 1 << 2 : 0));
            }
        }

        /// <summary>
        /// Является ли суббота рабочим днем
        /// </summary>
        public virtual bool SaturdayIsWorkDay
        {
            get
            {
                return (WorkDaysPlanScheme & (1 << 1)) != 0;
            }
            set
            {
                CheckPossibilityToEditWorkDaysPlan();

                WorkDaysPlanScheme = (byte)(WorkDaysPlanScheme & ~(1 << 1) | (value ? 1 << 1 : 0));
            }
        }

        /// <summary>
        /// Является ли воскресенье рабочим днем
        /// </summary>
        public virtual bool SundayIsWorkDay
        {
            get
            {
                return (WorkDaysPlanScheme & (1 << 0)) != 0;
            }
            set
            {
                CheckPossibilityToEditWorkDaysPlan();

                WorkDaysPlanScheme = (byte)(WorkDaysPlanScheme & ~(1 << 0) | (value ? 1 << 0 : 0));
            }
        }

        /// <summary>
        /// Список рабочих дней в виде строки
        /// </summary>
        public virtual string WorkDaysPlanString
        {
            get
            {
                return
                    (MondayIsWorkDay ? "ПН " : "") +
                    (TuesdayIsWorkDay ? "ВТ " : "") +
                    (WednesdayIsWorkDay ? "СР " : "") +
                    (ThursdayIsWorkDay ? "ЧТ " : "") +
                    (FridayIsWorkDay ? "ПТ " : "") +
                    (SaturdayIsWorkDay ? "СБ " : "") +
                    (SundayIsWorkDay ? "ВС " : "")
                    .TrimEnd();
            }
        }

        /// <summary>
        /// В заказе одна партия?
        /// </summary>
        public virtual bool IsIncludingOneBatch 
        {
            get
            {
                return Batches.Count() == 1;
            }
        }


        /// <summary>
        /// Комментарий
        /// </summary>
        /// <remarks>не более 4000 символов</remarks>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Транспортные листы по заказу
        /// </summary>
        public virtual IEnumerable<ProductionOrderTransportSheet> TransportSheets
        {
            get { return new ImmutableSet<ProductionOrderTransportSheet>(transportSheets); }
        }
        private Iesi.Collections.Generic.ISet<ProductionOrderTransportSheet> transportSheets;

        /// <summary>
        /// Листы дополнительных расходов по заказу
        /// </summary>
        public virtual IEnumerable<ProductionOrderExtraExpensesSheet> ExtraExpensesSheets
        {
            get { return new ImmutableSet<ProductionOrderExtraExpensesSheet>(extraExpensesSheets); }
        }
        private Iesi.Collections.Generic.ISet<ProductionOrderExtraExpensesSheet> extraExpensesSheets;

        /// <summary>
        /// Таможенные листы по заказу
        /// </summary>
        public virtual IEnumerable<ProductionOrderCustomsDeclaration> CustomsDeclarations
        {
            get { return new ImmutableSet<ProductionOrderCustomsDeclaration>(customsDeclarations); }
        }
        private Iesi.Collections.Generic.ISet<ProductionOrderCustomsDeclaration> customsDeclarations;

        #region Плановые затраты

        /// <summary>
        /// Плановые затраты на производство
        /// </summary>
        public virtual decimal ProductionOrderPlannedProductionExpensesInCurrency { get; set; }

        /// <summary>
        /// Плановые затраты на транспортировку
        /// </summary>
        public virtual decimal ProductionOrderPlannedTransportationExpensesInCurrency { get; set; }

        /// <summary>
        /// Плановые дополнительные затраты
        /// </summary>
        public virtual decimal ProductionOrderPlannedExtraExpensesInCurrency { get; set; }

        /// <summary>
        /// Плановые таможенные затраты
        /// </summary>
        public virtual decimal ProductionOrderPlannedCustomsExpensesInCurrency { get; set; }

        /// <summary>
        /// Можно ли считать заданным план затрат
        /// </summary>
        public virtual bool IsProductionOrderExpensesPlanSet
        {
            get
            {
                return ProductionOrderPlannedProductionExpensesInCurrency != 0M || ProductionOrderPlannedTransportationExpensesInCurrency != 0M ||
                    ProductionOrderPlannedExtraExpensesInCurrency != 0M || ProductionOrderPlannedCustomsExpensesInCurrency != 0M;
            }
        }

        #endregion

        #endregion

        #region Показатели

        /// <summary>
        /// Плановая стоимость заказа
        /// </summary>
        public virtual decimal ProductionOrderPlannedExpensesSumInCurrency
        {
            get
            {
                return ProductionOrderPlannedProductionExpensesInCurrency + ProductionOrderPlannedTransportationExpensesInCurrency + 
                    ProductionOrderPlannedExtraExpensesInCurrency + ProductionOrderPlannedCustomsExpensesInCurrency;
            }
        }

        /// <summary>
        /// Фактическая стоимость производства заказа в валюте
        /// </summary>
        public virtual decimal ProductionOrderProductionCostInCurrency { get { return batches.Sum(x => x.ProductionOrderBatchProductionCostInCurrency); } }

        /// <summary>
        /// Сумма оплат по заказу за производство в валюте
        /// </summary>
        public virtual decimal ProductionOrderProductionPaymentSumInCurrency
        {
            get
            {
                return payments.Where(x => x.Type == ProductionOrderPaymentType.ProductionOrderProductionPayment).Sum(x => x.SumInCurrency);
            }
        }

        /// <summary>
        /// Неоплаченный остаток по производству в валюте
        /// </summary>
        public virtual decimal ProductionOrderProductionDebtRemainderInCurrency { get { return ProductionOrderProductionCostInCurrency - ProductionOrderProductionPaymentSumInCurrency; } }

        /// <summary>
        /// Отклонение от плана
        /// </summary>
        public virtual int DivergenceFromPlan { get { return batches.Max(x => x.DivergenceFromPlan); } }

        /// <summary>
        /// Дата завершения, она же ожидаемая дата поставки
        /// </summary>
        public virtual DateTime EndDate
        {
            get
            {
                return batches.Max(x => x.EndDate);
            }
        }

        /// <summary>
        /// Полностью ли оплачен заказ
        /// </summary>
        public virtual bool IsFullyPaid
        {
            // Если все неоплаченные остатки равны 0, то заказ оплачен. Отрицательным может быть только неоплаченный остаток за производство
            get
            {
                return ProductionOrderProductionDebtRemainderInCurrency <= 0M && transportSheets.Sum(x => x.DebtRemainderInCurrency) <= 0M &&
                    extraExpensesSheets.Sum(x => x.DebtRemainderInCurrency) <= 0M && customsDeclarations.Sum(x => x.DebtRemainder) <= 0M;
            }
        }

        #endregion

        #region Возможность совершения операций
        
        public virtual void CheckPossibilityToAddBatch()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно добавить партию в  закрытый заказ.");
        }

        public virtual void CheckPossibilityToDeleteBatch(ProductionOrderBatch batch)
        {
            ValidationUtils.Assert(!IsIncludingOneBatch, "Невозможно удалить последнюю партию в заказе.");
            ValidationUtils.IsNull(batch.ReceiptWaybill, "Невозможно удалить партию, по которой создана приходная накладная.");
        }

        public virtual void CheckPossibilityToEdit()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно редактировать закрытый заказ.");
        }

        public virtual void CheckPossibilityToClose()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно закрыть уже закрытый заказ.");
            ValidationUtils.Assert(!Batches.Any(x => !x.IsClosed), "Невозможно закрыть заказ, если не закрыты партии.");
            
            //Нельзя успешно закрыть последнюю партию заказа, если заказ полностью не оплачен. 
            //Кроме тех случаев, если есть неуспешно закрытые партии
            if (!Batches.Any(x => x.IsClosed && !x.IsClosedSuccessfully))
                ValidationUtils.Assert(IsFullyPaid, "Невозможно успешно закрыть заказ, если заказ полностью не оплачен.");

            ValidationUtils.Assert(IsArticleTransportingPrimeCostCalculationTypeSet ||
                    !(Batches.Any(x => x.IsClosedSuccessfully)),
                    "Невозможно закрыть заказ с успешно закрытыми партиями, так как в заказе не указан способ учета транспортировки в себестоимости товаров.");
        }

        public virtual void CheckPossibilityToEditPlannedExpenses()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно редактировать закрытый заказ.");
        }

        public virtual void CheckPossibilityToCreatePlannedPayment()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно создавать планируемые оплаты в закрытом заказе.");
        }

        public virtual void CheckPossibilityToChangeCurrency()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно редактировать закрытый заказ.");
            ValidationUtils.Assert(!payments.Any(x => x.Type == ProductionOrderPaymentType.ProductionOrderProductionPayment),
                "Невозможно изменить валюту заказа при наличии оплат за производство");
            ValidationUtils.Assert(!payments.Any(x => x.Type == ProductionOrderPaymentType.ProductionOrderTransportSheetPayment &&
                x.As<ProductionOrderTransportSheetPayment>().TransportSheet.CurrencyDeterminationType == ProductionOrderCurrencyDeterminationType.ProductionOrderCurrency),
                "Невозможно изменить валюту заказа при наличии оплат по транспортным листам в валюте заказа");
            ValidationUtils.Assert(!payments.Any(x => x.Type == ProductionOrderPaymentType.ProductionOrderExtraExpensesSheetPayment &&
                x.As<ProductionOrderExtraExpensesSheetPayment>().ExtraExpensesSheet.CurrencyDeterminationType == ProductionOrderCurrencyDeterminationType.ProductionOrderCurrency),
                "Невозможно изменить валюту заказа при наличии оплат по листам дополнительных расходов в валюте заказа");
        }

        public virtual void CheckPossibilityToChangeCurrencyRate()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно редактировать закрытый заказ.");
        }

        /// <summary>
        /// Можно ли изменить способ расчета закупочных цен (а именно транспортных расходов) в приходах
        /// </summary>
        public virtual void CheckPossibilityToChangeArticleTransportingPrimeCostCalculationType()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно изменить способ расчета закупочных цен для закрытого заказа.");
        }

        public virtual void CheckPossibilityToEditContract()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно редактировать закрытый заказ.");
        }

        public virtual void CheckPossibilityToEditWorkDaysPlan()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно редактировать закрытый заказ.");
            ValidationUtils.Assert(IsIncludingOneBatch, "Невозможно редактировать график рабочих дней заказа при наличии нескольких партий.");
            ValidationUtils.Assert(batches.First().CurrentStage.OrdinalNumber == 1,
                String.Format("Невозможно редактировать график рабочих дней заказа на этапах, отличных от «{0}».", batches.First().FirstStage.Name));
        }

        #region Транспортные листы

        public virtual void CheckPossibilityToCreateTransportSheet()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно создавать транспортные листы в закрытом заказе.");
        }

        public virtual void CheckPossibilityToEditTransportSheet()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно редактировать транспортные листы по закрытому заказу.");
        }

        public virtual void CheckPossibilityToDeleteTransportSheet()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно удалять транспортные листы по закрытому заказу.");
        }

        #endregion

        #region Листы дополнительных расходов

        public virtual void CheckPossibilityToCreateExtraExpensesSheet()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно создавать листы дополнительных расходов в закрытом заказе.");
        }

        public virtual void CheckPossibilityToEditExtraExpensesSheet()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно редактировать листы дополнительных расходов по закрытому заказу."); 
        }

        public virtual void CheckPossibilityToDeleteExtraExpensesSheet()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно удалять листы дополнительных расходов по закрытому заказу.");
        }

        #endregion  

        #region Таможенные листы

        public virtual void CheckPossibilityToCreateCustomsDeclaration()
        {
            // Если заказ закрыт (т.е. все его партии закрыты), ничего создавать нельзя
            ValidationUtils.Assert(!IsClosed, "Невозможно создать таможенный лист по закрытому заказу.");
        }

        #endregion

        #region Оплаты

        public virtual void CheckPossibilityToCreatePayment()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно создавать оплаты в закрытом заказе.");            
        }

        public virtual void CheckPossibilityToEditPayment()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно редактировать оплаты в закрытом заказе.");
        }

        public virtual void CheckPossibilityToDeletePayment()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно удалять оплаты в закрытом заказе.");
        }
        
        #endregion

        #region Пакеты материалов

        public virtual void CheckPossibilityToCreateMaterialsPackage()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно создавать пакеты материалов в закрытом заказе.");
        }

        public virtual void CheckPossibilityToEditMaterialsPackage()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно редактировать пакеты материалов в закрытом заказе.");
        }

        public virtual void CheckPossibilityToDeleteMaterialsPackage()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно удалить пакеты материалов в закрытом заказе.");
        }

        #endregion

        #endregion

        #endregion

        #region Конструкторы

        protected ProductionOrder()
        {
        }

        public ProductionOrder(string name, Producer producer, Currency currency, ProductionOrderBatchStage calculationStage, ProductionOrderBatchStage successfullClosingStage,
            ProductionOrderBatchStage unsuccessfulClosingStage, ProductionOrderArticleTransportingPrimeCostCalculationType articleTransportingPrimeCostCalculationType,
            bool mondayIsWorkDay, bool tuesdayIsWorkDay, bool wednesdayIsWorkDay, bool thursdayIsWorkDay, bool fridayIsWorkDay, bool saturdayIsWorkDay, bool sundayIsWorkDay,
            User createdBy, DateTime currentDateTime)
        {
            ValidationUtils.Assert(!String.IsNullOrEmpty(name), "Название заказа не указано.");
            Name = name;

            ValidationUtils.NotNull(currency, "Валюта не указана.");
            this.currency = currency;
            this.currencyRate = null;

            ValidationUtils.NotNull(producer, "Производитель не указан.");
            Producer = producer;

            CreationDate = currentDateTime;
            Date = currentDateTime.Date;
            CreatedBy = createdBy;
            Curator = createdBy;
            ArticleTransportingPrimeCostCalculationType = articleTransportingPrimeCostCalculationType;
            Comment = "";

            batches = new HashedSet<ProductionOrderBatch>();
            payments = new HashedSet<ProductionOrderPayment>();
            plannedPayments = new HashedSet<ProductionOrderPlannedPayment>();
            transportSheets = new HashedSet<ProductionOrderTransportSheet>();
            extraExpensesSheets = new HashedSet<ProductionOrderExtraExpensesSheet>();
            customsDeclarations = new HashedSet<ProductionOrderCustomsDeclaration>();
            materialsPackages = new HashedSet<ProductionOrderMaterialsPackage>();

            var batch = new ProductionOrderBatch(calculationStage, successfullClosingStage, unsuccessfulClosingStage, createdBy, currentDateTime);
            AddBatch(batch);

            IsClosed = false;

            MondayIsWorkDay = mondayIsWorkDay;
            TuesdayIsWorkDay = tuesdayIsWorkDay;
            WednesdayIsWorkDay = wednesdayIsWorkDay;
            ThursdayIsWorkDay = thursdayIsWorkDay;
            FridayIsWorkDay = fridayIsWorkDay;
            SaturdayIsWorkDay = saturdayIsWorkDay;
            SundayIsWorkDay = sundayIsWorkDay;
            CheckWorkDaysPlan();
        }

        #endregion

        #region Методы

        #region Проверка корректности данных

        public virtual void CheckWorkDaysPlan()
        {
            DateTimeUtils.CheckWorkDaysPlan(MondayIsWorkDay, TuesdayIsWorkDay, WednesdayIsWorkDay, ThursdayIsWorkDay, FridayIsWorkDay, SaturdayIsWorkDay, SundayIsWorkDay);
        }

        #endregion

        #region Работа с вложенными сущностями

        /// <summary>
        /// Добавление партии
        /// </summary>
        /// <param name="batch">Партия</param>
        public virtual void AddBatch( ProductionOrderBatch batch)
        {                
            batches.Add(batch);
            batch.ProductionOrder = this;
        }

        /// <summary>
        /// Удаление партии
        /// </summary>
        /// <param name="batch">Партия</param>
        /// <param name="currentDateTime">Текущее время</param>
        public virtual void DeleteBatch(ProductionOrderBatch batch, DateTime currentDateTime)
        {
            batch.DeletionDate = currentDateTime;
            batches.Remove(batch);
        }

        /// <summary>
        /// Добавление оплаты
        /// </summary>
        /// <param name="payment">Оплата</param>
        protected internal virtual void AddPayment(ProductionOrderPayment payment)
        {
            if (payments.Contains(payment))
            {
                throw new Exception("Данная оплата уже связана с этим заказом.");
            }

            payments.Add(payment);
        }

        /// <summary>
        /// Удаление оплаты
        /// </summary>
        /// <param name="payment">Оплата</param>
        /// <param name="currentDateTime">Текущее время</param>
        public virtual void DeletePayment(ProductionOrderPayment payment, DateTime currentDateTime)
        {
            if (payment.ProductionOrderPlannedPayment != null)
            {
                payment.ProductionOrderPlannedPayment.DeletePayment(payment);
            }

            payment.DeletionDate = currentDateTime;
            payments.Remove(payment);
        }

        /// <summary>
        /// Добавление планируемой оплаты
        /// </summary>
        /// <param name="payment">Оплата</param>
        protected internal virtual void AddPlannedPayment(ProductionOrderPlannedPayment payment)
        {
            if (plannedPayments.Contains(payment))
            {
                throw new Exception("Данная оплата уже связана с этим заказом.");
            }

            plannedPayments.Add(payment);
        }

        /// <summary>
        /// Удаление планируемой оплаты
        /// </summary>
        /// <param name="payment">Оплата</param>
        /// <param name="currentDateTime">Текущее время</param>
        public virtual void DeletePlannedPayment(ProductionOrderPlannedPayment payment, DateTime currentDateTime)
        {
            payment.CheckPossibilityToDelete();

            payment.DeletionDate = currentDateTime;
            plannedPayments.Remove(payment);
        }

        /// <summary>
        /// Добавление контракта
        /// </summary>
        /// <param name="contract">Контракт</param>
        public virtual void AddContract(ProducerContract contract)
        {
            ValidationUtils.Assert(Contract == null, "По данному заказу уже создан контракт.");
            ValidationUtils.Assert(contract.AccountOrganization.Storages.Contains(Storage),
                String.Format("Указанная собственная организация («{0}») не связана с местом хранения заказа («{1}»).", contract.AccountOrganization.ShortName, Storage.Name));
            Contract = contract;
        }

        /// <summary>
        /// Добавление транспортного листа
        /// </summary>
        /// <param name="transportSheet">Транспортный лист</param>
        public virtual void AddTransportSheet(ProductionOrderTransportSheet transportSheet)
        {
            transportSheets.Add(transportSheet);
            transportSheet.ProductionOrder = this;
        }

        /// <summary>
        /// Удаление транспортного листа
        /// </summary>
        /// <param name="transportSheet">Транспортный лист</param>
        /// <param name="currentDateTime">Текущее время</param>
        public virtual void DeleteTransportSheet(ProductionOrderTransportSheet transportSheet, DateTime currentDateTime)
        {
            transportSheet.DeletionDate = currentDateTime;
            transportSheets.Remove(transportSheet);
        }

        /// <summary>
        /// Добавление листа дополнительных расходов
        /// </summary>
        /// <param name="extraExpensesSheet">Лист дополнительных расходов</param>
        public virtual void AddExtraExpensesSheet(ProductionOrderExtraExpensesSheet extraExpensesSheet)
        {
            extraExpensesSheets.Add(extraExpensesSheet);
            extraExpensesSheet.ProductionOrder = this;
        }

        /// <summary>
        /// Удаление листа дополнительных расходов
        /// </summary>
        /// <param name="extraExpensesSheet">Лист дополнительных расходов</param>
        /// <param name="currentDateTime">Текущее время</param>
        public virtual void DeleteExtraExpensesSheet(ProductionOrderExtraExpensesSheet extraExpensesSheet, DateTime currentDateTime)
        {
            extraExpensesSheet.DeletionDate = currentDateTime;
            extraExpensesSheets.Remove(extraExpensesSheet);
        }

        /// <summary>
        /// Добавление таможенного листа
        /// </summary>
        /// <param name="customsDeclaration">Таможенный лист</param>
        public virtual void AddCustomsDeclaration(ProductionOrderCustomsDeclaration customsDeclaration)
        {
            customsDeclarations.Add(customsDeclaration);
            customsDeclaration.ProductionOrder = this;
        }

        /// <summary>
        /// Удаление таможенного листа
        /// </summary>
        /// <param name="customsDeclaration">Таможенный лист</param>
        /// <param name="currentDateTime">Текущее время</param>
        public virtual void DeleteCustomsDeclaration(ProductionOrderCustomsDeclaration customsDeclaration, DateTime currentDateTime)
        {
            customsDeclaration.DeletionDate = currentDateTime;
            customsDeclarations.Remove(customsDeclaration);
        }

        #endregion

        #endregion
    }
}
