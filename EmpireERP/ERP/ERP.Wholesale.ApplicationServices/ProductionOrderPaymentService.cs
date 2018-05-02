using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.Entities.Security;
using System.Linq;

namespace ERP.Wholesale.ApplicationServices
{
    public class ProductionOrderPaymentService : IProductionOrderPaymentService
    {
        #region Поля

        private readonly ICurrencyService currencyService;

        private readonly IProductionOrderPaymentRepository productionOrderPaymentRepository;

        #endregion

        #region Конструкторы

        public ProductionOrderPaymentService(ICurrencyService currencyService, IProductionOrderPaymentRepository productionOrderPaymentRepository)
        {
            this.currencyService = currencyService;

            this.productionOrderPaymentRepository = productionOrderPaymentRepository;
        }

        #endregion

        #region Методы
        
        public ProductionOrderPayment GetById(Guid id)
        {
            return productionOrderPaymentRepository.GetById(id);
        }

        private ProductionOrderPayment GetById(Guid id, User user)
        {
            var type = user.GetPermissionDistributionType(Permission.ProductionOrderPayment_List_Details);

            // если права нет - то сразу возвращаем null
            if (type == PermissionDistributionType.None)
            {
                return null;
            }
            else
            {
                var payment = productionOrderPaymentRepository.GetById(id);

                if (type == PermissionDistributionType.All)
                {
                    return payment;
                }
                else
                {
                    var order = payment.ProductionOrder;

                    bool contains = (user.Teams.SelectMany(x => x.ProductionOrders).Contains(order));

                    if ((type == PermissionDistributionType.Personal && order.Curator == user && contains) ||
                        (type == PermissionDistributionType.Teams && contains))
                    {
                        return payment;
                    }
                }

                return null;
            }
        }

        public ProductionOrderPayment CheckProductionOrderPaymentExistence(Guid id, User user)
        {
            user.CheckPermission(Permission.ProductionOrderPayment_List_Details);

            var payment = GetById(id, user);
            ValidationUtils.NotNull(payment, "Оплата по заказу не найдена. Возможно, она была удалена.");

            return payment;
        }

        public IEnumerable<ProductionOrderPayment> GetAll()
        {
            return productionOrderPaymentRepository.GetAll();
        }

        public IEnumerable<ProductionOrderPayment> GetAll(User user, Permission permission)
        {
            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<ProductionOrderPayment>();

                case PermissionDistributionType.Personal:
                     var teamPayments = user.Teams.SelectMany(x => x.ProductionOrders.SelectMany(y => y.Payments));                        
                    return productionOrderPaymentRepository.GetAll().Where(x => x.ProductionOrder.Curator.Id == user.Id).Concat(teamPayments).Distinct();

                case PermissionDistributionType.Teams:                    
                    return user.Teams.SelectMany(x => x.ProductionOrders.SelectMany(y => y.Payments)).Distinct();

                case PermissionDistributionType.All:
                    return productionOrderPaymentRepository.GetAll();

                default:
                    return null;
            }
        }

        public IList<ProductionOrderPayment> GetFilteredList(object state, User user, ParameterString parameterString = null)
        {
            if (parameterString == null) parameterString = new ParameterString("");

            var type = user.GetPermissionDistributionType(Permission.ProductionOrderPayment_List_Details);

            switch (type)
            {
                case PermissionDistributionType.None:
                    return new List<ProductionOrderPayment>();

                case PermissionDistributionType.Personal:
                case PermissionDistributionType.Teams:
                    var list = GetAll(user, Permission.ProductionOrderPayment_List_Details).Select(x => x.Id).ToList();

                    if (type == PermissionDistributionType.Personal)
                    {
                        parameterString.Add("ProductionOrder.Curator", ParameterStringItem.OperationType.Eq, user.Id.ToString());
                    }

                    // если список пуст - то добавляем несуществующее значение
                    if (!list.Any()) { list.Add(Guid.Empty); }
                                        
                    parameterString.Add("Id", ParameterStringItem.OperationType.OneOf);
                    parameterString["Id"].Value = list;
                    break;

                case PermissionDistributionType.All:
                    break;
            }

            return productionOrderPaymentRepository.GetFilteredList(state, parameterString);

        }

        public IList<ProductionOrderPayment> GetFilteredList(object state)
        {
            return productionOrderPaymentRepository.GetFilteredList(state);
        }

        /// <summary>
        /// Установка нового курса валюты у оплаты по заказу
        /// </summary>
        /// <param name="productionOrderPayment">Оплата</param>
        /// <param name="newCurrencyRate">Новый курс валюты (null - текущий)</param>
        /// <param name="user">Пользователь</param>
        public void ChangeProductionOrderPaymentCurrencyRate(ProductionOrderPayment productionOrderPayment, CurrencyRate newCurrencyRate, User user)
        {
            CheckPossibilityToChangePaymentCurrencyRate(productionOrderPayment, user);

            Currency currency;
            CurrencyRate oldCurrencyRate;
            currencyService.GetCurrencyAndCurrencyRate(productionOrderPayment, out currency, out oldCurrencyRate);

            if (newCurrencyRate == null)
            {
                newCurrencyRate = currencyService.GetCurrentCurrencyRate(currency);
                ValidationUtils.NotNull(newCurrencyRate, "По выбранной валюте не существует текущего курса.");
            }

            if (newCurrencyRate != oldCurrencyRate)
            {
                productionOrderPayment.CurrencyRate = newCurrencyRate;
            }
        }

        #region Права на совершение операций

        #region Вспомогательные методы

        private bool IsPermissionToPerformOperation(ProductionOrderPayment productionOrderPayment, User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Personal:
                    result = productionOrderPayment.ProductionOrder.Curator == user && user.Teams.SelectMany(x => x.ProductionOrders).Contains(productionOrderPayment.ProductionOrder);
                    break;

                case PermissionDistributionType.Teams:
                    result = user.Teams.SelectMany(x => x.ProductionOrders).Contains(productionOrderPayment.ProductionOrder);
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(ProductionOrderPayment productionOrderPayment, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(productionOrderPayment, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        #endregion

        #region Смена курса валюты оплаты

        public bool IsPossibilityToChangeCurrencyRate(ProductionOrderPayment productionOrderPayment, User user)
        {
            try
            {
                CheckPossibilityToChangePaymentCurrencyRate(productionOrderPayment, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToChangePaymentCurrencyRate(ProductionOrderPayment productionOrderPayment, User user)
        {
            // права
            CheckPermissionToPerformOperation(productionOrderPayment, user, Permission.ProductionOrderPayment_Create_Edit);

            // сущность
            productionOrderPayment.CheckPossibilityToChangeCurrencyRate();
            productionOrderPayment.ProductionOrder.CheckPossibilityToEditPayment();
        }

        #endregion

        #endregion

        #endregion
    }
}
