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
    public class ProductionOrderCustomsDeclarationService : IProductionOrderCustomsDeclarationService
    {
        #region Поля

        private readonly IProductionOrderCustomsDeclarationRepository productionOrderCustomsDeclarationRepository;

        #endregion

        #region Конструкторы

        public ProductionOrderCustomsDeclarationService(IProductionOrderCustomsDeclarationRepository productionOrderCustomsDeclarationRepository)
        {
            this.productionOrderCustomsDeclarationRepository = productionOrderCustomsDeclarationRepository;
        }

        #endregion

        #region Методы

        public IList<ProductionOrderCustomsDeclaration> GetFilteredList(object state, User user, ParameterString parameterString = null)
        {
            if (parameterString == null) parameterString = new ParameterString("");

            var type = user.GetPermissionDistributionType(Permission.ProductionOrderCustomsDeclaration_List_Details);

            switch (type)
            {
                case PermissionDistributionType.None:
                    return new List<ProductionOrderCustomsDeclaration>();

                case PermissionDistributionType.Personal:
                case PermissionDistributionType.Teams:
                    var list = GetAll(user, Permission.ProductionOrderCustomsDeclaration_List_Details).Select(x => x.Id).ToList();

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

            return productionOrderCustomsDeclarationRepository.GetFilteredList(state, parameterString);

        }

        public IEnumerable<ProductionOrderCustomsDeclaration> GetAll(User user, Permission permission)
        {
            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<ProductionOrderCustomsDeclaration>();

                case PermissionDistributionType.Personal:
                    var CustomsDeclarations = user.Teams.SelectMany(x => x.ProductionOrders.SelectMany(y => y.CustomsDeclarations));

                    return productionOrderCustomsDeclarationRepository.GetAll().Where(x => x.ProductionOrder.Curator.Id == user.Id).Concat(CustomsDeclarations).Distinct();

                case PermissionDistributionType.Teams:
                    return user.Teams.SelectMany(x => x.ProductionOrders.SelectMany(y => y.CustomsDeclarations)).Distinct();

                case PermissionDistributionType.All:
                    return productionOrderCustomsDeclarationRepository.GetAll();

                default:
                    return null;
            }
        }

        #endregion
    }
}
