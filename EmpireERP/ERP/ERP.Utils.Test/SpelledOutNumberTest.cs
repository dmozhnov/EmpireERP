using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Utils.Test
{
    [TestClass]
    public class SpelledOutCurrencyTest
    {
        /// <summary>
        /// На входе имеем число, функция должна правильно отобразить число в виде строки с валютой - рублями
        /// </summary>
        [TestMethod]
        public void RusCurrencyIsSpelled_Ok()
        {
            Assert.AreEqual("Пятьсот восемьдесят шесть рублей 17 копеек", SpelledOutCurrency.Get(586.17M));
            Assert.AreEqual("Один рубль 00 копеек", SpelledOutCurrency.Get(1M));
            Assert.AreEqual("Ноль рублей 00 копеек", SpelledOutCurrency.Get(0M));
            Assert.AreEqual("Ноль рублей 22 копейки", SpelledOutCurrency.Get(.22M));
            Assert.AreEqual("Пять рублей 22 копейки", SpelledOutCurrency.Get(5.22M));
            Assert.AreEqual("Пять тысяч рублей 97 копеек", SpelledOutCurrency.Get(5000.97M));
            Assert.AreEqual("Минус пять тысяч рублей 97 копеек", SpelledOutCurrency.Get(-5000.97M));
            Assert.AreEqual("Три рубля 47 копеек", SpelledOutCurrency.Get(3.466M));
            Assert.AreEqual("Три рубля 47 копеек", SpelledOutCurrency.Get(3.474M));
            Assert.AreEqual("Три рубля 48 копеек", SpelledOutCurrency.Get(3.475M));
            Assert.AreEqual("Один квинтиллион один квадриллион двадцать один триллион один миллиард тридцать один миллион пятьсот восемьдесят одна тысяча девяносто один рубль 71 копейка",
                SpelledOutCurrency.Get(1001021001031581091.71M));
            Assert.AreEqual("Два квинтиллиона три квадриллиона двадцать четыре триллиона два миллиарда три миллиона пятьсот восемьдесят три тысячи девяносто два рубля 72 копейки",
                SpelledOutCurrency.Get(2003024002003583092.72M));
            Assert.AreEqual("Пять квинтиллионов шесть квадриллионов двадцать семь триллионов восемь миллиардов тридцать пять миллионов пятьсот восемьдесят тысяч девяносто шесть рублей 70 копеек",
                SpelledOutCurrency.Get(5006027008035580096.70M));
        }
    }
}
