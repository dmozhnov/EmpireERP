using System;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface ITaskPresenter
    {
        /// <summary>
        /// Список задач
        /// </summary>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        TaskListViewModel List(UserInfo currentUser);

        /// <summary>
        /// Получение модели для грида новых задач
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        GridData GetNewTaskGrid(GridState state, UserInfo currentUser);
        

        /// <summary>
        /// Получение модели для грида задач в работе
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        GridData GetExecutionTaskGrid(GridState state, UserInfo currentUser);

        /// <summary>
        /// Получение модели для грида завершенных задач
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        GridData GetCompletedTaskGrid(GridState state, UserInfo currentUser);
        
        /// <summary>
        /// Получение модели для заполнения combobox статусов
        /// </summary>
        /// <param name="taskTypeId">Идентификатор типа задачи</param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        object GetStates(short? taskTypeId, UserInfo currentUser);

        /// <summary>
        /// Создание новой задачи
        /// </summary>
        /// <param name="backUrl">Адрес возврата</param>
        /// <param name="executedById">Идентификатор ответственного за выполнение задачи</param>
        /// <param name="dealId">Идентификатор связанной сделки</param>
        /// <param name="contractorId">Идентификатор связанного контагента</param>
        /// <param name="productionOrderId">Идентификатор связанного заказа на производство</param>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <returns></returns>
        TaskEditViewModel Create(string backUrl, int? executedById, int? dealId, int? contractorId, Guid? productionOrderId, UserInfo currentUser);

        /// <summary>
        /// Сохранение задачи
        /// </summary>
        /// <param name="model">Модель</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Идентификатор сохраненной задачи</returns>
        string Save(TaskEditViewModel model, UserInfo currentUser);

        /// <summary>
        /// Детлаи задачи
        /// </summary>
        /// <param name="taskId">Идентификатор задачи</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        TaskDetailsViewModel Details(int taskId, string backUrl, UserInfo currentUser);

        /// <summary>
        /// История исполнения задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        TaskExecutionsViewModel GetTaskExecutions(int taskId, UserInfo currentUser);

        /// <summary>
        /// История задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        TaskHistoryViewModel GetTaskHistory(int taskId, UserInfo currentUser);

        /// <summary>
        /// Редактирование задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="backUrl"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        TaskEditViewModel Edit(int taskId, string backUrl, UserInfo currentUser);

        /// <summary>
        /// Создание исполнения
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="isCompleteTask">Признак завершения задачи</param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        TaskExecutionEditViewModel TaskExecutionCreate(int taskId, bool isCompleteTask, UserInfo currentUser);


        /// <summary>
        /// Сохранение исполнения
        /// </summary>
        /// <param name="model"></param>
        object TaskExecutionSave(TaskExecutionEditViewModel model, UserInfo currentUser);

        /// <summary>
        /// Одно исполнение задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskExecutionId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        TaskExecutionsViewModel GetTaskExecution(int taskId, int taskExecutionId, UserInfo currentUser);

        /// <summary>
        /// История задачи начиная с указанной даты
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskExecutionId">исполнение, для которого выводится история</param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        TaskHistoryViewModel GetTaskHistoryForTaskExecution(int taskId, int taskExecutionId, UserInfo currentUser);

        /// <summary>
        /// Реадктирвоание исполнения
        /// </summary>
        /// <param name="taskExecutionId">Исполнение</param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        TaskExecutionEditViewModel TaskExecutionEdit(int taskExecutionId, UserInfo currentUser);

        /// <summary>
        /// Удаление исполнения
        /// </summary>
        /// <param name="taskExecutionId"></param>
        /// <param name="message">Сообщение</param>
        /// <param name="currentUser"></param>
        void TaskExecutionDelete(int taskExecutionId, out string message, UserInfo currentUser);

        /// <summary>
        /// Удаление задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="currentUser"></param>
        void Delete(int taskId, UserInfo currentUser);

        /// <summary>
        /// Получение данных о клиенте по сделке
        /// </summary>
        /// <param name="dealId">Идентификатор сделки</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>имя и код клиента</returns>
        object GetClientByDeal(int dealId, UserInfo currentUser);

        /// <summary>
        /// Получение изменяемых индикаторов главных деталей
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        object GetMainChangeableIndicators(int taskId, UserInfo currentUser);
    }
}
