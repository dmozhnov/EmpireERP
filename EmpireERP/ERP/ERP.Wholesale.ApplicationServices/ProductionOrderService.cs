using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class ProductionOrderService : IProductionOrderService
    {
        #region Поля

        private readonly IArticlePriceService articlePriceService;
        private readonly ICurrencyService currencyService;
        private readonly IReceiptWaybillService receiptWaybillService;
        private readonly ITaskRepository taskRepository;

        private readonly IProductionOrderRepository productionOrderRepository;
        private readonly IProductionOrderBatchRepository productionOrderBatchRepository;
        private readonly IReceiptWaybillRepository receiptWaybillRepository;
        private readonly IDefaultProductionOrderStageRepository defaultProductionOrderStageRepository;

        #endregion

        #region Конструкторы

        public ProductionOrderService(IProductionOrderRepository productionOrderRepository, IProductionOrderBatchRepository productionOrderBatchRepository,
            IReceiptWaybillRepository receiptWaybillRepository, ITaskRepository taskRepository, IDefaultProductionOrderStageRepository defaultProductionOrderStageRepository)
        {
            this.productionOrderRepository = productionOrderRepository;
            this.productionOrderBatchRepository = productionOrderBatchRepository;
            this.receiptWaybillRepository = receiptWaybillRepository;
            this.defaultProductionOrderStageRepository = defaultProductionOrderStageRepository;
            this.taskRepository = taskRepository;

            articlePriceService = IoCContainer.Resolve<IArticlePriceService>();
            currencyService = IoCContainer.Resolve<ICurrencyService>();
            receiptWaybillService = IoCContainer.Resolve<IReceiptWaybillService>();
        }

        #endregion

        #region Методы

        #region Общие

        private ProductionOrder GetById(Guid id, User user)
        {
            return GetById(id, user, Permission.ProductionOrder_List_Details);
        }

        private ProductionOrder GetById(Guid id, User user, Permission permission)
        {
            var type = user.GetPermissionDistributionType(permission);

            // если права нет - то сразу возвращаем null
            if (type == PermissionDistributionType.None)
            {
                return null;
            }
            else
            {
                var order = productionOrderRepository.GetById(id);

                if (type == PermissionDistributionType.All)
                {
                    return order;
                }
                else
                {
                    bool contains = (user.Teams.SelectMany(x => x.ProductionOrders).Contains(order));

                    if ((type == PermissionDistributionType.Personal && order.Curator == user && contains) ||
                        (type == PermissionDistributionType.Teams && contains))
                    {
                        return order;
                    }
                }

                return null;
            }
        }

        public IEnumerable<ProductionOrder> FilterByUser(IEnumerable<ProductionOrder> list, User user, Permission permission)
        {
            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<ProductionOrder>();

                case PermissionDistributionType.Personal:
                    return user.Teams.SelectMany(x => x.ProductionOrders).Where(x => x.Curator == user).Intersect(list).Distinct();

                case PermissionDistributionType.Teams:
                    return user.Teams.SelectMany(x => x.ProductionOrders).Intersect(list).Distinct();

                case PermissionDistributionType.All:
                    return list;

                default:
                    return null;
            }
        }

        public Guid Save(ProductionOrder productionOrder, User user)
        {
            // Проверяем, нет ли этапов с повторяющимися названиями
            foreach (var batch in productionOrder.Batches)
            {
                CheckProductionOrderBatchStageNameUniqueness(batch);
            }

            // Проверяем, нет ли таможенных листов, у которых сумма оплат превышает сумму таможенного листа
            foreach (var customsDeclaration in productionOrder.CustomsDeclarations)
            {
                ValidationUtils.Assert(customsDeclaration.Sum >= customsDeclaration.PaymentSum,
                    "Сумма оплат по таможенному листу не может превосходить сумму таможенного листа.");
            }

            // признак необходимости добавления заказа в команды текущего пользователя
            bool needAddToTeams = false;

            var existingOrder = productionOrderRepository.GetById(productionOrder.Id); //проверка, был ли ранее создан заказ с этим идентификатором, если нет - значит создаем новый
            if (existingOrder == null)
            {
                needAddToTeams = true;
            }

            productionOrderRepository.Save(productionOrder);

            // при добавлении заказ добавляем его во все команды пользователя
            if (needAddToTeams)
            {
                foreach (var team in user.Teams)
                {
                    team.AddProductionOrder(productionOrder);
                }
            }

            return productionOrder.Id;
        }

        /// <summary>
        /// Закрываем заказ
        /// </summary>
        /// <returns>0 - если при разнесении закупочных цен по позициям  удалось добиться совпадения себестоимости и суммы накладных 
        /// (для связанного прихода), иначе сумма коррекции</returns>
        public decimal Close(ProductionOrder productionOrder, User user)
        {
            CheckPossibilityToClose(productionOrder, user);
            productionOrder.IsClosed = true;

            //расчет и запись закупочных цен
            var correctionSum = CalculatePurchaseCostByArticlePrimeCost(productionOrder);

            foreach (var batch in productionOrder.Batches.Where(x => x.ReceiptWaybill != null))
            {
                receiptWaybillService.SetPurchaseCosts(batch.ReceiptWaybill);
            }

            return correctionSum;

        }

        /// <summary>
        /// Открыть заказ (отменить закрытие)
        /// </summary>
        public void Open(ProductionOrder productionOrder, User user)
        {
            CheckPossibilityToOpen(productionOrder, user);
            productionOrder.IsClosed = false;

            //Cбрасываем в 0 закупочные цены связанных приходов и пересчитываем показатели по ним
            foreach (var batch in productionOrder.Batches.Where(x => x.ReceiptWaybill != null))
            {
                receiptWaybillService.ResetPurchaseCosts(batch.ReceiptWaybill);
            }

        }

        public ProductionOrderBatch GetProductionOrderBatchById(Guid id)
        {
            return productionOrderBatchRepository.GetById(id);
        }

        public Guid SaveProductionOrderBatch(ProductionOrderBatch productionOrderBatch)
        {
            CheckProductionOrderBatchStageNameUniqueness(productionOrderBatch);

            productionOrderBatchRepository.Save(productionOrderBatch);

            return productionOrderBatch.Id;
        }

        public DefaultProductionOrderBatchStage GetDefaultProductionOrderBatchStageById(short id)
        {
            return defaultProductionOrderStageRepository.GetById(id);
        }

        public ProductionOrder DeleteProductionOrderTransportSheet(ProductionOrder productionOrder, ProductionOrderTransportSheet transportSheet, User user, DateTime currentDateTime)
        {
            CheckPossibilityToDeleteTransportSheet(transportSheet, user);

            productionOrder.DeleteTransportSheet(transportSheet, currentDateTime);

            return productionOrder;
        }

        public ProductionOrder DeleteProductionOrderExtraExpensesSheet(ProductionOrder productionOrder, ProductionOrderExtraExpensesSheet extraExpensesSheet, User user, DateTime currentDateTime)
        {
            CheckPossibilityToDeleteExtraExpensesSheet(extraExpensesSheet, user);

            productionOrder.DeleteExtraExpensesSheet(extraExpensesSheet, currentDateTime);

            return productionOrder;
        }

        public ProductionOrder DeleteProductionOrderCustomsDeclaration(ProductionOrder productionOrder, ProductionOrderCustomsDeclaration customsDeclaration, User user, DateTime currentDateTime)
        {
            CheckPossibilityToDeleteCustomsDeclaration(customsDeclaration, user);

            productionOrder.DeleteCustomsDeclaration(customsDeclaration, currentDateTime);

            return productionOrder;
        }

        public void DeleteProductionOrderPlannedPayment(ProductionOrderPlannedPayment productionOrderPlannedPayment, User user, DateTime currentDateTime)
        {
            CheckPossibilityToDeletePlannedPayment(productionOrderPlannedPayment, user);

            var productionOrder = productionOrderPlannedPayment.ProductionOrder;
            productionOrder.DeletePlannedPayment(productionOrderPlannedPayment, currentDateTime);
        }

        public void DeleteProductionOrderPayment(ProductionOrder productionOrder, ProductionOrderPayment productionOrderPayment, User user, DateTime currentDateTime)
        {
            CheckPossibilityToDeletePayment(productionOrderPayment, user);

            switch (productionOrderPayment.Type)
            {
                case ProductionOrderPaymentType.ProductionOrderProductionPayment:
                    productionOrder.DeletePayment(productionOrderPayment.As<ProductionOrderPayment>(), currentDateTime);
                    break;
                case ProductionOrderPaymentType.ProductionOrderTransportSheetPayment:
                    productionOrderPayment.As<ProductionOrderTransportSheetPayment>().TransportSheet.DeletePayment(productionOrderPayment.As<ProductionOrderTransportSheetPayment>(), currentDateTime);
                    break;
                case ProductionOrderPaymentType.ProductionOrderExtraExpensesSheetPayment:
                    productionOrderPayment.As<ProductionOrderExtraExpensesSheetPayment>().ExtraExpensesSheet.DeletePayment(productionOrderPayment.As<ProductionOrderExtraExpensesSheetPayment>(), currentDateTime);
                    break;
                case ProductionOrderPaymentType.ProductionOrderCustomsDeclarationPayment:
                    productionOrderPayment.As<ProductionOrderCustomsDeclarationPayment>().CustomsDeclaration.DeletePayment(productionOrderPayment.As<ProductionOrderCustomsDeclarationPayment>(), currentDateTime);
                    break;
                default:
                    throw new Exception("Неизвестное назначение оплаты.");
            };
        }

        private IList<Guid> GetPermittedProductionOrderIdList(User user, Permission permission)
        {
            /* Сейчас этот метод вызывается только для PermissionDistributionType.Personal и PermissionDistributionType.Teams.
             Так что остальные ветки switch реально не используются. Надо уточнить у Юры для чего они писались. */
            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<Guid>();

                case PermissionDistributionType.Personal:
                    var teamOrderId = user.Teams.SelectMany(x => x.ProductionOrders).Select(x => x.Id).Distinct();

                    return productionOrderRepository.Query<ProductionOrder>()
                        .Where(x => x.Curator.Id == user.Id)
                        .Select(x => x.Id)
                        .ToList<Guid>()
                        .Intersect(teamOrderId)
                        .Distinct()
                        .ToList();

                case PermissionDistributionType.Teams:
                    return user.Teams.SelectMany(x => x.ProductionOrders).Select(x => x.Id).Distinct().ToList();

                case PermissionDistributionType.All:
                    return productionOrderRepository.Query<ProductionOrder>().Select(x => x.Id).ToList<Guid>();

                default:
                    return null;
            }
        }

        public IEnumerable<ProductionOrder> GetFilteredList(object state, User user, ParameterString parameterString = null)
        {
            if (parameterString == null) parameterString = new ParameterString("");

            var type = user.GetPermissionDistributionType(Permission.ProductionOrder_List_Details);

            switch (type)
            {
                case PermissionDistributionType.None:
                    return new List<ProductionOrder>();

                case PermissionDistributionType.Personal:
                    parameterString.Add("Curator", ParameterStringItem.OperationType.Eq, user.Id.ToString());
                    RestrictByTeams(parameterString, user);
                    break;

                case PermissionDistributionType.Teams:
                    RestrictByTeams(parameterString, user);
                    break;

                case PermissionDistributionType.All:
                    break;
            }

            return productionOrderRepository.GetFilteredList(state, parameterString);

        }

        private void RestrictByTeams(ParameterString parameterString, User user)
        {
            var list = GetPermittedProductionOrderIdList(user, Permission.ProductionOrder_List_Details);

            if (parameterString.Keys.Contains("Id"))
            {
                if (parameterString["Id"].Operation == ParameterStringItem.OperationType.NotOneOf)
                {
                    foreach (var excludeId in parameterString["Id"].Value as List<Guid>)
                    {
                        list.Remove(excludeId);
                    }
                    parameterString.Delete("Id");
                }
            }

            // если список пуст - то добавляем несуществующее значение
            if (!list.Any()) { list.Add(Guid.Empty); }

            parameterString.Add("Id", ParameterStringItem.OperationType.OneOf);
            parameterString["Id"].Value = list;
        }

        /// <summary>
        /// Получение заказа по id с проверкой его существования и прав пользователя
        /// </summary>
        /// <param name="id">Код</param>
        public ProductionOrder CheckProductionOrderExistence(Guid id, User user)
        {
            var productionOrder = GetById(id, user);
            ValidationUtils.NotNull(productionOrder, "Заказ не найден. Возможно, он был удален.");

            return productionOrder;
        }

        /// <summary>
        /// Получение заказа по id с проверкой его существования
        /// </summary>
        /// <param name="id">Код</param>
        public ProductionOrder CheckProductionOrderExistence(Guid id, User user, Permission permission)
        {
            var productionOrder = GetById(id, user, permission);
            ValidationUtils.NotNull(productionOrder, "Заказ не найден. Возможно, он был удален.");

            return productionOrder;
        }

        /// <summary>
        /// Получение партии заказа по id с проверкой ее существования
        /// </summary>
        /// <param name="id">Код</param>
        public ProductionOrderBatch CheckProductionOrderBatchExistence(Guid id, User user)
        {
            var productionOrderBatch = GetProductionOrderBatchById(id);

            ValidationUtils.NotNull(productionOrderBatch, "Партия заказа не найдена. Возможно, она была удалена.");

            CheckPermissionToPerformOperation(productionOrderBatch.ProductionOrder, user, Permission.ProductionOrderBatch_Details);

            return productionOrderBatch;
        }

        /// <summary>
        /// Получение позиции партии заказа по id с проверкой ее существования
        /// </summary>
        /// <param name="productionOrderBatch">Партия заказа</param>
        /// <param name="id">Код позиции</param>
        public ProductionOrderBatchRow CheckProductionOrderBatchRowExistence(ProductionOrderBatch productionOrderBatch, Guid id)
        {
            ProductionOrderBatchRow row = productionOrderBatch.Rows.Where(x => x.Id == id).FirstOrDefault();
            ValidationUtils.NotNull(row, "Позиция партии заказа не найдена. Возможно, она была удалена.");

            return row;
        }

        /// <summary>
        /// Получение этапа партии заказа по id с проверкой его существования
        /// </summary>
        /// <param name="productionOrderBatch">Партия заказа</param>
        /// <param name="id">Код этапа</param>
        public ProductionOrderBatchStage CheckProductionOrderBatchStageExistence(ProductionOrderBatch productionOrderBatch, Guid id)
        {
            ProductionOrderBatchStage stage = productionOrderBatch.Stages.Where(x => x.Id == id).FirstOrDefault();
            ValidationUtils.NotNull(stage, "Этап не найден. Возможно, он был удален.");

            return stage;
        }

        /// <summary>
        /// Получение транспортного листа по id с проверкой его существования
        /// </summary>
        /// <param name="productionOrder">Заказ</param>
        /// <param name="id">Код транспортного листа</param>
        public ProductionOrderTransportSheet CheckProductionOrderTransportSheetExistence(ProductionOrder productionOrder, Guid id, User user)
        {
            CheckPermissionToPerformOperation(productionOrder, user, Permission.ProductionOrderTransportSheet_List_Details);

            ProductionOrderTransportSheet transportSheet = productionOrder.TransportSheets.Where(x => x.Id == id).FirstOrDefault();
            ValidationUtils.NotNull(transportSheet, "Транспортный лист не найден. Возможно, он был удален.");

            return transportSheet;
        }

        /// <summary>
        /// Получение листа дополнительных расходов по id с проверкой его существования
        /// </summary>
        /// <param name="productionOrder">Заказ</param>
        /// <param name="id">Код листа дополнительных расходов</param>
        public ProductionOrderExtraExpensesSheet CheckProductionOrderExtraExpensesSheetExistence(ProductionOrder productionOrder, Guid id, User user)
        {
            CheckPermissionToPerformOperation(productionOrder, user, Permission.ProductionOrderExtraExpensesSheet_List_Details);

            ProductionOrderExtraExpensesSheet extraExpensesSheet = productionOrder.ExtraExpensesSheets.Where(x => x.Id == id).FirstOrDefault();
            ValidationUtils.NotNull(extraExpensesSheet, "Лист дополнительных расходов не найден. Возможно, он был удален.");

            return extraExpensesSheet;
        }

        /// <summary>
        /// Получение таможенного листа по id с проверкой его существования
        /// </summary>
        /// <param name="productionOrder">Заказ</param>
        /// <param name="id">Код таможенного листа</param>
        public ProductionOrderCustomsDeclaration CheckProductionOrderCustomsDeclarationExistence(ProductionOrder productionOrder, Guid id, User user)
        {
            CheckPermissionToPerformOperation(productionOrder, user, Permission.ProductionOrderCustomsDeclaration_List_Details);

            ProductionOrderCustomsDeclaration customsDeclaration = productionOrder.CustomsDeclarations.Where(x => x.Id == id).FirstOrDefault();
            ValidationUtils.NotNull(customsDeclaration, "Таможенный лист не найден. Возможно, он был удален.");

            return customsDeclaration;
        }

        /// <summary>
        /// Получение оплаты по id с проверкой ее существования
        /// </summary>
        /// <param name="productionOrder">Заказ</param>
        /// <param name="paymentId">Код оплаты</param>
        public ProductionOrderPayment CheckProductionOrderPaymentExistence(ProductionOrder productionOrder, Guid id, User user)
        {
            CheckPermissionToPerformOperation(productionOrder, user, Permission.ProductionOrderPayment_List_Details);

            ProductionOrderPayment productionOrderPayment = productionOrder.Payments.Where(x => x.Id == id).FirstOrDefault();
            ValidationUtils.NotNull(productionOrderPayment, "Оплата не найдена. Возможно, она была удалена.");

            return productionOrderPayment;
        }

        public ProductionOrderPlannedPayment CheckProductionOrderPlannedPaymentExistence(Guid id, User user)
        {
            ProductionOrderPlannedPayment productionOrderPlannedPayment = productionOrderRepository.Query<ProductionOrderPlannedPayment>()
                .Where(x => x.Id == id).FirstOrDefault<ProductionOrderPlannedPayment>();
            ValidationUtils.NotNull(productionOrderPlannedPayment, "Планируемая оплата не найдена. Возможно, она была удалена.");

            CheckPermissionToPerformOperation(productionOrderPlannedPayment.ProductionOrder, user, Permission.ProductionOrder_List_Details);

            return productionOrderPlannedPayment;
        }

        /// <summary>
        /// Проверка названия заказа на уникальность
        /// </summary>
        /// <param name="name">Название заказа</param>
        /// <param name="id">Код текущего заказа</param>
        /// <returns>Результат проверки</returns>
        public bool IsNameUnique(string name, Guid id)
        {
            return productionOrderRepository.Query<ProductionOrder>().Where(x => x.Name == name && x.Id != id).Count() == 0;
        }

        /// <summary>
        /// Проверка, нет ли в партии заказа этапов с повторяющимися названиями
        /// </summary>
        /// <param name="productionOrderBatch"></param>
        private void CheckProductionOrderBatchStageNameUniqueness(ProductionOrderBatch productionOrderBatch)
        {
            foreach (var stage in productionOrderBatch.Stages)
            {
                foreach (var anotherStage in productionOrderBatch.Stages)
                {
                    ValidationUtils.Assert(stage.Name != anotherStage.Name || stage == anotherStage, "Названия этапов не должны повторяться.");
                }
            }
        }

        #endregion

        #region Работа с позициями

        public void AddRow(ProductionOrderBatch batch, ProductionOrderBatchRow batchRow, User user)
        {
            CheckPossibilityToCreateBatchRow(batch, user);

            batch.AddRow(batchRow);
        }

        public void DeleteRow(ProductionOrderBatch batch, ProductionOrderBatchRow batchRow, User user, DateTime currentDateTime)
        {
            CheckPossibilityToDeleteBatchRow(batch, user);

            batch.DeleteRow(batchRow, currentDateTime);
        }

        #endregion

        #region Работа со статусами

        /// <summary>
        /// Провести (перевести в статус "Утверждение")
        /// </summary>
        /// <param name="user">Пользователь</param>
        public void Accept(ProductionOrderBatch productionOrderBatch, User user, DateTime currentDateTime)
        {
            CheckPossibilityToAccept(productionOrderBatch, user);

            productionOrderBatch.Accept(user, currentDateTime);
        }

        public void CancelAcceptance(ProductionOrderBatch productionOrderBatch, User user)
        {
            CheckPossibilityToCancelAcceptance(productionOrderBatch, user);

            productionOrderBatch.CancelAcceptance();
        }

        /// <summary>
        /// Одобрить (перевести в статус "Готово" после нажатия кнопки "Готово")
        /// </summary>
        public void Approve(ProductionOrderBatch productionOrderBatch, User user, DateTime currentDateTime)
        {
            CheckPossibilityToApprove(productionOrderBatch, user);

            productionOrderBatch.Approve(user, currentDateTime);
        }

        /// <summary>
        /// Отменить готовность (перевести назад в статус "Утверждение" после нажатия кнопки "Отменить готовность")
        /// </summary>
        public void CancelApprovement(ProductionOrderBatch productionOrderBatch, User user)
        {
            CheckPossibilityToCancelApprovement(productionOrderBatch, user);

            productionOrderBatch.CancelApprovement();
        }

        /// <summary>
        /// Утвердить от имени действующего лица (нажатие кнопки "Утвердить: ..." при статусе "Утверждение")
        /// </summary>
        public void Approve(ProductionOrderBatch productionOrderBatch, User user, ProductionOrderApprovementActor actor, DateTime currentDateTime)
        {
            CheckPossibilityToApproveByActor(productionOrderBatch, actor, user);
            productionOrderBatch.Approve(user, actor, currentDateTime);
        }

        /// <summary>
        /// Отменить утверждение от имени действующего лица (нажатие кнопки "Отменить утверждение: ..." при статусе "Утверждение")
        /// </summary>
        /// <param name="productionOrderBatch"></param>
        /// <param name="actor"></param>
        public void CancelApprovement(ProductionOrderBatch productionOrderBatch, User user, ProductionOrderApprovementActor actor)
        {
            CheckPossibilityToCancelApprovementByActor(productionOrderBatch, actor, user);

            productionOrderBatch.CancelApprovement(actor);
        }

        #endregion

        #region Работа с этапами

        #region Переходы между этапами

        /// <summary>
        /// Перевести партию заказа на следующий этап
        /// </summary>
        /// <param name="productionOrderBatch">Партия заказа</param>
        /// <param name="user">Пользователь</param>
        /// <returns</returns>
        public void MoveToNextStage(ProductionOrderBatch productionOrderBatch, User user, DateTime currentDateTime)
        {
            CheckPossibilityToMoveToNextStage(productionOrderBatch, user);

            productionOrderBatch.MoveToNextStage(currentDateTime);
        }

        /// <summary>
        /// Перевести партию заказа на предыдущий этап
        /// </summary>
        /// <param name="productionOrderBatch">Партия заказа</param>
        /// <param name="user">Пользователь</param>
        public void MoveToPreviousStage(ProductionOrderBatch productionOrderBatch, User user)
        {
            var currentDateTime = DateTimeUtils.GetCurrentDateTime();
            
            // Переводит партию заказа из состояния ("Открыто", "Закрыто"?) в состояние "Открыто"
            // Переводит заказ из состояния ("Открыто", "Закрыто"?) в состояние "Открыто" - если из "Закрыто", сбрасываем ЗЦ по всем приходам

            CheckPossibilityToMoveToPreviousStage(productionOrderBatch, user);

            bool isClosed = productionOrderBatch.ProductionOrder.IsClosed;

            // Метод MoveToPreviousStage может изменить свойство IsClosed, поэтому мы сохранили его значение
            productionOrderBatch.MoveToPreviousStage();

            // Удаляем накладную, если на данном этапе невозможно ее иметь (возможно, ее закупочные цены были сброшены в коде выше)
            if (productionOrderBatch.ReceiptWaybill != null && !IsPossibilityToHaveReceiptWaybill(productionOrderBatch))
            {
                receiptWaybillService.Delete(productionOrderBatch.ReceiptWaybill, currentDateTime, user);
            }
        }

        /// <summary>
        /// Перевести партию заказа на этап "Неуспешное закрытие"
        /// </summary>
        /// <param name="productionOrderBatch">Партия заказа</param>
        /// <param name="user">Пользователь</param>
        public void MoveToUnsuccessfulClosingStage(ProductionOrderBatch productionOrderBatch, User user, DateTime currentDateTime)
        {
            CheckPossibilityToMoveToUnsuccessfulClosingStage(productionOrderBatch, user);

            productionOrderBatch.MoveToUnsuccessfulClosingStage(currentDateTime);

            // Удаляем накладную, если есть возможность. Теперь при расчете закупочных цен она не будет учитываться и влиять на показатели
            // Из-за этого вызова внутри Delete в репозитории должен стоять Flush, т.к. иначе вычисление ЗЦ после этого получит старые данные.
            // Правда, вероятно, что показатели, затрагиваемые при отмене проводки, не пересекаются с показателями, затрагиваемыми при расчете ЗЦ
            if (productionOrderBatch.ReceiptWaybill != null)
            {
                receiptWaybillService.Delete(productionOrderBatch.ReceiptWaybill, currentDateTime, user);
            }

        }

        #endregion

        #region Редактирование этапов

        public void ClearCustomStages(ProductionOrderBatch productionOrderBatch, User user)
        {
            CheckPossibilityToEditStages(productionOrderBatch, user);

            productionOrderBatch.ClearCustomStages();
        }

        public void LoadStagesFromTemplate(ProductionOrderBatch productionOrderBatch, ProductionOrderBatchLifeCycleTemplate productionOrderBatchLifeCycleTemplate, User user)
        {
            CheckPossibilityToEditStages(productionOrderBatch, user);

            productionOrderBatch.LoadStagesFromTemplate(productionOrderBatchLifeCycleTemplate);
        }

        /// <summary>
        /// Заполнить список этапов всеми типами этапов, кроме типа "Закрыто".
        /// </summary>
        public IEnumerable<ProductionOrderBatchStageType> GetProductionOrderBatchStageTypeList()
        {
            var list = new List<ProductionOrderBatchStageType>();
            foreach (ProductionOrderBatchStageType item in Enum.GetValues(typeof(ProductionOrderBatchStageType)))
            {
                list.Add(item);
            }

            list.Remove(ProductionOrderBatchStageType.Closed);

            return list;
        }

        #endregion

        #endregion

        #region Работа с партиями

        /// <summary>
        /// Удаление партии заказа
        /// </summary>
        /// <param name="batch">Партия</param>
        /// <param name="user">Пользователь</param>
        /// <param name="currentDateTime">Время</param>
        public void DeleteBatch(ProductionOrderBatch batch, User user, DateTime currentDateTime)
        {
            CheckPossibilityToDeleteBatch(batch, user);

            var productionOrder = batch.ProductionOrder;
            productionOrder.DeleteBatch(batch, currentDateTime);
        }

        /// <summary>
        /// Разбор строки с информацией о разделяемых позициях
        /// </summary>
        /// <param name="splitInfo">Строка</param>
        public IDictionary<Guid, decimal> ParseSplitInfo(string splitInfo)
        {
            var result = new Dictionary<Guid, decimal>();

            var splitInfoList = splitInfo.Split(';');
            for (int i = 0; i < splitInfoList.Length - 1; i++)
            {
                string rowInfo = splitInfoList[i];

                var rowInfoParams = rowInfo.Split(new char[] { '=' });

                Guid productionOrderBatchRowId = ValidationUtils.TryGetNotEmptyGuid(rowInfoParams[0]);
                decimal splittedCount = ValidationUtils.TryGetDecimal(rowInfoParams[1]);

                result.Add(productionOrderBatchRowId, splittedCount);
            }

            return result;
        }

        /// <summary>
        /// Разделение партии заказа
        /// </summary>
        /// <param name="productionOrderBatch">Разделяемая партия заказа</param>
        /// <param name="splitInfo">Информация с количеством разделяемых товаров по позициям</param>
        public Guid SplitBatch(ProductionOrderBatch productionOrderBatch, IDictionary<Guid, decimal> splitInfo, User user, DateTime currentDateTime)
        {
            CheckPossibilityToSplitBatch(productionOrderBatch, user);

            //Если название предка слишком длинное, то обрезаем его, чтобы не выйти за пределы при добавлении к имени родителя строчки "Партия отделенная от"
            var parentBatchName = productionOrderBatch.Name;
            if (parentBatchName.Length > 170)
                parentBatchName = parentBatchName.Remove(170);

            // Создаем новую, пустую партию заказа на основе разделяемой и добавляем ее в заказ
            //Дату создания указываем такую же как и у партии-предка
            var newProductionOrderBatch = new ProductionOrderBatch(productionOrderBatch, user,
                                                productionOrderBatch.CreationDate, String.Format("Партия отделенная от «{0}»", parentBatchName));

            productionOrderBatch.ProductionOrder.AddBatch(newProductionOrderBatch);

            // Получаем список позиций старой партии заказа, которые переходят в создаваемую партию
            var productionOrderBatchRowList = productionOrderBatch.Rows.Where(x => splitInfo.ContainsKey(x.Id));
            ValidationUtils.Assert(productionOrderBatchRowList.Count() == splitInfo.Count, "Информация о разделении партии содержит повторяющиеся элементы.");

            // Проходим все позиции, отсортированные так, чтобы сохранять порядок (в том числе при разделении партии, созданной разделением)
            int counter = 0;
            foreach (var row in productionOrderBatchRowList.OrderBy(x => x.OrdinalNumber).ThenBy(x => x.CreationDate))
            {
                decimal splittedCount = splitInfo[row.Id];

                if (splittedCount > 0M)
                {
                    ValidationUtils.Assert(splittedCount <= row.Count,
                        String.Format("Невозможно разделить партию по товару с кодом {0}: задано слишком большое количество товара к разделению - {1} (больше количества товара в партии - {2}).",
                            row.Article.Id.ToString(), splittedCount, row.Count));

                    // row.Count не может быть равен 0: splittedCount > 0, и row.Count >= splittedCount
                    decimal newProductionOrderBatchRowCostInCurrency = Math.Round(splittedCount * row.ProductionOrderBatchRowCostInCurrency / row.Count, 6);
                    var newRow = new ProductionOrderBatchRow(row, newProductionOrderBatchRowCostInCurrency, splittedCount, ++counter);
                    newProductionOrderBatch.AddSplittedRow(newRow);

                    // Уменьшаем количество товара и сумму в валюте в старой позиции, удаляем ее, если товара в ней не осталось
                    row.Count -= splittedCount;
                    row.ProductionOrderBatchRowCostInCurrency -= newProductionOrderBatchRowCostInCurrency;
                    if (row.Count == 0)
                    {
                        productionOrderBatch.DeleteSplittedRow(row, currentDateTime);
                    }
                }
            }

            ValidationUtils.Assert(productionOrderBatch.RowCount > 0, "Невозможно разделить партию: задано нулевое количество остающегося товара.");
            ValidationUtils.Assert(newProductionOrderBatch.RowCount > 0, "Невозможно разделить партию: задано нулевое количество разделяемого товара.");
            var guid = SaveProductionOrderBatch(newProductionOrderBatch);

            return guid;
        }

        #endregion

        #region Расчет показателей

        /// <summary>
        /// Расчет основных показателей для заказа
        /// </summary>
        /// <param name="productionOrder">Заказ</param>
        /// <param name="calculatePaymentIndicators">Рассчитывать ли показатели по оплатам</param>
        /// <param name="calculateAccountingPriceIndicators">Рассчитывать ли показатели по учетным ценам</param>
        /// <param name="includeUnsuccessfullyClosedBatchesForCustomsExpenses">Включать ли в расчет показателей таможни неуспешно закрытые партии</param>
        /// <returns>Объект с показателями</returns>
        public ProductionOrderMainIndicators CalculateMainIndicators(ProductionOrder productionOrder, bool calculateActualCost = false,
            bool calculatePaymentIndicators = false, bool calculatePaymentPercent = false, bool calculatePlannedExpenses = false,
            bool calculateAccountingPriceIndicators = false, bool calculatePlannedPaymentIndicators = false,
            bool includeUnsuccessfullyClosedBatchesForCustomsExpenses = true)
        {
            if (calculatePaymentPercent)
            {
                calculateActualCost = true; calculatePaymentIndicators = true;
            }

            if (calculateAccountingPriceIndicators)
            {
                calculateActualCost = true;
            }

            var result = new ProductionOrderMainIndicators();

            // Фактическая стоимость, фактическая стоимость в базовой валюте (по курсам документов), сумма оплат в валюте заказа
            Currency currencyFrom, currencyTo;
            CurrencyRate currencyRateFrom, currencyRateTo;

            if (calculateActualCost)
            {
                result.ActualCostSumInCurrency = 0M; result.ActualCostSumInBaseCurrency = 0M;
                var productionOrderProductionCostInCurrency = productionOrder.ProductionOrderProductionCostInCurrency;
                result.ActualCostSumInCurrency += productionOrderProductionCostInCurrency;
                currencyService.GetCurrencyAndCurrencyRate(productionOrder, out currencyFrom, out currencyRateFrom);
                decimal? productionOrderProductionCostInBaseCurrency =
                    currencyService.CalculateSumInBaseCurrency(currencyFrom, currencyRateFrom, productionOrderProductionCostInCurrency);
                result.ActualCostSumInBaseCurrency += productionOrderProductionCostInBaseCurrency ?? 0M;
            }
            if (calculatePaymentIndicators)
            {
                result.PaymentSumInCurrency = 0M;
                result.PaymentSumInCurrency += productionOrder.ProductionOrderProductionPaymentSumInCurrency;
            }

            if (calculateActualCost)
                result.ActualTransportationCostSumInBaseCurrency = 0M;
            foreach (var transportSheet in productionOrder.TransportSheets)
            {
                currencyService.GetCurrencyAndCurrencyRate(transportSheet, out currencyFrom, out currencyRateFrom);
                currencyService.GetCurrencyAndCurrencyRate(productionOrder, out currencyTo, out currencyRateTo);

                if (calculateActualCost)
                {
                    decimal? transportSheetCostInProductionOrderCurrency =
                        currencyService.CalculateSumInCurrency(currencyFrom, currencyRateFrom, transportSheet.CostInCurrency, currencyTo, currencyRateTo);
                    result.ActualCostSumInCurrency += transportSheetCostInProductionOrderCurrency ?? 0M;

                    decimal? transportSheetCostInBaseCurrency = currencyService.CalculateSumInBaseCurrency(currencyFrom, currencyRateFrom, transportSheet.CostInCurrency);
                    result.ActualCostSumInBaseCurrency += transportSheetCostInBaseCurrency ?? 0M;
                    result.ActualTransportationCostSumInBaseCurrency += transportSheetCostInBaseCurrency ?? 0M;
                }

                if (calculatePaymentIndicators)
                {
                    decimal? transportSheetPaymentSumInProductionOrderCurrency =
                        currencyService.CalculateSumInCurrency(currencyFrom, currencyRateFrom, transportSheet.PaymentSumInCurrency, currencyTo, currencyRateTo);
                    result.PaymentSumInCurrency += transportSheetPaymentSumInProductionOrderCurrency ?? 0M;
                }
            }

            if (calculateActualCost)
                result.ActualExtraExpensesCostSumInBaseCurrency = 0M;
            foreach (var extraExpensesSheet in productionOrder.ExtraExpensesSheets)
            {
                currencyService.GetCurrencyAndCurrencyRate(extraExpensesSheet, out currencyFrom, out currencyRateFrom);
                currencyService.GetCurrencyAndCurrencyRate(productionOrder, out currencyTo, out currencyRateTo);

                if (calculateActualCost)
                {
                    decimal? extraExpensesSheetCostInProductionOrderCurrency =
                    currencyService.CalculateSumInCurrency(currencyFrom, currencyRateFrom, extraExpensesSheet.CostInCurrency, currencyTo, currencyRateTo);
                    result.ActualCostSumInCurrency += extraExpensesSheetCostInProductionOrderCurrency ?? 0M;

                    decimal? extraExpensesSheetCostInBaseCurrency = currencyService.CalculateSumInBaseCurrency(currencyFrom, currencyRateFrom, extraExpensesSheet.CostInCurrency);
                    result.ActualCostSumInBaseCurrency += extraExpensesSheetCostInBaseCurrency ?? 0M;
                    result.ActualExtraExpensesCostSumInBaseCurrency += extraExpensesSheetCostInBaseCurrency ?? 0M;
                }

                if (calculatePaymentIndicators)
                {
                    decimal? extraExpensesSheetPaymentSumInProductionOrderCurrency =
                        currencyService.CalculateSumInCurrency(currencyFrom, currencyRateFrom, extraExpensesSheet.PaymentSumInCurrency, currencyTo, currencyRateTo);
                    result.PaymentSumInCurrency += extraExpensesSheetPaymentSumInProductionOrderCurrency ?? 0M;
                }
            }

            if (calculateActualCost)
            {
                result.ActualCustomsExpensesCostSumInBaseCurrency = 0M;
                result.ActualImportCustomsDutiesSumInBaseCurrency = 0M;
                result.ActualExportCustomsDutiesSumInBaseCurrency = 0M;
                result.ActualValueAddedTaxSumInBaseCurrency = 0M;
                result.ActualExciseSumInBaseCurrency = 0M;
                result.ActualCustomsFeesSumInBaseCurrency = 0M;
                result.ActualCustomsValueCorrectionSumInBaseCurrency = 0M;
            }

            foreach (var customsDeclaration in productionOrder.CustomsDeclarations)
            {
                currencyService.GetCurrencyAndCurrencyRate(productionOrder, out currencyTo, out currencyRateTo);

                if (calculateActualCost)
                {
                    decimal? customsDeclarationCostInProductionOrderCurrency =
                        currencyService.CalculateSumInCurrency(currencyService.GetCurrentBaseCurrency(), null, customsDeclaration.Sum, currencyTo, currencyRateTo);
                    result.ActualCostSumInCurrency += customsDeclarationCostInProductionOrderCurrency ?? 0M;

                    result.ActualCostSumInBaseCurrency += customsDeclaration.Sum;
                    result.ActualCustomsExpensesCostSumInBaseCurrency += customsDeclaration.Sum;
                    result.ActualImportCustomsDutiesSumInBaseCurrency += customsDeclaration.ImportCustomsDutiesSum;
                    result.ActualExportCustomsDutiesSumInBaseCurrency += customsDeclaration.ExportCustomsDutiesSum;
                    result.ActualValueAddedTaxSumInBaseCurrency += customsDeclaration.ValueAddedTaxSum;
                    result.ActualExciseSumInBaseCurrency += customsDeclaration.ExciseSum;
                    result.ActualCustomsFeesSumInBaseCurrency += customsDeclaration.CustomsFeesSum;
                    result.ActualCustomsValueCorrectionSumInBaseCurrency += customsDeclaration.CustomsValueCorrection;
                }

                if (calculatePaymentIndicators)
                {
                    decimal? customsDeclarationPaymentSumInProductionOrderCurrency =
                        currencyService.CalculateSumInCurrency(currencyService.GetCurrentBaseCurrency(), null, customsDeclaration.PaymentSum, currencyTo, currencyRateTo);
                    result.PaymentSumInCurrency += customsDeclarationPaymentSumInProductionOrderCurrency ?? 0M;
                }
            }

            // Сумма оплат в базовой валюте (по каждой оплате курс считается отдельно)
            if (calculatePaymentIndicators)
            {
                result.PaymentSumInBaseCurrency = 0M; result.PaymentProductionSumInCurrency = 0M; result.PaymentProductionSumInBaseCurrency = 0M;
                result.PaymentTransportationSumInBaseCurrency = 0M;
                result.PaymentExtraExpensesSumInBaseCurrency = 0M; result.PaymentCustomsExpensesSumInBaseCurrency = 0M;
                result.PaymentImportCustomsDutiesSumInBaseCurrency = 0M; result.PaymentExportCustomsDutiesSumInBaseCurrency = 0M;
                result.PaymentValueAddedTaxSumInBaseCurrency = 0M; result.PaymentExciseSumInBaseCurrency = 0M;
                result.PaymentCustomsFeesSumInBaseCurrency = 0M; result.PaymentCustomsValueCorrectionSumInBaseCurrency = 0M;
                foreach (var payment in productionOrder.Payments)
                {
                    currencyService.GetCurrencyAndCurrencyRate(payment, out currencyFrom, out currencyRateFrom);
                    var paymentPartialSumInBaseCurrency = currencyService.CalculateSumInBaseCurrency(currencyFrom, currencyRateFrom, payment.SumInCurrency);
                    result.PaymentSumInBaseCurrency += paymentPartialSumInBaseCurrency ?? 0M;
                    switch (payment.Type)
                    {
                        case ProductionOrderPaymentType.ProductionOrderProductionPayment:
                            result.PaymentProductionSumInCurrency += payment.SumInCurrency;
                            result.PaymentProductionSumInBaseCurrency += paymentPartialSumInBaseCurrency ?? 0M;
                            break;
                        case ProductionOrderPaymentType.ProductionOrderTransportSheetPayment:
                            result.PaymentTransportationSumInBaseCurrency += paymentPartialSumInBaseCurrency ?? 0M;
                            break;
                        case ProductionOrderPaymentType.ProductionOrderExtraExpensesSheetPayment:
                            result.PaymentExtraExpensesSumInBaseCurrency += paymentPartialSumInBaseCurrency ?? 0M;
                            break;
                        case ProductionOrderPaymentType.ProductionOrderCustomsDeclarationPayment:
                            result.PaymentCustomsExpensesSumInBaseCurrency += paymentPartialSumInBaseCurrency ?? 0M;
                            var customsDeclaration = payment.As<ProductionOrderCustomsDeclarationPayment>().CustomsDeclaration;
                            ValidationUtils.NotNullOrDefault(customsDeclaration.Sum, "Сумма по таможенному листу не может быть равна 0.");
                            result.PaymentImportCustomsDutiesSumInBaseCurrency += (paymentPartialSumInBaseCurrency ?? 0M) *
                                customsDeclaration.ImportCustomsDutiesSum / customsDeclaration.Sum;
                            result.PaymentExportCustomsDutiesSumInBaseCurrency += (paymentPartialSumInBaseCurrency ?? 0M) *
                                customsDeclaration.ExportCustomsDutiesSum / customsDeclaration.Sum;
                            result.PaymentValueAddedTaxSumInBaseCurrency += (paymentPartialSumInBaseCurrency ?? 0M) *
                                customsDeclaration.ValueAddedTaxSum / customsDeclaration.Sum;
                            result.PaymentExciseSumInBaseCurrency += (paymentPartialSumInBaseCurrency ?? 0M) *
                                customsDeclaration.ExciseSum / customsDeclaration.Sum;
                            result.PaymentCustomsFeesSumInBaseCurrency += (paymentPartialSumInBaseCurrency ?? 0M) *
                                customsDeclaration.CustomsFeesSum / customsDeclaration.Sum;
                            result.PaymentCustomsValueCorrectionSumInBaseCurrency += (paymentPartialSumInBaseCurrency ?? 0M) *
                                customsDeclaration.CustomsValueCorrection / customsDeclaration.Sum;
                            break;
                        default:
                            throw new Exception("Неизвестное назначение оплаты.");
                    };
                }
            }

            // Процент оплаты
            if (calculatePaymentPercent)
                result.PaymentPercent = result.ActualCostSumInCurrency != 0M ? result.PaymentSumInCurrency * 100M / result.ActualCostSumInCurrency : 0M;

            // Плановые затраты в базовой валюте
            if (calculatePlannedExpenses)
            {
                currencyService.GetCurrencyAndCurrencyRate(productionOrder, out currencyFrom, out currencyRateFrom);
                result.PlannedExpensesSumInBaseCurrency = currencyService.CalculateSumInBaseCurrency(currencyFrom, currencyRateFrom, productionOrder.ProductionOrderPlannedExpensesSumInCurrency);
                result.PlannedProductionExpensesInBaseCurrency = currencyService.CalculateSumInBaseCurrency(currencyFrom, currencyRateFrom, productionOrder.ProductionOrderPlannedProductionExpensesInCurrency);
                result.PlannedTransportationExpensesInBaseCurrency = currencyService.CalculateSumInBaseCurrency(currencyFrom, currencyRateFrom, productionOrder.ProductionOrderPlannedTransportationExpensesInCurrency);
                result.PlannedExtraExpensesInBaseCurrency = currencyService.CalculateSumInBaseCurrency(currencyFrom, currencyRateFrom, productionOrder.ProductionOrderPlannedExtraExpensesInCurrency);
                result.PlannedCustomsExpensesInBaseCurrency = currencyService.CalculateSumInBaseCurrency(currencyFrom, currencyRateFrom, productionOrder.ProductionOrderPlannedCustomsExpensesInCurrency);
            }

            // План оплат по заказу
            if (calculatePlannedPaymentIndicators)
            {
                result.PlannedProductionPaymentsInBaseCurrency = result.PlannedTransportationPaymentsInBaseCurrency =
                    result.PlannedExtraExpensesPaymentsInBaseCurrency = result.PlannedCustomsPaymentsInBaseCurrency = 0M;
                foreach (var plannedPayment in productionOrder.PlannedPayments)
                {
                    decimal sumInBaseCurrency = currencyService.CalculateSumInBaseCurrency(
                        plannedPayment.Currency, plannedPayment.CurrencyRate, plannedPayment.SumInCurrency) ?? 0M;
                    switch (plannedPayment.PaymentType)
                    {
                        case ProductionOrderPaymentType.ProductionOrderProductionPayment:
                            result.PlannedProductionPaymentsInBaseCurrency += sumInBaseCurrency;
                            break;
                        case ProductionOrderPaymentType.ProductionOrderTransportSheetPayment:
                            result.PlannedTransportationPaymentsInBaseCurrency += sumInBaseCurrency;
                            break;
                        case ProductionOrderPaymentType.ProductionOrderExtraExpensesSheetPayment:
                            result.PlannedExtraExpensesPaymentsInBaseCurrency += sumInBaseCurrency;
                            break;
                        case ProductionOrderPaymentType.ProductionOrderCustomsDeclarationPayment:
                            result.PlannedCustomsPaymentsInBaseCurrency += sumInBaseCurrency;
                            break;
                        default:
                            throw new Exception("Неизвестное назначение оплаты.");
                    }
                }
            }

            // Сумма в учетных ценах и прибыль
            if (calculateAccountingPriceIndicators)
            {
                result.AccountingPriceSum = productionOrder.Batches.Sum(x => CalculateAccountingPriceSum(x));
                result.MarkupPendingSum = result.AccountingPriceSum - result.ActualCostSumInBaseCurrency;
            }

            return result;
        }

        /// <summary>
        /// Расчет суммы в учетных ценах для партии заказа
        /// </summary>
        /// <param name="batch">Партия</param>
        /// <returns></returns>
        public decimal CalculateAccountingPriceSum(ProductionOrderBatch batch)
        {
            decimal accountingPriceSum = 0M;

            var articleSubquery = productionOrderBatchRepository.SubQuery<ProductionOrderBatchRow>()
                .Where(x => x.Batch.Id == batch.Id)
                .Select(x => x.Article.Id);

            var accountingPriceList = articlePriceService.GetAccountingPrice(batch.ProductionOrder.Storage.Id, articleSubquery);

            foreach (var row in batch.Rows)
            {
                decimal? accountingPrice = accountingPriceList[row.Article.Id];
                accountingPriceSum += (accountingPrice ?? 0M) * row.Count;
            }

            return accountingPriceSum;
        }

        /// <summary>
        /// Расчет суммы оплат в базовой валюте по данной планируемой оплате
        /// </summary>
        /// <param name="plannedPayment">Планируемая оплата</param>
        public void CalculatePlannedPaymentIndicators(ProductionOrderPlannedPayment plannedPayment, out decimal sumInCurrency, out decimal sumInBaseCurrency)
        {
            sumInCurrency = sumInBaseCurrency = 0M;
            Currency paymentCurrency;
            CurrencyRate paymentCurrencyRate;

            foreach (var payment in plannedPayment.Payments)
            {
                currencyService.GetCurrencyAndCurrencyRate(payment, out paymentCurrency, out paymentCurrencyRate);

                sumInCurrency += currencyService.CalculateSumInCurrency(paymentCurrency, paymentCurrencyRate, payment.SumInCurrency,
                    plannedPayment.Currency, plannedPayment.CurrencyRate) ?? 0M;

                sumInBaseCurrency += currencyService.CalculateSumInBaseCurrency(paymentCurrency, paymentCurrencyRate, payment.SumInCurrency) ?? 0M;
            }
        }

        #endregion

        #region Расчет себестоимости

        /// <summary>
        /// Расчет себестоимости по заказу в целом
        /// </summary>
        public virtual ProductionOrderBatchArticlePrimeCost CalculateProductionOrderBatchArticlePrimeCost(ProductionOrder productionOrder,
            ProductionOrderArticlePrimeCostCalculationType articlePrimeCostCalculationType, bool divideCustomsExpenses, bool showArticleVolumeAndWeight,
            ProductionOrderArticleTransportingPrimeCostCalculationType articleTransportingPrimeCostCalculationType, bool includeUnsuccessfullyClosedBatches,
            bool includeUnapprovedBatches)
        {
            var result = new ProductionOrderBatchArticlePrimeCost();

            if (articlePrimeCostCalculationType == ProductionOrderArticlePrimeCostCalculationType.PlannedExpenses)
                divideCustomsExpenses = false;

            Currency productionOrderCurrency; CurrencyRate productionOrderCurrencyRate;
            currencyService.GetCurrencyAndCurrencyRate(productionOrder, out productionOrderCurrency, out productionOrderCurrencyRate);

            //Выбираем партии: успешно закрытые и  подготовленные но незакрытые всегда, неуспешно закрытые и неподготовленные в зависимости от настроек 
            var batchList = productionOrder.Batches.Where(x => (!x.IsClosed || x.IsClosedSuccessfully || includeUnsuccessfullyClosedBatches) &&
                (x.IsApprovedState ||  includeUnapprovedBatches)).ToList();

            var indicators = CalculateMainIndicators(productionOrder, calculateActualCost: true, calculatePaymentIndicators: true, calculatePlannedExpenses: true,
                includeUnsuccessfullyClosedBatchesForCustomsExpenses: includeUnsuccessfullyClosedBatches);

            // I. Фактические показатели и показатели, рассчитанные по оплатам (для всего заказа)
            // 1. Стоимость производства заказа в валюте и рублях
            var productionOrderProductionCostInCurrency = batchList.Sum(x => x.ProductionOrderBatchProductionCostInCurrency);
            ValidationUtils.NotNullOrDefault(productionOrderProductionCostInCurrency, "Фактическая стоимость производства заказа в валюте не может быть равна 0.");
            result.ProductionOrderBatchProductionCostInCurrency.ActualValue = productionOrderProductionCostInCurrency;
            result.ProductionOrderBatchProductionCostInCurrency.PaymentValue = indicators.PaymentProductionSumInCurrency;
            var productionOrderBatchProductionCostInBaseCurrency = currencyService.CalculateSumInBaseCurrency(productionOrderCurrency, productionOrderCurrencyRate,
                productionOrderProductionCostInCurrency);
            ValidationUtils.NotNull(productionOrderBatchProductionCostInBaseCurrency, "Не задан курс валюты заказа.");
            result.ProductionOrderBatchProductionCostInBaseCurrency.ActualValue = productionOrderBatchProductionCostInBaseCurrency.Value;
            result.ProductionOrderBatchProductionCostInBaseCurrency.PaymentValue = indicators.PaymentProductionSumInBaseCurrency;

            // 3. Стоимость транспортировки заказа
            // 4. Стоимость таможенных затрат заказа
            // 5. Стоимость дополнительных расходов заказа
            result.ProductionOrderBatchTransportationCostInBaseCurrency.ActualValue = indicators.ActualTransportationCostSumInBaseCurrency;
            result.ProductionOrderBatchTransportationCostInBaseCurrency.PaymentValue = indicators.PaymentTransportationSumInBaseCurrency;
            result.ProductionOrderBatchCustomsExpensesCostSum.ActualValue = indicators.ActualCustomsExpensesCostSumInBaseCurrency;
            result.ProductionOrderBatchCustomsExpensesCostSum.PaymentValue = indicators.PaymentCustomsExpensesSumInBaseCurrency;
            if (divideCustomsExpenses)
            {
                result.ProductionOrderBatchImportCustomsDutiesSum.ActualValue = indicators.ActualImportCustomsDutiesSumInBaseCurrency;
                result.ProductionOrderBatchImportCustomsDutiesSum.PaymentValue = indicators.PaymentImportCustomsDutiesSumInBaseCurrency;
                result.ProductionOrderBatchExportCustomsDutiesSum.ActualValue = indicators.ActualExportCustomsDutiesSumInBaseCurrency;
                result.ProductionOrderBatchExportCustomsDutiesSum.PaymentValue = indicators.PaymentExportCustomsDutiesSumInBaseCurrency;
                result.ProductionOrderBatchValueAddedTaxSum.ActualValue = indicators.ActualValueAddedTaxSumInBaseCurrency;
                result.ProductionOrderBatchValueAddedTaxSum.PaymentValue = indicators.PaymentValueAddedTaxSumInBaseCurrency;
                result.ProductionOrderBatchExciseSum.ActualValue = indicators.ActualExciseSumInBaseCurrency;
                result.ProductionOrderBatchExciseSum.PaymentValue = indicators.PaymentExciseSumInBaseCurrency;
                result.ProductionOrderBatchCustomsFeesSum.ActualValue = indicators.ActualCustomsFeesSumInBaseCurrency;
                result.ProductionOrderBatchCustomsFeesSum.PaymentValue = indicators.PaymentCustomsFeesSumInBaseCurrency;
                result.ProductionOrderBatchCustomsValueCorrection.ActualValue = indicators.ActualCustomsValueCorrectionSumInBaseCurrency;
                result.ProductionOrderBatchCustomsValueCorrection.PaymentValue = indicators.PaymentCustomsValueCorrectionSumInBaseCurrency;
            }
            result.ProductionOrderBatchExtraExpensesSumInBaseCurrency.ActualValue = indicators.ActualExtraExpensesCostSumInBaseCurrency;
            result.ProductionOrderBatchExtraExpensesSumInBaseCurrency.PaymentValue = indicators.PaymentExtraExpensesSumInBaseCurrency;

            // IIa. Плановые показатели
            result.ProductionOrderPlannedProductionExpensesInCurrency = productionOrder.ProductionOrderPlannedProductionExpensesInCurrency;
            result.ProductionOrderPlannedProductionExpensesInBaseCurrency = indicators.PlannedProductionExpensesInBaseCurrency;
            result.ProductionOrderPlannedTransportationExpensesInBaseCurrency = indicators.PlannedTransportationExpensesInBaseCurrency;
            result.ProductionOrderPlannedCustomsExpensesInBaseCurrency = indicators.PlannedCustomsExpensesInBaseCurrency;
            result.ProductionOrderPlannedExtraExpensesInBaseCurrency = indicators.PlannedExtraExpensesInBaseCurrency;

            // 1. Стоимость производства заказа в валюте и рублях. При расчете "по плановой стоимости" реально берется фактическая стоимость
            // У производства при расчете "по плановой стоимости" реально берется фактическая стоимость
            if (articlePrimeCostCalculationType != ProductionOrderArticlePrimeCostCalculationType.PaymentSum)
            { // Расчет по фактической стоимости
                result.ProductionOrderBatchProductionCostInCurrency.CurrentValue = result.ProductionOrderBatchProductionCostInCurrency.ActualValue;
                result.ProductionOrderBatchProductionCostInBaseCurrency.CurrentValue = result.ProductionOrderBatchProductionCostInBaseCurrency.ActualValue;
            }
            else
            { // Расчет по величине оплат
                result.ProductionOrderBatchProductionCostInCurrency.CurrentValue = result.ProductionOrderBatchProductionCostInCurrency.PaymentValue;
                result.ProductionOrderBatchProductionCostInBaseCurrency.CurrentValue = result.ProductionOrderBatchProductionCostInBaseCurrency.PaymentValue;
            }

            // 2. Вес и объем заказа
            var productionOrderVolume = batchList.Sum(x => x.Volume);
            var productionOrderWeight = batchList.Sum(x => x.Weight);
            result.Volume = productionOrderVolume;
            result.Weight = productionOrderWeight;

            // 3. Стоимость транспортировки заказа
            // 4. Стоимость таможенных затрат заказа
            // 5. Стоимость дополнительных расходов заказа
            switch (articlePrimeCostCalculationType)
            {
                case ProductionOrderArticlePrimeCostCalculationType.PlannedExpenses:
                    result.ProductionOrderBatchTransportationCostInBaseCurrency.CurrentValue = result.ProductionOrderPlannedTransportationExpensesInBaseCurrency ?? 0M;
                    result.ProductionOrderBatchCustomsExpensesCostSum.CurrentValue = result.ProductionOrderPlannedCustomsExpensesInBaseCurrency ?? 0M;
                    result.ProductionOrderBatchExtraExpensesSumInBaseCurrency.CurrentValue = result.ProductionOrderPlannedExtraExpensesInBaseCurrency ?? 0M;
                    break;
                case ProductionOrderArticlePrimeCostCalculationType.ActualExpenses:
                    result.ProductionOrderBatchTransportationCostInBaseCurrency.CurrentValue = result.ProductionOrderBatchTransportationCostInBaseCurrency.ActualValue;
                    result.ProductionOrderBatchCustomsExpensesCostSum.CurrentValue = result.ProductionOrderBatchCustomsExpensesCostSum.ActualValue;
                    if (divideCustomsExpenses)
                    {
                        result.ProductionOrderBatchImportCustomsDutiesSum.CurrentValue = result.ProductionOrderBatchImportCustomsDutiesSum.ActualValue;
                        result.ProductionOrderBatchExportCustomsDutiesSum.CurrentValue = result.ProductionOrderBatchExportCustomsDutiesSum.ActualValue;
                        result.ProductionOrderBatchValueAddedTaxSum.CurrentValue = result.ProductionOrderBatchValueAddedTaxSum.ActualValue;
                        result.ProductionOrderBatchExciseSum.CurrentValue = result.ProductionOrderBatchExciseSum.ActualValue;
                        result.ProductionOrderBatchCustomsFeesSum.CurrentValue = result.ProductionOrderBatchCustomsFeesSum.ActualValue;
                        result.ProductionOrderBatchCustomsValueCorrection.CurrentValue = result.ProductionOrderBatchCustomsValueCorrection.ActualValue;
                    }
                    result.ProductionOrderBatchExtraExpensesSumInBaseCurrency.CurrentValue = result.ProductionOrderBatchExtraExpensesSumInBaseCurrency.ActualValue;
                    break;
                case ProductionOrderArticlePrimeCostCalculationType.PaymentSum:
                    result.ProductionOrderBatchTransportationCostInBaseCurrency.CurrentValue = result.ProductionOrderBatchTransportationCostInBaseCurrency.PaymentValue;
                    result.ProductionOrderBatchCustomsExpensesCostSum.CurrentValue = result.ProductionOrderBatchCustomsExpensesCostSum.PaymentValue;
                    if (divideCustomsExpenses)
                    {
                        result.ProductionOrderBatchImportCustomsDutiesSum.CurrentValue = result.ProductionOrderBatchImportCustomsDutiesSum.PaymentValue;
                        result.ProductionOrderBatchExportCustomsDutiesSum.CurrentValue = result.ProductionOrderBatchExportCustomsDutiesSum.PaymentValue;
                        result.ProductionOrderBatchValueAddedTaxSum.CurrentValue = result.ProductionOrderBatchValueAddedTaxSum.PaymentValue;
                        result.ProductionOrderBatchExciseSum.CurrentValue = result.ProductionOrderBatchExciseSum.PaymentValue;
                        result.ProductionOrderBatchCustomsFeesSum.CurrentValue = result.ProductionOrderBatchCustomsFeesSum.PaymentValue;
                        result.ProductionOrderBatchCustomsValueCorrection.CurrentValue = result.ProductionOrderBatchCustomsValueCorrection.PaymentValue;
                    }
                    result.ProductionOrderBatchExtraExpensesSumInBaseCurrency.CurrentValue = result.ProductionOrderBatchExtraExpensesSumInBaseCurrency.PaymentValue;
                    break;
                default:
                    throw new Exception("Неизвестный способ подсчета себестоимости.");
            };

            foreach (var batch in batchList.OrderBy(x => x.Date))
            {
                foreach (var row in batch.Rows.OrderBy(x => x.OrdinalNumber).ThenBy(x => x.CreationDate))
                {
                    var rowInfo = new ProductionOrderBatchRowArticlePrimeCost() { ProductionOrderBatchRow = row };

                    // 1. Стоимость производства позиции в рублях. При расчете "по плановой стоимости" реально берется фактическая стоимость
                    if (articlePrimeCostCalculationType != ProductionOrderArticlePrimeCostCalculationType.PaymentSum)
                    { // Расчет по фактической стоимости. Не делим общую стоимость на количество, а берем стоимость позиции (так точнее)
                        var rowProductionCostInBaseCurrency = currencyService.CalculateSumInBaseCurrency(productionOrderCurrency, productionOrderCurrencyRate,
                            row.ProductionOrderBatchRowCostInCurrency);
                        ValidationUtils.NotNull(rowProductionCostInBaseCurrency, "Не задан курс валюты заказа.");
                        rowInfo.RowProductionCostInBaseCurrency = rowProductionCostInBaseCurrency.Value;
                    }
                    else
                    { // Расчет по величине оплат. Берем общую стоимость, рассчитанную ранее
                        rowInfo.RowProductionCostInBaseCurrency = result.ProductionOrderBatchProductionCostInBaseCurrency.CurrentValue *
                            row.ProductionOrderBatchRowCostInCurrency / productionOrderProductionCostInCurrency;
                    }

                    // 2. Вес и объем позиции
                    rowInfo.Volume = row.TotalVolume;
                    rowInfo.Weight = row.TotalWeight;

                    // 3. Стоимость транспортировки позиции
                    switch (articleTransportingPrimeCostCalculationType)
                    {
                        case ProductionOrderArticleTransportingPrimeCostCalculationType.Volume:
                            ValidationUtils.Assert(productionOrderVolume != 0M, "Общий объем партии не может быть равен 0.");
                            rowInfo.TransportationCostInBaseCurrency =
                                result.ProductionOrderBatchTransportationCostInBaseCurrency.CurrentValue * row.TotalVolume / productionOrderVolume;
                            break;
                        case ProductionOrderArticleTransportingPrimeCostCalculationType.Weight:
                            ValidationUtils.Assert(productionOrderWeight != 0M, "Общий вес партии не может быть равен 0.");
                            rowInfo.TransportationCostInBaseCurrency =
                                result.ProductionOrderBatchTransportationCostInBaseCurrency.CurrentValue * row.TotalWeight / productionOrderWeight;
                            break;
                        default:
                            throw new Exception("Неизвестный способ подсчета себестоимости транспортировки.");
                    };

                    // 4. Стоимость таможенных затрат позиции. При расчете по фактической стоимости и по оплатам берется по таможенному листу партии, при расчете по плану -
                    // по всему заказу.
                    if (articlePrimeCostCalculationType != ProductionOrderArticlePrimeCostCalculationType.PlannedExpenses)
                    {
                        decimal customsExpensesCostSum = result.ProductionOrderBatchCustomsExpensesCostSum.CurrentValue;
                        decimal importCustomsDutiesSum = result.ProductionOrderBatchImportCustomsDutiesSum.CurrentValue;
                        decimal exportCustomsDutiesSum = result.ProductionOrderBatchExportCustomsDutiesSum.CurrentValue;
                        decimal valueAddedTaxSum = result.ProductionOrderBatchValueAddedTaxSum.CurrentValue;
                        decimal exciseSum = result.ProductionOrderBatchExciseSum.CurrentValue;
                        decimal customsFeesSum = result.ProductionOrderBatchCustomsFeesSum.CurrentValue;
                        decimal customsValueCorrection = result.ProductionOrderBatchCustomsValueCorrection.CurrentValue;

                        rowInfo.CustomsExpensesCostSum = customsExpensesCostSum *
                            row.ProductionOrderBatchRowCostInCurrency / productionOrderProductionCostInCurrency;
                        if (divideCustomsExpenses)
                        {
                            rowInfo.ImportCustomsDutiesSum = importCustomsDutiesSum *
                                row.ProductionOrderBatchRowCostInCurrency / productionOrderProductionCostInCurrency;
                            rowInfo.ExportCustomsDutiesSum = exportCustomsDutiesSum *
                                row.ProductionOrderBatchRowCostInCurrency / productionOrderProductionCostInCurrency;
                            rowInfo.ValueAddedTaxSum = valueAddedTaxSum *
                                row.ProductionOrderBatchRowCostInCurrency / productionOrderProductionCostInCurrency;
                            rowInfo.ExciseSum = exciseSum *
                                row.ProductionOrderBatchRowCostInCurrency / productionOrderProductionCostInCurrency;
                            rowInfo.CustomsFeesSum = customsFeesSum *
                                row.ProductionOrderBatchRowCostInCurrency / productionOrderProductionCostInCurrency;
                            rowInfo.CustomsValueCorrection = customsValueCorrection *
                                row.ProductionOrderBatchRowCostInCurrency / productionOrderProductionCostInCurrency;
                        }
                    }
                    else
                    { // При расчете по плановым показателям никогда не делим таможенные затраты по статьям
                        rowInfo.CustomsExpensesCostSum = result.ProductionOrderBatchCustomsExpensesCostSum.CurrentValue *
                            row.ProductionOrderBatchRowCostInCurrency / productionOrderProductionCostInCurrency;
                    }

                    // 5. Стоимость дополнительных расходов позиции
                    rowInfo.ExtraExpensesSumInBaseCurrency = result.ProductionOrderBatchExtraExpensesSumInBaseCurrency.CurrentValue *
                        row.ProductionOrderBatchRowCostInCurrency / productionOrderProductionCostInCurrency;

                    result.ProductionOrderBatchRowArticlePrimeCostList.Add(rowInfo);
                }
            }

            return result;
        }

        #endregion

        #region Расчет закупочных цен приходной накладной по себестоимости и точное разнесение суммы себестоимости по позициям

        /// <summary>
        /// Рассчитать закупочные цены и записать их во все накладные, уже созданные к тому моменту по партиям данного заказа
        /// </summary>
        /// <param name="productionOrder">Заказ на производство товаров</param>
        /// <returns>0 -  если  для всех накладных заказа при разнесении закупочных цен по позициям
        /// удалось добиться совпадения себестоимости и суммы накладной, иначе сумму коррекции</returns>
        public decimal CalculatePurchaseCostByArticlePrimeCost(ProductionOrder productionOrder)
        {
            return CalculatePurchaseCostByArticlePrimeCost(
                receiptWaybillRepository.GetList(receiptWaybillRepository.GetProductionOrderReceiptWaybillSubQuery(productionOrder.Id))
                .ToDictionary(x => x.Value.ProductionOrderBatch.Id, x => x.Value));
        }

        /// <summary>
        /// Рассчитать закупочные цены и записать в данную приходную накладную, созданную по партии заказа
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная</param>
        /// <returns>0 -  если для всех накладных заказа при разнесении закупочных цен по позициям
        /// удалось добиться совпадения себестоимости и суммы накладной, иначе сумму коррекции</returns>
        public decimal CalculatePurchaseCostByArticlePrimeCost(ReceiptWaybill receiptWaybill)
        {
            ValidationUtils.Assert(receiptWaybill.IsCreatedFromProductionOrderBatch, "Рассчитать закупочные цены можно только для накладной, созданной по партии заказа.");

            return CalculatePurchaseCostByArticlePrimeCost(
                new Dictionary<Guid, ReceiptWaybill> { { receiptWaybill.ProductionOrderBatch.Id, receiptWaybill } });
        }

        /// <summary>
        /// Рассчитать закупочные цены и записать их в накладные, созданные по партиям заказа.
        /// Расчет идет по всем партиям, значения записываются только в накладные из переданного словаря.
        /// Пользуемся тем, что метод вызывается только для закрытого заказа (все партии закрыты)
        /// </summary>
        /// <param name="receiptWaybillList">Словарь приходных накладных (ключ - код партии, значение - приходная накладная)</param>
        /// <returns>0 - если  для всех переданных в словаре накладных при разнесении закупочных цен по позициям
        /// удалось добиться совпадения себестоимости и суммы накладной, иначе сумму коррекции</returns>
        private decimal CalculatePurchaseCostByArticlePrimeCost(IDictionary<Guid, ReceiptWaybill> receiptWaybillList)
        {
            ValidationUtils.NotNull(receiptWaybillList, "Не указаны приходные накладные.");

            if (receiptWaybillList.Count == 0)
            {
                return 0M;
            }

            foreach (var item in receiptWaybillList)
            {
                ValidationUtils.Assert(item.Value.IsCreatedFromProductionOrderBatch, "Рассчитать закупочные цены можно только для накладной, созданной по партии заказа.");
                ValidationUtils.Assert(item.Key == item.Value.ProductionOrderBatch.Id, "Переданный ключ не соответствует приходной накладной.");
            }

            ValidationUtils.Assert(receiptWaybillList.Select(x => x.Value.Id).Distinct().Count() == receiptWaybillList.Count,
                "Приходные накладные в списке не должны повторяться.");
            ValidationUtils.Assert(receiptWaybillList.Select(x => x.Value.ProductionOrderBatch.ProductionOrder.Id).Distinct().Count() == 1,
                "Невозможно рассчитать закупочную стоимость по накладным, созданным по разным заказам.");

            // Вычисляем себестоимость по оплатам (все партии заказа)
            var productionOrder = receiptWaybillList.First().Value.ProductionOrderBatch.ProductionOrder;
            ValidationUtils.Assert(productionOrder.IsArticleTransportingPrimeCostCalculationTypeSet,
                "Невозможно рассчитать закупочные цены в приходах по партиям заказа, так как в заказе не указан способ учета транспортировки в себестоимости товаров.");
            var result = CalculateProductionOrderBatchArticlePrimeCost(productionOrder,
                ProductionOrderArticlePrimeCostCalculationType.PaymentSum, false, false, productionOrder.ArticleTransportingPrimeCostCalculationType,
                includeUnsuccessfullyClosedBatches: false, includeUnapprovedBatches: false);

            // Сначала разносим точно общую сумму заказа по всем позициям всех партий. Может быть, уже при этом суммы по партиям, округленные до 2 знаков,
            // сойдутся с суммой по заказу, округленной до 2 знаков
            var productionOrderBatchRowInfoList = result.ProductionOrderBatchRowArticlePrimeCostList
                .ToDictionary(x => x.ProductionOrderBatchRow.Id,
                    x => new ProductionOrderBatchRowInfo(x.ProductionOrderBatchRow, x.RowCostInBaseCurrency));
            decimal expectedProductionOrderPurchaseCostSum = Math.Round(result.ProductionOrderBatchPaymentCostInBaseCurrency, 2);
            TryPurchaseCostDistribute(productionOrderBatchRowInfoList, expectedProductionOrderPurchaseCostSum);

            decimal distributionError = 0M;

            // Если партия одна, сразу записываем сумму в приходную накладную (этот метод может быть вызван, только когда по ней уже создана накладная)
            if (productionOrder.IsIncludingOneBatch)
            {
                var productionOrderBatch = productionOrder.Batches.First();

                SetReceiptWaybillPurchaseCostsByProductionOrderBatch(productionOrderBatch, productionOrderBatchRowInfoList);

                distributionError = expectedProductionOrderPurchaseCostSum - productionOrderBatch.ReceiptWaybill.PendingSum;
            }
            else
            {
                // Иначе несколько партий. Суммы ЗЦ по партиям (кроме неуспешно закрытых), округленные до 2 знаков,
                // могут сойтись с суммой по заказу, округленной до 2 знаков, а могут не сойтись
                var productionOrderBatchPurchaseCostSumDictionary = new Dictionary<Guid, decimal>();
                foreach (var productionOrderBatch in productionOrder.Batches.Where(x => !x.IsClosed || x.IsClosedSuccessfully))
                {
                    productionOrderBatchPurchaseCostSumDictionary[productionOrderBatch.Id] = Math.Round(productionOrderBatch.Rows
                        .Sum(x => Math.Round(productionOrderBatchRowInfoList[x.Id].PurchaseCost * x.Count, 6)), 2);
                }

                // Если суммы сходятся, можно просто записать все ЗЦ в приходы и закончить работу
                decimal purchaseCostSumDifference = expectedProductionOrderPurchaseCostSum - productionOrderBatchPurchaseCostSumDictionary.Sum(x => x.Value);
                if (purchaseCostSumDifference == 0M)
                {
                    foreach (var productionOrderBatch in productionOrder.Batches.Where(x => x.ReceiptWaybill != null))
                    {
                        SetReceiptWaybillPurchaseCostsByProductionOrderBatch(productionOrderBatch, productionOrderBatchRowInfoList);
                    }

                    return 0M;
                }

                // Суммы не сходятся. Надо изменить текущие суммы так, чтобы они сходились. (Как и при вычислении себестоимости, работаем с партиями,
                // которые не являются закрытыми неуспешно). Для этого разносим сумму расхождения на них почти поровну,
                // с разницей не более одной копейки (начиная с наибольших по сумме) - метод не самый лучший, но в основном расхождение будет меньше N копеек,
                // где N - количество партий, и на приходы придется от 0 до 1 копейки изменения суммы. Например, у заказа из 5 партий расхождение 3 копейки.
                // Тогда трем партиям с наибольшими суммами будет прибавлено (или вычтено) по 1 копейке (большее приращение), а прочим - по 0 копеек (меньшее).
                // Чтобы от вызова к вызову метод давал одинаковые результаты (он ведь может быть вызван для заказа с несколькими созданными накладными из 10,
                // а потом вызываться при создании остальных приходов по одному) делаем вторичную сортировку по Id партии.
                decimal differenceSign = purchaseCostSumDifference >= 0M ? Decimal.One : Decimal.MinusOne;
                purchaseCostSumDifference *= differenceSign;
                purchaseCostSumDifference *= 100M; // Вычисляем число копеек, чтобы деление нацело работало с копейками

                // Работаем с модулями приращений

                // Количество партий, для которых вычисляется сумма в ЗЦ
                int batchCount = productionOrder.Batches.Where(x => !x.IsClosed || x.IsClosedSuccessfully).Count();
                // Меньшее приращение (большее равно quotient + 1)
                decimal quotient = Math.Floor(Decimal.Divide(purchaseCostSumDifference, (decimal)batchCount));
                // Количество партий, которым надо прибавить или вычесть большее приращение
                decimal maxQuantity = Decimal.Remainder(purchaseCostSumDifference, (decimal)batchCount);

                // Словарь новых (откорректированных) сумм в ЗЦ по каждой партии заказа "код партии заказа - новая сумма в ЗЦ"
                var productionOrderBatchNewPurchaseCostSumDictionary = new Dictionary<Guid, decimal>();

                // Собственно вычисляем (разносим) новые суммы в ЗЦ по каждой партии заказа (кроме неуспешно закрытых)
                decimal productionOrderBatchCount = 0M;
                foreach (var productionOrderBatch in productionOrder.Batches.Where(x => !x.IsClosed || x.IsClosedSuccessfully)
                    .OrderByDescending(x => productionOrderBatchPurchaseCostSumDictionary[x.Id])
                    .ThenBy(x => x.Id))
                {
                    decimal productionOrderBatchPurchaseCostSumDifference = quotient;
                    if (productionOrderBatchCount < maxQuantity)
                        productionOrderBatchPurchaseCostSumDifference++;

                    productionOrderBatchPurchaseCostSumDifference *= 0.01M; // Переводим опять из копеек в рубли
                    productionOrderBatchPurchaseCostSumDifference *= differenceSign;

                    productionOrderBatchNewPurchaseCostSumDictionary.Add(productionOrderBatch.Id,
                        productionOrderBatchPurchaseCostSumDictionary[productionOrderBatch.Id] + productionOrderBatchPurchaseCostSumDifference);

                    productionOrderBatchCount++;
                }

                // Теперь сумма новых сумм по партиям обязана сходиться с суммой по всему заказу.
                ValidationUtils.Assert(expectedProductionOrderPurchaseCostSum == productionOrderBatchNewPurchaseCostSumDictionary.Sum(x => x.Value),
                    "Рассчитанная сумма закупочных цен по партиям не сходится с суммой по всему заказу.");

                // Распределяем сумму для каждой партии заново. (По тем, по которым нет приходов, не делаем ничего, т.к. это не повлияет на дальнейшее.
                // Те же, у кого есть приходы, не попадают в множество неуспешно закрытых партий, которое мы игнорируем - поэтому мы
                // проверку на неуспешное закрытие не ставим)
                foreach (var productionOrderBatch in productionOrder.Batches.Where(x => x.ReceiptWaybill != null))
                {
                    var receiptWaybillProductionOrderBatchRowInfoList = productionOrderBatchRowInfoList
                        .Where(x => x.Value.ProductionOrderBatchId == productionOrderBatch.Id)
                        .ToDictionary(x => x.Key, x => x.Value);

                    decimal expectedProductionOrderBatchPurchaseCostSum = productionOrderBatchNewPurchaseCostSumDictionary[productionOrderBatch.Id];
                    TryPurchaseCostDistribute(receiptWaybillProductionOrderBatchRowInfoList, expectedProductionOrderBatchPurchaseCostSum);

                    // Пишем ЗЦ в накладную. Если сумма по ней не разнеслась точно, устанавливаем признак ошибки.
                    SetReceiptWaybillPurchaseCostsByProductionOrderBatch(productionOrderBatch, receiptWaybillProductionOrderBatchRowInfoList);

                    distributionError = expectedProductionOrderBatchPurchaseCostSum - productionOrderBatch.ReceiptWaybill.PendingSum;
                  
                }
            }

            return distributionError;
        }

        /// <summary>
        /// Попытка разнести закупочные цены так, чтобы их общая сумма сходилась с заданным значением
        /// </summary>
        /// <param name="productionOrderBatchRowInfoList">Словарь значений "код позиции партии заказа - информация о ней"</param>
        /// <param name="purchaseCostSum">Требуемое значение суммы в ЗЦ</param>
        private void TryPurchaseCostDistribute(Dictionary<Guid, ProductionOrderBatchRowInfo> productionOrderBatchRowInfoList, decimal purchaseCostSum)
        {
            ValidationUtils.NotNull(productionOrderBatchRowInfoList, "Не указан список с информацией о позициях партии заказа.");

            // Коэффициент, на который надо умножать предварительную сумму в закупочных ценах (т.е. сумму оплат), чтобы сумма по всем позициям равнялась заданной (purchaseCostSum)
            double factor = purchaseCostSum != 0M ? (double)(purchaseCostSum) / (double)productionOrderBatchRowInfoList.Sum(x => x.Value.PaymentSum) : 0.0;

            foreach (var item in productionOrderBatchRowInfoList)
            {
                item.Value.PurchaseCost = Math.Round((decimal)((double)item.Value.PaymentSum * factor / (double)item.Value.Count), 6);
            }
        }

        /// <summary>
        /// Записать закупочные цены из списка вычисленных ЗЦ в приходную накладную по данной партии заказа
        /// </summary>
        /// <param name="productionOrderBatch">Партия заказа</param>
        /// <param name="productionOrderBatchRowInfoList">Словарь значений "код позиции партии заказа - информация о ней" с вычисленными ЗЦ</param>
        private void SetReceiptWaybillPurchaseCostsByProductionOrderBatch(ProductionOrderBatch productionOrderBatch,
            IDictionary<Guid, ProductionOrderBatchRowInfo> productionOrderBatchRowInfoList)
        {
            ValidationUtils.NotNull(productionOrderBatch.ReceiptWaybill, "Указанная партия заказа не имеет связанной приходной накладной.");

            foreach (var row in productionOrderBatch.Rows)
            {
                row.ReceiptWaybillRow.InitialPurchaseCost = row.ReceiptWaybillRow.PurchaseCost = productionOrderBatchRowInfoList[row.Id].PurchaseCost;
                row.ReceiptWaybillRow.RecalculatePendingSum();
                row.ReceiptWaybillRow.ProviderSum = row.ReceiptWaybillRow.ProviderSum.HasValue ? row.ReceiptWaybillRow.PendingSum : (decimal?)null;
                row.ReceiptWaybillRow.ApprovedPurchaseCost = row.ReceiptWaybillRow.ApprovedPurchaseCost.HasValue ? row.ReceiptWaybillRow.PurchaseCost : (decimal?)null;
                if (row.ReceiptWaybillRow.ApprovedSum.HasValue)
                {
                    row.ReceiptWaybillRow.RecalculateApprovedSum();
                }
            }

            // Записываем параметры самой накладной (зависят от записанного на прошлом шаге в позиции)
            productionOrderBatch.ReceiptWaybill.PendingSum = productionOrderBatch.ReceiptWaybill.PendingSumByRows;
            productionOrderBatch.ReceiptWaybill.ApprovedSum = productionOrderBatch.ReceiptWaybill.ApprovedSum.HasValue ?
                productionOrderBatch.ReceiptWaybill.PendingSum : (decimal?)null;
        }

        #endregion

        #region Права на совершение операций

        #region Вспомогательные методы

        private bool IsPermissionToPerformOperation(ProductionOrder order, User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Personal:
                    result = order.Curator == user && user.Teams.SelectMany(x => x.ProductionOrders).Contains(order);
                    break;

                case PermissionDistributionType.Teams:
                    result = user.Teams.SelectMany(x => x.ProductionOrders).Contains(order);
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(ProductionOrder order, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(order, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        private void CheckPermissionToPerformOperation(ProductionOrderBatch orderBatch, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(orderBatch.ProductionOrder, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        #endregion

        #region Просмотр деталей

        public bool IsPossibilityToViewDetails(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToViewDetails(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewDetails(ProductionOrder order, User user)
        {
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrder_List_Details);
        }

        #endregion

        #region Редактирование

        public bool IsPossibilityToEdit(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToEdit(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEdit(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrder_Create_Edit);

            // сущность
            order.CheckPossibilityToEdit();
        }

        #endregion

        #region Закрытие/открытие заказа

        public bool IsPossibilityToClose(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToClose(order, user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToClose(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrder_Stage_MoveToNext);

            // сущность
            order.CheckPossibilityToClose();

            //считаем процент оплат
            var indicator = CalculateMainIndicators(order, false, false, true);

            ValidationUtils.Assert(indicator.PaymentPercent <= 100M, "Невозможно успешно закрыть заказ, если оплачено более 100% заказа.");
        }

        public bool IsPossibilityToOpen(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToOpen(order, user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToOpen(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrder_Stage_MoveToPrevious);

            ValidationUtils.Assert(order.IsClosed, "Нельзя открыть уже открытый заказ.");
        }

        #endregion

        #region Смена валюты

        public bool IsPossibilityToChangeCurrency(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToChangeCurrency(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToChangeCurrency(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrder_Create_Edit);

            // сущность
            order.CheckPossibilityToChangeCurrency();
        }

        #endregion

        #region Смена способа расчета закупочных цен (а именно транспортных расходов) в приходах

        public bool IsPossibilityToChangeArticleTransportingPrimeCostCalculationType(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToChangeArticleTransportingPrimeCostCalculationType(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CheckPossibilityToChangeArticleTransportingPrimeCostCalculationType(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrder_Create_Edit);

            // сущность
            order.CheckPossibilityToChangeArticleTransportingPrimeCostCalculationType();
        }

        #endregion

        #region Редактирование / просмотр финансового плана

        public bool IsPossibilityToViewPlannedExpenses(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToViewPlannedExpenses(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewPlannedExpenses(ProductionOrder order, User user)
        {
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrder_PlannedExpenses_List_Details);
        }

        public bool IsPossibilityToEditPlannedExpenses(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToEditPlannedExpenses(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditPlannedExpenses(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrder_PlannedExpenses_Create_Edit);

            // сущность
            order.CheckPossibilityToEditPlannedExpenses();
        }

        #endregion

        #region Редактирование / просмотр плана платежей

        public bool IsPossibilityToViewPlannedPaymentList(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToViewPlannedPaymentList(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewPlannedPaymentList(ProductionOrder order, User user)
        {
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrder_PlannedPayments_List_Details);
        }

        public bool IsPossibilityToCreatePlannedPayment(ProductionOrder productionOrder, User user)
        {
            try
            {
                CheckPossibilityToCreatePlannedPayment(productionOrder, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreatePlannedPayment(ProductionOrder productionOrder, User user)
        {
            // права
            CheckPermissionToPerformOperation(productionOrder, user, Permission.ProductionOrder_PlannedPayments_Create);

            // сущность
            productionOrder.CheckPossibilityToCreatePlannedPayment();
        }

        public bool IsPossibilityToEditPlannedPayment(ProductionOrderPlannedPayment plannedPayment, User user)
        {
            try
            {
                CheckPossibilityToEditPlannedPayment(plannedPayment, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditPlannedPayment(ProductionOrderPlannedPayment plannedPayment, User user)
        {
            // права
            CheckPermissionToPerformOperation(plannedPayment.ProductionOrder, user, Permission.ProductionOrder_PlannedPayments_Edit);

            // сущность
            plannedPayment.CheckPossibilityToEdit();
        }

        public bool IsPossibilityToDeletePlannedPayment(ProductionOrderPlannedPayment plannedPayment, User user)
        {
            try
            {
                CheckPossibilityToDeletePlannedPayment(plannedPayment, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDeletePlannedPayment(ProductionOrderPlannedPayment plannedPayment, User user)
        {
            // права
            CheckPermissionToPerformOperation(plannedPayment.ProductionOrder, user, Permission.ProductionOrder_PlannedPayments_Delete);

            // сущность
            plannedPayment.CheckPossibilityToDelete();
        }

        public bool IsPossibilityToEditPlannedPaymentSum(ProductionOrderPlannedPayment plannedPayment, User user)
        {
            try
            {
                CheckPossibilityToEditPlannedPaymentSum(plannedPayment, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditPlannedPaymentSum(ProductionOrderPlannedPayment plannedPayment, User user)
        {
            // права
            CheckPermissionToPerformOperation(plannedPayment.ProductionOrder, user, Permission.ProductionOrder_PlannedPayments_Edit);

            // сущность
            plannedPayment.CheckPossibilityToEditSum();
        }

        #endregion

        #region Редактирование курса валюты

        public bool IsPossibilityToChangeCurrencyRate(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToChangeCurrencyRate(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToChangeCurrencyRate(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrder_CurrencyRate_Change);

            // сущность
            order.CheckPossibilityToChangeCurrencyRate();
        }

        #endregion

        #region Добавление / редактирование контракта

        public bool IsPossibilityToEditContract(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToEditContract(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditContract(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrder_Contract_Change);

            // сущность
            order.CheckPossibilityToEditContract();
        }

        #endregion

        #region Редактирование организаций договора

        /// <summary>
        /// Можно ли изменять организации, указанные в договоре
        /// </summary>        
        public bool IsPossibilityToEditOrganization(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToEditOrganization(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditOrganization(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrder_Contract_Change);

            // сущность
            ValidationUtils.Assert(order.Batches.All(x => x.ReceiptWaybill == null), "Невозможно редактировать организации в договоре заказа, по которой уже есть дальнейшее товародвижение.");
        }

        #endregion
        
        #region Разделение партии

        public bool IsPossibilityToSplitBatch(ProductionOrderBatch batch, User user)
        {
            try
            {
                CheckPossibilityToSplitBatch(batch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToSplitBatch(ProductionOrderBatch batch, User user)
        {
            CheckPermissionToPerformOperation(batch, user, Permission.ProductionOrderBatch_Split);

            batch.CheckPossibilityToSplitBatch();
        }

        #endregion

        #region Склеивание партии

        public bool IsPossibilityToJoinBatch(ProductionOrderBatch batch, User user)
        {
            try
            {
                CheckPossibilityToJoinBatch(batch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToJoinBatch(ProductionOrderBatch batch, User user)
        {
            CheckPermissionToPerformOperation(batch, user, Permission.ProductionOrderBatch_Join);

            // batch.CheckPossibilityToJoinBatch();
        }

        #endregion

        #region Изменение информации о размещении в контейнерах

        public bool IsPossibilityToRecalculatePlacement(ProductionOrderBatch batch, User user)
        {
            try
            {
                CheckPossibilityToRecalculatePlacement(batch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToRecalculatePlacement(ProductionOrderBatch batch, User user)
        {
            CheckPermissionToPerformOperation(batch, user, Permission.ProductionOrderBatch_Edit_Placement_In_Containers);

            // batch.CheckPossibilityToEditPlacement();
        }

        #endregion

        #region Редактирование транспортных листов

        public bool IsPossibilityToEditTransportSheetPaymentDependentFields(ProductionOrderTransportSheet transportSheet, User user)
        {
            try
            {
                CheckPossibilityToEditTransportSheetPaymentDependentFields(transportSheet, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditTransportSheetPaymentDependentFields(ProductionOrderTransportSheet transportSheet, User user)
        {
            // Условие включает в себя условие на редактирование листа
            CheckPossibilityToEditTransportSheet(transportSheet, user);

            transportSheet.CheckPossibilityToEditPaymentDependentFields();
        }

        #endregion

        #region Редактирование листов дополнительных расходов

        public bool IsPossibilityToEditExtraExpensesSheetPaymentDependentFields(ProductionOrderExtraExpensesSheet extraExpensesSheet, User user)
        {
            try
            {
                CheckPossibilityToEditExtraExpensesSheetPaymentDependentFields(extraExpensesSheet, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditExtraExpensesSheetPaymentDependentFields(ProductionOrderExtraExpensesSheet extraExpensesSheet, User user)
        {
            // Условие включает в себя условие на редактирование листа
            CheckPossibilityToEditExtraExpensesSheet(extraExpensesSheet, user);

            extraExpensesSheet.CheckPossibilityToEditPaymentDependentFields();
        }

        #endregion

        #region Удаление листов дополнительных расходов

        public bool IsPossibilityToDeleteExtraExpensesSheet(ProductionOrderExtraExpensesSheet extraExpensesSheet)
        {
            try
            {
                CheckPossibilityToDeleteExtraExpensesSheet(extraExpensesSheet);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDeleteExtraExpensesSheet(ProductionOrderExtraExpensesSheet extraExpensesSheet) // TODO: потом юзера добавить
        {
            ValidationUtils.Assert(!extraExpensesSheet.ProductionOrder.IsClosed, "Невозможно удалять листы дополнительных расходов в закрытом заказе.");
            ValidationUtils.Assert(!extraExpensesSheet.Payments.Any(), "Невозможно удалить лист дополнительных расходов, по которому есть оплаты.");
        }

        #endregion

        #region Создание приходной накладной по партии заказа

        /// <summary>
        /// Разрешено ли создание накладной по партии заказа
        /// </summary>
        /// <param name="orderBatch">Партия заказа</param>
        /// <param name="checkLogic">true - вызывать проверки сущности (при уже созданной накладной ее будет нельзя создать повторно),
        /// false - проверка только, имеет ли пользователь право на создание</param>
        /// <param name="user">Пользователь</param>
        public bool IsPossibilityToCreateReceiptWaybill(ProductionOrderBatch orderBatch, User user)
        {
            try
            {
                CheckPossibilityToCreateReceiptWaybill(orderBatch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Разрешено ли создание накладной по партии заказа
        /// </summary>
        /// <param name="orderBatch">Партия заказа</param>
        /// <param name="user">Пользователь</param>
        public void CheckPossibilityToCreateReceiptWaybill(ProductionOrderBatch orderBatch, User user)
        {
            // права
            CheckPermissionToPerformOperation(orderBatch.ProductionOrder, user, Permission.ReceiptWaybill_CreateFromProductionOrderBatch);

            // сущность
            orderBatch.CheckPossibilityToCreateReceiptWaybill();
        }

        #endregion

        #region Возможность иметь связанную приходную накладную на данном этапе

        public bool IsPossibilityToHaveReceiptWaybill(ProductionOrderBatch orderBatch)
        {
            try
            {
                CheckPossibilityToHaveReceiptWaybill(orderBatch);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToHaveReceiptWaybill(ProductionOrderBatch orderBatch)
        {
            // сущность
            orderBatch.CheckPossibilityToHaveReceiptWaybill();
        }

        #endregion

        #region Переходы между этапами

        /// <summary>
        /// Можно ли показать окно смены стадии
        /// </summary>
        public void CheckPossibilityToChangeStage(ProductionOrderBatch orderBatch, User user)
        {
            CheckPossibilityToViewStageList(orderBatch, user);
            ValidationUtils.Assert(!orderBatch.ProductionOrder.IsClosed, "Нельзя редактировать партии в закрытом заказе.");
        }

        /// <summary>
        /// Можно ли показать окно смены стадии
        /// </summary>
        public bool IsPossibilityToChangeStage(ProductionOrderBatch orderBatch, User user)
        {
            try
            {
                CheckPossibilityToChangeStage(orderBatch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Переход на следующий этап

        public bool IsPossibilityToMoveToNextStage(ProductionOrderBatch orderBatch, User user)
        {
            try
            {
                CheckPossibilityToMoveToNextStage(orderBatch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToMoveToNextStage(ProductionOrderBatch orderBatch, User user)
        {
            // права
            CheckPermissionToPerformOperation(orderBatch.ProductionOrder, user, Permission.ProductionOrder_Stage_MoveToNext);

            // сущность
            orderBatch.CheckPossibilityToMoveToNextStage();
        }

        #endregion

        #region Переход на предыдущий этап

        public bool IsPossibilityToMoveToPreviousStage(ProductionOrderBatch orderBatch, User user)
        {
            try
            {
                CheckPossibilityToMoveToPreviousStage(orderBatch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToMoveToPreviousStage(ProductionOrderBatch orderBatch, User user)
        {
            // права
            CheckPermissionToPerformOperation(orderBatch.ProductionOrder, user, Permission.ProductionOrder_Stage_MoveToPrevious);

            // сущность
            orderBatch.CheckPossibilityToMoveToPreviousStage();
        }

        #endregion

        #region Перемещение этапа вверх

        public bool IsPossibilityToMoveStageUp(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false)
        {
            try
            {
                CheckPossibilityToMoveStageUp(orderBatch, stage, user, onlyLogic);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToMoveStageUp(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false)
        {
            if (!onlyLogic)
            {
                // права
                CheckPermissionToPerformOperation(orderBatch.ProductionOrder, user, Permission.ProductionOrder_Stage_Create_Edit);
            }

            // сущность
            orderBatch.CheckPossibilityToMoveStageUp(stage);
        }

        #endregion

        #region Перемещение этапа вниз

        public bool IsPossibilityToMoveStageDown(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false)
        {
            try
            {
                CheckPossibilityToMoveStageDown(orderBatch, stage, user, onlyLogic);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToMoveStageDown(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false)
        {
            if (!onlyLogic)
            {
                // права
                CheckPermissionToPerformOperation(orderBatch.ProductionOrder, user, Permission.ProductionOrder_Stage_Create_Edit);
            }

            // сущность
            orderBatch.CheckPossibilityToMoveStageDown(stage);
        }

        #endregion

        #region Переход на этап "Неуспешное закрытие"

        public bool IsPossibilityToMoveToUnsuccessfulClosingStage(ProductionOrderBatch orderBatch, User user)
        {
            try
            {
                CheckPossibilityToMoveToUnsuccessfulClosingStage(orderBatch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToMoveToUnsuccessfulClosingStage(ProductionOrderBatch orderBatch, User user)
        {
            // права
            CheckPermissionToPerformOperation(orderBatch.ProductionOrder, user, Permission.ProductionOrder_Stage_MoveToUnsuccessfulClosing);

            // сущность
            orderBatch.CheckPossibilityToMoveToUnsuccessfulClosingStage();
        }

        #endregion

        #endregion

        #region Создание / Редактирование / просмотр списка этапов

        public bool IsPossibilityToViewStageList(ProductionOrderBatch batch, User user)
        {
            try
            {
                CheckPossibilityToViewStageList(batch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewStageList(ProductionOrderBatch batch, User user)
        {
            CheckPermissionToPerformOperation(batch.ProductionOrder, user, Permission.ProductionOrder_Stage_List_Details);
        }

        public bool IsPossibilityToEditStages(ProductionOrderBatch productionOrderBatch, User user)
        {
            try
            {
                CheckPossibilityToEditStages(productionOrderBatch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditStages(ProductionOrderBatch productionOrderBatch, User user)
        {
            // права
            CheckPermissionToPerformOperation(productionOrderBatch, user, Permission.ProductionOrder_Stage_Create_Edit);

            // сущность
            productionOrderBatch.CheckPossibilityToEditStages();
        }

        public bool IsPossibilityToCreateStage(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false)
        {
            try
            {
                CheckPossibilityToCreateStage(orderBatch, stage, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreateStage(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false)
        {
            if (!onlyLogic)
            {
                // права
                CheckPermissionToPerformOperation(orderBatch.ProductionOrder, user, Permission.ProductionOrder_Stage_Create_Edit);
            }

            // сущность
            orderBatch.CheckPossibilityToCreateStage(stage);
        }

        public bool IsPossibilityToEditStage(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false)
        {
            try
            {
                CheckPossibilityToEditStage(orderBatch, stage, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditStage(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false)
        {
            if (!onlyLogic)
            {
                // права
                CheckPermissionToPerformOperation(orderBatch.ProductionOrder, user, Permission.ProductionOrder_Stage_Create_Edit);
            }

            // сущность
            orderBatch.CheckPossibilityToEditStage(stage);
        }

        public bool IsPossibilityToDeleteStage(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false)
        {
            try
            {
                CheckPossibilityToDeleteStage(orderBatch, stage, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDeleteStage(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false)
        {
            if (!onlyLogic)
            {
                // права
                CheckPermissionToPerformOperation(orderBatch.ProductionOrder, user, Permission.ProductionOrder_Stage_Create_Edit);
            }

            // сущность
            orderBatch.CheckPossibilityToDeleteStage(stage);
        }

        #endregion

        #region Редактирование графика рабочих дней заказа

        public bool IsPossibilityToEditWorkDaysPlan(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToEditWorkDaysPlan(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditWorkDaysPlan(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrder_Create_Edit);

            // сущность
            order.CheckPossibilityToEditWorkDaysPlan();
        }

        #endregion

        #region Просмотр сводной информации о партиях

        public bool IsPossibilityToViewBatchList(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToViewBatchList(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewBatchList(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrderBatch_List);

            //// сущность
            //order.CheckPossibilityToViewBatchList(); //нет никаких дополнительных проверок
        }

        #endregion

        #region Просмотр деталей партии

        public bool IsPossibilityToViewBatchDetails(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToViewBatchDetails(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewBatchDetails(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrderBatch_Details);

        }

        #endregion

        #region Транспортные листы

        public bool IsPossibilityToViewTransportSheetList(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToViewTransportSheetList(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewTransportSheetList(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrderTransportSheet_List_Details);
        }

        public bool IsPossibilityToCreateTransportSheet(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToCreateTransportSheet(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreateTransportSheet(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrderTransportSheet_Create_Edit);

            // сущность
            order.CheckPossibilityToCreateTransportSheet();
        }

        public bool IsPossibilityToEditTransportSheet(ProductionOrderTransportSheet transportSheet, User user)
        {
            try
            {
                CheckPossibilityToEditTransportSheet(transportSheet, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditTransportSheet(ProductionOrderTransportSheet transportSheet, User user)
        {
            // права
            CheckPermissionToPerformOperation(transportSheet.ProductionOrder, user, Permission.ProductionOrderTransportSheet_Create_Edit);

            //// сущность
            transportSheet.CheckPossibilityToEdit();
            transportSheet.ProductionOrder.CheckPossibilityToEditTransportSheet();
        }

        public bool IsPossibilityToDeleteTransportSheet(ProductionOrderTransportSheet transportSheet, User user)
        {
            try
            {
                CheckPossibilityToDeleteTransportSheet(transportSheet, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDeleteTransportSheet(ProductionOrderTransportSheet transportSheet, User user)
        {
            // права
            CheckPermissionToPerformOperation(transportSheet.ProductionOrder, user, Permission.ProductionOrderTransportSheet_Delete);

            //// сущность
            transportSheet.CheckPossibilityToDelete();
            transportSheet.ProductionOrder.CheckPossibilityToDeleteTransportSheet();
        }

        #endregion

        #region Листы дополнительных расходов

        public bool IsPossibilityToViewExtraExpensesSheetList(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToViewExtraExpensesSheetList(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewExtraExpensesSheetList(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrderExtraExpensesSheet_List_Details);
        }

        public bool IsPossibilityToCreateExtraExpensesSheet(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToCreateExtraExpensesSheet(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreateExtraExpensesSheet(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrderExtraExpensesSheet_Create_Edit);

            // сущность
            order.CheckPossibilityToCreateExtraExpensesSheet();
        }

        public bool IsPossibilityToEditExtraExpensesSheet(ProductionOrderExtraExpensesSheet expensesSheet, User user)
        {
            try
            {
                CheckPossibilityToEditExtraExpensesSheet(expensesSheet, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditExtraExpensesSheet(ProductionOrderExtraExpensesSheet expensesSheet, User user)
        {
            // права
            CheckPermissionToPerformOperation(expensesSheet.ProductionOrder, user, Permission.ProductionOrderExtraExpensesSheet_Create_Edit);

            // сущность
            expensesSheet.CheckPossibilityToEdit();
            expensesSheet.ProductionOrder.CheckPossibilityToEditExtraExpensesSheet();
        }

        public bool IsPossibilityToDeleteExtraExpensesSheet(ProductionOrderExtraExpensesSheet expensesSheet, User user)
        {
            try
            {
                CheckPossibilityToDeleteExtraExpensesSheet(expensesSheet, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDeleteExtraExpensesSheet(ProductionOrderExtraExpensesSheet expensesSheet, User user)
        {
            // права
            CheckPermissionToPerformOperation(expensesSheet.ProductionOrder, user, Permission.ProductionOrderExtraExpensesSheet_Delete);

            // сущность
            expensesSheet.CheckPossibilityToDelete();
            expensesSheet.ProductionOrder.CheckPossibilityToDeleteExtraExpensesSheet();
        }

        #endregion

        #region Таможенные листы

        public bool IsPossibilityToViewCustomsDeclarationList(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToViewCustomsDeclarationList(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewCustomsDeclarationList(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrderCustomsDeclaration_List_Details);
        }

        public bool IsPossibilityToCreateCustomsDeclaration(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToCreateCustomsDeclaration(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreateCustomsDeclaration(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrderCustomsDeclaration_Create_Edit);

            // сущность
            order.CheckPossibilityToCreateCustomsDeclaration();
        }

        public bool IsPossibilityToEditCustomsDeclaration(ProductionOrderCustomsDeclaration customsDeclaration, User user)
        {
            try
            {
                CheckPossibilityToEditCustomsDeclaration(customsDeclaration, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditCustomsDeclaration(ProductionOrderCustomsDeclaration customsDeclaration, User user)
        {
            // права
            CheckPermissionToPerformOperation(customsDeclaration.ProductionOrder, user, Permission.ProductionOrderCustomsDeclaration_Create_Edit);

            // сущность
            customsDeclaration.CheckPossibilityToEdit();
        }

        public bool IsPossibilityToDeleteCustomsDeclaration(ProductionOrderCustomsDeclaration customsDeclaration, User user)
        {
            try
            {
                CheckPossibilityToDeleteCustomsDeclaration(customsDeclaration, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDeleteCustomsDeclaration(ProductionOrderCustomsDeclaration customsDeclaration, User user)
        {
            // права
            CheckPermissionToPerformOperation(customsDeclaration.ProductionOrder, user, Permission.ProductionOrderCustomsDeclaration_Delete);

            // сущность
            customsDeclaration.CheckPossibilityToDelete();
        }

        #endregion

        #region Пакеты материалов

        public bool IsPossibilityToViewMaterialsPackageList(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToViewMaterialsPackageList(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewMaterialsPackageList(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrderMaterialsPackage_List_Details);
        }

        public bool IsPossibilityToCreateMaterialsPackage(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToCreateMaterialsPackage(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreateMaterialsPackage(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrderMaterialsPackage_Create_Edit);

            // сущность
            order.CheckPossibilityToCreateMaterialsPackage();
        }

        public bool IsPossibilityToEditMaterialsPackage(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToEditMaterialsPackage(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditMaterialsPackage(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrderMaterialsPackage_Create_Edit);

            // сущность
            order.CheckPossibilityToEditMaterialsPackage();
        }

        public bool IsPossibilityToDeleteMaterialsPackage(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToDeleteMaterialsPackage(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDeleteMaterialsPackage(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrderMaterialsPackage_Delete);

            // сущность
            order.CheckPossibilityToDeleteMaterialsPackage();
        }

        #endregion

        #region Оплаты по заказу

        public bool IsPossibilityToViewPaymentList(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToViewPaymentList(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewPaymentList(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrderPayment_List_Details);
        }

        public bool IsPossibilityToCreatePayment(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToCreatePayment(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreatePayment(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrderPayment_Create_Edit);

            // сущность            
            order.CheckPossibilityToCreatePayment();
        }

        public bool IsPossibilityToEditPayment(ProductionOrderPayment payment, User user)
        {
            try
            {
                CheckPossibilityToEditPayment(payment, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditPayment(ProductionOrderPayment payment, User user)
        {
            // права
            CheckPermissionToPerformOperation(payment.ProductionOrder, user, Permission.ProductionOrderPayment_Create_Edit);

            //// сущность
            payment.CheckPossibilityToEdit();
            payment.ProductionOrder.CheckPossibilityToEditPayment();
        }

        public bool IsPossibilityToDeletePayment(ProductionOrderPayment payment, User user)
        {
            try
            {
                CheckPossibilityToDeletePayment(payment, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDeletePayment(ProductionOrderPayment payment, User user)
        {
            // права
            CheckPermissionToPerformOperation(payment.ProductionOrder, user, Permission.ProductionOrderPayment_Delete);

            // сущность
            payment.CheckPossibilityToDelete();
            payment.ProductionOrder.CheckPossibilityToDeletePayment();
        }

        #endregion

        #region Добавление и редактирование позиций партий заказа

        public bool IsPossibilityToCreateBatchRow(ProductionOrderBatch batch, User user)
        {
            try
            {
                CheckPossibilityToCreateBatchRow(batch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreateBatchRow(ProductionOrderBatch batch, User user)
        {
            // права
            CheckPermissionToPerformOperation(batch, user, Permission.ProductionOrderBatch_Row_Create_Edit);

            // сущность
            batch.CheckPossibilityToCreateRow();
        }

        public bool IsPossibilityToEditBatchRow(ProductionOrderBatch batch, User user)
        {
            try
            {
                CheckPossibilityToEditBatchRow(batch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditBatchRow(ProductionOrderBatch batch, User user)
        {
            // права
            CheckPermissionToPerformOperation(batch, user, Permission.ProductionOrderBatch_Row_Create_Edit);

            // сущность
            batch.CheckPossibilityToEditRow();
        }

        #endregion

        #region Удаление позиций партий заказа

        public bool IsPossibilityToDeleteBatchRow(ProductionOrderBatch batch, User user)
        {
            try
            {
                CheckPossibilityToDeleteBatchRow(batch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

                public void CheckPossibilityToDeleteBatchRow(ProductionOrderBatch batch, User user)
        {
            // права
            CheckPermissionToPerformOperation(batch, user, Permission.ProductionOrderBatch_Row_Delete);

            // сущность
            batch.CheckPossibilityToDeleteRow();
        }

        #endregion

        #region Создание и удаление партии заказа
        
        /// <summary>
        /// Проверка возможности добавления партии в заказ
        /// </summary>
        public void CheckPossibilityToAddBatch(ProductionOrder order, User user)
        {
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrderBatch_Create);
            order.CheckPossibilityToAddBatch();
        }

        /// <summary>
        /// Проверка возможности добавления партии в заказ
        /// </summary>
        public bool IsPossibilityToAddBatch(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToAddBatch(order, user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка возможности удаления партии из заказа
        /// </summary>
        public void CheckPossibilityToDeleteBatch(ProductionOrderBatch batch, User user)
        {
            CheckPermissionToPerformOperation(batch, user, Permission.ProductionOrderBatch_Delete);
            batch.ProductionOrder.CheckPossibilityToDeleteBatch(batch);
        }

        /// <summary>
        /// Проверка возможности удаления партии из заказа
        /// </summary>
        public bool IsPossibilityToDeleteBatch(ProductionOrderBatch batch, User user)
        {
            try
            {
                CheckPossibilityToDeleteBatch(batch, user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Переименование партии
        /// </summary>
        public void  CheckPossibilityToRenameBatch(ProductionOrderBatch batch, User user)
        {
            CheckPermissionToPerformOperation(batch, user, Permission.ProductionOrderBatch_Create);

            batch.CheckPossibilityToRename();
        }

        /// <summary>
        /// Переименование партии
        /// </summary>
        public bool IsPossibilityToRenameBatch(ProductionOrderBatch batch, User user)
        {
            try
            {
                CheckPossibilityToRenameBatch(batch, user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Проводка партии заказа

        public bool IsPossibilityToAccept(ProductionOrderBatch batch, User user)
        {
            try
            {
                CheckPossibilityToAccept(batch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToAccept(ProductionOrderBatch batch, User user)
        {
            // права
            CheckPermissionToPerformOperation(batch, user, Permission.ProductionOrderBatch_Accept);

            // сущность
            batch.CheckPossibilityToAccept();
        }

        #endregion

        #region Отмена проводки партии заказа

        public bool IsPossibilityToCancelAcceptance(ProductionOrderBatch batch, User user)
        {
            try
            {
                CheckPossibilityToCancelAcceptance(batch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCancelAcceptance(ProductionOrderBatch batch, User user)
        {
            // права
            CheckPermissionToPerformOperation(batch, user, Permission.ProductionOrderBatch_Cancel_Acceptance);

            // сущность
            batch.CheckPossibilityToCancelAcceptation();
        }

        #endregion

        #region Утверждение партии заказа

        public bool IsPossibilityToApprove(ProductionOrderBatch batch, User user)
        {
            try
            {
                CheckPossibilityToApprove(batch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToApprove(ProductionOrderBatch batch, User user)
        {
            // права
            CheckPermissionToPerformOperation(batch, user, Permission.ProductionOrderBatch_Approve);

            // сущность
            batch.CheckPossibilityToApprove();
        }

        #endregion

        #region Отмена утверждения партии заказа

        public bool IsPossibilityToCancelApprovement(ProductionOrderBatch batch, User user)
        {
            try
            {
                CheckPossibilityToCancelApprovement(batch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCancelApprovement(ProductionOrderBatch batch, User user)
        {
            // права
            CheckPermissionToPerformOperation(batch, user, Permission.ProductionOrderBatch_Cancel_Approvement);

            // сущность
            batch.CheckPossibilityToCancelApprovement();
        }

        #endregion

        #region Утверждение / отмена утверждения партии действующими лицами

        public bool IsPossibilityToApproveByActor(ProductionOrderBatch batch, ProductionOrderApprovementActor actor, User user)
        {
            try
            {
                CheckPossibilityToApproveByActor(batch, actor, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToApproveByActor(ProductionOrderBatch batch, ProductionOrderApprovementActor actor, User user)
        {
            Permission permission;

            switch (actor)
            {
                case ProductionOrderApprovementActor.LineManager: permission = Permission.ProductionOrderBatch_Approve_By_LineManager; break;
                case ProductionOrderApprovementActor.FinancialDepartment: permission = Permission.ProductionOrderBatch_Approve_By_FinancialDepartment; break;
                case ProductionOrderApprovementActor.SalesDepartment: permission = Permission.ProductionOrderBatch_Approve_By_SalesDepartment; break;
                case ProductionOrderApprovementActor.AnalyticalDepartment: permission = Permission.ProductionOrderBatch_Approve_By_AnalyticalDepartment; break;
                case ProductionOrderApprovementActor.ProjectManager: permission = Permission.ProductionOrderBatch_Approve_By_ProjectManager; break;
                default: throw new Exception("Неизвестный тип лица, утверждающего партию заказа.");
            }
            // права
            CheckPermissionToPerformOperation(batch, user, permission);

            // сущность
            batch.CheckPossibilityToApproveByActor(actor);
        }

        public bool IsPossibilityToCancelApprovementByActor(ProductionOrderBatch batch, ProductionOrderApprovementActor actor, User user)
        {
            try
            {
                CheckPossibilityToCancelApprovementByActor(batch, actor, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCancelApprovementByActor(ProductionOrderBatch batch, ProductionOrderApprovementActor actor, User user)
        {
            Permission permission;

            switch (actor)
            {
                case ProductionOrderApprovementActor.LineManager: permission = Permission.ProductionOrderBatch_Cancel_Approvement_By_LineManager; break;
                case ProductionOrderApprovementActor.FinancialDepartment: permission = Permission.ProductionOrderBatch_Cancel_Approvement_By_FinancialDepartment; break;
                case ProductionOrderApprovementActor.SalesDepartment: permission = Permission.ProductionOrderBatch_Cancel_Approvement_By_SalesDepartment; break;
                case ProductionOrderApprovementActor.AnalyticalDepartment: permission = Permission.ProductionOrderBatch_Cancel_Approvement_By_AnalyticalDepartment; break;
                case ProductionOrderApprovementActor.ProjectManager: permission = Permission.ProductionOrderBatch_Cancel_Approvement_By_ProjectManager; break;
                default: throw new Exception("Неизвестный тип лица, утверждающего партию заказа.");
            }
            // права
            CheckPermissionToPerformOperation(batch, user, permission);

            // сущность
            batch.CheckPossibilityToCancelApprovementByActor(actor);
        }

        #endregion

        #region Просмотр себестоимости

        public bool IsPossibilityToViewArticlePrimeCostForm(ProductionOrder order, User user)
        {
            try
            {
                CheckPossibilityToViewArticlePrimeCostForm(order, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewArticlePrimeCostForm(ProductionOrder order, User user)
        {
            // права
            CheckPermissionToPerformOperation(order, user, Permission.ProductionOrder_ArticlePrimeCostPrintingForm_View);
        }

        #endregion

        #endregion

        #endregion
    }
}