using System.Data;
using ERP.Infrastructure.UnitOfWork;


namespace ERP.Infrastructure.NHibernate.UnitOfWork
{
    public class NHibernateUnitOfWorkFactory : IUnitOfWorkFactory
    {
        #region IUnitOfWorkFactory Members

        public IUnitOfWork Create(IsolationLevel isolationLevel)
        {
            return new NHibernateUnitOfWork(isolationLevel);
        }

        public IUnitOfWork Create()
        {
            return Create(IsolationLevel.ReadCommitted);
        }

        #endregion
    }
}