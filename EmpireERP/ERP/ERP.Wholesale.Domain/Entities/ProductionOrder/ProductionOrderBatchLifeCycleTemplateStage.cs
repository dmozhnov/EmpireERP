using System;
using ERP.Infrastructure.Entities;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Этап шаблона жизненного цикла заказа
    /// </summary>
    public class ProductionOrderBatchLifeCycleTemplateStage : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Шаблон, к которому относится данный этап
        /// </summary>
        public virtual ProductionOrderBatchLifeCycleTemplate Template { get; protected internal set; }

        /// <summary>
        /// Название
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string Name
        {
            get { return name; }
            set
            {
                ValidationUtils.Assert(!IsDefault, "Невозможно отредактировать системный этап.");
                ValidationUtils.Assert(!String.IsNullOrEmpty(value), "Название этапа не указано.");

                name = value;
            }
        }
        private string name;

        /// <summary>
        /// Тип (перегружено)
        /// </summary>
        public virtual ProductionOrderBatchStageType Type
        {
            get { return type; }
            set
            {
                ValidationUtils.Assert(!IsDefault, "Невозможно отредактировать системный этап.");
                ValidationUtils.Assert(Enum.IsDefined(typeof(ProductionOrderBatchStageType), value), "Недопустимый тип этапа.");
                ValidationUtils.Assert(value != ProductionOrderBatchStageType.Closed, String.Format("Невозможно присвоить этапу тип «{0}».", value.GetDisplayName()));

                type = value;
            }
        }
        private ProductionOrderBatchStageType type;

        /// <summary>
        /// Порядковый номер в списке
        /// </summary>
        public virtual short OrdinalNumber { get; set; }

        /// <summary>
        /// Планируемая длительность этапа, дни
        /// </summary>
        public virtual short? PlannedDuration
        {
            get { return plannedDuration; }
            set
            {
                ValidationUtils.Assert(!IsDefault, "Невозможно отредактировать системный этап.");

                plannedDuration = value;
            }
        }
        private short? plannedDuration;

        /// <summary>
        /// Рассчитывается ли длительность этапа в рабочих днях (вместо календарных)
        /// </summary>
        public virtual bool InWorkDays { get; set; }

        /// <summary>
        /// Признак того, что этап создан из этапа по умолчанию (неуничтожимый и нередактируемый)
        /// </summary>
        public virtual bool IsDefault { get; protected set; }

        #endregion

        #region Конструкторы

        protected ProductionOrderBatchLifeCycleTemplateStage()
        {
        }

        /// <summary>
        /// Создание пользовательского этапа
        /// </summary>
        /// <param name="name">Название</param>
        /// <param name="type">Тип</param>
        public ProductionOrderBatchLifeCycleTemplateStage(string name, ProductionOrderBatchStageType type, short plannedDuration, bool inWorkDays)
        {
            IsDefault = false;
            this.plannedDuration = plannedDuration;

            ValidationUtils.Assert(!String.IsNullOrEmpty(name), "Название этапа не указано.");
            this.name = name;
            this.type = type;
            InWorkDays = inWorkDays;
        }

        public ProductionOrderBatchLifeCycleTemplateStage(DefaultProductionOrderBatchStage defaultProductionOrderBatchStage)
        {
            IsDefault = true;

            ValidationUtils.Assert(!String.IsNullOrEmpty(defaultProductionOrderBatchStage.Name), "Название этапа не указано.");
            this.name = defaultProductionOrderBatchStage.Name;
            this.type = defaultProductionOrderBatchStage.Type;
        }

        #endregion

        #region Методы

        #region Проверка на возможность совершения операций

        public virtual void CheckPossibilityToCreateStageAfter()
        {
            ValidationUtils.Assert(OrdinalNumber == 1 || !IsDefault, "Невозможно добавить этап.");
        }

        public virtual void CheckPossibilityToEdit()
        {
            ValidationUtils.Assert(!IsDefault, "Невозможно отредактировать системный этап.");
        }

        public virtual void CheckPossibilityToDelete()
        {
            ValidationUtils.Assert(!IsDefault, "Невозможно удалить системный этап.");
        }

        public virtual void CheckPossibilityToMoveUp()
        {
            ValidationUtils.Assert(!IsDefault, "Невозможно переместить системный этап.");
            ValidationUtils.Assert(OrdinalNumber > 2, "Невозможно переместить этап.");
        }

        public virtual void CheckPossibilityToMoveDown()
        {
            ValidationUtils.Assert(!IsDefault, "Невозможно переместить системный этап.");
            ValidationUtils.Assert(OrdinalNumber < (Template.StageCount - 2), "Невозможно переместить этап.");
        }





        #endregion

        #endregion
    }
}
