﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ERP.FrontSite.UI.Web.Controllers;

namespace ERP.FrontSite.UI.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);

            // регистрация маршрутов
            RegisterRoutes(RouteTable.Routes);
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        #region Регистрация маршрутов приложения

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ico/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "{action}/{controller}",
                new { action = "About", controller = "Home" }
            );

            routes.MapRoute(
                "features",
                "features/{tab}",
                new { controller = "Home", tab = 1 }
            );
        }
        #endregion

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            Response.Clear();

            HttpException httpException = exception as HttpException;

            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");

            if (httpException == null)
            {
                routeData.Values.Add("action", "Error");
            }
            else //It's an Http Exception, Let's handle it. 
            {
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // Page not found. 
                        routeData.Values.Add("action", "HttpError404");
                        break;                    

                    // Here you can handle Views to other error codes. 
                    // I choose a General error template   
                    default:
                        routeData.Values.Add("action", "Error");
                        break;
                }
            }

            // Pass exception details to the target error View. 
            routeData.Values.Add("error", exception);

            // Clear the error on server. 
            Server.ClearError();

            // Avoid IIS7 getting in the middle 
            Response.TrySkipIisCustomErrors = true;

            // Call target Controller and pass the routeData. 
            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(
                 new HttpContextWrapper(Context), routeData));
        } 
    }
}