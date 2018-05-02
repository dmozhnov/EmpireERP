using System;
using ERP.Infrastructure.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.Entities
{
    public class Employee : Entity<int>
    {
        #region Свойства
        
        /// <summary>
        /// Пользователь системы, связанный с сотрудником
        /// </summary>
        public virtual User User { get; protected internal set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        /// <remarks>не более 100</remarks>
        public virtual string LastName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        /// <remarks>не более 100</remarks>
        public virtual string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        /// <remarks>не более 100</remarks>
        public virtual string Patronymic { get; set; }

        /// <summary>
        /// ФИО
        /// </summary>
        public virtual string FullName
        {
            get { return LastName + " " + FirstName + " " + Patronymic; }
        }

        /// <summary>
        /// Должность
        /// </summary>
        public virtual EmployeePost Post { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Кто создал
        /// </summary>
        public virtual User CreatedBy { get; protected set; }
        
        #endregion

        #region Конструкторы

        protected Employee()
        {
            CreationDate = DateTime.Now;
        }

        public Employee(string firstName, string lastName, string patronymic, EmployeePost post, User createdBy) : this()
        {
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
            Post = post;

            CreatedBy = createdBy;
        }

        #endregion

        #region Методы

        #endregion
    }
}
