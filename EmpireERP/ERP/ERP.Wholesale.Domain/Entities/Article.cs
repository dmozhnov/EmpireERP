using ERP.Infrastructure.Entities;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Товар
    /// </summary>
    public class Article : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Полное название
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string FullName { get; set; }

        /// <summary>
        /// Краткое название
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string ShortName { get; set; }

        /// <summary>
        /// Артикул
        /// </summary>
        /// <remarks>не более 30 символов</remarks>
        public virtual string Number { get; set; }

        /// <summary>
        /// Заводской артикул
        /// </summary>
        /// <remarks>не более 30 символов</remarks>
        public virtual string ManufacturerNumber { get; set; }

        /// <summary>
        /// Группа товара
        /// </summary>
        public virtual ArticleGroup ArticleGroup { get; set; }

        /// <summary>
        /// Торговая марка
        /// </summary>
        public virtual Trademark Trademark { get; set; }

        /// <summary>
        /// Изготовитель
        /// </summary>
        public virtual Manufacturer Manufacturer { get; set; }

        /// <summary>
        /// Страна производства
        /// </summary>
        public virtual Country ProductionCountry { get; set; }

        /// <summary>
        /// Единица измерения
        /// </summary>
        public virtual MeasureUnit MeasureUnit { get; set; }

        /// <summary>
        /// Сертификат товара
        /// </summary>
        public virtual ArticleCertificate Certificate { get; set; }

        /// <summary>
        /// Длина упаковки товара
        /// </summary>
        public virtual int PackLength 
        {
            get { return packLength; }
            set
            {
                packLength = value;
                packVolume = CalculatePackVolume();
            }
        }
        private int packLength;

        /// <summary>
        /// Высота упаковки товара
        /// </summary>
        public virtual int PackHeight
        {
            get { return packHeight; }
            set
            {
                packHeight = value;
                packVolume = CalculatePackVolume();
            }
        }
        private int packHeight;

        /// <summary>
        /// Ширина упаковки товара
        /// </summary>
        public virtual int PackWidth
        {
            get { return packWidth; }
            set
            {
                packWidth = value;
                packVolume = CalculatePackVolume();
            }
        }
        private int packWidth;

        /// <summary>
        /// Количество в упаковке
        /// </summary>
        public virtual decimal PackSize
        {
            get
            {
                return packSize;
            }
            set {
                ValidationUtils.CheckDecimalScale(value, MeasureUnit.Scale, "Количество в упаковке имеет слишком большое число цифр после запятой.");
                packSize = value;
            }
        }
        private decimal packSize;

        /// <summary>
        /// Вес упаковки
        /// </summary>
        /// <remarks>вещественное (8, 3)</remarks>
        public virtual decimal PackWeight { get; set; }

        /// <summary>
        /// Объем упаковки
        /// </summary>
        /// <remarks>вещественное (15, 6)</remarks>
        public virtual decimal PackVolume {
            get { return packVolume; }
            set 
            {
                ValidationUtils.CheckDecimalScale(value, 6, "Объем упаковки имеет слишком большое число цифр после запятой.");
                packVolume = value;

                // Обнуляем линейные размеры, т.к. при прямой установке объема эти величины неизвестны.
                packHeight = 0;
                packWidth = 0;
                packLength = 0;
            }
        }
        private decimal packVolume;

        /// <summary>
        /// Процент, идущий в заработную плату
        /// </summary>
        /// <remarks>вещественное (4, 2)</remarks>
        public virtual decimal SalaryPercent
        {
            get
            {
                if (!IsSalaryPercentFromGroup)
                {
                    // Из товара
                    return salaryPercentValue;
                }
                else   // из группы товара
                {
                    if (ArticleGroup != null)
                    {
                        return ArticleGroup.SalaryPercent;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            set
            {
                if (!IsSalaryPercentFromGroup)
                {
                    salaryPercentValue = value;
                }
                else
                {
                    if (ArticleGroup != null)
                    {
                        salaryPercentValue = ArticleGroup.SalaryPercent;
                    }
                    else { salaryPercentValue = 0; }
                }
                //IsSalaryPercentFromGroup = false; // Сбрасываем флаг чтения значения процента из группы
            }
        }
        private decimal salaryPercentValue;

        /// <summary>
        /// Признак необходимости брать значение процента, идущего в заработную плату, из группы
        /// </summary>
        public virtual bool IsSalaryPercentFromGroup { get; set; }

        /// <summary>
        /// Признак того, что товар устарел (не появляется при добавлении товаров списком)
        /// </summary>
        public virtual bool IsObsolete { get; set; }

        /// <summary>
        /// Процент наценки
        /// </summary>
        /// <remarks>вещественное (6, 2)</remarks>
        public virtual decimal MarkupPercent { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        /// <remarks>не более 4000 символов</remarks>
        public virtual string Comment { get; set; }

        #endregion

        #region Конструкторы

        protected Article()
        {
        }

        public Article(string fullName, ArticleGroup articleGroup, MeasureUnit measureUnit, bool percentFromArticleGroup)            
        {
            MeasureUnit = measureUnit;

            PackSize = 1;
            salaryPercentValue = 0;
            MarkupPercent = 0;
            Comment = string.Empty;
            Number = string.Empty;
            
            FullName = fullName;
            ShortName = fullName;
            ArticleGroup = articleGroup;
            
            IsSalaryPercentFromGroup = percentFromArticleGroup;
            IsObsolete = false;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Вычисление объема упаковки
        /// </summary>
        private decimal CalculatePackVolume()
        {
            var volume = packVolume;

            // Если установлены все 3 линейных размера, то вычисляем объем.
            if (packLength != 0 && packHeight != 0 && packWidth != 0)
            {
                volume = packLength * packHeight * packWidth / 1000000000M;    //т.к. линейные размеры в мм, а объем в м^3
            }
            // иначе оставляем объем без изменений

            return volume;
        }

        #endregion
    }
}
