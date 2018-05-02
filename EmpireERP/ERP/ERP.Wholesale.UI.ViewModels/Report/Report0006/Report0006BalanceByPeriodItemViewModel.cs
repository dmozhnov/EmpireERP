using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0006
{
    /// <summary>
    /// Элемент таблицы 6 отчета, отражающий строку таблицы с названием, полями "Начальное сальдо", "Дебет", "Кредит" и "Конечное сальдо"
    /// </summary>
    public class Report0006BalanceByPeriodItemViewModel
    {
        #region Свойства

        /// <summary>
        /// Название строки
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Начальное сальдо
        /// </summary>
        public string StartingBalance { get; protected set; }

        /// <summary>
        /// Дебет
        /// </summary>
        public string Debit { get; protected set; }

        /// <summary>
        /// Кредит
        /// </summary>
        public string Credit { get; protected set; }

        /// <summary>
        /// Конечное сальдо
        /// </summary>
        public string EndingBalance { get; protected set; }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название строки</param>
        /// <param name="startingBalance">Начальное сальдо</param>
        /// <param name="debit">Дебет</param>
        /// <param name="credit">Кредит</param>
        /// <param name="endingBalance">Конечное сальдо</param>
        public Report0006BalanceByPeriodItemViewModel(string name, decimal startingBalance, decimal debit, decimal credit, decimal endingBalance)
        {
            Name = name;
            StartingBalance = startingBalance.ForDisplay(ValueDisplayType.Money);
            Debit = debit != 0M ? debit.ForDisplay(ValueDisplayType.Money) : "";
            Credit = credit != 0M ? credit.ForDisplay(ValueDisplayType.Money) : "";
            EndingBalance = endingBalance.ForDisplay(ValueDisplayType.Money);
        }

        #endregion
    }
}
