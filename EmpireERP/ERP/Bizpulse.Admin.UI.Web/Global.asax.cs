using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bizpulse.Admin.Domain.AbstractServices;
using Bizpulse.Admin.Domain.NHibernate.Repositories;
using Bizpulse.Admin.Domain.Repositories;
using Bizpulse.Admin.Domain.Services;
using Bizpulse.Infrastructure.Mvc;
using ERP.Infrastructure.IoC;
using ERP.Infrastructure.NHibernate;
using ERP.Infrastructure.NHibernate.SessionManager;
using ERP.Infrastructure.NHibernate.UnitOfWork;
using ERP.Infrastructure.SessionManager;
using ERP.Infrastructure.UnitOfWork;

namespace Bizpulse.Admin.UI.Web
{
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// Менеджер сессий ORM
        /// </summary>
        ISingleDBSessionManager SessionManager = new NHibernateSingleDBSessionManager();

        protected void Application_Start()
        {
            if (AppSettings.DebugMode)
            {
                // инициализация профайлера NHibernate
                HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            }

            ModelBinders.Binders.DefaultBinder = new BizpulseModelBinder();    //Обрезает пробелы в начале и конце строки

            // регистрация глобальных фильтров действий контроллеров
            RegisterGlobalFilters(GlobalFilters.Filters);

            // регистрация маршрутов
            RegisterRoutes(RouteTable.Routes);

            // регистрация фабрики контроллеров
            RegisterControllerFactory();

            // регистрация репозиториев
            RegisterRepositories();

            // регистрация служб
            RegisterServices();

            // регистрация фабрики UOW
            IoCContainer.Register<IUnitOfWorkFactory>(new NHibernateUnitOfWorkFactory());

            // регистрация INHibernateSingleDBInitializer
            IoCContainer.Register<INHibernateSingleDBInitializer>(new FluentInitializer());
        }

        #region Регистрация глобальных фильтров

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        #endregion

        #region Регистрация маршрутов приложения

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ico/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Client", action = "Register", id = UrlParameter.Optional }
            );
        }
        
        #endregion

        #region Регистрация фабрики контроллеров

        /// <summary>
        /// Регистрация фабрики контроллеров
        /// </summary>
        public static void RegisterControllerFactory()
        {
            ControllerBuilder.Current.SetControllerFactory(new LinFuControllerFactory(IoCContainer.Container));
        }

        #endregion

        #region Регистрация репозиториев

        /// <summary>
        /// Регистрация репозиториев
        /// </summary>
        public void RegisterRepositories()
        {
            IoCContainer.RegisterSingleton<IAdministratorRepository, AdministratorRepository>();
            IoCContainer.RegisterSingleton<IRegionRepository, RegionRepository>();
            IoCContainer.RegisterSingleton<ICityRepository, CityRepository>();
            IoCContainer.RegisterSingleton<IClientRepository, ClientRepository>();
            IoCContainer.RegisterSingleton<IClientUserRepository, ClientUserRepository>();
            IoCContainer.RegisterSingleton<IRateRepository, RateRepository>();
        }

        #endregion

        #region Регистрация служб

        /// <summary>
        /// Регистрация служб
        /// </summary>
        public void RegisterServices()
        {
            IoCContainer.RegisterSingleton<IAdministratorService, AdministratorService>();
            IoCContainer.RegisterSingleton<IClientService, ClientService>();
            IoCContainer.RegisterSingleton<IClientUserService, ClientUserService>();
        }

        #endregion

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.CurrentExecutionFilePathExtension != ".css" &&
                Request.CurrentExecutionFilePathExtension != ".js" &&
                Request.CurrentExecutionFilePathExtension != ".png" &&
                Request.CurrentExecutionFilePathExtension != ".gif" &&
                Request.CurrentExecutionFilePathExtension != ".ico")
            {
                SessionManager.CreateSession();
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            SessionManager.DisposeSession();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //var error = Server.GetLastError();
            //Response.Clear();
            //Server.ClearError();

            //string path = Request.Path;

            //var httpException = error as HttpException;
            //if (httpException != null)
            //{
            //    Context.RewritePath(string.Format("~/Error/Http{0}", httpException.GetHttpCode()), false);
            //}
            //else
            //{
            //    Context.RewritePath("~/Error/Error", false);
            //}
                        
            //IHttpHandler httpHandler = new MvcHttpHandler();
            //httpHandler.ProcessRequest(Context);
            //Context.RewritePath(path, false);
        }

    }
}