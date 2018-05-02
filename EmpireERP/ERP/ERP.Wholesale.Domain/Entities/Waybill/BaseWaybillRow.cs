using System;
using ERP.Infrastructure.Entities;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Базовый класс для позиции накладной
    /// </summary>
    public abstract class BaseWaybillRow : Entity<Guid>
    {
        #region Свойства

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Дата удаления
        /// </summary>
        public virtual DateTime? DeletionDate
        {
            get { return deletionDate; }
            set { if (deletionDate == null && value != null) deletionDate = value; }   // запрещаем повторную пометку об удалении
        }
        protected DateTime? deletionDate;

        /// <summary>
        /// Товар
        /// </summary>
        public virtual Article Article
        {
            get { return article; }
            protected set { article = value; }
        }
        protected Article article;

        /// <summary>
        /// Количество товара по позиции
        /// </summary>
        protected abstract decimal ArticleCount { get; }

        /// <summary>
        /// Вес позиции
        /// </summary>
        public virtual decimal Weight
        {
            get
            {
                // если кол-во товаров в упаковке = 0, то посчитать вес позиции мы не сможем
                if (Article.PackSize == 0) return 0;

                return (Article.PackWeight / Article.PackSize) * ArticleCount;
            }
        }

        /// <summary>
        /// Объем позиции
        /// </summary>
        public virtual decimal Volume
        {
            get
            {
                // если кол-во товаров в упаковке = 0, то посчитать объем позиции мы не сможем
                if (Article.PackSize == 0) return 0;

                return (Article.PackVolume / Article.PackSize) * ArticleCount;
            }
        }

        /// <summary>
        /// Количество ЕУ по позиции
        /// </summary>
        public virtual decimal PackCount
        {
            get
            {
                // если кол-во товаров в упаковке = 0, то посчитать количество упаковок мы не сможем
                if (Article.PackSize == 0) return 0;

                return ArticleCount / Article.PackSize;
            }
        }

        /// <summary>
        /// Тип накладной, которой принадлежит позиция
        /// </summary>
        public virtual WaybillType WaybillType { get; private set; }

        #endregion

        #region Конструкторы

        protected BaseWaybillRow()
        {

        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="waybillType">Тип накладной, которой принадлежит позиция</param>
        protected BaseWaybillRow(WaybillType waybillType)
        {
            CreationDate = DateTimeUtils.GetCurrentDateTime();
            WaybillType = waybillType;
        }

        #endregion

        #region Методы

        

        #endregion
    }
}
