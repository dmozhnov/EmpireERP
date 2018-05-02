using System;
using System.Web;
using System.Web.Mvc;
using ERP.Infrastructure.IoC;
using ERP.Utils;
using ERP.Wholesale.Settings;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.LogItem;

namespace ERP.Wholesale.UI.Web.Infrastructure
{
    /// <summary>
    /// Базовый контроллер приложения для опта
    /// </summary>
    public class WholesaleController : Controller
    {
        #region Поля

        private readonly ILogItemPresenter logItemPresenter; 

        #endregion

        #region Конструкторы

        public WholesaleController()
        {
            logItemPresenter = IoCContainer.Resolve<ILogItemPresenter>();
        }

        #endregion

        #region Передача файлов

        protected override FileContentResult File(byte[] fileContents, string contentType, string fileDownloadName)
        {
            if (Request.Cookies["FileDownloadCompleted"] != null)
                Response.Cookies["FileDownloadCompleted"].Expires = DateTime.Now.AddYears(-1);
            Response.SetCookie(new HttpCookie("FileDownloadCompleted", "true") { Path = "/" });

            return base.File(fileContents, contentType, fileDownloadName);
        }

        protected FileContentResult ExcelFile(byte[] fileContents, string fileDownloadName)
        {
            return this.File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileDownloadName);
        }
        #endregion

        /// <summary>
        /// Обработка исключения
        /// </summary>
        /// <param name="ex">Отловленное исключение</param>
        /// <returns>Сообщение об ошибке для пользователя</returns>
        protected string ProcessException(Exception ex)
        {
            var curentDateTime = DateTimeUtils.GetCurrentDateTime();
            
            Response.StatusCode = 500;

            string messageForUser = "";
            string systemMessage = ex.Message + (ex.InnerException != null ? ". " + ex.InnerException.Message: "");

            switch (ex.GetBaseException().GetType().Name)
            {
                case "SqlException":
                    messageForUser = (AppSettings.DebugMode ? systemMessage : "Ошибка связи с сервером базы данных. Повторите попытку через несколько минут.");
                    break;

                default:
                    messageForUser = ex.Message;
                    break;
            }
            
            var userId = (UserSession.CurrentUserInfo == null ? (int?)null : UserSession.CurrentUserInfo.Id);
            /*
            logItemPresenter.Save(
                new LogItemEditViewModel()
                {
                    Time = curentDateTime,
                    UserId = userId,
                    Url = Request.Url.ToString(),
                    UserMessage = messageForUser,
                    // если сообщения равны - храним только сообщение для пользователя
                    SystemMessage = (systemMessage == messageForUser ? "" : systemMessage)
                });
            */
            return messageForUser;
        }        
    }
}