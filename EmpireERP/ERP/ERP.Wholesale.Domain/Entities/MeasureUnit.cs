using ERP.Infrastructure.Entities;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Единица измерения
    /// </summary>
    public class MeasureUnit : Entity<short>
    {
        /// <summary>
        /// Цифровой код
        /// </summary>
        /// <remarks>3 символа</remarks>
        public virtual string NumericCode { get; set; }

        /// <summary>
        /// Краткое наименование
        /// </summary>
        /// <remarks>Строка, не более 7 символов, обязательное</remarks>
        public virtual string ShortName { get; set; }

        /// <summary>
        /// Полное наименование
        /// </summary>
        /// <remarks>Строка, не более 20 символов, обязательное</remarks>
        public virtual string FullName { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        /// <remarks>Строка, не более 500 символов</remarks>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Кол-во знаков после запятой
        /// </summary>
        public virtual byte Scale { get; set; }

        protected MeasureUnit()
        {            
        }

        public MeasureUnit(string shortName, string fullName, string numericCode, byte scale)
        {            
            ShortName = shortName;
            FullName = fullName;
            Scale = scale;
            Comment = string.Empty;
            NumericCode = numericCode;
        }
    }
}
