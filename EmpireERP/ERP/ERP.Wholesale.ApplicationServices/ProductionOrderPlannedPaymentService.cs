using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class ProductionOrderPlannedPaymentService : IProductionOrderPlannedPaymentService
    {
        #region Поля

        private readonly IProductionOrderPlannedPaymentRepository productionOrderPlannedPaymentRepository;

        #endregion

        #region Конструкторы

        public ProductionOrderPlannedPaymentService()
        {
            productionOrderPlannedPaymentRepository = IoCContainer.Resolve<IProductionOrderPlannedPaymentRepository>();
        }

        #endregion

        #region Методы

        #region Список

        public IEnumerable<ProductionOrderPlannedPayment> GetFilteredList(object state, User user, ParameterString parameterString = null)
        {
            if (parameterString == null) parameterString = new ParameterString("");

            if (user.HasPermission(Permission.ProductionOrder_List_Details))
            {
                return productionOrderPlannedPaymentRepository.GetFilteredList(state, parameterString);
            }

            return new List<ProductionOrderPlannedPayment>();
        } 

        #endregion
        
        #endregion
    }
}
