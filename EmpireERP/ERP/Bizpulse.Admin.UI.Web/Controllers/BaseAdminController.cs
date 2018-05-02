using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Infrastructure.UnitOfWork;
using ERP.Infrastructure.IoC;

namespace Bizpulse.Admin.UI.Web.Controllers
{
    public abstract class BaseAdminController : Controller
    {
        #region Поля
            
        protected readonly IUnitOfWorkFactory unitOfWorkFactory;
        
        #endregion

        #region Конструкторы

        protected BaseAdminController()
        {
            unitOfWorkFactory = IoCContainer.Resolve<IUnitOfWorkFactory>();
        }

        #endregion

        /// <summary>
        /// Обработка исключения
        /// </summary>
        /// <param name="ex">Отловленное исключение</param>
        /// <returns>Сообщение об ошибке для пользователя</returns>
        public string ProcessException(Exception ex)
        {
            Response.StatusCode = 500;

            string messageForUser = "";

            switch (ex.GetBaseException().GetType().Name)
            {
                case "SqlException":
                    messageForUser = (AppSettings.DebugMode ? ex.Message + ". " + ex.InnerException.Message : "Ошибка связи с сервером базы данных.");
                    break;

                default:
                    messageForUser = ex.Message;
                    break;
            }

            return messageForUser;
        }
    }
}