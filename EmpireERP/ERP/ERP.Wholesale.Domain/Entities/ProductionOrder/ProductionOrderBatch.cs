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
    /// Партия заказа на производство
    /// </summary>
    public class ProductionOrderBatch : Entity<Guid>
    {
        #region Свойства

        #region Общие

        /// <summary>
        /// Название партии
        /// </summary>
        /// <remarks>Не более 200 символов</remarks>
        public virtual string Name { get; set; }

        /// <summary>
        /// Заказ, к которому относится данная партия
        /// </summary>
        public virtual ProductionOrder ProductionOrder { get; protected internal set; }

        /// <summary>
        /// Приход, с которым связана партия заказа
        /// </summary>
        public virtual ReceiptWaybill ReceiptWaybill { get; protected internal set; }

        /// <summary>
        /// Дата создания
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

                    // ставим позициям текущую дату удаления
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
        /// Дата
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
        /// Позиции партии заказа на производство
        /// </summary>
        public virtual IEnumerable<ProductionOrderBatchRow> Rows
        {
            get { return new ImmutableSet<ProductionOrderBatchRow>(rows); }
        }
        private Iesi.Collections.Generic.ISet<ProductionOrderBatchRow> rows;

        /// <summary>
        /// Количество позиций
        /// </summary>
        public virtual int RowCount { get { return rows.Count; } }

        /// <summary>
        /// Закрыта ли партия заказа
        /// </summary>
        public virtual bool IsClosed
        {
            get { return isClosed; }
            protected set
            {
                ValidationUtils.Assert(!ProductionOrder.IsClosed, "Нельзя открыть партию принадлежащую закрытому заказу.");
                isClosed = value;
            }
        }
        private bool isClosed;

        /// <summary>
        /// Закрыта ли партия успешно
        /// </summary>
        public virtual bool IsClosedSuccessfully { get { return IsClosed && currentStage != UnsuccessfulClosingStage; } }

        /// <summary>
        /// Последняя ли партия (есть ли кроме нее открытые)
        /// </summary>
        public virtual bool IsLastBatch { get { return !ProductionOrder.Batches.Any(x => x.IsClosed == false && x != this); } }

        #endregion

        #region Статусы

        /// <summary>
        /// Статус
        /// </summary>
        public virtual ProductionOrderBatchState State
        {
            get { return state; }
            protected set
            {
                if (!Enum.IsDefined(typeof(ProductionOrderBatchState), value))
                {
                    throw new Exception("Невозможно присвоить полю «Статус» недопустимое значение.");
                }

                if (state == ProductionOrderBatchState.Tabulation && value == ProductionOrderBatchState.Approved)
                {
                    throw new Exception(String.Format("Невозможно перевести этап из статуса «{0}» в статус «{1}».", state.GetDisplayName(), value.GetDisplayName()));
                }

                if (state == ProductionOrderBatchState.Approved && value == ProductionOrderBatchState.Tabulation)
                {
                    throw new Exception(String.Format("Невозможно перевести этап из статуса «{0}» в статус «{1}».", state.GetDisplayName(), value.GetDisplayName()));
                }

                state = value;
            }
        }
        private ProductionOrderBatchState state;

        /// <summary>
        /// Дата переведения на этап "Утверждение"
        /// </summary>
        public virtual DateTime? MovementToApprovementStateDate { get; protected set; }

        /// <summary>
        /// Кто перевел на этап "Утверждение"
        /// </summary>
        public virtual User MovedToApprovementStateBy { get; protected set; }

        /// <summary>
        /// Дата переведения на этап "Готово"
        /// </summary>
        public virtual DateTime? MovementToApprovedStateDate { get; protected set; }

        /// <summary>
        /// Переведено ли на этап "Готово"
        /// </summary>
        public virtual bool IsApprovedState { get { return MovementToApprovedStateDate != null; } }

        /// <summary>
        /// Кто перевел на этап "Готово"
        /// </summary>
        public virtual User MovedToApprovedStateBy { get; protected set; }


        /// <summary>
        /// Утверждено ли линейным руководителем
        /// </summary>
        public virtual bool IsApprovedByLineManager { get { return ApprovedByLineManagerDate != null; } }

        /// <summary>
        /// Дата утверждения линейным руководителем
        /// </summary>
        public virtual DateTime? ApprovedByLineManagerDate { get; protected set; }

        /// <summary>
        /// Кто утвердил как линейный руководитель
        /// </summary>
        public virtual User ApprovedLineManager { get; protected set; }


        /// <summary>
        /// Утверждено ли финансовым отделом
        /// </summary>
        public virtual bool IsApprovedByFinancialDepartment { get { return ApprovedByFinancialDepartmentDate != null; } }

        /// <summary>
        /// Дата утверждения финансовым отделом
        /// </summary>
        public virtual DateTime? ApprovedByFinancialDepartmentDate { get; protected set; }

        /// <summary>
        /// Кто утвердил от финансового отдела
        /// </summary>
        public virtual User FinancialDepartmentApprover { get; protected set; }


        /// <summary>
        /// Утверждено ли отделом продаж
        /// </summary>
        public virtual bool IsApprovedBySalesDepartment { get { return ApprovedBySalesDepartmentDate != null; } }

        /// <summary>
        /// Дата утверждения отделом продаж
        /// </summary>
        public virtual DateTime? ApprovedBySalesDepartmentDate { get; protected set; }

        /// <summary>
        /// Кто утвердил от отдела продаж
        /// </summary>
        public virtual User SalesDepartmentApprover { get; protected set; }


        /// <summary>
        /// Утверждено ли аналитическим отделом
        /// </summary>
        public virtual bool IsApprovedByAnalyticalDepartment { get { return ApprovedByAnalyticalDepartmentDate != null; } }

        /// <summary>
        /// Дата утверждения аналитическим отделом
        /// </summary>
        public virtual DateTime? ApprovedByAnalyticalDepartmentDate { get; protected set; }

        /// <summary>
        /// Кто утвердил от аналитического отдела
        /// </summary>
        public virtual User AnalyticalDepartmentApprover { get; protected set; }


        /// <summary>
        /// Утверждено ли руководителем проекта
        /// </summary>
        public virtual bool IsApprovedByProjectManager { get { return ApprovedByProjectManagerDate != null; } }

        /// <summary>
        /// Дата утверждения руководителем проекта
        /// </summary>
        public virtual DateTime? ApprovedByProjectManagerDate { get; protected set; }

        /// <summary>
        /// Кто утвердил как руководитель проекта
        /// </summary>
        public virtual User ApprovedProjectManager { get; protected set; }

        #endregion

        #region Этапы

        /// <summary>
        /// Этапы партии заказа на производство
        /// </summary>
        public virtual IEnumerable<ProductionOrderBatchStage> Stages
        {
            get { return new ImmutableSet<ProductionOrderBatchStage>(stages); }
        }
        private Iesi.Collections.Generic.ISet<ProductionOrderBatchStage> stages;

        /// <summary>
        /// Количество этапов
        /// </summary>
        public virtual int StageCount { get { return stages.Count; } }

        /// <summary>
        /// Количество пользовательских этапов
        /// </summary>
        public virtual int CustomStageCount { get { return stages.Count(x => x.IsDefault == false); } }

        /// <summary>
        /// Можно ли считать заданным план этапов заказа
        /// </summary>
        public virtual bool IsProductionOrderStagePlanSet { get { return CustomStageCount > 0; } }

        /// <summary>
        /// Первый (системный) этап партии заказа
        /// </summary>
        public virtual ProductionOrderBatchStage FirstStage
        {
            get
            {
                short minOrdinalNumber = stages.Min(x => x.OrdinalNumber);

                return stages.Single(x => x.OrdinalNumber == minOrdinalNumber);
            }
        }

        /// <summary>
        /// Последний пользовательский этап партии заказа
        /// </summary>
        public virtual ProductionOrderBatchStage LastCustomStage
        {
            get
            {
                if (CustomStageCount > 0)
                {
                    short maxCustomStageOrdinalNumber = stages.Where(x => x.IsDefault == false).Max(x => x.OrdinalNumber);

                    return stages.Single(x => x.OrdinalNumber == maxCustomStageOrdinalNumber);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Текущий этап партии заказа
        /// </summary>
        public virtual ProductionOrderBatchStage CurrentStage
        {
            get { return currentStage; }
            protected set
            {
                currentStage = value;

                IsClosed = value.Type == ProductionOrderBatchStageType.Closed;
            }
        }
        private ProductionOrderBatchStage currentStage;

        /// <summary>
        /// Следующий (планируемый) этап партии заказа
        /// </summary>
        public virtual ProductionOrderBatchStage NextStage
        {
            get
            {
                if (IsClosed) { return null; }

                return stages.Where(x => x.ActualStartDate == null).OrderBy(x => x.OrdinalNumber).First();
            }
        }

        /// <summary>
        /// Предыдущий этап
        /// </summary>
        public virtual ProductionOrderBatchStage PreviousStage
        {
            get
            {
                if (CurrentStage.OrdinalNumber == 1)
                {
                    return null;
                }

                return stages.Where(x => x.ActualStartDate != null && x.OrdinalNumber != CurrentStage.OrdinalNumber).OrderBy(x => x.OrdinalNumber).Last();
            }
        }

        /// <summary>
        /// Этап неуспешного закрытия
        /// </summary>
        public virtual ProductionOrderBatchStage UnsuccessfulClosingStage
        {
            get
            {
                // Пользуемся тем, что у данного этапа последний порядковый номер
                return stages.OrderBy(x => x.OrdinalNumber).Last();
            }
        }

        #endregion

        #region Даты и сроки

        /// <summary>
        /// Есть ли незавершенные этапы с типом «Производство»
        /// </summary>
        public virtual bool IsProductionPendingStage
        {
            get
            {
                return stages.Where(x => x.Type == ProductionOrderBatchStageType.Producing).ToList().Any(x => x.ActualEndDate == null);
            }
        }

        /// <summary>
        /// Ожидаемый срок производства
        /// </summary>
        public virtual DateTime? ProducingPendingDate
        {
            get
            {
                // Для неуспешно закрытой партии заказа возвращаем null.
                // Могли бы возвращать null, только если вдобавок есть хоть один незакрытый этап с типом «Производство»,
                // но при переходе с последнего этапа с типом «Производство» на неуспешное закрытие
                // этому этапу «Производство» ставится дата завершения, и она будет показываться как ожидаемая дата производства -
                // поэтому так делать нельзя.
                if (IsClosed && !IsClosedSuccessfully)
                {
                    return null;
                }

                // Получаем все этапы с типом «Производство»
                List<ProductionOrderBatchStage> list = stages.Where(x => x.Type == ProductionOrderBatchStageType.Producing).ToList();
                if (list.Any())
                {
                    // Если есть хоть один незакрытый этап
                    if (list.Any(x => x.ActualEndDate == null))
                    {
                        // Возвращаем максимальную предполагаемую дату закрытия (реальный срок производства не достигнут)
                        return list.Max(x => x.CalculateSupposedEndDate(DateTime.Now));
                    }
                    else
                    {
                        // Иначе возвращаем максимальную дату закрытия (реальный срок производства достигнут)
                        return list.Max(x => x.ActualEndDate);
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Планируемая дата завершения заказа
        /// </summary>
        public virtual DateTime PlannedEndDate
        {
            get
            {
                var lastCustomStage = LastCustomStage;

                return lastCustomStage != null ? lastCustomStage.PlannedEndDate.Value : FirstStage.PlannedEndDate.Value;
            }
        }

        /// <summary>
        /// Дата завершения (реальная - для закрытой партии, или ожидаемая дата поставки с учетом просрочки).
        /// Если нет пользовательских этапов, она равна дате завершения первого системного этапа
        /// </summary>
        public virtual DateTime EndDate
        {
            get
            {
                // Если закрыт, возвращает реальную дату перехода на текущий этап (закрытие)
                if (IsClosed)
                    return CurrentStage.ActualStartDate.Value;

                //Если есть пользовательские этапы то возращаем ожидаемую дату конца последнего этапа
                //иначе дату конца этапа создания
                if (LastCustomStage == null)
                    return FirstStage.ExpectedEndDate.Value;
                else
                    return LastCustomStage.ExpectedEndDate.Value;
            }
        }

        /// <summary>
        /// Отклонение от плана (всегда в календарных днях)
        /// </summary>
        public virtual int DivergenceFromPlan
        {
            get
            {
                // Находим разницу: фактическая дата начала - плановая дата начала
                int startDateDifference = (int)Math.Round((decimal)(CurrentStage.ActualStartDate.Value.Date - CurrentStage.PlannedStartDate).TotalDays);

                // Если закрыт, возвращаем эту разницу
                if (IsClosed)
                {
                    return startDateDifference;
                }

                // Иначе планируемая дата конца будет существовать
                // Если текущий этап просрочен, то к получившемуся значению (разнице) необходимо добавить количество дней просрочки текущего этапа
                if (DateTime.Now.Date > CurrentStage.ExpectedEndDate.Value)
                {
                    startDateDifference += (int)Math.Round((decimal)(DateTime.Now.Date - CurrentStage.ExpectedEndDate.Value).TotalDays);
                }

                return startDateDifference;
            }
        }

        #endregion

        #region Финансовые показатели, вес и объем

        /// <summary>
        /// Стоимость производства партии в валюте
        /// </summary>
        public virtual decimal ProductionOrderBatchProductionCostInCurrency { get { return rows.Sum(x => x.ProductionOrderBatchRowCostInCurrency); } }

        /// <summary>
        /// Общий вес партии
        /// </summary>
        public virtual decimal Weight { get { return rows.Sum(x => x.TotalWeight); } }

        /// <summary>
        /// Общий объем партии
        /// </summary>
        public virtual decimal Volume { get { return rows.Sum(x => x.TotalVolume); } }

        #endregion

        #endregion

        #region Конструкторы

        protected ProductionOrderBatch()
        {
        }

        public ProductionOrderBatch(ProductionOrderBatchStage calculationStage, ProductionOrderBatchStage successfullClosingStage,
            ProductionOrderBatchStage unsuccessfulClosingStage, User createdBy, DateTime currentDateTime, string name="")
        {
            CreationDate = currentDateTime;
            Date = currentDateTime;
            CreatedBy = createdBy;
            Curator = createdBy;
            SetName(name, currentDateTime);

            rows = new HashedSet<ProductionOrderBatchRow>();
            stages = new HashedSet<ProductionOrderBatchStage>();

            AddStage(calculationStage);
            AddStage(successfullClosingStage);
            AddStage(unsuccessfulClosingStage);
            currentStage = calculationStage;
            isClosed = false;

            state = ProductionOrderBatchState.Tabulation;
        }

        public ProductionOrderBatch(ProductionOrderBatch oldBatch, User createdBy, DateTime currentDateTime, string name="")
        {
            CreationDate = currentDateTime;
            Date = currentDateTime;
            CreatedBy = createdBy;
            Curator = oldBatch.Curator;

            SetName(name, currentDateTime);

            rows = new HashedSet<ProductionOrderBatchRow>();
            stages = new HashedSet<ProductionOrderBatchStage>();

            foreach (var stage in oldBatch.stages.OrderBy(x => x.OrdinalNumber))
            {
                var newStage = new ProductionOrderBatchStage(stage);
                AddStage(newStage);
            }
            currentStage = stages.Where(x => x.OrdinalNumber == oldBatch.currentStage.OrdinalNumber).SingleOrDefault();
            ValidationUtils.NotNull(currentStage.ActualStartDate);
            ValidationUtils.Assert(currentStage.ActualEndDate == null);

            isClosed = false;

            state = oldBatch.state;

            MovementToApprovementStateDate = oldBatch.MovementToApprovementStateDate;
            MovedToApprovementStateBy = oldBatch.MovedToApprovementStateBy;
            MovementToApprovedStateDate = oldBatch.MovementToApprovedStateDate;
            MovedToApprovedStateBy = oldBatch.MovedToApprovedStateBy;

            ApprovedByLineManagerDate = oldBatch.ApprovedByLineManagerDate;
            ApprovedLineManager = oldBatch.ApprovedLineManager;
            ApprovedByFinancialDepartmentDate = oldBatch.ApprovedByFinancialDepartmentDate;
            FinancialDepartmentApprover = oldBatch.FinancialDepartmentApprover;
            ApprovedBySalesDepartmentDate = oldBatch.ApprovedBySalesDepartmentDate;
            SalesDepartmentApprover = oldBatch.SalesDepartmentApprover;
            ApprovedByAnalyticalDepartmentDate = oldBatch.ApprovedByAnalyticalDepartmentDate;
            AnalyticalDepartmentApprover = oldBatch.AnalyticalDepartmentApprover;
            ApprovedByProjectManagerDate = oldBatch.ApprovedByProjectManagerDate;
            ApprovedProjectManager = oldBatch.ApprovedProjectManager;
        }



        #endregion

        #region Методы

        private void SetName(string name, DateTime currentDateTime)
        {
            if (name == "")
                Name = String.Format("Партия от {0}", currentDateTime.ToString());
            else
                Name = name;
        }
        #region Работа с позициями

        /// <summary>
        /// Добавление позиции
        /// </summary>
        /// <param name="row">Позиция</param>
        public virtual void AddRow(ProductionOrderBatchRow row)
        {
            ValidationUtils.Assert(State == ProductionOrderBatchState.Tabulation,
                String.Format("Невозможно добавить позицию в партию со статусом «{0}».", State.GetDisplayName()));
            ValidationUtils.Assert(!rows.Any(x => x.Article == row.Article), "Данный товар уже содержится в партии.");
            CheckPossibilityToSetManufacturerForRow(row.Manufacturer);

            rows.Add(row);
            row.Batch = this;
        }

        /// <summary>
        /// Добавление позиции во время разделения партии (без проверок на статус)
        /// </summary>
        /// <param name="row">Позиция</param>
        public virtual void AddSplittedRow(ProductionOrderBatchRow row)
        {
            rows.Add(row);
            row.Batch = this;
        }

        /// <summary>
        /// Удаление позиции
        /// </summary>
        /// <param name="row">Позиция</param>
        /// <param name="currentDateTime">Текущее время</param>
        public virtual void DeleteRow(ProductionOrderBatchRow row, DateTime currentDateTime)
        {
            ValidationUtils.Assert(State == ProductionOrderBatchState.Tabulation,
                String.Format("Невозможно удалить позицию в партии со статусом «{0}».", State.GetDisplayName()));

            row.DeletionDate = currentDateTime;
            rows.Remove(row);
        }

        /// <summary>
        /// Удаление позиции во время разделения партии (без проверок на статус)
        /// </summary>
        /// <param name="row">Позиция</param>
        /// <param name="currentDateTime">Текущее время</param>
        public virtual void DeleteSplittedRow(ProductionOrderBatchRow row, DateTime currentDateTime)
        {
            row.DeletionDate = currentDateTime;
            rows.Remove(row);
        }

        #endregion

        #region Работа со статусами

        /// <summary>
        /// Провести (перевести в статус "Утверждение")
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="currentDateTime">Текущее время</param>
        public virtual void Accept(User user, DateTime currentDateTime)
        {
            CheckPossibilityToAccept();

            State = ProductionOrderBatchState.Approvement;

            MovementToApprovementStateDate = currentDateTime;
            MovedToApprovementStateBy = user;
        }

        public virtual void CancelAcceptance()
        {
            CheckPossibilityToCancelAcceptation();

            State = ProductionOrderBatchState.Tabulation;

            MovementToApprovementStateDate = null;
            MovedToApprovementStateBy = null;
        }

        /// <summary>
        /// Одобрить (перевести в статус "Готово" после нажатия кнопки "Готово")
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="currentDateTime">Текущее время</param>
        public virtual void Approve(User user, DateTime currentDateTime)
        {
            CheckPossibilityToApprove();

            State = ProductionOrderBatchState.Approved;

            MovementToApprovedStateDate = currentDateTime;
            MovedToApprovedStateBy = user;
        }

        /// <summary>
        /// Отменить готовность (перевести назад в статус "Утверждение" после нажатия кнопки "Отменить готовность")
        /// </summary>
        public virtual void CancelApprovement()
        {
            CheckPossibilityToCancelApprovement();

            State = ProductionOrderBatchState.Approvement;

            MovementToApprovedStateDate = null;
            MovedToApprovedStateBy = null;

            ApprovedByLineManagerDate = null;
            ApprovedLineManager = null;
            ApprovedByFinancialDepartmentDate = null;
            FinancialDepartmentApprover = null;
            ApprovedBySalesDepartmentDate = null;
            SalesDepartmentApprover = null;
            ApprovedByAnalyticalDepartmentDate = null;
            AnalyticalDepartmentApprover = null;
            ApprovedByProjectManagerDate = null;
            ApprovedProjectManager = null;
        }

        /// <summary>
        /// Утвердить от имени действующего лица (нажатие кнопки "Утвердить: ..." при статусе "Утверждение")
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="actor">Тип лица утверждающего или отменяющего утверждения партии</param>
        /// <param name="currentDateTime">Текущее время</param>
        public virtual void Approve(User user, ProductionOrderApprovementActor actor, DateTime currentDateTime)
        {
            CheckPossibilityToApproveByActor(actor);

            switch (actor)
            {
                case ProductionOrderApprovementActor.LineManager:
                    ApprovedByLineManagerDate = currentDateTime;
                    ApprovedLineManager = user;
                    break;
                case ProductionOrderApprovementActor.FinancialDepartment:

                    ApprovedByFinancialDepartmentDate = currentDateTime;
                    FinancialDepartmentApprover = user;
                    break;
                case ProductionOrderApprovementActor.SalesDepartment:
                    ApprovedBySalesDepartmentDate = currentDateTime;
                    SalesDepartmentApprover = user;
                    break;
                case ProductionOrderApprovementActor.AnalyticalDepartment:
                    ApprovedByAnalyticalDepartmentDate = currentDateTime;
                    AnalyticalDepartmentApprover = user;
                    break;
                case ProductionOrderApprovementActor.ProjectManager:
                    ApprovedByProjectManagerDate = currentDateTime;
                    ApprovedProjectManager = user;
                    break;
                default:
                    throw new Exception("Неизвестный тип лица, утверждающего партию заказа.");
            }
        }

        /// <summary>
        /// Отменить утверждение от имени действующего лица (нажатие кнопки "Отменить утверждение: ..." при статусе "Утверждение")
        /// </summary>
        /// <param name="actor"></param>
        public virtual void CancelApprovement(ProductionOrderApprovementActor actor)
        {
            CheckPossibilityToCancelApprovementByActor(actor);

            switch (actor)
            {
                case ProductionOrderApprovementActor.LineManager:
                    ApprovedByLineManagerDate = null;
                    ApprovedLineManager = null;
                    ApprovedByFinancialDepartmentDate = null;
                    FinancialDepartmentApprover = null;
                    ApprovedBySalesDepartmentDate = null;
                    SalesDepartmentApprover = null;
                    ApprovedByAnalyticalDepartmentDate = null;
                    AnalyticalDepartmentApprover = null;
                    ApprovedByProjectManagerDate = null;
                    ApprovedProjectManager = null;
                    break;
                case ProductionOrderApprovementActor.FinancialDepartment:
                    ApprovedByFinancialDepartmentDate = null;
                    FinancialDepartmentApprover = null;
                    ApprovedByProjectManagerDate = null;
                    ApprovedProjectManager = null;
                    break;
                case ProductionOrderApprovementActor.SalesDepartment:
                    ApprovedBySalesDepartmentDate = null;
                    SalesDepartmentApprover = null;
                    ApprovedByProjectManagerDate = null;
                    ApprovedProjectManager = null;
                    break;
                case ProductionOrderApprovementActor.AnalyticalDepartment:
                    ApprovedByAnalyticalDepartmentDate = null;
                    AnalyticalDepartmentApprover = null;
                    ApprovedByProjectManagerDate = null;
                    ApprovedProjectManager = null;
                    break;
                case ProductionOrderApprovementActor.ProjectManager:
                    ApprovedByProjectManagerDate = null;
                    ApprovedProjectManager = null;
                    break;
                default:
                    throw new Exception("Неизвестный тип лица, утверждающего партию заказа.");
            }
        }

        #endregion

        #region Работа с этапами

        #region Редактирование этапов

        /// <summary>
        /// Перерасчет порядковых номеров этапов
        /// </summary>
        private void RecalculateOrdinalNumbers()
        {
            var stageList = new List<ProductionOrderBatchStage>(Stages.OrderBy(x => x.OrdinalNumber));

            short number = 1;

            foreach (var stage in stageList)
            {
                stage.OrdinalNumber = number;
                number++;
            }
        }

        /// <summary>
        /// Проверка правильности следования этапов
        /// </summary>
        public virtual void CheckStageOrder()
        {
            bool isLastStageCalculation = true; // Имел ли предыдущий этап тип "Расчет заказа"
            bool isCurrentStageCalculation; // Имеет ли текущий этап тип "Расчет заказа"
            foreach (var stage in stages.OrderBy(x => x.OrdinalNumber))
            {
                isCurrentStageCalculation = stage.Type == ProductionOrderBatchStageType.Calculation;
                if (isCurrentStageCalculation && !isLastStageCalculation)
                {
                    throw new Exception(String.Format("Этапы с типом «{0}» не могут идти после других этапов.",
                        ProductionOrderBatchStageType.Calculation.GetDisplayName()));
                }

                isLastStageCalculation = isCurrentStageCalculation;
            }
        }

        /// <summary>
        /// Удалить все пользовательские этапы
        /// </summary>
        public virtual void ClearCustomStages()
        {
            var stageList = new List<ProductionOrderBatchStage>(Stages.Where(x => x.IsDefault == false));
            foreach (var stage in stageList)
            {
                stage.Batch = null;
                stages.Remove(stage);
            }

            RecalculateOrdinalNumbers();
        }

        /// <summary>
        /// Добавить этап в конец списка (применяется только для системных этапов при конструировании)
        /// </summary>
        /// <param name="stage">Этап</param>
        private void AddStage(ProductionOrderBatchStage stage)
        {
            RecalculateOrdinalNumbers();
            stages.Add(stage);
            stage.Batch = this;
            stage.OrdinalNumber = (short)StageCount;
            CheckStageOrder();
        }

        /// <summary>
        /// Добавить этап после этапа с заданным порядковым номером
        /// </summary>
        /// <param name="stage">Этап</param>
        /// <param name="position">Порядковый номер этапа, после которого осуществляется вставка</param>
        public virtual void AddStage(ProductionOrderBatchStage stage, short position)
        {
            if (position < 1 || position > StageCount - 2)
            {
                throw new Exception("Невозможно вставить этап на заданную позицию.");
            }

            RecalculateOrdinalNumbers();

            var stageList = new List<ProductionOrderBatchStage>(Stages.OrderBy(x => x.OrdinalNumber));

            foreach (var productionOrderBatchLifeCycleTemplateStage in stageList)
            {
                if (productionOrderBatchLifeCycleTemplateStage.OrdinalNumber > position)
                    productionOrderBatchLifeCycleTemplateStage.OrdinalNumber++;
            }

            stages.Add(stage);
            stage.Batch = this;
            stage.OrdinalNumber = (short)(position + 1);

            CheckStageOrder();
        }

        /// <summary>
        /// Удалить этап
        /// </summary>
        /// <param name="stage">Этап</param>
        public virtual void DeleteStage(ProductionOrderBatchStage stage)
        {
            if (!stages.Contains(stage))
            {
                throw new Exception("Этап не найден. Возможно, он был удален.");
            }

            if (stage.IsDefault)
            {
                throw new Exception("Невозможно удалить системный этап.");
            }

            stage.Batch = null;
            stages.Remove(stage);
            RecalculateOrdinalNumbers();
            CheckStageOrder();
        }

        /// <summary>
        /// Переместить этап вверх
        /// </summary>
        /// <param name="stage">Этап</param>
        public virtual void MoveStageUp(ProductionOrderBatchStage stage)
        {
            RecalculateOrdinalNumbers();

            var otherStage = stages.SingleOrDefault(x => x.OrdinalNumber == stage.OrdinalNumber - 1);
            ValidationUtils.NotNull(otherStage, "Невозможно переместить этап.");

            if (otherStage.IsDefault)
            {
                throw new Exception("Невозможно переместить этап.");
            }

            var tempOrdinalNumber = stage.OrdinalNumber;
            stage.OrdinalNumber = otherStage.OrdinalNumber;
            otherStage.OrdinalNumber = tempOrdinalNumber;
            CheckStageOrder();
        }

        /// <summary>
        /// Переместить этап вниз
        /// </summary>
        /// <param name="stage">Этап</param>
        public virtual void MoveStageDown(ProductionOrderBatchStage stage)
        {
            RecalculateOrdinalNumbers();

            var otherStage = stages.Where(x => x.OrdinalNumber == stage.OrdinalNumber + 1).SingleOrDefault();
            ValidationUtils.NotNull(otherStage, "Невозможно переместить этап.");

            if (otherStage.IsDefault)
            {
                throw new Exception("Невозможно переместить этап.");
            }

            var tempOrdinalNumber = stage.OrdinalNumber;
            stage.OrdinalNumber = otherStage.OrdinalNumber;
            otherStage.OrdinalNumber = tempOrdinalNumber;
            CheckStageOrder();
        }

        /// <summary>
        /// Удалить все старые пользовательские этапы и загрузить их из шаблона заказа
        /// </summary>
        /// <param name="productionOrderBatchLifeCycleTemplate">Шаблон</param>
        public virtual void LoadStagesFromTemplate(ProductionOrderBatchLifeCycleTemplate productionOrderBatchLifeCycleTemplate)
        {
            ClearCustomStages();

            short startingPosition = 1;

            foreach (var templateStage in productionOrderBatchLifeCycleTemplate.Stages.Where(x => x.IsDefault == false).OrderBy(x => x.OrdinalNumber))
            {
                var newStage = new ProductionOrderBatchStage(templateStage);

                AddStage(newStage, startingPosition);
                startingPosition++;
            }

            CheckStageOrder();
        }

        #endregion

        #region Переходы между этапами

        public virtual void MoveToNextStage(DateTime currentDateTime)
        {
            var newStage = NextStage;
            ValidationUtils.NotNull(newStage, String.Format("Невозможно перейти на следующий этап с этапа «{0}»", CurrentStage.Name));
            CheckPossibilityToMoveToNextStage();

            CurrentStage.ActualEndDate = newStage.ActualStartDate = currentDateTime;
            CurrentStage = newStage;
        }

        public virtual void MoveToPreviousStage()
        {
            var newStage = PreviousStage;
            ValidationUtils.NotNull(newStage, String.Format("Невозможно перейти на предыдущий этап с этапа «{0}»", CurrentStage.Name));
            CheckPossibilityToMoveToPreviousStage();

            CurrentStage.ActualStartDate = newStage.ActualEndDate = null;
            CurrentStage = newStage;
        }

        /// <summary>
        /// Перейти на этап "Неуспешное закрытие"
        /// </summary>
        /// <param name="currentDateTime">Текущее время</param>
        public virtual void MoveToUnsuccessfulClosingStage(DateTime currentDateTime)
        {
            var newStage = UnsuccessfulClosingStage;
            ValidationUtils.Assert(!IsClosed, String.Format("Невозможно перейти на этап «{0}» с этапа «{1}»", newStage.Name, CurrentStage.Name));
            CheckPossibilityToMoveToUnsuccessfulClosingStage();

            CurrentStage.ActualEndDate = newStage.ActualStartDate = currentDateTime;
            CurrentStage = newStage;
        }

        #endregion

        #endregion

        #region Возможность совершения операций

        public virtual void CheckPossibilityToCreateRow()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно добавлять позиции в закрытую партию заказа.");
            ValidationUtils.Assert(State == ProductionOrderBatchState.Tabulation,
                String.Format("Невозможно добавить позицию в партию со статусом «{0}».", State.GetDisplayName()));
        }

        public virtual void CheckPossibilityToEditRow()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно редактировать закрытую партию заказа.");
            ValidationUtils.Assert(State == ProductionOrderBatchState.Tabulation, 
                String.Format("Невозможно редактировать партию со статусом «{0}».", State.GetDisplayName()));
        }

        public virtual void CheckPossibilityToDeleteRow()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно удалить закрытую партию.");
            ValidationUtils.Assert(State == ProductionOrderBatchState.Tabulation,
                String.Format("Невозможно удалить партию со статусом «{0}».", State.GetDisplayName()));
        }

        /// <summary>
        /// Возможность создать связанную приходную накладную
        /// </summary>
        public virtual void CheckPossibilityToCreateReceiptWaybill()
        {
            ValidationUtils.IsNull(ReceiptWaybill, "По данной партии заказа уже создана приходная накладная.");
            ValidationUtils.Assert(RowCount > 0, "Невозможно создать приходную накладную по партии заказа без позиций.");
            ValidationUtils.NotNull(ProductionOrder.Contract, "Невозможно создать приходную накладную, так как по заказу не создан контракт.");

            CheckPossibilityToHaveReceiptWaybill();
        }

        /// <summary>
        /// Возможность иметь связанную приходную накладную на данном этапе
        /// </summary>
        public virtual void CheckPossibilityToHaveReceiptWaybill()
        {
            ValidationUtils.Assert(IsApprovedState,
                 "Невозможно создать приходную накладную по партии заказа, так как партия не переведена в статус «Готово».");

            ValidationUtils.Assert(currentStage.Type != ProductionOrderBatchStageType.Calculation,
                String.Format("Невозможно создать приходную накладную по партии заказа на этапе с типом «{0}».",
                currentStage.Type.GetDisplayName()));

            //если неуспешно закрыто, вывести сообщение
            ValidationUtils.Assert(!IsClosed || IsClosedSuccessfully,
                String.Format("Невозможно создать приходную накладную по партии заказа на этапе «{0}».", currentStage.Name));
        }

        public virtual void CheckPossibilityToEditStages()
        {
            ValidationUtils.Assert(CurrentStage.OrdinalNumber == 1,
                String.Format("Невозможно редактировать этапы партии на этапах, отличных от «{0}».", FirstStage.Name));
            ValidationUtils.IsNull(ReceiptWaybill, "Невозможно редактировать этапы партии, для которой создана накладная прихода.");
        }

        public virtual void CheckPossibilityToMoveToNextStage()
        {
            ValidationUtils.Assert(!ProductionOrder.IsClosed, "Невозможно редактировать партию в закрытом заказе.");
            ValidationUtils.Assert(!IsClosed, "Невозможно редактировать закрытую партию заказа.");
            ValidationUtils.NotNull(NextStage, "Партия не имеет следующего этапа.");

            ValidationUtils.Assert(!(NextStage.Type == ProductionOrderBatchStageType.Closed && !IsApprovedState),
                "Невозможно успешно закрыть неподготовленную партию");
            ValidationUtils.Assert(IsProductionOrderStagePlanSet, "План этапов не задан.");
        }

        public virtual void CheckPossibilityToMoveToPreviousStage()
        {
            ValidationUtils.Assert(!ProductionOrder.IsClosed, "Невозможно редактировать партию в закрытом заказе.");
            if (CurrentStage.OrdinalNumber == 1)
            {
                throw new Exception("Партия не имеет предыдущего этапа.");
            }
        }


        public virtual void CheckPossibilityToMoveToUnsuccessfulClosingStage()
        {
            ValidationUtils.Assert(!ProductionOrder.IsClosed, "Невозможно редактировать партию в закрытом заказе.");
            ValidationUtils.Assert(!IsClosed, "Невозможно редактировать закрытую партию заказа.");
        }

        public virtual void CheckPossibilityToMoveStageUp(ProductionOrderBatchStage stage)
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно изменять порядок этапов в закрытой партии заказа.");
            ValidationUtils.Assert(currentStage.OrdinalNumber == 1, String.Format("Невозможно изменять порядок этапов на этапах, отличных от «{0}».", FirstStage.Name));

            if (!stages.Contains(stage))
            {
                throw new Exception("Этап не найден. Возможно, он был удален.");
            }

            if (stage.IsDefault)
            {
                throw new Exception("Невозможно переместить системный этап.");
            }

            if (stage.OrdinalNumber <= 2)
            {
                throw new Exception("Невозможно переместить этап.");
            }
        }

        public virtual void CheckPossibilityToMoveStageDown(ProductionOrderBatchStage stage)
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно изменять порядок этапов в закрытой партии заказа.");
            ValidationUtils.Assert(currentStage.OrdinalNumber == 1, String.Format("Невозможно изменять порядок этапов на этапах, отличных от «{0}».", FirstStage.Name));

            if (!stages.Contains(stage))
            {
                throw new Exception("Этап не найден. Возможно, он был удален.");
            }

            if (stage.IsDefault)
            {
                throw new Exception("Невозможно переместить системный этап.");
            }

            if (stage.OrdinalNumber >= (StageCount - 2))
            {
                throw new Exception("Невозможно переместить этап.");
            }
        }

        /// <summary>
        /// Проверка возможности создать новый этап ПОСЛЕ указанного
        /// </summary>
        /// <param name="stage">Этап, после которого хотим создать новый</param>
        public virtual void CheckPossibilityToCreateStage(ProductionOrderBatchStage stage)
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно редактировать этапы в закрытой партии заказа.");
            ValidationUtils.Assert(currentStage.OrdinalNumber == 1, String.Format("Невозможно редактировать этапы на этапах, отличных от «{0}».", FirstStage.Name));

            if (!stages.Contains(stage))
            {
                throw new Exception("Этап не найден. Возможно, он был удален.");
            }

            if (stage.OrdinalNumber != 1 && stage.IsDefault)
            {
                throw new Exception("Невозможно создать пользовательский этап в области системных.");
            }
        }

        /// <summary>
        /// Проверка возможности отредактировать этап
        /// </summary>
        /// <param name="stage">Этап</param>
        public virtual void CheckPossibilityToEditStage(ProductionOrderBatchStage stage)
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно редактировать этапы в закрытой партии заказа.");
            ValidationUtils.Assert(currentStage.OrdinalNumber == 1, String.Format("Невозможно редактировать этапы на этапах, отличных от «{0}».", FirstStage.Name));

            if (!stages.Contains(stage))
            {
                throw new Exception("Этап не найден. Возможно, он был удален.");
            }

            if (stage.IsDefault)
            {
                throw new Exception("Невозможно изменить системный этап.");
            }
        }

        /// <summary>
        /// Проверка возможности удалить этап
        /// </summary>
        /// <param name="stage">Этап</param>
        public virtual void CheckPossibilityToDeleteStage(ProductionOrderBatchStage stage)
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно удалять этапы в закрытой партии заказа.");
            ValidationUtils.Assert(currentStage.OrdinalNumber == 1, String.Format("Невозможно удалять этапы на этапах, отличных от «{0}».", FirstStage.Name));

            if (!stages.Contains(stage))
            {
                throw new Exception("Этап не найден. Возможно, он был удален.");
            }

            if (stage.IsDefault)
            {
                throw new Exception("Невозможно удалить системный этап.");
            }
        }

        public virtual void CheckPossibilityToAccept()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно провести закрытую партию заказа.");

            if (State != ProductionOrderBatchState.Tabulation)
            {
                throw new Exception(String.Format("Невозможно провести партию со статусом «{0}».", State.GetDisplayName()));
            }

            if (RowCount == 0)
            {
                throw new Exception("Невозможно провести пустую партию.");
            }
        }

        public virtual void CheckPossibilityToCancelAcceptation()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно отменить проводку закрытой партии заказа.");

            if (State != ProductionOrderBatchState.Approvement)
            {
                throw new Exception(String.Format("Невозможно отменить проводку партии со статусом «{0}».", State.GetDisplayName()));
            }

            if (IsApprovedByLineManager || IsApprovedByFinancialDepartment || IsApprovedBySalesDepartment || IsApprovedByAnalyticalDepartment || IsApprovedByProjectManager)
            {
                throw new Exception("Невозможно отменить проводку утвержденной партии.");
            }
        }

        private void CheckCommonPossibilityToApprove()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно утвердить закрытую партию заказа.");

            if (State != ProductionOrderBatchState.Approvement)
            {
                throw new Exception(String.Format("Невозможно утвердить партию со статусом «{0}».", State.GetDisplayName()));
            }
        }

        private void CheckCommonPossibilityToCancelApprovement()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно отменить утверждение закрытой партии заказа.");
            ValidationUtils.Assert(State == ProductionOrderBatchState.Approvement,
                String.Format("Невозможно отменить утверждение партии со статусом «{0}».", State.GetDisplayName()));
        }

        public virtual void CheckPossibilityToApprove()
        {
            CheckCommonPossibilityToApprove();

            ValidationUtils.Assert(IsApprovedByLineManager && IsApprovedByFinancialDepartment && IsApprovedBySalesDepartment && IsApprovedByAnalyticalDepartment &&
                IsApprovedByProjectManager, "Невозможно одобрить партию, пока она не утверждена всеми действующими лицами.");
        }

        public virtual void CheckPossibilityToRename()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно переименовать закрытую партию");
        }
        /// <summary>
        /// Возможность отмены статуса "Готово"
        /// </summary>
        public virtual void CheckPossibilityToCancelApprovement()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно отменить утверждение закрытой партии заказа.");
            ValidationUtils.Assert(State == ProductionOrderBatchState.Approved, "Невозможно отменить утверждение не утвержденной партии заказа.");
            ValidationUtils.IsNull(ReceiptWaybill, "Невозможно отменить утверждение партии заказа, по которой создана приходная накладная.");
        }

        public virtual void CheckPossibilityToSplitBatch()
        {
            ValidationUtils.Assert(!IsClosed, "Невозможно разделять партии в закрытом заказе.");
            ValidationUtils.Assert(state == ProductionOrderBatchState.Approved, String.Format("Невозможно разделить партию со статусом «{0}».", state.GetDisplayName()));
            ValidationUtils.Assert(currentStage.Type != ProductionOrderBatchStageType.Calculation, String.Format("Невозможно разделить партию на этапе с типом «{0}».",
                currentStage.Type.GetDisplayName()));
            ValidationUtils.IsNull(ReceiptWaybill, "Невозможно разделять партии, по которым создана приходная накладная.");
        }

        public virtual void CheckPossibilityToApproveByActor(ProductionOrderApprovementActor actor)
        {
            CheckCommonPossibilityToApprove();

            switch (actor)
            {
                case ProductionOrderApprovementActor.LineManager:
                    ValidationUtils.Assert(!IsApprovedByLineManager, () => String.Format("Данная партия заказа уже утверждена руководителем (пользователь «{0}» {1}).",
                        ApprovedLineManager.DisplayName, ApprovedByLineManagerDate.ForDisplay()));
                    break;
                case ProductionOrderApprovementActor.FinancialDepartment:
                    ValidationUtils.Assert(!IsApprovedByFinancialDepartment, () => String.Format("Данная партия заказа уже утверждена финансовым отделом (пользователь «{0}» {1}).",
                        FinancialDepartmentApprover.DisplayName, ApprovedByFinancialDepartmentDate.ForDisplay()));
                    ValidationUtils.Assert(IsApprovedByLineManager, "Партия еще не утверждена руководителем.");
                    break;
                case ProductionOrderApprovementActor.SalesDepartment:
                    ValidationUtils.Assert(!IsApprovedBySalesDepartment, () => String.Format("Данная партия заказа уже утверждена отделом продаж (пользователь «{0}» {1}).",
                        SalesDepartmentApprover.DisplayName, ApprovedBySalesDepartmentDate.ForDisplay()));
                    ValidationUtils.Assert(IsApprovedByLineManager, "Партия еще не утверждена руководителем.");
                    break;
                case ProductionOrderApprovementActor.AnalyticalDepartment:
                    ValidationUtils.Assert(!IsApprovedByAnalyticalDepartment, () => String.Format("Данная партия заказа уже утверждена аналитическим отделом (пользователь «{0}» {1}).",
                        AnalyticalDepartmentApprover.DisplayName, ApprovedByAnalyticalDepartmentDate.ForDisplay()));
                    ValidationUtils.Assert(IsApprovedByLineManager, "Партия еще не утверждена руководителем.");
                    break;
                case ProductionOrderApprovementActor.ProjectManager:
                    ValidationUtils.Assert(!IsApprovedByProjectManager, () => String.Format("Данная партия заказа уже утверждена руководителем проекта (пользователь «{0}» {1}).",
                        ApprovedProjectManager.DisplayName, ApprovedByProjectManagerDate.ForDisplay()));
                    ValidationUtils.Assert(IsApprovedByLineManager, "Партия еще не утверждена руководителем.");
                    ValidationUtils.Assert(IsApprovedByFinancialDepartment, "Партия еще не утверждена финансовым отделом.");
                    ValidationUtils.Assert(IsApprovedBySalesDepartment, "Партия еще не утверждена отделом продаж.");
                    ValidationUtils.Assert(IsApprovedByAnalyticalDepartment, "Партия еще не утверждена аналитическим отделом.");
                    break;
            }
        }

        public virtual void CheckPossibilityToCancelApprovementByActor(ProductionOrderApprovementActor actor)
        {
            CheckCommonPossibilityToCancelApprovement();

            switch (actor)
            {
                case ProductionOrderApprovementActor.LineManager:
                    ValidationUtils.Assert(IsApprovedByLineManager, "Партия еще не утверждена руководителем.");
                    break;
                case ProductionOrderApprovementActor.FinancialDepartment:
                    ValidationUtils.Assert(IsApprovedByFinancialDepartment, "Партия еще не утверждена финансовым отделом.");
                    break;
                case ProductionOrderApprovementActor.SalesDepartment:
                    ValidationUtils.Assert(IsApprovedBySalesDepartment, "Партия еще не утверждена отделом продаж.");
                    break;
                case ProductionOrderApprovementActor.AnalyticalDepartment:
                    ValidationUtils.Assert(IsApprovedByAnalyticalDepartment, "Партия еще не утверждена аналитическим отделом.");
                    break;
                case ProductionOrderApprovementActor.ProjectManager:
                    ValidationUtils.Assert(IsApprovedByProjectManager, "Партия еще не утверждена руководителем проекта.");
                    break;
            }
        }

        public virtual void CheckPossibilityToSetManufacturerForRow(Manufacturer manufacturer)
        {
            ValidationUtils.NotNull(manufacturer, "Изготовитель не указан.");

            ValidationUtils.Assert(ProductionOrder.Producer.Manufacturers.Contains(manufacturer),
                "Невозможно указать для позиции партии изготовителя, не связанного с производителем.");
        }

        #endregion

        #endregion
    }
}
