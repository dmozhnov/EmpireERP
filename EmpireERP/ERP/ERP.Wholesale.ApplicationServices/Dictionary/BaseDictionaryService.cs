using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    /// <summary>
    /// Базовый абстрактный класс, представляющий сервис для типовых справочников. Реализует основной общий функционал
    /// </summary>
    /// <typeparam name="T">Тип сущности для справочника</typeparam>
    public abstract class BaseDictionaryService<T> : IBaseDictionaryService<T> where T : BaseDictionary
    {
        #region Поля
        
        /// <summary>
        /// Ссылка на типизированный репозиторий
        /// </summary>
        protected readonly IBaseDictionaryRepository<T> baseDictionaryRepository;
        
        /// <summary>
        /// Строка сообщения об ошибке уникальности элемента
        /// </summary>
        protected abstract string UniquenessErrorString { get; }
        
        /// <summary>
        /// Строка сообщения ошибки при создании о существовании уже такого элемента
        /// </summary>
        protected abstract string CheckExistenceErrorString { get; }

        #region Права на совершение операций

        /// <summary>
        /// Право на создание
        /// </summary>
        public abstract Permission CreationPermission { get; }

        /// <summary>
        /// Право на редактирование
        /// </summary>
        public abstract Permission EditingPermission { get; }

        /// <summary>
        /// Право на удаление
        /// </summary>
        public abstract Permission DeletionPermission { get; }

        /// <summary>
        /// Право на просмотр списка
        /// </summary>
        public abstract Permission ListViewingPermission { get; }

        #endregion

        #endregion

        #region Конструктор

        public BaseDictionaryService(IBaseDictionaryRepository<T> baseDictionaryRepository)
        {
            this.baseDictionaryRepository = baseDictionaryRepository;
        }

        #endregion

        #region Методы

        #region Чтение/редактирование

        /// <summary>
        /// Безусловное получение списка сущностей
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetList()
        {
            return baseDictionaryRepository.GetAll();
        }

        /// <summary>
        /// Получение списка сущностей с проверкой права пользователя
        /// </summary>
        /// <param name="user">Пользователь, для которого проверяется видимость</param>
        /// <returns></returns>
        public IEnumerable<T> GetList(User user)
        {
            if (user.HasPermission(ListViewingPermission))
            {
                return baseDictionaryRepository.GetAll();
            }

            return new List<T>();
        }

        private T GetById(short id, User user)
        {
            if (user.HasPermission(ListViewingPermission))
            {
                return baseDictionaryRepository.GetById(id);
            }

            return null;
        }

        /// <summary>
        /// Проверка существования сущности и видимости данному пользователю с настраиваемым сообщением об ошибке.
        /// </summary>
        /// <param name="id">id сущности</param>
        /// <param name="user">Пользователь</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <returns></returns>
        public T CheckExistence(short id, User user, string message = "")
        {
            T entity = GetById(id, user);
            ValidationUtils.NotNull(entity, String.IsNullOrEmpty(message) ? CheckExistenceErrorString : message);

            return entity;
        }

        /// <summary>
        /// Получение сущности по ID
        /// </summary>
        /// <param name="id">id сущности</param>
        /// <returns></returns>
        public T CheckExistence(short id)
        {
            var entity = baseDictionaryRepository.GetById(id);
            ValidationUtils.NotNull(entity, CheckExistenceErrorString);

            return entity;
        }

        /// <summary>
        /// Получение отфильтрованного списка сущностей
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <returns></returns>
        public IList<T> GetFilteredList(object state)
        {
            return baseDictionaryRepository.GetFilteredList(state);
        }

        /// <summary>
        /// Получение отфильтрованного списка сущностей
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="parameterString">Строка праметров</param>
        /// <returns></returns>
        public IEnumerable<T> GetFilteredList(object state, ParameterString parameterString)
        {
            return baseDictionaryRepository.GetFilteredList(state, parameterString);
        }

        /// <summary>
        /// Сохранить сущность
        /// </summary>
        /// <param name="entity">Сущность для сохранения</param>
        /// <returns></returns>
        public virtual short Save(T entity)
        {
            CheckUniqueness(entity);

            baseDictionaryRepository.Save(entity);

            return entity.Id;
        }

        /// <summary>
        /// Удалить сущность
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <param name="user">Пользователь</param>
        public void Delete(T entity, User user)
        {
            CheckPossibilityToDelete(entity, user);

            baseDictionaryRepository.Delete(entity);
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Проверка на уникальность
        /// </summary>
        /// <param name="entity">Сущность</param>
        public virtual void CheckUniqueness(T entity)
        {
            CheckNameUniqueness(entity.Id, entity.Name);
        }

        /// <summary>
        /// Проверка наименования на уникальность
        /// </summary>
        /// <param name="id">id сущности</param>
        /// <param name="name">Наименование сущности</param>
        public void CheckNameUniqueness(short id, string name)
        {
            int count = baseDictionaryRepository.Query<T>().Where(x => x.Name == name && x.Id != id).Count();

            if (count > 0)
            {
                throw new Exception(UniquenessErrorString);
            }
        }

        #endregion

        #region Возможность выполнения операций

        private bool IsPermissionToPerformOperation(User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        protected void CheckPermissionToPerformOperation(User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        #region Удаление

        /// <summary>
        /// Проверка возможности удалить сущность.
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <param name="user">Пользователь</param>
        /// <param name="checkLogic">Проверять ли логику</param>
        /// <returns></returns>
        public virtual bool IsPossibilityToDelete(T entity, User user, bool checkLogic = true)
        {
            try
            {
                CheckPossibilityToDelete(entity, user, checkLogic);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка возможности удалить сущность. При невозможности - выбрасывается исключение.
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <param name="user">Пользователь</param>
        /// <param name="checkLogic">Проверять ли логику</param>
        protected abstract void CheckPossibilityToDelete(T entity, User user, bool checkLogic = true);

        #endregion

        #endregion

        #endregion
    }
}