using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class ProductionOrderExtraExpensesSheetService : IProductionOrderExtraExpensesSheetService
    {
        #region Поля

        private readonly IProductionOrderExtraExpensesSheetRepository productionOrderExtraExpensesSheetRepository;

        #endregion

        #region Конструкторы

        public ProductionOrderExtraExpensesSheetService(IProductionOrderExtraExpensesSheetRepository productionOrderExtraExpensesSheetRepository)
        {
            this.productionOrderExtraExpensesSheetRepository = productionOrderExtraExpensesSheetRepository;
        }

        #endregion

        #region Методы

        public IList<ProductionOrderExtraExpensesSheet> GetFilteredList(object state, User user, ParameterString parameterString = null)
        {
            if (parameterString == null) parameterString = new ParameterString("");

            var type = user.GetPermissionDistributionType(Permission.ProductionOrderExtraExpensesSheet_List_Details);

            switch (type)
            {
                case PermissionDistributionType.None:
                    return new List<ProductionOrderExtraExpensesSheet>();

                case PermissionDistributionType.Personal:
                case PermissionDistributionType.Teams:
                    var list = GetAll(user, Permission.ProductionOrderExtraExpensesSheet_List_Details).Select(x => x.Id).ToList();

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

            return productionOrderExtraExpensesSheetRepository.GetFilteredList(state, parameterString);

        }

        public IEnumerable<ProductionOrderExtraExpensesSheet> GetAll(User user, Permission permission)
        {
            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<ProductionOrderExtraExpensesSheet>();

                case PermissionDistributionType.Personal:
                    var extraExpensesSheets = user.Teams.SelectMany(x => x.ProductionOrders.SelectMany(y => y.ExtraExpensesSheets));

                    return productionOrderExtraExpensesSheetRepository.GetAll().Where(x => x.ProductionOrder.Curator.Id == user.Id).Concat(extraExpensesSheets).Distinct();

                case PermissionDistributionType.Teams:
                    return user.Teams.SelectMany(x => x.ProductionOrders.SelectMany(y => y.ExtraExpensesSheets)).Distinct();

                case PermissionDistributionType.All:
                    return productionOrderExtraExpensesSheetRepository.GetAll();

                default:
                    return null;
            }
        }

        #endregion
    }
}