using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderBatchMainDetailsViewModel
    {
        /// <summary>
        /// Единая ли партия
        /// </summary>
        public bool IsSingleBatch { get; set; }

        /// <summary>
        /// Название статуса
        /// </summary>
        [DisplayName("Статус партии")]
        public string StateName { get; set; }

        /// <summary>
        /// Находится ли партия в статусе "Утверждение"
        /// </summary>
        public bool IsApprovementState { get; set; }

        /// <summary>
        /// Утверждено ли линейным руководителем
        /// </summary>
        [DisplayName("Рук.")]
        public bool IsApprovedByLineManager { get; set; }

        /// <summary>
        /// Утверждено ли финансовым отделом
        /// </summary>
        [DisplayName("Фин.")]
        public bool IsApprovedByFinancialDepartment { get; set; }

        /// <summary>
        /// Утверждено ли отделом продаж
        /// </summary>
        [DisplayName("Прод.")]
        public bool IsApprovedBySalesDepartment { get; set; }

        /// <summary>
        /// Утверждено ли аналитическим отделом
        /// </summary>
        [DisplayName("Аналит.")]
        public bool IsApprovedByAnalyticalDepartment { get; set; }

        /// <summary>
        /// Утверждено ли руководителем проекта
        /// </summary>
        [DisplayName("РП")]
        public bool IsApprovedByProjectManager { get; set; }

        /// <summary>
        /// Куратор
        /// </summary>
        [DisplayName("Куратор партии")]
        public string CuratorName { get; set; }

        /// <summary>
        /// Код куратора
        /// </summary>
        public string CuratorId { get; set; }

        /// <summary>
        /// Название заказа
        /// </summary>
        [DisplayName("Заказ")]
        public string ProductionOrderName { get; set; }

        /// <summary>
        /// Код заказа
        /// </summary>
        public string ProductionOrderId { get; set; }

        /// <summary>
        /// Производитель
        /// </summary>
        [DisplayName("Производитель")]
        public string ProducerName { get; set; }

        /// <summary>
        /// Код производителя
        /// </summary>
        public string ProducerId { get; set; }

        #region Для разделенной партии

        /// <summary>
        /// Этап партии
        /// </summary>
        [DisplayName("Этап")]
        public string CurrentStageName { get; set; }

        /// <summary>
        /// Дата начала этапа
        /// </summary>
        [DisplayName("Дата начала этапа")]
        public string CurrentStageActualStartDate { get; set; }

        /// <summary>
        /// Количество дней, прошедших с начала этапа
        /// </summary>
        public string CurrentStageDaysPassed { get; set; }

        /// <summary>
        /// Завершение этапа
        /// </summary>
        [DisplayName("Завершение этапа")]
        public string CurrentStageExpectedEndDate { get; set; }

        /// <summary>
        /// Количество дней, оставшихся до конца этапа
        /// </summary>
        public string CurrentStageDaysLeft { get; set; }

        #endregion

        /// <summary>
        /// Размещение в контейнерах
        /// </summary>
        [DisplayName("Размещение в контейнерах")]
        public string ContainerPlacement { get; set; }

        /// <summary>
        /// Свободный объем при размещении в контейнерах
        /// </summary>
        public string ContainerPlacementFreeVolume { get; set; }

        /// <summary>
        /// Валюта заказа
        /// </summary>
        [DisplayName("Валюта заказа")]
        public string CurrencyLiteralCode { get; set; }

        /// <summary>
        /// Курс (числовое выражение)
        /// </summary>
        [DisplayName("Курс")]
        public string CurrencyRate { get; set; }

        /// <summary>
        /// Наименование курса ("Текущий" или "от 99.99.99")
        /// </summary>
        public string CurrencyRateName { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        [DisplayName("Дата создания")]
        public string Date { get; set; }

        /// <summary>
        /// Ожидаемый срок производства
        /// </summary>
        [DisplayName("Ожидаемый срок производства")]
        public string ProducingPendingDate { get; set; }

        /// <summary>
        /// Ожидаемый срок поставки партии
        /// </summary>
        [DisplayName("Ожидаемый срок поставки партии")]
        public string DeliveryPendingDate { get; set; }

        /// <summary>
        /// Отклонение от плана
        /// </summary>
        [DisplayName("Отклонение от плана")]
        public string DivergenceFromPlan { get; set; }

        /// <summary>
        /// Общий вес партии
        /// </summary>
        [DisplayName("Общий вес партии")]
        public string Weight { get; set; }

        /// <summary>
        /// Общий объем партии
        /// </summary>
        [DisplayName("Общий объем партии")]
        public string Volume { get; set; }

        /// <summary>
        /// Стоимость производства
        /// </summary>
        [DisplayName("Стоимость производства")]
        public string ProductionCostSumInCurrency { get; set; }
        public string ProductionCostSumInBaseCurrency { get; set; }

        /// <summary>
        /// Сумма в УЦ (текущая)
        /// </summary>
        [DisplayName("Сумма в УЦ (текущая)")]
        public string AccountingPriceSum { get; set; }

        /// <summary>
        /// Соответствующая приходная накладная
        /// </summary>
        [DisplayName("Приходная накладная")]
        public string ReceiptWaybillName { get; set; }

        /// <summary>
        /// Код соответствующей приходной накладной
        /// </summary>
        public string ReceiptWaybillId { get; set; }

        #region Допустимые действия

        /// <summary>
        /// Разрешено ли переименование партии.
        /// </summary>
        public bool AllowToRename { get; set; }

        /// <summary>
        /// Разрешено ли удаление партии. Последнию партию удалять нельзя.
        /// </summary>
        public bool AllowToDeleteBatch { get; set; }
        
        /// <summary>
        /// Разрешено ли менять куратора
        /// </summary>
        public bool AllowToChangeCurator { get; set; }

        /// <summary>
        /// Разрешено ли просматривать детали куратора
        /// </summary>
        public bool AllowToViewCuratorDetails { get; set; }

        /// <summary>
        /// Разрешено ли просматривать детали связанной приходной накладной
        /// </summary>
        public bool AllowToViewReceiptWaybillDetails { get; set; }

        /// <summary>
        /// Разрешено ли создание связанной приходной накладной (при уже созданной накладной запрещено)
        /// </summary>
        public bool AllowToCreateReceiptWaybill { get; set; }

        /// <summary>
        /// Разрешено ли удаление связанной приходной накладной
        /// </summary>
        public bool AllowToDeleteReceiptWaybill { get; set; }

        /// <summary>
        /// Разрешено ли менять этапы
        /// </summary>
        public bool AllowToChangeStage { get; set; }

        /// <summary>
        /// Разрешено ли редактировать список этапов
        /// </summary>
        public bool AllowToEditStages { get; set; }

        /// <summary>
        /// Разрешено ли перерассчитывать размещение по контейнерам
        /// </summary>
        public bool AllowToRecalculatePlacement { get; set; }

        /// <summary>
        /// Разрешено ли разделить партию
        /// </summary>
        public bool AllowToSplitBatch { get; set; }

        /// <summary>
        /// Разрешено ли добавлять/редактировать позиции в партии
        /// </summary>
        public bool AllowToEditRows { get; set; }

        public bool AllowToViewStageList { get; set; }

        public bool AllowToAccept { get; set; }
        public bool AllowToCancelAcceptance { get; set; }
        public bool AllowToApprove { get; set; }
        public bool AllowToCancelApprovement { get; set; }

        public bool AllowToApproveByLineManager { get; set; }
        public bool AllowToCancelApprovementByLineManager { get; set; }
        public bool AllowToApproveByFinancialDepartment { get; set; }
        public bool AllowToCancelApprovementByFinancialDepartment { get; set; }
        public bool AllowToApproveBySalesDepartment { get; set; }
        public bool AllowToCancelApprovementBySalesDepartment { get; set; }
        public bool AllowToApproveByAnalyticalDepartment { get; set; }
        public bool AllowToCancelApprovementByAnalyticalDepartment { get; set; }
        public bool AllowToApproveByProjectManager { get; set; }
        public bool AllowToCancelApprovementByProjectManager { get; set; }

        #endregion
    }
}