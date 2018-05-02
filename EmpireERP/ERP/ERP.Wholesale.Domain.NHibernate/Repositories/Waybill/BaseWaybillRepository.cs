using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    /// <summary>
    /// Базовый репозиторий для накладных
    /// </summary>
    public class BaseWaybillRepository<T> : BaseNHRepository where T : BaseWaybill
    {
        /// <summary>
        /// Получение накладной по Id
        /// </summary>
        public T GetById(Guid id)
        {
            return Query<T>().Where(x => x.Id == id).FirstOrDefault<T>();
        }

        //public void Save(T entity)
        //{
        //    CurrentSession.SaveOrUpdate(entity);
        //    CurrentSession.Flush();
        //}

        /// <summary>
        /// Получение списка позиций накладной по Id из подзапроса
        /// </summary>
        public IEnumerable<RT> GetRows<RT>(ISubQuery rowsSubQuery) where RT : BaseWaybillRow
        {
            return Query<RT>().PropertyIn(x => x.Id, rowsSubQuery).ToList<RT>();
        }

        /// <summary>
        /// Получение списка накладных по подзапросу
        /// </summary>
        protected Dictionary<Guid, TT> GetList<TT>(ISubQuery waybillSubQuery) where TT : BaseWaybill
        {
            return Query<TT>().PropertyIn(x => x.Id, waybillSubQuery).ToList<TT>().ToDictionary(x => x.Id);
        }
    }
}
