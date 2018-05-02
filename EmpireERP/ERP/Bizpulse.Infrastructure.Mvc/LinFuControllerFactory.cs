using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LinFu.IoC;

namespace Bizpulse.Infrastructure.Mvc
{
    /// <summary>
    /// Фабрика контроллеров (с использованием IoC LinFU)
    /// </summary>
    public class LinFuControllerFactory : DefaultControllerFactory
    {
        protected ServiceContainer Container { get; private set; }

        public LinFuControllerFactory(ServiceContainer serviceContainer)
        {
            if (serviceContainer == null)
            {
                throw new ArgumentNullException("serviceContainer");
            }
            this.Container = serviceContainer;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(404, string.Format(
                    "The controller for path '{0}' could not be found or it does not implement IController.",
                    requestContext.HttpContext.Request.Path));
            }
            
            return (IController)controllerType.AutoCreateFrom(this.Container);
        }
    }
}