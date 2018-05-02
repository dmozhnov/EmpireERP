using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    /// <summary>
    /// Интерфейс базового абстрактного класса, представляющий презентер для типовых справочников. Реализует основной общий функционал
    /// </summary>
    /// <typeparam name="T">Тип сущности для справочника</typeparam>
    public interface IBaseDictionaryService<T>
    {
        /// <summary>
        /// Право на создание
        /// </summary>
        Permission CreationPermission { get; }

        /// <summary>
        /// Право на редактирование
        /// </summary>
        Permission EditingPermission { get; }

        /// <summary>
        /// Право на удаление
        /// </summary>
        Permission DeletionPermission { get; }

        /// <summary>
        /// Право на просмотр списка
        /// </summary>
        Permission ListViewingPermission { get; }

        /// <summary>
        /// Безусловное получение списка сущностей
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetList();

        /// <summary>
        /// Получение списка сущностей с проверкой права пользователя
        /// </summary>
        /// <param name="user">Пользователь, для которого проверяется видимость</param>
        /// <returns></returns>
        IEnumerable<T> GetList(User user);

        /// <summary>
        /// Проверка существования сущности и видимости данному пользователю с настраиваемым сообщением об ошибке.
        /// </summary>
        /// <param name="id">id сущности</param>
        /// <param name="user">Пользователь</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <returns></returns>
        T CheckExistence(short id, User user, string message = "");

        /// <summary>
        /// Получение сущности по ID
        /// </summary>
        /// <param name="id">id сущности</param>
        /// <returns></returns>
        T CheckExistence(short id);

        /// <summary>
        /// Получение отфильтрованного списка сущностей
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <returns></returns>
        IList<T> GetFilteredList(object state);

        /// <summary>
        /// Получение отфильтрованного списка сущностей
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="parameterString">Строка праметров</param>
        /// <returns></returns>
        IEnumerable<T> GetFilteredList(object state, ParameterString parameterString);

        /// <summary>
        /// Сохранить сущность
        /// </summary>
        /// <param name="entity">Сущность для сохранения</param>
        /// <returns></returns>
        short Save(T entity);

        /// <summary>
        /// Удалить сущность
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <param name="user">Пользователь</param>
        void Delete(T entity, User user);

        /// <summary>
        /// Проверка наименования на уникальность
        /// </summary>
        /// <param name="id">id сущности</param>
        /// <param name="name">Наименование сущности</param>
        void CheckNameUniqueness(short id, string name);

        /// <summary>
        /// Проверка на уникальность
        /// </summary>
        /// <param name="entity">Сущность</param>
        void CheckUniqueness(T entity);

        /// <summary>
        /// Проверка возможности удалить сущность.
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <param name="user">Пользователь</param>
        /// <param name="checkLogic">Проверять ли логику</param>
        /// <returns></returns>
        bool IsPossibilityToDelete(T entity, User user, bool checkLogic = true);
    }
}