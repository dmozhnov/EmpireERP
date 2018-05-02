using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0006
{
    /// <summary>
    /// Элемент таблицы 6 отчета / печатной формы, отражающий строку таблицы с названием, полями "Дебет" и "Кредит"
    /// </summary>
    public class Report0006BalanceItemViewModel
    {
        #region Свойства

        /// <summary>
        /// Номер по порядку
        /// </summary>
        public string Number { get; protected set; }

        /// <summary>
        /// Название строки
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Строка с дополнительной информацией (вторая строка названия)
        /// </summary>
        public string AdditionalInfo1 { get; protected set; }

        /// <summary>
        /// Строка с дополнительной информацией (третья строка названия)
        /// </summary>
        public string AdditionalInfo2 { get; protected set; }

        /// <summary>
        /// Является ли заголовком (суммой оборота или сальдо на период) - выделяется
        /// </summary>
        public bool IsHeader { get; protected set; }

        /// <summary>
        /// Уровень группирующего заголовка.
        /// Если 0, данный элемент не является группирующим заголовком, это либо обычная строка либо простой заголовок (IsHeader).
        /// 1 - является заголовком верхнего уровня, 2 - более нижнего и т.д.
        /// У заголовков при выводе не показываются столбцы "Номер", "Дебет" и "Кредит", только "Название"
        /// </summary>
        public int GroupHeaderLevel { get; set; }

        /// <summary>
        /// Является ли элемент группирующим заголовком
        /// </summary>
        public bool IsGroupHeader
        {
            get
            {
                return GroupHeaderLevel > 0;
            }
        }

        /// <summary>
        /// Дебет
        /// </summary>
        public string Debit { get; protected set; }

        /// <summary>
        /// Кредит
        /// </summary>
        public string Credit { get; protected set; }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор для обычной строки (два числа - дебет и кредит)
        /// </summary>
        /// <param name="number">Номер строки</param>
        /// <param name="name">Название (текст в строке)</param>
        /// <param name="debit">Сумма дебета</param>
        /// <param name="credit">Сумма кредита</param>
        /// <param name="isHeader">Является ли элемент заголовком</param>
        /// <param name="forceZeroes">Писать ли нули для нулевых значений (false = ничего не выводить для нулевых значений)</param>
        public Report0006BalanceItemViewModel(int number, decimal debit, decimal credit, string name, string additionalInfo1 = "", string additionalInfo2 = "",
            bool isHeader = false, bool forceZeroes = false)
        {
            Number = number.ToString();
            Name = name;
            AdditionalInfo1 = additionalInfo1;
            AdditionalInfo2 = additionalInfo2;
            Debit = debit != 0M || forceZeroes ? debit.ForDisplay(ValueDisplayType.Money) : "";
            Credit = credit != 0M || forceZeroes ? credit.ForDisplay(ValueDisplayType.Money) : "";
            IsHeader = isHeader;
        }

        /// <summary>
        /// Конструктор для строки сальдо (одно число, пишется в соответствующий столбец согласно знаку)
        /// </summary>
        /// <param name="number">Номер строки</param>
        /// <param name="name">Название (текст в строке)</param>
        /// <param name="balance">Сумма сальдо</param>
        /// <param name="isHeader">Является ли элемент заголовком</param>
        public Report0006BalanceItemViewModel(int number, decimal balance, string name, bool isHeader = false)
        {
            Number = number.ToString();
            Name = name;
            Debit = balance >= 0M ? balance.ForDisplay(ValueDisplayType.Money) : "";
            Credit = balance < 0M ? (-balance).ForDisplay(ValueDisplayType.Money) : "";
            IsHeader = isHeader;
        }

        /// <summary>
        /// Конструктор для группирующего заголовка
        /// </summary>
        /// <param name="groupHeaderLevel">Уровень группирующего заголовка</param>
        /// <param name="name">Название (текст в строке)</param>
        public Report0006BalanceItemViewModel(int groupHeaderLevel, string name)
        {
            GroupHeaderLevel = groupHeaderLevel;
            Name = name;
        }

        #endregion
    }
}
