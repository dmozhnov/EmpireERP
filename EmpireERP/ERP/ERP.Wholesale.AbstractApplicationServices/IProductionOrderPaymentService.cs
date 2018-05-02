using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IProductionOrderPaymentService
    {
        ProductionOrderPayment GetById(Guid id);
        ProductionOrderPayment CheckProductionOrderPaymentExistence(Guid id, User user);
        IEnumerable<ProductionOrderPayment> GetAll();
        IEnumerable<ProductionOrderPayment> GetAll(User user, Permission permission);
        IList<ProductionOrderPayment> GetFilteredList(object state);
        IList<ProductionOrderPayment> GetFilteredList(object state, User user, ParameterString parameterString = null);

        /// <summary>
        /// Установка нового курса валюты у оплаты по заказу
        /// </summary>
        /// <param name="productionOrderPayment">Оплата</param>
        /// <param name="newCurrencyRate">Новый курс валюты (null - текущий)</param>
        /// <param name="user">Пользователь</param>
        void ChangeProductionOrderPaymentCurrencyRate(ProductionOrderPayment productionOrderPayment, CurrencyRate newCurrencyRate, User user);

        bool IsPossibilityToChangeCurrencyRate(ProductionOrderPayment productionOrderPayment, User user);
        void CheckPossibilityToChangePaymentCurrencyRate(ProductionOrderPayment productionOrderPayment, User user);
    }
}
