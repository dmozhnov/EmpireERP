using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IProducerService
    {
        Producer GetById(int id);
        int Save(Producer producer);
        void Delete(Producer producer, User user);
        IEnumerable<Producer> GetFilteredList(object state);
        IEnumerable<Producer> GetFilteredList(object state, ParameterString parameterString);
        Producer CheckProducerExistence(int id);
        ProducerOrganization GetProducerOrganizationByName(string name);
        Manufacturer GetManufacturerByName(string name);
        void AddManufacturer(Producer producer, Manufacturer manufacturer, User user);
        void RemoveManufacturer(Producer producer, Manufacturer manufacturer, User user);
        IList<ProductionOrderPayment> GetPaymentsFilteredList(object state, ParameterString parameterString);
        IList<ProductionOrder> GetProductionOrders(Producer producer);

        bool IsPossibilityToDelete(Producer producer, User user);
        void CheckPossibilityToDelete(Producer producer, User user);
        bool IsPossibilityToRemoveManufacturer(Producer producer, Manufacturer manufacturer,  User user);
        void CheckPossibilityToRemoveManufacturer(Producer producer, Manufacturer manufacturer, User user);
        bool IsPossibilityToAddManufacturer(Producer producer, Manufacturer manufacturer, User user);
        void CheckPossibilityToAddManufacturer(Producer producer, Manufacturer manufacturer, User user);
    }
}
