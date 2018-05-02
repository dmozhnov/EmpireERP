using System;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using System.Linq;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Позиция приходной накладной
    /// </summary>
    public class ProductionOrderBatchRow : Entity<Guid>
    {
        #region Свойства

        /// <summary>
        /// Партия заказа, частью которой является позиция
        /// </summary>
        public virtual ProductionOrderBatch Batch { get; protected internal set; }

        /// <summary>
        /// Позиция связанной приходной накладной, созданная по данной позиции
        /// </summary>
        public virtual ReceiptWaybillRow ReceiptWaybillRow { get; set; }

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

            set
            {
                // TODO: поставить проверку на этап. Нельзя будет (?) менять товар в позиции партии, которая прошла первый этап

                ValidationUtils.NotNull(value.MeasureUnit, "Для товара не задана единица измерения.");
                ArticleMeasureUnitScale = value.MeasureUnit.Scale;

                ValidationUtils.NotNull(value.PackSize, "Для товара не задано количество в упаковке.");
                PackSize = value.PackSize;

                article = value;
            }
        }
        private Article article;

        /// <summary>
        /// Кол-во знаков после запятой (из единицы измерения товара)
        /// </summary>
        public virtual byte ArticleMeasureUnitScale { get; set; }

        /// <summary>
        /// Количество производимое (в штуках)
        /// </summary>
        public virtual decimal Count
        {
            get { return count; }
            set
            {
                ValidationUtils.CheckDecimalScale(value, ArticleMeasureUnitScale, "Ожидаемое количество имеет слишком большое число цифр после запятой.");
                ValidationUtils.Assert(value % PackSize == 0, "Ожидаемое количество не кратно количеству товара в упаковке.");
                count = value;
            }
        }
        private decimal count;

        /// <summary>
        /// Количество упаковок
        /// </summary>
        public virtual int PackCount { get { return Convert.ToInt32(Math.Ceiling(Count / PackSize)); } }

        /// <summary>
        /// Количество товара в упаковке
        /// </summary>
        public virtual decimal PackSize
        {
            get
            {
                return packSize;
            }
            set
            {
                ValidationUtils.CheckDecimalScale(value, ArticleMeasureUnitScale, "Количество в упаковке имеет слишком большое число цифр после запятой.");
                packSize = value;
            }
        }
        private decimal packSize;

        /// <summary>
        /// Цена производства 1 единицы продукции из позиции заказа в валюте
        /// </summary>
        public virtual decimal ProductionCostInCurrency { get { return Math.Round(ProductionOrderBatchRowCostInCurrency / Count, 6); } }

        /// <summary>
        /// Стоимость производства позиции заказа в валюте
        /// </summary>
        /// <remarks>вещественное (18, 6)</remarks>
        public virtual decimal ProductionOrderBatchRowCostInCurrency { get; set; }

        /// <summary>
        /// Валюта
        /// </summary>
        public virtual Currency Currency { get; set; }

        /// <summary>
        /// Изготовитель
        /// </summary>
        public virtual Manufacturer Manufacturer
        {
            get { return manufacturer; }
            set
            {
                if (Batch != null)
                {
                    Batch.CheckPossibilityToSetManufacturerForRow(value);
                }

                manufacturer = value;
            }
        }
        protected Manufacturer manufacturer;

        /// <summary>
        /// Страна производства
        /// </summary>
        public virtual Country ProductionCountry { get; set; }

        /// <summary>
        /// Длина упаковки товара
        /// </summary>
        /// <remarks>вещественное (6, 0), необязательное</remarks>
        public virtual decimal PackLength { get; set; }

        /// <summary>
        /// Высота упаковки товара
        /// </summary>
        /// <remarks>вещественное (6, 0), необязательное</remarks>
        public virtual decimal PackHeight { get; set; }

        /// <summary>
        /// Ширина упаковки товара
        /// </summary>
        /// <remarks>вещественное (6, 0), необязательное</remarks>
        public virtual decimal PackWidth { get; set; }

        /// <summary>
        /// Объем упаковки товара
        /// </summary>
        public virtual decimal PackVolume
        {
            get
            {
                return (PackLength * PackHeight * PackWidth) > 0 ? PackLength * PackHeight * PackWidth / 1000000000M : packVolume;
            }
            set
            {
                packVolume = value;
            }
        } //мм3 переводим в м3
        private decimal packVolume;

        /// <summary>
        /// Вес упаковки товара
        /// </summary>
        /// <remarks>вещественное (8, 3)</remarks>
        public virtual decimal PackWeight { get; set; }

        /// <summary>
        /// Общий объем позиции
        /// </summary>
        public virtual decimal TotalVolume { get { return PackCount * PackVolume; } }

        /// <summary>
        /// Общий вес позиции
        /// </summary>
        public virtual decimal TotalWeight { get { return PackCount * PackWeight; } }

        /// <summary>
        /// Порядковый номер для сортировки
        /// </summary>
        public virtual int OrdinalNumber { get; protected set; }

        #endregion

        #region Конструкторы

        protected ProductionOrderBatchRow()
        {
        }

        public ProductionOrderBatchRow(Article article, Currency currency, decimal productionOrderBatchRowCostInCurrency,
            decimal count, decimal packWeight, Country producerCountry, Manufacturer manufacturer)
        {
            ValidationUtils.Assert(count > 0, "Количество производимого товара должно быть положительным числом.");
            ValidationUtils.Assert(packWeight > 0, "Вес упаковки должен быть положительным числом.");
            ValidationUtils.Assert(productionOrderBatchRowCostInCurrency >= 0, "Невозможно создать позицию партии заказа с отрицательной суммой.");

            CreationDate = DateTime.Now;
            Article = article;

            Currency = currency;
            ProductionOrderBatchRowCostInCurrency = productionOrderBatchRowCostInCurrency;
            Count = count;

            ProductionCountry = producerCountry;
            this.manufacturer = manufacturer;

            // По умолчанию размеры берутся из товара
            PackLength = article.PackLength; 
            PackHeight = article.PackHeight;
            PackWidth = article.PackWidth;
            PackVolume = article.PackVolume;

            PackWeight = packWeight;

            OrdinalNumber = int.MaxValue;
        }

        /// <summary>
        /// Конструктор для копирования при разделении партии заказа
        /// </summary>
        public ProductionOrderBatchRow(ProductionOrderBatchRow productionOrderBatchRow, decimal productionOrderBatchRowCostInCurrency,
            decimal count, int ordinalNumber)
        {
            ValidationUtils.Assert(count > 0, "Количество производимого товара должно быть положительным числом.");
            ValidationUtils.Assert(productionOrderBatchRowCostInCurrency >= 0, "Невозможно создать позицию партии заказа с отрицательной суммой.");

            CreationDate = DateTime.Now;
            article = productionOrderBatchRow.Article;

            this.ArticleMeasureUnitScale = productionOrderBatchRow.ArticleMeasureUnitScale;
            this.PackSize = productionOrderBatchRow.PackSize;

            this.Count = count;
            this.ProductionOrderBatchRowCostInCurrency = productionOrderBatchRowCostInCurrency;

            this.Currency = productionOrderBatchRow.Currency;
            this.Manufacturer = productionOrderBatchRow.Manufacturer;
            this.PackHeight = productionOrderBatchRow.PackHeight;
            this.PackLength = productionOrderBatchRow.PackLength;
            this.PackWeight = productionOrderBatchRow.PackWeight;
            this.PackWidth = productionOrderBatchRow.PackWidth;
            this.ProductionCountry = productionOrderBatchRow.ProductionCountry;

            OrdinalNumber = ordinalNumber;
        }

        #endregion

    }
}
