using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.Web.Infrastructure;
using ERP.Wholesale.UI.ViewModels;

namespace ERP.Wholesale.UI.Web.Controllers
{
    /// <summary>
    /// Контроллер задач
    /// </summary>
    [OutputCache(Location = OutputCacheLocation.None)]
    public class TaskController : WholesaleController
    {
        #region Поля

        private readonly ITaskPresenter taskPresenter;

        #endregion

        #region Конструкторы

        public TaskController(ITaskPresenter taskPresenter)
        {
            this.taskPresenter = taskPresenter;
        }

        #endregion

        #region Методы
        
        
        public ActionResult List()
        {
            try
            {
                var model = taskPresenter.List(UserSession.CurrentUserInfo);

                model.NewTaskGrid.GridPartialViewAction = "/Task/ShowNewTaskGrid/";
                model.NewTaskGrid.HelpContentUrl = "/Help/GetHelp_Task_List_NewTaskGrid";

                model.ExecutingTaskGrid.GridPartialViewAction = "/Task/ShowExecutingTaskGrid/";
                model.ExecutingTaskGrid.HelpContentUrl = "/Help/GetHelp_User_Details_ExecutingTaskGrid";

                model.CompletedTaskGrid.GridPartialViewAction = "/Task/ShowCompletedTaskGrid/";
                model.CompletedTaskGrid.HelpContentUrl = "/Help/GetHelp_User_Details_CompletedTaskGrid";

                return View("List", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowNewTaskGrid(GridState state)
        {
            try
            {
                var model = taskPresenter.GetNewTaskGrid(state, UserSession.CurrentUserInfo);
                model.GridPartialViewAction = "/Task/ShowNewTaskGrid/";

                return PartialView("NewTaskGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowExecutingTaskGrid(GridState state)
        {
            try
            {
                var model = taskPresenter.GetExecutionTaskGrid(state, UserSession.CurrentUserInfo);
                model.GridPartialViewAction = "/Task/ShowExecutingTaskGrid/";

                return PartialView("ExecutingTaskGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowCompletedTaskGrid(GridState state)
        {
            try
            {
                var model = taskPresenter.GetCompletedTaskGrid(state, UserSession.CurrentUserInfo);
                model.GridPartialViewAction = "/Task/ShowCompletedTaskGrid/";

                return PartialView("CompletedTaskGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение спика статусов для приоритета
        /// </summary>
        /// <param name="taskTypeId">Тип задачи</param>
        /// <returns></returns>
        public ActionResult GetStates(short? taskTypeId)
        {
            try
            {
                object obj = taskPresenter.GetStates(taskTypeId, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Создание задачи
        /// </summary>
        /// <param name="backURL"></param>
        /// <returns></returns>
        public ActionResult Create(string backURL, int? executedById, int? dealId, int? contractorId, Guid? productionOrderId)
        {
            try
            {
                return View("Edit", taskPresenter.Create(backURL, executedById, dealId, contractorId, productionOrderId, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="backURL"></param>
        /// <returns></returns>
        public ActionResult Edit(int taskId, string backURL)
        {
            try
            {
                return View("Edit", taskPresenter.Edit(taskId, backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Сохранение задачи
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(TaskEditViewModel model)
        {
            try
            {
                var result = taskPresenter.Save(model, UserSession.CurrentUserInfo);

                return Content(result); 
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение данных о клиенте по сделке
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <returns></returns>
        public ActionResult GetClientByDeal(int dealId)
        {
            try
            {
                return Json(taskPresenter.GetClientByDeal(dealId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Детали задачи
        /// </summary>
        /// <param name="id">Идентификатор задачи</param>
        /// <returns></returns>
        public ActionResult Details(int id, string backURL)
        {
            try
            {
                return View("Details", taskPresenter.Details(id, backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult GetTaskExecutions(int taskId)
        {
            try
            {
                return View("TaskExecutions", taskPresenter.GetTaskExecutions(taskId, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult GetTaskExecution(int taskId, int taskExecutionId)
        {
            try
            {
                return View("TaskExecutions", taskPresenter.GetTaskExecution(taskId, taskExecutionId, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult GetTaskHistory(int taskId)
        {
            try
            {
                return View("TaskHistory", taskPresenter.GetTaskHistory(taskId, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult GetTaskHistoryForTaskExecution(int taskId, int taskExecutionId)
        {
            try
            {
                return View("TaskHistory", taskPresenter.GetTaskHistoryForTaskExecution(taskId, taskExecutionId, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult TaskExecutionCreate(int taskId)
        {
            try
            {
                return View("TaskExecutionCreate", taskPresenter.TaskExecutionCreate(taskId, false, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult TaskExecutionSave(TaskExecutionEditViewModel model)
        {
            try
            {
                var obj = taskPresenter.TaskExecutionSave(model, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult TaskExecutionEdit(int taskExecutionId)
        {
            try
            {
                return View("TaskExecutionEdit", taskPresenter.TaskExecutionEdit(taskExecutionId,  UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult CompleteTask(int taskId)
        {
            try
            {
                return View("TaskExecutionCreate", taskPresenter.TaskExecutionCreate(taskId, true, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult TaskExecutionDelete(int taskExecutionId)
        {
            try
            {
                string message;
                taskPresenter.TaskExecutionDelete(taskExecutionId, out message, UserSession.CurrentUserInfo);

                return Content(message);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Delete(int taskId)
        {
            try
            {
                taskPresenter.Delete(taskId, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult GetMainChangeableIndicators(int taskId)
        {
            try
            {
                var obj = taskPresenter.GetMainChangeableIndicators(taskId, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
    }
}
