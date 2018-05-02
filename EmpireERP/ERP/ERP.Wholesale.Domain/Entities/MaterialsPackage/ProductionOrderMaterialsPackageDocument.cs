using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Infrastructure.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Utils;
using System.IO;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Материал (к пакету материалов в заказе)
    /// </summary>
    public class ProductionOrderMaterialsPackageDocument: Entity<Guid>
    {
        #region Свойства

        /// <summary>
        /// Имя файла
        /// </summary>
        public virtual string FileName { get; set; }

        /// <summary>
        /// Путь к физическому файлу на сервере
        /// </summary>
        public virtual string Path
        {
            get
            {
                return MaterialsPackage.Id.ToString() + "\\" + Id.ToString();
            }
        }

        /// <summary>
        /// Описание материала
        /// </summary>
        public virtual string Description {
            get
            {
                return description;
            }
            set
            {
                ValidationUtils.NotNull(value, "Невозможно присвоить описанию значение «null».");
                description = value;
                LastChangeDate = DateTime.Now;  //Сохраняем дату последнего редактирования
            }
        }
        private string description;

        /// <summary>
        /// Дата создания материала
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Дата последнего изменения материала
        /// </summary>
        public virtual DateTime? LastChangeDate { get; protected set; }

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
        /// Размер файла в Мегабайтах
        /// </summary>
        public virtual decimal Size { get; protected set; }

        /// <summary>
        /// Автор (Кто добавил)
        /// </summary>
        public virtual User CreatedBy {get; protected set;}

        /// <summary>
        /// Пакет материалов, которому принадлежит материал
        /// </summary>
        public virtual ProductionOrderMaterialsPackage MaterialsPackage { get; set; }
        
        #endregion

        #region Конструкторы

        protected ProductionOrderMaterialsPackageDocument()
        {
        }

        public ProductionOrderMaterialsPackageDocument(string fileName, string description, User author)
        {
            ValidationUtils.NotNull(fileName, "Не указано имя файла документа пакета материалов.");
            ValidationUtils.NotNull(description, "Невозможно присвоить описанию значение «null».");
            ValidationUtils.NotNull(author, "Не указан автор материала.");

            CreationDate = DateTime.Now;
            FileName = fileName;
            Description = description;
            CreatedBy = author;
        }

        #endregion

        #region Методы

        public virtual void SetFileSizeInBytes(long size)
        {
            Size = Convert.ToDecimal((size / (1024.0 * 1024.0)));
        }
        #endregion
    }
}
