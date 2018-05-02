// A set of C# classes for spelling Russian numerics 
// Copyright (c) 2002 RSDN Group

using System;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Collections.Specialized;

namespace ERP.Utils
{
    public class SpelledOutNumber
    {
        private static string[] hunds =
        {
            "", "сто ", "двести ", "триста ", "четыреста ",
            "пятьсот ", "шестьсот ", "семьсот ", "восемьсот ", "девятьсот "
        };

        private static string[] tens =
        {
            "", "десять ", "двадцать ", "тридцать ", "сорок ", "пятьдесят ",
            "шестьдесят ", "семьдесят ", "восемьдесят ", "девяносто "
        };

        public static string Get(decimal val, bool male, string one, string two, string five)
        {
            string[] frac20 =
            {
                "", "один ", "два ", "три ", "четыре ", "пять ", "шесть ",
                "семь ", "восемь ", "девять ", "десять ", "одиннадцать ",
                "двенадцать ", "тринадцать ", "четырнадцать ", "пятнадцать ",
                "шестнадцать ", "семнадцать ", "восемнадцать ", "девятнадцать "
            };

            decimal num = val % 1000M;
            if (0M == num) return "";
            if (num < 0M) throw new ArgumentOutOfRangeException("val", "Параметр не может быть отрицательным");
            if (!male)
            {
                frac20[1] = "одна ";
                frac20[2] = "две ";
            }

            int integerNum = (int)num;

            StringBuilder r = new StringBuilder(hunds[integerNum / 100]);

            if (integerNum % 100 < 20)
            {
                r.Append(frac20[integerNum % 100]);
            }
            else
            {
                r.Append(tens[integerNum % 100 / 10]);
                r.Append(frac20[integerNum % 10]);
            }
            
            r.Append(Case(num, one, two, five));

            if(r.Length != 0) r.Append(" ");

            return r.ToString();
        }

        public static string Case(decimal val, string one, string two, string five)
        {
            int t = (int)((val % 100M > 20M) ? val % 10M : val % 20M);

            switch (t)
            {
                case 1: return one;
                case 2: case 3: case 4: return two;
                default: return five;
            }
        }
    };

    struct CurrencyInfo
    {
        public bool male;
        public string seniorOne, seniorTwo, seniorFive;
        public string juniorOne, juniorTwo, juniorFive;
    };

    class RusCurrencySectionHandler:IConfigurationSectionHandler
    {
        public object Create( object parent, object configContext, XmlNode section )
        {
            foreach(XmlNode curr in section.ChildNodes)
            {
                if(curr.Name=="currency")
                {
                    XmlNode senior=curr["senior"];
                    XmlNode junior=curr["junior"];
                    SpelledOutCurrency.Register(   
                        curr.Attributes["code"].InnerText,
                        (curr.Attributes["male"].InnerText == "1"),
                        senior.Attributes["one"].InnerText,
                        senior.Attributes["two"].InnerText,
                        senior.Attributes["five"].InnerText,
                        junior.Attributes["one"].InnerText,
                        junior.Attributes["two"].InnerText,
                        junior.Attributes["five"].InnerText);
                }
            }

            return null;
        }
    };
            
    public class SpelledOutCurrency
    {
        private static HybridDictionary currencies = new HybridDictionary();

        static SpelledOutCurrency()
        {
            Register("RUR", true, "рубль", "рубля", "рублей", "копейка", "копейки", "копеек");          
            Register("EUR", true, "евро", "евро", "евро", "евроцент", "евроцента", "евроцентов");           
            Register("USD", true, "доллар", "доллара", "долларов", "цент", "цента", "центов");          
        }

        public static void Register(string currency, bool male, 
            string seniorOne, string seniorTwo, string seniorFive,
            string juniorOne, string juniorTwo, string juniorFive)
        {
            CurrencyInfo info;
            info.male = male;
            info.seniorOne = seniorOne; info.seniorTwo = seniorTwo; info.seniorFive = seniorFive; 
            info.juniorOne = juniorOne; info.juniorTwo = juniorTwo; info.juniorFive = juniorFive;
            currencies.Add(currency, info);
        }

        public static string Get(decimal val)
        {
            return Get(val, "RUR");
        }

        public static string Get(decimal val, string currency)
        {
            if(!currencies.Contains(currency)) 
                throw new ArgumentOutOfRangeException("currency", "Валюта \"" + currency + "\" не зарегистрирована");
            
            CurrencyInfo info = (CurrencyInfo)currencies[currency];

            return Get(val, info.male, 
                info.seniorOne, info.seniorTwo, info.seniorFive,
                info.juniorOne, info.juniorTwo, info.juniorFive);
        }

        public static string Get(decimal val, bool male, 
            string seniorOne, string seniorTwo, string seniorFive,
            string juniorOne, string juniorTwo, string juniorFive, bool outJunior = true)
        {
            bool minus = false;
            if (val < 0) { val = -val; minus = true; }

            decimal n = Math.Floor(val);
            decimal remainder = Math.Floor((val - n + 0.005M) * 100M);

            StringBuilder r = new StringBuilder();

            if (0M == n) r.Append("Ноль ");
            if (n % 1000M != 0M)
                r.Append(SpelledOutNumber.Get(n, male, seniorOne, seniorTwo, seniorFive));
            else
                r.Append(seniorFive + " ");

            n = Math.Floor(n / 1000M);
         
            r.Insert(0, SpelledOutNumber.Get(n, false, "тысяча", "тысячи", "тысяч"));
            n = Math.Floor(n / 1000M);
         
            r.Insert(0, SpelledOutNumber.Get(n, true, "миллион", "миллиона", "миллионов"));
            n = Math.Floor(n / 1000M);
         
            r.Insert(0, SpelledOutNumber.Get(n, true, "миллиард", "миллиарда", "миллиардов"));
            n = Math.Floor(n / 1000M);
         
            r.Insert(0, SpelledOutNumber.Get(n, true, "триллион", "триллиона", "триллионов"));
            n = Math.Floor(n / 1000M);

            r.Insert(0, SpelledOutNumber.Get(n, true, "квадриллион", "квадриллиона", "квадриллионов"));
            n = Math.Floor(n / 1000M);

            r.Insert(0, SpelledOutNumber.Get(n, true, "квинтиллион", "квинтиллиона", "квинтиллионов"));
            if (minus) r.Insert(0, "минус ");

            if (outJunior)  //Если запрошен вывод младшей части, то
            {
                // выводим младшую часть
                r.Append(remainder.ToString("00 "));
                r.Append(SpelledOutNumber.Case(remainder, juniorOne, juniorTwo, juniorFive));
            }

            //Делаем первую букву заглавной
            r[0] = char.ToUpper(r[0]);

            return r.ToString();
        }
    };
};