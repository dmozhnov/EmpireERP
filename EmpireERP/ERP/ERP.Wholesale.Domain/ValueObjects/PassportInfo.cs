using System;

namespace ERP.Wholesale.Domain.ValueObjects
{
    /// <summary>
    /// Паспортные данные
    /// </summary>
    public class PassportInfo
    {
        /// <summary>
        /// Серия
        /// </summary>
        /// <remarks>Строка, не более 10 символов</remarks>
        public virtual string Series { get; set; }

        /// <summary>
        /// Номер
        /// </summary>
        /// <remarks>Строка, не более 10 символов</remarks>
        public virtual string Number { get; set; }

        /// <summary>
        /// Кем выдан
        /// </summary>
        /// <remarks>Строка, не более 200 символов</remarks>
        public virtual string IssuedBy { get; set; }

        /// <summary>
        /// Дата выдачи
        /// </summary>
        public virtual DateTime? IssueDate { get; set; }

        /// <summary>
        /// Код подразделения
        /// </summary>
        /// <remarks>Строка, не более 10 символов</remarks>
        public virtual string DepartmentCode { get; set; }


        public PassportInfo()
        {
            Series = "";
            Number = "";
            IssuedBy = "";
            DepartmentCode = "";
        }
    }
}
