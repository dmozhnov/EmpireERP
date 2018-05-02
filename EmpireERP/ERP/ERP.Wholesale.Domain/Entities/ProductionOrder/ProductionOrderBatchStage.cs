using System;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Этап партии заказа на производство
    /// </summary>
    public class ProductionOrderBatchStage : Entity<Guid>
    {
        #region Свойства

        /// <summary>
        /// Партия заказа, к которой относится данный этап
        /// </summary>
        public virtual ProductionOrderBatch Batch { get; protected internal set; }

        /// <summary>
        /// Название
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string Name
        {
            get { return name; }
            set
            {
                if (IsDefault)
                {
                    throw new Exception("Невозможно отредактировать системный этап.");
                }

                name = value;
            }
        }
        private string name;

        /// <summary>
        /// Планируемая длительность этапа, дни
        /// </summary>
        public virtual short? PlannedDuration
        {
            get { return plannedDuration; }
            set
            {
                if (IsDefault)
                {
                    throw new Exception("Невозможно отредактировать системный этап.");
                }

                plannedDuration = value;
            }
        }
        private short? plannedDuration;

        /// <summary>
        /// Рассчитывается ли длительность этапа в рабочих днях (вместо календарных)
        /// </summary>
        public virtual bool InWorkDays { get; set; }

        /// <summary>
        /// Планируемая дата начала этапа
        /// </summary>
        public virtual DateTime PlannedStartDate
        {
            get
            {
                var date = Batch.CreationDate;

                if (OrdinalNumber > 1)
                {
                    var stages = Batch.Stages.Where(x => x.OrdinalNumber < OrdinalNumber && x.PlannedDuration.HasValue).OrderBy(x => x.OrdinalNumber);

                    foreach (var stage in stages)
                    {
                        date = stage.AddDays(date, stage.PlannedDuration.Value);
                    }
                }

                return date;
            }
        }

        public virtual DateTime ExpectedStartDate
        {
            get
            {
                DateTime currentStageSupposedEndDate;
                ProductionOrderBatchStage stage;
                // Вычисляем предполагаемую дату закрытия текущего этапа, и берем ее точкой отсчета
                if (Batch.CurrentStage.ExpectedEndDate.HasValue)
                {
                    currentStageSupposedEndDate = Batch.CurrentStage.ExpectedEndDate.Value;
                    // Если текущий этап просрочен, считаем, что он завершится сегодня
                    if (DateTime.Now.Date > currentStageSupposedEndDate)
                        currentStageSupposedEndDate = DateTime.Now.Date;

                    stage = Batch.CurrentStage;
                }
                //если мы на этапе неуспешного закрытия, то точкой отсчета берем последний закрытый этап
                else if (Batch.CurrentStage.Type == ProductionOrderBatchStageType.Closed)
                {
                    stage = Batch.Stages.OrderByDescending(x => x.OrdinalNumber).FirstOrDefault(x => x.ActualEndDate != null);
                    currentStageSupposedEndDate = stage.ActualEndDate.Value;
                }
                else
                {
                    throw new Exception("Ошибка при расчете ожидаемой даты начала этапа."); 
                }

                var stages = Batch.Stages.Where(x => x.OrdinalNumber > stage.OrdinalNumber &&
                                                     x.OrdinalNumber < OrdinalNumber &&
                                                     x.PlannedDuration.HasValue)
                                         .OrderBy(x => x.OrdinalNumber);

                foreach (var batchStage in stages)
                {
                    currentStageSupposedEndDate = batchStage.AddDays(currentStageSupposedEndDate, batchStage.PlannedDuration.Value);
                }

                return currentStageSupposedEndDate;
            }
        }

        /// <summary>
        /// Ожидаемая / ожидавшаяся дата конца этапа (дата начала этапа/ожидаемая дата начала этапа + планируемая длительность этапа), 
        /// только дата (без времени).
        /// </summary>
        public virtual DateTime? ExpectedEndDate
        {
            get
            {
                DateTime startDate;

                if (ActualStartDate.HasValue)
                    startDate = ActualStartDate.Value;
                else 
                    startDate = ExpectedStartDate;

                return PlannedDuration.HasValue  ? AddDays(startDate.Date, PlannedDuration.Value) : (DateTime?)null;
            }
        }

        /// <summary>
        /// Планируемая дата конца этапа (планируемая дата начала этапа + планируемая длительность этапа)
        /// </summary>
        public virtual DateTime? PlannedEndDate
        {
            get
            {
                return PlannedDuration.HasValue ? AddDays(PlannedStartDate, PlannedDuration.Value) : (DateTime?)null;
            }
        }

        /// <summary>
        /// Реальная дата начала этапа
        /// </summary>
        public virtual DateTime? ActualStartDate { get; set; }

        /// <summary>
        /// Реальная дата завершения этапа
        /// </summary>
        public virtual DateTime? ActualEndDate { get; set; }

        /// <summary>
        /// Порядковый номер этапа в пределах партии
        /// </summary>
        public virtual short OrdinalNumber { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public virtual ProductionOrderBatchStageType Type
        {
            get { return type; }
            set
            {
                if (IsDefault)
                {
                    throw new Exception("Невозможно отредактировать системный этап.");
                }

                if (!Enum.IsDefined(typeof(ProductionOrderBatchStageType), value))
                {
                    throw new Exception("Недопустимый тип этапа.");
                }
                if (value == ProductionOrderBatchStageType.Closed)
                {
                    throw new Exception(String.Format("Невозможно присвоить этапу тип «{0}».", value.GetDisplayName()));
                }

                type = value;
            }
        }
        private ProductionOrderBatchStageType type;

        /// <summary>
        /// Признак того, что этап является системным (неуничтожимым и не редактируемым)
        /// </summary>
        public virtual bool IsDefault { get; protected internal set; }

        #endregion

        #region Конструкторы

        protected ProductionOrderBatchStage()
        {
        }

        public ProductionOrderBatchStage(string name, ProductionOrderBatchStageType type, short plannedDuration, bool inWorkDays)
        {
            ValidationUtils.Assert(!String.IsNullOrEmpty(name), "Название этапа не указано.");
            this.name = name;

            this.type = type;
            this.plannedDuration = plannedDuration;
            InWorkDays = inWorkDays;
            IsDefault = false;
        }

        public ProductionOrderBatchStage(ProductionOrderBatchLifeCycleTemplateStage productionOrderBatchLifeCycleTemplateStage)
        {
            this.name = productionOrderBatchLifeCycleTemplateStage.Name;
            this.type = productionOrderBatchLifeCycleTemplateStage.Type;
            this.plannedDuration = productionOrderBatchLifeCycleTemplateStage.PlannedDuration;
            InWorkDays = productionOrderBatchLifeCycleTemplateStage.InWorkDays;
            IsDefault = productionOrderBatchLifeCycleTemplateStage.IsDefault;
        }

        public ProductionOrderBatchStage(DefaultProductionOrderBatchStage defaultProductionOrderBatchStage, short? plannedDuration = null)
        {  
            this.name = defaultProductionOrderBatchStage.Name;
            this.type = defaultProductionOrderBatchStage.Type;
            this.plannedDuration = plannedDuration;
            IsDefault = true;
        }

        protected internal ProductionOrderBatchStage(ProductionOrderBatchStage productionOrderBatchStage)
        {
            this.name = productionOrderBatchStage.Name;
            this.type = productionOrderBatchStage.Type;
            this.plannedDuration = productionOrderBatchStage.PlannedDuration;
            this.ActualStartDate = productionOrderBatchStage.ActualStartDate;
            this.ActualEndDate = productionOrderBatchStage.ActualEndDate;
            InWorkDays = productionOrderBatchStage.InWorkDays;
            IsDefault = productionOrderBatchStage.IsDefault;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавить требуемое количество дней к дате (рабочих или календарных - зависит от этапа)
        /// </summary>
        /// <param name="startDate">Дата</param>
        /// <param name="daysCount">Количество дней</param>
        /// <returns></returns>
        public virtual DateTime AddDays(DateTime startDate, int daysCount)
        {
            if (InWorkDays)
            {
                var productionOrder = Batch.ProductionOrder;

                return DateTimeUtils.AddWorkDays(startDate, daysCount,
                    productionOrder.MondayIsWorkDay,
                    productionOrder.TuesdayIsWorkDay,
                    productionOrder.WednesdayIsWorkDay,
                    productionOrder.ThursdayIsWorkDay,
                    productionOrder.FridayIsWorkDay,
                    productionOrder.SaturdayIsWorkDay,
                    productionOrder.SundayIsWorkDay);
            }
            else
            {
                return startDate.AddDays(daysCount);
            }
        }

        /// <summary>
        /// Получить предполагаемую дату конца этапа (с учетом всего), только дата (без времени)
        /// </summary>
        /// <param name="currentDate">Текущая дата</param>
        public virtual DateTime CalculateSupposedEndDate(DateTime currentDate)
        {
            // Если этап уже завершен, возвращаем реальную дату конца
            if (ActualEndDate.HasValue)
            {
                return ActualEndDate.Value.Date;
            }

            ValidationUtils.Assert(!Batch.IsClosed || Batch.IsClosedSuccessfully,
                "Невозможно рассчитать предполагаемую дату конца незавершенного этапа для неуспешно закрытой партии заказа.");

            // Значит, текущий этап - это данный (или текущий идет раньше данного)
            // Вычисляем предполагаемую дату конца текущего этапа, а потом прибавляем к ней планируемые длительности этапов после него до данного этапа
            short currentStageOrdinalNumber = Batch.CurrentStage.OrdinalNumber;
            DateTime currentStageSupposedEndDate = Batch.CurrentStage.ExpectedEndDate.Value;
            // Если текущий этап просрочен, считаем, что он завершится сегодня
            if (currentDate.Date > currentStageSupposedEndDate)
            {
                currentStageSupposedEndDate = currentDate.Date;
            }

            foreach (var stage in Batch.Stages.Where(x => x.OrdinalNumber > currentStageOrdinalNumber 
                && x.OrdinalNumber <= OrdinalNumber && x.PlannedDuration.HasValue).OrderBy(x => x.OrdinalNumber))
            {
                currentStageSupposedEndDate = stage.AddDays(currentStageSupposedEndDate, stage.PlannedDuration.Value);
            }

            return currentStageSupposedEndDate;
        }

        #endregion
    }
}
