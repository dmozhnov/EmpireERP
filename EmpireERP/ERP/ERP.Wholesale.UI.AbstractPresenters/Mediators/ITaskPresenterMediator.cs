using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.UI.AbstractPresenters.Mediators
{
    public interface ITaskPresenterMediator
    {
        #region Связанные сущности
        
        #region Поставщик

        /// <summary>
        /// Формирование грида для поставщика
        /// </summary>
        /// <param name="provider">Поставщик, для которого выводится грид</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        GridData GetTaskGridForProvider(Provider provider, User user);
        
        /// <summary>
        /// Формирование грида для поставщика
        /// </summary>
        /// <param name="provider">Поставщик, для которого выводится грид</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        GridData GetTaskGridForProvider(GridState state, User user);

        #endregion

        #region Производитель
        
        /// <summary>
        /// Формирование грида для производителя
        /// </summary>
        /// <param name="producer">Производитель, для которого выводится грид</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        GridData GetTaskGridForProducer(Producer producer, User user);

        /// <summary>
        /// Формирование грида для производителя
        /// </summary>
        /// <param name="provider">Производителя, для которого выводится грид</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        GridData GetTaskGridForProducer(GridState state, User user);

        #endregion

        #region Клиент
        
        /// <summary>
        /// Формирование грида для клиента
        /// </summary>
        /// <param name="client">Клиент, для которого выводится грид</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        GridData GetTaskGridForClient(Client client, User user);

        /// <summary>
        /// Формирование грида для клиента
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        GridData GetTaskGridForClient(GridState state, User user);

        #endregion

        #region Сделка

        /// <summary>
        /// Формирование грида для сделок
        /// </summary>
        /// <param name="deal">Сделка, для которой выводится грид</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        GridData GetTaskGridForDeal(Deal deal, User user);

        /// <summary>
        /// Формирование грида для клиента
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        GridData GetTaskGridForDeal(GridState state, User user);

        #endregion

        #region Заказ

        /// <summary>
        /// Формирование грида для заказов
        /// </summary>
        /// <param name="productionOrder">Заказ, для которого выводится грид</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        GridData GetTaskGridForProductionOrder(ProductionOrder productionOrder, User user);

        /// <summary>
        /// Формирование грида для заказов
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        GridData GetTaskGridForProductionOrder(GridState state, User user);

        #endregion

        #endregion

        #region Пользователи

        #region Детали пользователя

        /// <summary>
        /// Получение грида задач для пользователя
        /// </summary>
        /// <param name="forUser">Пользователь, для которого ыводятся задачи</param>
        /// <param name="stateType">Статус задач для вывода</param>
        /// <param name="user">Текущий пользователь</param>
        /// <returns></returns>
        GridData GetTaskGridForUser(User forUser, TaskExecutionStateType stateType, User user);
        
        /// <summary>
        /// Получение грида задач для пользователя
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="user">Текущий пользователь</param>
        /// <returns></returns>
        GridData GetTaskGridForUser(GridState state, User user);

        #endregion

        #region Домашняя страница
        
        /// <summary>
        /// Получение грида задач для домашней страницы пользователя
        /// </summary>
        /// <param name="userAsExecutor">Признак вывода грида где пользователь исполнитель. False - пользователь автор задачи</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        GridData GetTaskGridForUserHomePage(bool userAsExecutor, User user);

        /// <summary>
        /// Получение грида задач для домашней страницы пользователя
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        GridData GetTaskGridForUserHomePage(GridState state, User user);

        #endregion

        #endregion
    }
}
