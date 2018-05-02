using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class RussianBankRepository: BaseRepository, IRussianBankRepository
    {
        public RussianBankRepository()
        {
        }

        public RussianBank GetById(int id)
        {
            return Query<RussianBank>().Where(x => x.Id == id).FirstOrDefault<RussianBank>();
        }

        public void Save(RussianBank entity)
        {
            CurrentSession.SaveOrUpdate(entity);   
        }

        public void Delete(RussianBank entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }        

        public RussianBank GetByBIC(string bic)
        {
            return Query<RussianBank>().Where(x => x.BIC == bic).FirstOrDefault<RussianBank>();
        }

        public IList<RussianBank> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return base.GetBaseFilteredList<RussianBank>(state, ignoreDeletedRows);
        }

        public IList<RussianBank> GetFilteredList(object state, Utils.ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return base.GetBaseFilteredList<RussianBank>(state, parameterString, ignoreDeletedRows);
        }
    }
}
