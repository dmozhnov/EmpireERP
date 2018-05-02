using System;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Правило формирования последней цифры
    /// </summary>
    public class LastDigitCalcRule
    {
        /// <summary>
        /// Правило по умолчанию, которое будет применено, в случае если указанное правило применить не получится
        /// </summary>
        public LastDigitCalcRule DefaultRule { get; set; }

        /// <summary>
        /// Тип правила формирования последней цифры
        /// </summary>
        public virtual LastDigitCalcRuleType Type
        {
            get { return type; }
            protected internal set
            {
                if (type!= 0 && value == LastDigitCalcRuleType.SetCustom)
                {
                    throw new Exception("Невозможно установить данный тип правила.");
                }

                type = value;
            }
        }
        private LastDigitCalcRuleType type;

        /// <summary>
        /// Последняя цифра целой части
        /// </summary>
        /// <remarks>целое от 0 до 9</remarks>
        public virtual byte? LastDigit
        {
            get { return lastDigit; }
            set
            {
                if(value != null)
                {
                    lastDigit = value;
                    type = LastDigitCalcRuleType.SetCustom;
                }
            }
        }
        private byte? lastDigit;

        /// <summary>
        /// Дробная часть (копейки)
        /// </summary>
        public virtual short? Decimals
        {
            get { return decimals; }
            set
            {
                if (value != null)
                {
                    decimals = value;
                    type = LastDigitCalcRuleType.SetCustom;
                }
            }
        }
        private short? decimals;

        /// <summary>
        /// Учетная цена на неком складе (для правила "Оставить последнюю цифру как на ...")
        /// </summary>
        public virtual DynamicDictionary<int, decimal?> AccountingPricesAtChosenStorage { get; set; }

        /// <summary>
        /// Склад, с которого брать учетную цену (для правила "Оставить последнюю цифру как на ...")
        /// </summary>
        public virtual Storage Storage { get; set; }

        #region Конструкторы
        protected LastDigitCalcRule()
        {
        }

        public LastDigitCalcRule(LastDigitCalcRuleType type)
        {
            Type = type;
        }

        /// <summary>
        /// Конструктор для правила "Назначить для всех товаров указанную цифру" с указанием последней цифры и копеек
        /// </summary>
        /// <param name="decimals">Последняя цифра цены</param>
        /// <param name="decimals">Копейки</param>
        public LastDigitCalcRule(byte? lastDigit, short? decimals)
        {
            if (!lastDigit.HasValue && !decimals.HasValue)
            {
                throw new Exception("Должны быть указаны копейки или последняя цифра.");
            }
            LastDigit = lastDigit;
            Decimals = decimals;
        }

        /// <summary>
        /// Конструктор для правила "Назначить для всех товаров указанную цифру" с указанием только последней цифры
        /// </summary>
        /// <param name="decimals">Последняя цифра цены</param>
        public LastDigitCalcRule(byte lastDigit)
        {
            LastDigit = lastDigit;
            Decimals = null;
        }

        /// <summary>
        /// Конструктор для правила "Назначить для всех товаров указанную цифру" с указанием только копеек
        /// </summary>
        /// <param name="decimals">Копейки</param>
        public LastDigitCalcRule(short decimals)
        {
            LastDigit = null;
            Decimals = decimals;
        }

        /// <summary>
        /// Конструктор для правила "Оставить последнюю цифру как на ..."
        /// </summary>
        /// <param name="accountingPrice">Учетная цена на указанном складе</param>
        public LastDigitCalcRule(Storage storage)
        {
            Storage = storage;
            type = LastDigitCalcRuleType.LeaveLastDigitFromStorage;
        }

        #endregion

        public static LastDigitCalcRule GetDefault()
        {
            return new LastDigitCalcRule((LastDigitCalcRuleType)1);
        }

        #region Расчет последней цифры

        public virtual decimal? CalculateLastDigit(Article article, decimal accountingPrice)
        {
            switch (Type)
            {
                case LastDigitCalcRuleType.LeaveAsIs:
                    return accountingPrice;

                case LastDigitCalcRuleType.LeaveLastDigitFromStorage:
                    var accPrices = AccountingPricesAtChosenStorage;
                    if (accPrices == null)
                    {
                        return null;
                    }

                    var accountingPriceOnStorage = accPrices[article.Id];

                    if (accountingPriceOnStorage == null) { return null; }

                    return ChangeLastDigit(accountingPrice, accountingPriceOnStorage.Value);

                case LastDigitCalcRuleType.RoundDecimalsAndLeaveLastDigit:
                    return ChangeLastDigit(accountingPrice, null, 0);

                case LastDigitCalcRuleType.SetCustom:
                    return ChangeLastDigit(accountingPrice, LastDigit, Decimals);


                default:
                    throw new Exception("Неизвестное правило расчета последней цифры.");
            }
        }

        /// <summary>
        /// Заменяет последнюю цифру и "копейки" числа
        /// </summary>
        /// <param name="oldNumber"></param>
        /// <param name="lastDigit"></param>
        /// <param name="decimalValue"></param>
        /// <returns></returns>
        private decimal ChangeLastDigit(decimal oldNumber, byte? lastDigit, short? decimalValue)
        {
            decimal newNumber = oldNumber;
            if (lastDigit.HasValue)
            {
                if (lastDigit < 0 || lastDigit > 9)
                {
                    throw new Exception("Последняя цифра должна быть числом от 0 до 9.");
                }
                newNumber = newNumber - (byte)(newNumber % 10) + lastDigit.Value;
            }
            if (decimalValue.HasValue)
            {
                newNumber = Math.Floor(newNumber) + (decimal)(decimalValue.Value) / 100;
            }

            return newNumber;
        }

        /// <summary>
        /// Заменяет последнюю цифру и "копейки" числа как в источнике
        /// </summary>
        /// <param name="oldNumber">Число, в котором будут заменены цифры</param>
        /// <param name="source">Число, чья последняя цифра и копейки будут взяты для замены</param>
        /// <returns>Новое число с заменами</returns>
        private decimal ChangeLastDigit(decimal oldNumber, decimal source)
        {
            byte sourceLastDigit = (byte)(source % 10);
            short sourceDecimalValue = (short)(Math.Round(source - Math.Floor(source), 2) * 100);

            return ChangeLastDigit(oldNumber, sourceLastDigit, sourceDecimalValue);
        }

        #endregion
    }
}
