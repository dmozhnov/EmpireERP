using System.Collections.Generic;
using System.Web;
using ERP.Infrastructure.IoC;
using ERP.Infrastructure.SessionManager;
using NHibernate;

namespace ERP.Infrastructure.NHibernate.SessionManager
{
    public class NHibernateSessionManager : ISessionManager
    {
        private static readonly object lockObject = new object();                
        private static IDictionary<string, ISessionFactory> sessionFactoryList = new Dictionary<string, ISessionFactory>();

        public static ISession CurrentSession
        {
            get
            {
                return (ISession)HttpContext.Current.Items["NHibernateSession"];
            }
            set
            {
                HttpContext.Current.Items["NHibernateSession"] = value;
            }
        }
        
        private static ISessionFactory GetSessionFactory(string dbServer, string dbName)
        {
            // фабрика сессий создается для каждой пары dbServer и dbName            
            // если фабрика уже создана - получаем ее из коллекции
            if (sessionFactoryList.ContainsKey(dbServer + "/" + dbName))
            {
                return sessionFactoryList[dbServer + "/" + dbName];
            }
            // иначе - создаем новую фабрику сессий и записываем ее в коллекцию
            else
            {
                lock (lockObject)
                {
                    ISessionFactory sessionFactory = CreateSessionFactory(dbServer, dbName);

                    sessionFactoryList[dbServer + "/" + dbName] = sessionFactory;

                    return sessionFactory;
                }                
            }
        }
        
        private static ISessionFactory CreateSessionFactory(string dbServer, string dbName)
        {
            lock (lockObject)
            {
                return IoCContainer.Resolve<INHibernateInitializer>().GetConfiguration(dbServer, dbName).BuildSessionFactory();
            }
        }       

        void ISessionManager.CreateSession(string dbServer, string dbName)
        {
            CurrentSession = GetSessionFactory(dbServer, dbName).OpenSession();
            CurrentSession.FlushMode = FlushMode.Commit;
        }

        void ISessionManager.DisposeSession()
        {
            if (CurrentSession != null)
            {
                CurrentSession.Dispose();
            }
        }
    }
}
