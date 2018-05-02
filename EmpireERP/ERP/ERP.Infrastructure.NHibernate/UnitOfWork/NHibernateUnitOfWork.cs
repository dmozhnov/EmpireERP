using System.Data;
using ERP.Infrastructure.NHibernate.SessionManager;
using ERP.Infrastructure.UnitOfWork;
using NHibernate;

namespace ERP.Infrastructure.NHibernate.UnitOfWork
{
    internal class NHibernateUnitOfWork : IUnitOfWork
    {
        private readonly ISession session;
        private ITransaction transaction;

        public NHibernateUnitOfWork(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            session = NHibernateSessionManager.CurrentSession;            
            transaction = session.BeginTransaction(isolationLevel);
        }

        #region IUnitOfWork Members

        public void Dispose()
        {
            if (!transaction.WasCommitted && !transaction.WasRolledBack)
            {
                transaction.Rollback();
            }
            transaction.Dispose();
            transaction = null;
        }

        public void Commit()
        {
            transaction.Commit();
            // TODO: подумать, надо ли очищать сессию для получения более актуальных данных из БД, а не из сессии
            // session.Clear();
        }

        #endregion
    }
}