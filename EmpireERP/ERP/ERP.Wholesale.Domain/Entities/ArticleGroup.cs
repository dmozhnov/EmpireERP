using System;
using System.Collections.Generic;
using ERP.Infrastructure.Entities;
using Iesi.Collections.Generic;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Группа товаров
    /// </summary>
    public class ArticleGroup : Entity<short>
    {
        #region Свойства

        /// <summary>
        /// Наименование
        /// </summary>
        /// <remarks>обязательное, не более 200 символов</remarks>
        public virtual string Name { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        /// <remarks>обязательное, не более 4000 символов</remarks>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Процент наценки
        /// </summary>
        /// <remarks>вещественное (6,2)</remarks>
        public virtual decimal MarkupPercent { get; set; }

        /// <summary>
        /// Процент продавцу
        /// </summary>
        /// <remarks>вещественное (4,2)</remarks>
        public virtual decimal SalaryPercent { get; set; }

        /// <summary>
        /// Родительская группа
        /// </summary>
        public virtual ArticleGroup Parent { get; set; }

        /// <summary>
        /// Бухгалтерское наименование (для выгрузки в 1С)
        /// </summary>
        /// <remarks>обязательное, не более 200 символов</remarks>
        public virtual string NameFor1C { get; set; }

        /// <summary>
        /// Перечень дочерних групп
        /// </summary>
        public virtual IEnumerable<ArticleGroup> Childs
        {
            get { return new ImmutableSet<ArticleGroup>(childs); }
        }
        private Iesi.Collections.Generic.ISet<ArticleGroup> childs;

        /// <summary>
        /// Полное имя (3 уровня вложенности)
        /// </summary>
        public virtual string FullName
        {
            get 
            {
                string articleGroupString = this.Name;
                
                if (Parent != null)
                {
                    articleGroupString = Parent.Name + "  ::  " + articleGroupString;

                    if (Parent.Parent != null)
                    {
                        articleGroupString = Parent.Parent.Name + "  ::  " + articleGroupString;
                    }
                }

                return articleGroupString;
            }
        }

        #endregion

        #region Конструкторы

        protected ArticleGroup()
        {            
        }

        public ArticleGroup(string name, string nameFor1C)
        {
            ValidationUtils.Assert(!String.IsNullOrEmpty(name), "Укажите название группы товаров.");
            ValidationUtils.Assert(!String.IsNullOrEmpty(nameFor1C), "Укажите бухгалтерское название группы товаров.");

            Name = name;
            NameFor1C = nameFor1C;
            childs = new HashedSet<ArticleGroup>();
            Comment = String.Empty;
            SalaryPercent = 0.0M;
            MarkupPercent = 0.0M;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавление дочерней группы
        /// </summary>
        /// <param name="childGroup">Дочерняя группа</param>
        public virtual void AddChildGroup(ArticleGroup childGroup)
        {
            childs.Add(childGroup);
            childGroup.Parent = this;
        }

        /// <summary>
        /// Удаление дочерней группы
        /// </summary>
        /// <param name="childGroup">Дочерняя группа</param>
        public virtual void RemoveChildGroup(ArticleGroup childGroup)
        {
            childs.Remove(childGroup);
            childGroup.Parent = null;
        }

        #endregion
    }
}
