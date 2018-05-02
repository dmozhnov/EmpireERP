using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ERP.Wholesale.Domain.Entities;
using NHibernate.Criterion;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Repositories;
using ERP.Utils;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class MeasureUnitRepository : BaseRepository, IMeasureUnitRepository
    {      
        public MeasureUnitRepository() : base()
        {
        }

        public MeasureUnit GetById(short Id)
        {
            return CurrentSession.Get<MeasureUnit>(Id);
        }

        public void Save(MeasureUnit Value)
        {
            CurrentSession.SaveOrUpdate(Value);                        
        }

        public void Delete(MeasureUnit Value)
        {
            CurrentSession.Delete(Value);            
        }

        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        public IList<MeasureUnit> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<MeasureUnit>(state, ignoreDeletedRows);
        }

        public IList<MeasureUnit> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<MeasureUnit>(state, parameterString, ignoreDeletedRows);
        }
    }
}
