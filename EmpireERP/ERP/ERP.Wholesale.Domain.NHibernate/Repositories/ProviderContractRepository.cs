using System;
using System.Linq;
using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{    
    public class ProviderContractRepository : BaseRepository, IProviderContractRepository
    {
        public ProviderContractRepository()
            : base()
        {
        }

        public ProviderContract GetById(short id)
        {
            return CurrentSession.Get<ProviderContract>(id);
        }

        public void Save(ProviderContract value)
        {
            CurrentSession.SaveOrUpdate(value);                
        }

        public void Delete(ProviderContract value)
        {
            value.DeletionDate = DateTime.Now;
            CurrentSession.SaveOrUpdate(value);                
        }

        public IEnumerable<ProviderContract> GetAll()
        {
            return CurrentSession.CreateCriteria(typeof(ProviderContract)).List<ProviderContract>();
        }

        /// <summary>
        /// Есть ли по указанному договору какие-нибудь приходы
        /// </summary>
        public bool AnyReceipts(ProviderContract contract)
        {
            return CurrentSession.Query<ReceiptWaybill>().Any(x => x.ProviderContract.Id == contract.Id && x.DeletionDate == null);
        }
    }
}
