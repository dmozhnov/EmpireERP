using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Infrastructure.NHibernate.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ForeignBankRepository : BaseRepository, IForeignBankRepository
    {
        public ForeignBankRepository()
            : base()
        {
        }

        public ForeignBank GetById(int id)
        {
            return Query<ForeignBank>().Where(x => x.Id == id).FirstOrDefault<ForeignBank>();
        }

        public void Save(ForeignBank entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(ForeignBank entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public ForeignBank GetBySWIFT(string swift)
        {
            return Query<ForeignBank>().Where(x => x.SWIFT == swift).FirstOrDefault<ForeignBank>();
        }


        public IList<ForeignBank> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ForeignBank>(state, ignoreDeletedRows);
        }

        public IList<ForeignBank> GetFilteredList(object state, Utils.ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ForeignBank>(state, parameterString, ignoreDeletedRows);
        }
    }
}
