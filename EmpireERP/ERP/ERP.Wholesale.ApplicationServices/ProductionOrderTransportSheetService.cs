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
    public class ProductionOrderTransportSheetService : IProductionOrderTransportSheetService
    {
        #region Поля

        private readonly IProductionOrderTransportSheetRepository productionOrderTransportSheetRepository;

        #endregion

        #region Конструкторы

        public ProductionOrderTransportSheetService(IProductionOrderTransportSheetRepository productionOrderTransportSheetRepository)
        {
            this.productionOrderTransportSheetRepository = productionOrderTransportSheetRepository;
        } 

        #endregion

        #region Методы

        public IList<ProductionOrderTransportSheet> GetFilteredList(object state, User user, ParameterString parameterString = null)
        {
            if (parameterString == null) parameterString = new ParameterString("");

            var type = user.GetPermissionDistributionType(Permission.ProductionOrderTransportSheet_List_Details);

            switch (type)
            {
                case PermissionDistributionType.None:
                    return new List<ProductionOrderTransportSheet>();

                case PermissionDistributionType.Personal:
                case PermissionDistributionType.Teams:
                    var list = GetAll(user, Permission.ProductionOrderTransportSheet_List_Details).Select(x => x.Id).ToList();

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

            return productionOrderTransportSheetRepository.GetFilteredList(state, parameterString);

        }

        public IEnumerable<ProductionOrderTransportSheet> GetAll(User user, Permission permission)
        {
            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<ProductionOrderTransportSheet>();

                case PermissionDistributionType.Personal:
                    var transportSheets = user.Teams.SelectMany(x => x.ProductionOrders.SelectMany(y => y.TransportSheets));

                    return productionOrderTransportSheetRepository.GetAll().Where(x => x.ProductionOrder.Curator.Id == user.Id).Concat(transportSheets).Distinct();

                case PermissionDistributionType.Teams:
                    return user.Teams.SelectMany(x => x.ProductionOrders.SelectMany(y => y.TransportSheets)).Distinct();

                case PermissionDistributionType.All:
                    return productionOrderTransportSheetRepository.GetAll();

                default:
                    return null;
            }
        }

        
        #region Права на совершение операций

        #region Вспомогательные методы
        
        private bool IsPermissionToPerformOperation(ProductionOrderTransportSheet template, User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;
               
                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(ProductionOrderTransportSheet template, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(template, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        #endregion      

        #endregion

        #endregion
    }
}
