using System;
using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class AccountingPriceListWaybillTakingRepository : BaseRepository, IAccountingPriceListWaybillTakingRepository
    {
        public AccountingPriceListWaybillTakingRepository() : base()
        {
        }

        public AccountingPriceListWaybillTaking GetById(Guid id)
        {
            return CurrentSession.Get<AccountingPriceListWaybillTaking>(id);
        }

        public void Save(AccountingPriceListWaybillTaking value)
        {
            CurrentSession.SaveOrUpdate(value);
        }

        public void Delete(AccountingPriceListWaybillTaking value)
        {
            CurrentSession.Delete(value);
        }

        /// <summary>
        /// Получение списка связей по подзапросу для позиций накладной
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапросу для позиций накладной</param>        
        public IEnumerable<AccountingPriceListWaybillTaking> GetList(ISubQuery waybillRowsSubQuery)
        {
            return Query<AccountingPriceListWaybillTaking>()                
                .PropertyIn(x => x.WaybillRowId, waybillRowsSubQuery)
                .ToList<AccountingPriceListWaybillTaking>();
        }

        /// <summary>
        /// Получение списка связей по подзапросу для позиций накладной
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапросу для позиций накладной</param>
        /// <param name="storageId">Код МХ для фильтрации связей</param>
        /// <param name="accountOrganizationId">Код собственной организации для фильтрации связей</param>
        public IEnumerable<AccountingPriceListWaybillTaking> GetList(ISubQuery waybillRowsSubQuery, short storageId, int accountOrganizationId)
        {
            return Query<AccountingPriceListWaybillTaking>()
                .Where(x => x.AccountOrganizationId == accountOrganizationId && x.StorageId == storageId)
                .PropertyIn(x => x.WaybillRowId, waybillRowsSubQuery)
                .ToList<AccountingPriceListWaybillTaking>();
        }
    }
}
