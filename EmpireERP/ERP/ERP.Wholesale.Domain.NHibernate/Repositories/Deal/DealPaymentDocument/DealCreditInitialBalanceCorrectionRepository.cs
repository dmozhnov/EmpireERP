using System;
using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class DealCreditInitialBalanceCorrectionRepository : BaseNHRepository, IDealCreditInitialBalanceCorrectionRepository
    {
        public DealCreditInitialBalanceCorrectionRepository()
        {
        }

        public DealCreditInitialBalanceCorrection GetById(Guid id)
        {
            return Query<DealCreditInitialBalanceCorrection>().Where(x => x.Id == id).FirstOrDefault<DealCreditInitialBalanceCorrection>();
        }

        public void Save(DealCreditInitialBalanceCorrection value)
        {
            CurrentSession.SaveOrUpdate(value);
            CurrentSession.Flush();
        }

        public void Delete(DealCreditInitialBalanceCorrection value)
        {
            CurrentSession.SaveOrUpdate(value);
        }

        public IList<DealCreditInitialBalanceCorrection> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<DealCreditInitialBalanceCorrection>(state, ignoreDeletedRows);
        }
        public IList<DealCreditInitialBalanceCorrection> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<DealCreditInitialBalanceCorrection>(state, parameterString, ignoreDeletedRows);
        }
    }
}
