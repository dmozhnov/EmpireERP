using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Infrastructure.UnitOfWork;
using System.Data;
using Bizpulse.Admin.Domain.AbstractServices;
using ERP.Infrastructure.Security;
using Bizpulse.Admin.UI.Web.Infrastructure;

namespace Bizpulse.Admin.UI.Web.Controllers
{
    public class ClientUserController : BaseAdminController
    {
        #region Свойства

        private readonly IClientUserService clientUserService;

        #endregion

        #region Конструкторы

        public ClientUserController(IClientUserService clientUserService)
        {
            this.clientUserService = clientUserService;
        }

        #endregion

        #region Методы

        #endregion

        [HttpPost]
        public ActionResult TryLogin(string accountNumber, string login, string passwordHash)
        {
            try
            {
                using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
                {
                    var clientUser = clientUserService.TryLogin(accountNumber, login, passwordHash);
                    
                    // если пользователь успешно аутентифицирован
                    if (clientUser != null)
                    {
                        var clientUserInfo = new UserInfo()
                        {
                            Id = clientUser.Id,
                            DisplayName = clientUser.FirstName + " " + clientUser.LastName,
                            Login = clientUser.Login,
                            PasswordHash = clientUser.PasswordHash,
                            ClientAccountId = clientUser.Client.Id, 
                            IsSystemAdmin = clientUser.IsClientAdmin
                        };

                        UserSession.CurrentClientAdministratorInfo = clientUserInfo;
                        UserSession.AlreadyEntered = true;
                    }

                    return new RedirectResult("~/Client/Details");
                }
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }            
        }
        
    }
}
