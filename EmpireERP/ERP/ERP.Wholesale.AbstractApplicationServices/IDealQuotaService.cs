using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IDealQuotaService
    {
        DealQuota CheckDealQuotaExistence(int id, User user, string message = "");
        
        IEnumerable<DealQuota> GetFilteredList(object state, ParameterString param, User user);
        IEnumerable<DealQuota> GetActiveDealQuotasList(User user);
        IEnumerable<DealQuota> FilterByUser(IEnumerable<DealQuota> list, User user, Permission permission);

        void Save(DealQuota dealQuota, User user);
        
        void Delete(DealQuota dealQuota, User user);

        void RemoveDealQuotaFromAllDeals(DealQuota quota);

        bool IsPossibilityToCreate(User user);
        bool IsPossibilityToEdit(DealQuota dealQuota, User user, bool checkLogic = true);
        bool IsPossibilityToDelete(DealQuota dealQuota, User user);

        void CheckPossibilityToCreate(User user);
        void CheckPossibilityToEdit(DealQuota dealQuota, User user, bool checkLogic = true);
        void CheckPossibilityToDelete(DealQuota dealQuota, User user);
    }
}
