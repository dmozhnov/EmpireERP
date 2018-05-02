using System.Web;
using ERP.Infrastructure.IoC;
using ERP.Infrastructure.SessionManager;
using NHibernate;
using NHibernate.Cfg;

namespace ERP.Infrastructure.NHibernate.SessionManager
{
    public class NHibernateSingleDBSessionManager : ISingleDBSessionManager
    {
        private readonly object lockObject = new object();
        
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
                
        private ISessionFactory SessionFactory
        {
            get
            {
                if (sessionFactory == null)
                {
                    lock (lockObject)
                    {
                        if (sessionFactory == null)
                        {
                            sessionFactory = Configuration.BuildSessionFactory();
                        }
                    }
                }

                return sessionFactory;
            }
        }
        private ISessionFactory sessionFactory;
        
        private Configuration Configuration
        {
            get
            {
                if (configuration == null)
                {
                    lock (lockObject)
                    {
                        if (configuration == null)
                        {
                            configuration = IoCContainer.Resolve<INHibernateSingleDBInitializer>().GetConfiguration();
                        }
                    }
                }

                return configuration;
            }
        }
        private Configuration configuration;
   
        public void CreateSession()
        {
            CurrentSession = SessionFactory.OpenSession();
            CurrentSession.FlushMode = FlushMode.Commit;
        }

        public void DisposeSession()
        {
            if (CurrentSession != null)
            {
                CurrentSession.Dispose();
            }
        }
    }
}
