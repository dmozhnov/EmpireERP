using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Пакет материалов
    /// </summary>
    public class ProductionOrderMaterialsPackage : Entity<Guid>
    {
        #region Свойства

        /// <summary>
        /// Заказ, которому принадлежит пакет материалов
        /// </summary>
        public virtual ProductionOrder ProductionOrder { get; set; }

        /// <summary>
        /// Название пакета
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Описание пакета
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Дата изменения пакета
        /// </summary>
        public virtual DateTime LastChangeDate { get; protected set; }

        /// <summary>
        /// Количество материалов в пакете
        /// </summary>
        public virtual int DocumentCount
        {
            get
            {
                // Возможно NHibernate будет дергать данные из БД, что весьма не хорошо.Тогда придется сделать счетчик руками.
                return documents.Count;
            }
        }

        /// <summary>
        /// Материалы пакета
        /// </summary>
        public virtual IEnumerable<ProductionOrderMaterialsPackageDocument> Documents
        {
            get
            {
                return new ImmutableSet<ProductionOrderMaterialsPackageDocument>(documents);
            }
        }
        private Iesi.Collections.Generic.ISet<ProductionOrderMaterialsPackageDocument> documents;

        /// <summary>
        /// Дата удаления материала
        /// </summary>
        public virtual DateTime? DeletionDate
        {
            get
            {
                return deletionDate;
            }
            set
            {
                if (deletionDate == null && value != null)  //Запрещаем изменение даты удаления
                {
                    deletionDate = value;
                }
            }
        }
        private DateTime? deletionDate;

        /// <summary>
        /// Обем материалов пакета в мегабайтах
        /// </summary>
        public virtual decimal ProductionOrderMaterialsPackageSize { get; protected set; }

        #endregion

        #region Конструкторы

        protected ProductionOrderMaterialsPackage()
        {
        }

        public ProductionOrderMaterialsPackage(ProductionOrder productionOrder, string name)
        {
            ProductionOrder = productionOrder;
            Name = name;
            var date = DateTime.Now;
            CreationDate = LastChangeDate = date;
            documents = new HashedSet<ProductionOrderMaterialsPackageDocument>();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавление материала
        /// </summary>
        /// <param name="material"></param>
        public virtual void AddDocument(ProductionOrderMaterialsPackageDocument material)
        {
            ValidationUtils.NotNull(material, "Не указан документ для добавления в пакет материалов заказа.");
            if (Documents.Contains(material))
            {
                throw new Exception("Этот доккумент уже добавлен в пакет материалов заказа.");
            }

            documents.Add(material);    //Добавляем материал
            material.MaterialsPackage = this;

            ProductionOrderMaterialsPackageSize += material.Size;   //Добавляем размер материала к размеру пакета
            LastChangeDate = DateTime.Now;  //Выставляем дату изменения пакета
        }

        /// <summary>
        /// Удаление материала
        /// </summary>
        /// <param name="document"></param>
        public virtual void DeleteDocument(ProductionOrderMaterialsPackageDocument document)
        {
            ValidationUtils.NotNull(document, "Не указан документ, удаляемый из пакета материалов заказа.");
            if (!documents.Contains(document))
            {
                throw new Exception("Указанный документ не содержится в пакете материалов заказа.");
            }

            documents.Remove(document); //Удаляем материал

            ProductionOrderMaterialsPackageSize -= document.Size;   //Добавляем размер материала к размеру пакета
            LastChangeDate = DateTime.Now;  //Выставляем дату изменения пакета
        }

        #endregion
    }
}
