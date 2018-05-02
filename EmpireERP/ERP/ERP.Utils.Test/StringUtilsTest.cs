using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ERP.Utils.Test
{
    [TestClass]
    public class StringUtilsTest
    {
        #region ForDisplay

        [TestMethod]
        public void StringUtilsTest_ForDisplay_Int_Must_Be_Correct()
        {
            Assert.AreEqual("0", 0.ForDisplay());
            Assert.AreEqual("1", 1.ForDisplay());
            Assert.AreEqual("10 000", 10000.ForDisplay());
            Assert.AreEqual("1 000 000 000", 1000000000.ForDisplay());
            Assert.AreEqual("-1", (-1).ForDisplay());
            Assert.AreEqual("-1 000", (-1000).ForDisplay());
            Assert.AreEqual("10 000", ((short)(10000)).ForDisplay());
        }

        [TestMethod]
        public void StringUtilsTest_ForDisplay_Decimal_Must_Be_Correct()
        {
            Assert.AreEqual("0", (0.0M).ForDisplay());
            Assert.AreEqual("1", 1M.ForDisplay());
            Assert.AreEqual("1.1", (1.1M).ForDisplay());
            Assert.AreEqual("123 456 789.123456", (123456789.123456M).ForDisplay());
            Assert.AreEqual("0.01", (0.0100M).ForDisplay());
            Assert.AreEqual("-1.01", (-1.01M).ForDisplay());
            Assert.AreEqual("123.01", (123.01M).ForDisplay());
            Assert.AreEqual("-123.01", (-123.01M).ForDisplay());
            Assert.AreEqual("1 234.01", (1234.01M).ForDisplay());
            Assert.AreEqual("-1 234.01", (-1234.01M).ForDisplay());
            Assert.AreEqual("12 345.01", (12345.01M).ForDisplay());
            Assert.AreEqual("-12 345.01", (-12345.01M).ForDisplay());
        }

        [TestMethod]
        public void StringUtilsTest_ForDisplay_With_Different_ValueDisplayTypes_Must_Be_Correct()
        {
            Assert.AreEqual("23.46", (23.45678M).ForDisplay(ValueDisplayType.Percent));
            Assert.AreEqual("-23.46", (-23.45678M).ForDisplay(ValueDisplayType.Percent));

            Assert.AreEqual("23.45", (23.453678M).ForDisplay(ValueDisplayType.Percent));
            Assert.AreEqual("23.45678", (23.45678M).ForDisplay(ValueDisplayType.Default));

            Assert.AreEqual("23.45", (23.45M).ForDisplay(ValueDisplayType.Percent));
            Assert.AreEqual("23.4", (23.4M).ForDisplay(ValueDisplayType.Percent));
            Assert.AreEqual("23", (23M).ForDisplay(ValueDisplayType.Percent));
        }

        #endregion

        #region ForEdit

        [TestMethod]
        public void StringUtilsTest_ForEdit_Decimal_Must_Be_Correct()
        {
            Assert.AreEqual("0", (0.0M).ForEdit());
            Assert.AreEqual("1", 1M.ForEdit());
            Assert.AreEqual("1.1", (1.1M).ForEdit());
            Assert.AreEqual("123456789.123456", (123456789.123456M).ForEdit());
            Assert.AreEqual("0.01", (0.0100M).ForEdit());
            Assert.AreEqual("1.01", (1.01M).ForEdit());
            Assert.AreEqual("-1.01", (-1.01M).ForEdit());
        }

        #endregion

        #region PadLeftZeros

        [TestMethod]
        public void StringUtilsTest_PadLeftZeros_Must_Be_Correct()
        {
            Assert.AreEqual("0", String.Empty.PadLeftZeroes(1));
            Assert.AreEqual("00000", String.Empty.PadLeftZeroes(5));
            Assert.AreEqual("00000000", String.Empty.PadLeftZeroes(8));
            Assert.AreEqual("0", "".PadLeftZeroes(1));
            Assert.AreEqual("00000", "".PadLeftZeroes(5));
            Assert.AreEqual("00000000", "".PadLeftZeroes(8));
            Assert.AreEqual("1", "1".PadLeftZeroes(1));
            Assert.AreEqual("00000001", "1".PadLeftZeroes(8));
            Assert.AreEqual("08", "8".PadLeftZeroes(2));
            Assert.AreEqual("00008_p_", "8_p_".PadLeftZeroes(8));
            Assert.AreEqual("00000171", "171".PadLeftZeroes(8));
            Assert.AreEqual("1897", "1897".PadLeftZeroes(2));
            Assert.AreEqual("18239876а", "18239876а".PadLeftZeroes(8));
            Assert.AreEqual("18239876а", "18239876а".PadLeftZeroes(9));
        }

        #endregion

        #region DayCount

        [TestMethod]
        public void StringUtilsTest_DayCount_With_Different_ValueTypes_Must_Be_Correct()
        {
            Assert.AreEqual("дней", StringUtils.DayCount(0)); // 0 дней
            Assert.AreEqual("день", StringUtils.DayCount(1)); // 1 день
            Assert.AreEqual("день", StringUtils.DayCount(-1)); // -1 день
            Assert.AreEqual("дня", StringUtils.DayCount(2)); // 2 дня
            Assert.AreEqual("дня", StringUtils.DayCount(-2)); // -2 дня
            Assert.AreEqual("дня", StringUtils.DayCount(3)); // 3 дня
            Assert.AreEqual("дня", StringUtils.DayCount(4)); // 4 дня
            Assert.AreEqual("дней", StringUtils.DayCount(5)); // 5 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-5)); // -5 дней
            Assert.AreEqual("дней", StringUtils.DayCount(6)); // 6 дней
            Assert.AreEqual("дней", StringUtils.DayCount(7)); // 7 дней
            Assert.AreEqual("дней", StringUtils.DayCount(8)); // 8 дней
            Assert.AreEqual("дней", StringUtils.DayCount(9)); // 9 дней
            Assert.AreEqual("дней", StringUtils.DayCount(10)); // 10 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-10)); // -10 дней
            Assert.AreEqual("дней", StringUtils.DayCount(11)); // 11 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-11)); // -11 дней
            Assert.AreEqual("дней", StringUtils.DayCount(12)); // 12 дней
            Assert.AreEqual("дней", StringUtils.DayCount(19)); // 19 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-19)); // -19 дней
            Assert.AreEqual("дней", StringUtils.DayCount(20)); // 20 дней
            Assert.AreEqual("день", StringUtils.DayCount(21)); // 21 день
            Assert.AreEqual("день", StringUtils.DayCount(-21)); // -21 день
            Assert.AreEqual("дня", StringUtils.DayCount(22)); // 22 дня
            Assert.AreEqual("дня", StringUtils.DayCount(-22)); // -22 дня
            Assert.AreEqual("дня", StringUtils.DayCount(23)); // 23 дня
            Assert.AreEqual("дня", StringUtils.DayCount(24)); // 24 дня
            Assert.AreEqual("дня", StringUtils.DayCount(-24)); // -24 дня
            Assert.AreEqual("дней", StringUtils.DayCount(25)); // 25 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-25)); // -25 дней
            Assert.AreEqual("дней", StringUtils.DayCount(30)); // 30 дней
            Assert.AreEqual("день", StringUtils.DayCount(31)); // 31 день
            Assert.AreEqual("дней", StringUtils.DayCount(99)); // 99 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-99)); // -99 дней
            Assert.AreEqual("дней", StringUtils.DayCount(100)); // 100 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-100)); // -100 дней
            Assert.AreEqual("день", StringUtils.DayCount(101)); // 101 день
            Assert.AreEqual("день", StringUtils.DayCount(-101)); // -101 день
            Assert.AreEqual("дня", StringUtils.DayCount(102)); // 102 дня
            Assert.AreEqual("дней", StringUtils.DayCount(110)); // 110 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-110)); // -110 дней
            Assert.AreEqual("дней", StringUtils.DayCount(111)); // 111 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-111)); // -111 дней
            Assert.AreEqual("дней", StringUtils.DayCount(112)); // 112 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-112)); // -112 дней
            Assert.AreEqual("дней", StringUtils.DayCount(113)); // 113 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-113)); // -113 дней
            Assert.AreEqual("дней", StringUtils.DayCount(114)); // 114 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-114)); // -114 дней
            Assert.AreEqual("дней", StringUtils.DayCount(115)); // 115 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-115)); // -115 дней
            Assert.AreEqual("дней", StringUtils.DayCount(119)); // 119 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-119)); // -119 дней
            Assert.AreEqual("дней", StringUtils.DayCount(120)); // 120 дней
            Assert.AreEqual("день", StringUtils.DayCount(121)); // 121 день
            Assert.AreEqual("день", StringUtils.DayCount(-121)); // -121 день
            Assert.AreEqual("дня", StringUtils.DayCount(122)); // 122 дня
            Assert.AreEqual("дня", StringUtils.DayCount(-122)); // -122 дня
            Assert.AreEqual("дня", StringUtils.DayCount(123)); // 123 дня
            Assert.AreEqual("дня", StringUtils.DayCount(124)); // 124 дня
            Assert.AreEqual("дня", StringUtils.DayCount(-124)); // -124 дня
            Assert.AreEqual("дней", StringUtils.DayCount(125)); // 125 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-125)); // -125 дней
            Assert.AreEqual("дней", StringUtils.DayCount(130)); // 130 дней
            Assert.AreEqual("день", StringUtils.DayCount(131)); // 131 день
            Assert.AreEqual("дней", StringUtils.DayCount(199)); // 199 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-199)); // -199 дней
            Assert.AreEqual("дней", StringUtils.DayCount(200)); // 200 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-200)); // -200 дней
            Assert.AreEqual("день", StringUtils.DayCount(201)); // 201 день
            Assert.AreEqual("день", StringUtils.DayCount(-201)); // -201 день
            Assert.AreEqual("дня", StringUtils.DayCount(202)); // 202 дня
            Assert.AreEqual("дней", StringUtils.DayCount(210)); // 210 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-210)); // -210 дней
            Assert.AreEqual("дней", StringUtils.DayCount(211)); // 211 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-211)); // -211 дней
            Assert.AreEqual("дней", StringUtils.DayCount(212)); // 212 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-212)); // -212 дней
            Assert.AreEqual("дней", StringUtils.DayCount(213)); // 213 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-213)); // -213 дней
            Assert.AreEqual("дней", StringUtils.DayCount(214)); // 214 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-214)); // -214 дней
            Assert.AreEqual("дней", StringUtils.DayCount(215)); // 215 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-215)); // -215 дней
            Assert.AreEqual("дней", StringUtils.DayCount(299)); // 299 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-299)); // -299 дней
            Assert.AreEqual("дней", StringUtils.DayCount(300)); // 300 дней
            Assert.AreEqual("дней", StringUtils.DayCount(300)); // 400 дней
            Assert.AreEqual("дней", StringUtils.DayCount(500)); // 500 дней
            Assert.AreEqual("дней", StringUtils.DayCount(-500)); // -500 дней
            Assert.AreEqual("день", StringUtils.DayCount(691)); // 691 день
            Assert.AreEqual("дней", StringUtils.DayCount(1111)); // 1111 дней
            Assert.AreEqual("дня", StringUtils.DayCount(2222)); // 2222 дня
            Assert.AreEqual("дня", StringUtils.DayCount(-3333)); // -3333 дня
        }

        #endregion

        #region ToHtml/FromHtml

        /// <summary>
        /// Курсивный текст. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestEmphasis()
        {
            string markup = "I _believe_ every word.";
            string html = "I <i>believe</i> every word.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Курсивный текст со специальным символом курсива внутри. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestEmphasis_EmphasisSpecialSymbolInside()
        {
            string markup = "I __ _believe_ every word.";
            string html = "I <i>_ _believe</i> every word.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Курсивный и жирный текст. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestStrongAndEmphasis()
        {
            string markup = "I _*believe*_ every word.";
            string html = "I <i><b>believe</b></i> every word.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Курсивный и жирный текст, порядок закрытия "тегов" отличен от их открытия. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_StrongAndEmphasis_Exchange()
        {
            string markup = "I _*believe_* every word.";
            string html = "I <i><b>believe</i></b> every word.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Перед тегом жирного текста находится текст. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestCyrillic_LettersBeforeStrong()
        {
            string markup = "Я не*верю* вашим словам.";
            string html = "Я не*верю* вашим словам.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Жирный текст на кирилице. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestCyrillicStrong()
        {
            string markup = "Я не *верю* вашим словам.";
            string html = "Я не <b>верю</b> вашим словам.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Специальный символ курсива перед тегом жирного текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestCyrillic_EmphasisBeforeStrong()
        {
            string markup = "Я не _*верю* вашим словам.";
            string html = "Я не _<b>верю</b> вашим словам.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Специальный символ жирного текста внутри жирного текста с пробелами по обе стороны. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestCyrillic_StrongSpicailCharAndSpacesIndside()
        {
            string markup = "Я не *ве * рю* вашим словам.";
            string html = "Я не <b>ве * рю</b> вашим словам.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Несколько курсивных текстов. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestMultipleEmphasis()
        {
            string markup = "I _believe_ every word from _you_ my friend.";
            string html = "I <i>believe</i> every word from <i>you</i> my friend.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Специальный символ курсива с пробелом после него внутри жирного текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestComplexTest()
        {
            string markup = "I _believe *every_ word* from you my friend.";
            string html = "I <i>believe <b>every</i> word</b> from you my friend.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Специальные символы жирного и курсивного текста с пробелом между ними внутри жирного. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestComplexTest2()
        {
            string markup = "I _believe *every_ *word* from you my friend.";
            string html = "I <i>believe <b>every</i> *word</b> from you my friend.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Первый символ жирного текста - пробел. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_SpaceFirstChar()
        {
            string markup = "I * believe* from you my friend.";
            string html = "I * believe* from you my friend.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Последний символ жирного текста - пробел. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_SpaceLastChar()
        {
            string markup = "I *believe * from you my friend.";
            string html = "I *believe * from you my friend.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Пробел первый и последний символ жирного текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_Space_FirstAndLastChar()
        {
            string markup = "I * believe * from you my friend.";
            string html = "I * believe * from you my friend.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Первый символ жирного текста - перевод строки. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_LineFeedFirst()
        {
            string markup = "I *\r\nbelieve* from you my friend.";
            string html = "I <b><br />believe</b> from you my friend.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Первый символ жирного текста - перевод строки, последний символ - пробел. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_LineFeedFirst_SpaceLastChar()
        {
            string markup = "I *\r\nbelieve * from you my friend.";
            string html = "I *<br />believe * from you my friend.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Перевод строки перед жирным текстом. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_LineFeedBeforeStrong()
        {
            string markup = "I\r\n*believe* from you my friend.";
            string html = "I<br /><b>believe</b> from you my friend.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Перевод строки и специальный символ курсива в конце жирного текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_LineFeedAndEmphasisSpecialCharInEnd()
        {
            string markup = "I *believe\r\n_* from you my friend.";
            string html = "I <b>believe<br />_</b> from you my friend.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Перевод строки внутри жирного текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestLineFeedInsideStrong()
        {
            string markup = "I *believe\r\nqwerty* from you my friend.";
            string html = "I <b>believe<br />qwerty</b> from you my friend.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Двойные специальные символы жирного текста вокруг текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestDoubleStrong()
        {
            string markup = "I **believe** from you my friend.";
            string html = "I *<b>believe</b>* from you my friend.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Двойные специальные символы жирного текста вокруг знака препинания. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_DoubleStrongAroundPunctuationSimbol()
        {
            string markup = "I **?** from you my friend.";
            string html = "I *<b>?*</b> from you my friend.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Текст после специального символа жирного текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestLettersAfterStrong()
        {
            string markup = "I *believ*e from you my friend.";
            string html = "I *believ*e from you my friend.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Жирный текст в начале строки. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_StrongInTheBeginningString()
        {
            string markup = "*I believe* from you my friend.";
            string html = "<b>I believe</b> from you my friend.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Двойной специальный символ в начале жирного текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestNestedStrongs()
        {
            string markup = "I **believe* from you* my friend.";
            string html = "I *<b>believe</b> from you* my friend.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Знак препинания после жирного текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_StrongPunctiationSymbolAfterStrong()
        {
            string markup = "And then? She *fell*!";
            string html = "And then? She <b>fell</b>!";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Знак препинания и текст поле жирного текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_StrongPunctiationSymbolAndLettersAfterStrong()
        {
            string markup = "And then? She *fell*!qwerty";
            string html = "And then? She <b>fell</b>!qwerty";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Знаки препинания вокруг жирного текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_PunctiationSymbolAroundStrong()
        {
            string markup = "And then?*She fell*!";
            string html = "And then?*She fell*!";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Три подряд идущих специальных символов жирного текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_StrongSpicialSymbolInsideStrong()
        {
            string markup = "And then? She ***!";
            string html = "And then? She *<b></b>!";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Жирный и курсив вокруг знака препинания. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_EmphasisWithPunctiationSymbolInsideStrong()
        {
            string markup = "And then? She *_!_*";
            string html = "And then? She <b>_!_</b>";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Жирный и курсив вокруг текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_EmphasisWithLettersInsideStrong()
        {
            string markup = "And then? She *_y_*";
            string html = "And then? She <b><i>y</i></b>";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Жирный и курсивный перевод строки. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_EmphasisWithLineFeedInsideStrong()
        {
            string markup = "And then? She *_\r\n_*";
            string html = "And then? She <b><i><br /></i></b>";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Два жирных фрагмента текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_TwoStrongInString()
        {
            string markup = "*one* *two*";
            string html = "<b>one</b> <b>two</b>";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Зачеркнутый текст. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestDeletedLetters()
        {
            string markup = "I'm -sure- not sure.";
            string html = "I'm <del>sure</del> not sure.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Зачеркнутый знак препинания. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_DeletedPunctiationSymbol()
        {
            string markup = "I'm -!- not sure.";
            string html = "I'm -!- not sure.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Зачеркнутый жирный текст. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_StrongLettersInsideDeleted()
        {
            string markup = "I'm -*qwerty*- not sure.";
            string html = "I'm <del><b>qwerty</b></del> not sure.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Перевод строки внутри жирного курсивного текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_StrongWithLettersAndLineFeedInsideDeleted()
        {
            string markup = "I'm -*qwe\r\nrty*- not sure.";
            string html = "I'm <del><b>qwe<br />rty</b></del> not sure.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Двойные специальные символы зачеркнутого текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestDoubleDeleted()
        {
            string markup = "I'm --qwe-- not sure.";
            string html = "I'm -<del>qwe</del>- not sure.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Знак препинания перед зачеркнутым. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_PunctiationSymbolBeforeDeleted()
        {
            string markup = "I'm?-qwerty- not sure.";
            string html = "I'm?-qwerty- not sure.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Знак препинания между зачеркнутыми текстами. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_PunctiationSymbolBetweenDeleted()
        {
            string markup = "I'm -qwe-!-rty- not sure.";
            string html = "I'm <del>qwe</del>!-rty- not sure.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Знаки препинания и пробел между зачеркнутыми текстами. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_PunctiationSymbolBetweenTwoDeleted()
        {
            string markup = "I'm -qwe-! ?-rty- not sure.";
            string html = "I'm <del>qwe</del>! ?-rty- not sure.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Перевод строки между специальными символами зачеркнутого текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestLineFeedDeleted()
        {
            string markup = "I'm -\r\n- not sure.";
            string html = "I'm <del><br /></del> not sure.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Перевод строки. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestLineFeed()
        {
            string markup = "I'm \r\n not sure.";
            string html = "I'm <br /> not sure.";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Ссылка. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestLink()
        {
            string markup = "\"Яндекс\":www.ya.ru. Найдется все.";
            string html = "<a href='http://www.ya.ru'>Яндекс</a>. Найдется все.";

            TestFormatting(markup, html, markup.Replace("www.", "http://www."));
        }

        /// <summary>
        /// Ссылка с кирилицей. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestCyrillicLink()
        {
            string markup = "\"текст\":www.ya.ru. Найдется все.";
            string html = "<a href='http://www.ya.ru'>текст</a>. Найдется все.";

            TestFormatting(markup, html, markup.Replace("www.", "http://www."));
        }

        /// <summary>
        /// Ссылка со специальными символами жирного текста без пробелов. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_CyrillicStrongLinkWithoutSpaces()
        {
            string markup = "\"*текст*\":www.ya.ru. Найдется все.";
            string html = "<a href='http://www.ya.ru'><b>текст</b></a>. Найдется все.";

            TestFormatting(markup, html, markup.Replace("www.", "http://www."));
        }

        /// <summary>
        /// Ссылка со специальными символами жирного текста и переводом строки без пробелов. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_CyrillicStrongLinkWithLineFeedAndWithoutSpaces()
        {
            string markup = "\"*те\r\nкст*\":www.ya.ru. Найдется все.";
            string html = "<a href='http://www.ya.ru'><b>те<br />кст</b></a>. Найдется все.";

            TestFormatting(markup, html, markup.Replace("www.", "http://www."));
        }

        /// <summary>
        /// Ссылка со специальными символами жирного текста с пробелами. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_CyrillicStrongLinkWithSpaces()
        {
            string markup = "\"qwe *текст* \":www.ya.ru. Найдется все.";
            string html = "<a href='http://www.ya.ru'>qwe <b>текст</b> </a>. Найдется все.";

            TestFormatting(markup, html, markup.Replace("www.", "http://www."));
        }

        /// <summary>
        /// Ссылка со специальными символами зачеркнутого текста без пробелов. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_CyrillicDeleteLinkWithoutSpaces()
        {
            string markup = "\"-текст-\":www.ya.ru. Найдется все.";
            string html = "<a href='http://www.ya.ru'><del>текст</del></a>. Найдется все.";

            TestFormatting(markup, html, markup.Replace("www.", "http://www."));
        }

        /// <summary>
        /// Ссылка со специальными символами жирного и зачеркнутого текста без пробелов. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_CyrillicDeleteStrongLinkWithoutSpaces()
        {
            string markup = "\"-*текст*-\":www.ya.ru. Найдется все.";
            string html = "<a href='http://www.ya.ru'><del>*текст*</del></a>. Найдется все.";

            TestFormatting(markup, html, markup.Replace("www.", "http://www."));
        }

        /// <summary>
        /// Ссылка со специальными символами жирного текста со знаками препинания. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_CyrillicDeleteStrongLinkWithPunctiationSymbol()
        {
            string markup = "\"!-*текст*-!\":www.ya.ru. Найдется все.";
            string html = "<a href='http://www.ya.ru'>!-*текст*-!</a>. Найдется все.";

            TestFormatting(markup, html, markup.Replace("www.", "http://www."));
        }

        /// <summary>
        /// Ссылка со специальными символами жирного курсивного текста и предшествующим переводом строки, без пробелов. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_CyrillicDeleteStrongLinkWithLineFeedInStart()
        {
            string markup = "asd \"\r\n-*текст*-\":www.ya.ru. Найдется все.";
            string html = "asd <a href='http://www.ya.ru'><br /><del><b>текст</b></del></a>. Найдется все.";

            TestFormatting(markup, html, markup.Replace("www.", "http://www."));
        }

        /// <summary>
        /// Список с текстом. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestUlWithLetters()
        {
            string markup = "первый список\r\n* раз\r\n* два\r\n\r\nвторой список\r\n* три \r\n* четыре\r\n\r\nконец";
            string html = "первый список<ul><li>раз</li><li>два</li></ul>второй список<ul><li>три </li><li>четыре</li></ul>конец";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Список с зачеркнутым текстом. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestUlWithDeleteletters()
        {
            string markup = "первый список\r\n* -раз- \r\n* два\r\n\r\nвторой список\r\n* три \r\n* четыре\r\n\r\nконец";
            string html = "первый список<ul><li><del>раз</del> </li><li>два</li></ul>второй список<ul><li>три </li><li>четыре</li></ul>конец";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Список с жирным текстом с пробелами. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestUlWithLineFeedItem()
        {
            string markup = "первый список\r\n* * раз * \r\n* два\r\n\r\nвторой список\r\n* три \r\n* четыре\r\n\r\nконец";
            string html = "первый список<ul><li>* раз * </li><li>два</li></ul>второй список<ul><li>три </li><li>четыре</li></ul>конец";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Список с зачеркнутым текстом без пробелов. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestUlLineFeedAmdStrong()
        {
            string markup = "первый список\r\n* *раз* \r\n* два\r\n\r\nвторой список\r\n* три \r\n* четыре\r\n\r\nконец";
            string html = "первый список<ul><li><b>раз</b> </li><li>два</li></ul>второй список<ul><li>три </li><li>четыре</li></ul>конец";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Список без окончания. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestUlWithoutEndOfUl()
        {
            string markup = "первый список\r\n* раз \r\n* два\r\n* три\r\nконец";
            string html = "первый список<ul><li>раз </li><li>два</li><li>три<br />конец</li></ul>";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Список без предшествующего текста. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestUlWithoutLetterBefore()
        {
            string markup = "* раз\r\n* два\r\n* три";
            string html = "<ul><li>раз</li><li>два</li><li>три</li></ul>";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Несколько списков. Перевод из textile в html и обратно.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestUlMixed()
        {
            string markup = "первый список\r\n* раз\r\n* два\r\n\r\n*второй* список\r\n* три \r\n* _четыре_\r\n\r\nконец";
            string html = @"первый список<ul><li>раз</li><li>два</li></ul><b>второй</b> список<ul><li>три </li><li><i>четыре</i></li></ul>конец";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Ссылка со сложным адресом.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestLinkWithUnusualSymbols()
        {
            string markup = "\"Заказ на производство\":demo.bizpulse.ru/ProductionOrder/Details?id=f0996dfd-43bd-43c4-8dfc-a03e014264a4&backURL=http%3A%2F%2Fdemo.bizpulse.ru%2FProductionOrder%2FList";
            string html = @"<a href='http://demo.bizpulse.ru/ProductionOrder/Details?id=f0996dfd-43bd-43c4-8dfc-a03e014264a4&backURL=http%3A%2F%2Fdemo.bizpulse.ru%2FProductionOrder%2FList'>Заказ на производство</a>";

            TestFormatting(markup, html, markup.Replace(":demo.", ":http://demo."));
        }

        /// <summary>
        /// Простое опознание url в тексте и преобразование его в ссылку.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestSimpleLink()
        {
            string markup = "Официальный твиттер президента: www.twitter.com/#!/KremlinRussia, личный микроблог: https://twitter.com/#!/medvedevrussia";
            string html = "Официальный твиттер президента: <a href='http://www.twitter.com/#!/KremlinRussia'>www.twitter.com/#!/KremlinRussia</a>, личный микроблог: <a href='https://twitter.com/#!/medvedevrussia'>https://twitter.com/#!/medvedevrussia</a>";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Синтаксис "имя":ссылка
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestLinkWithNames()
        {
            string markup = "\"Официальный твиттер президента\":www.twitter.com/#!/KremlinRussia, \"личный микроблог\":https://twitter.com/#!/medvedevrussia";
            string html = "<a href='http://www.twitter.com/#!/KremlinRussia'>Официальный твиттер президента</a>, <a href='https://twitter.com/#!/medvedevrussia'>личный микроблог</a>";

            TestFormatting(markup, html, markup.Replace("www.", "http://www."));
        }

        /// <summary>
        /// Ведущих символов w слишком много.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestLinkWithTooManyWInStart()
        {
            string markup = "wwwwww.twitter.com/#!/KremlinRussia";
            string html = "www<a href='http://www.twitter.com/#!/KremlinRussia'>www.twitter.com/#!/KremlinRussia</a>";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Ссылка без доменного имени
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestLinkWithoutDomainName()
        {
            string markup = "www.com";
            string html = "<a href='http://www.com'>www.com</a>";

            TestFormatting(markup, html);
        }

        /// <summary>
        /// Ссылка без доменного имени
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestLinkBeginWithDot()
        {
            string markup = "http://.twitter";
            string html = "<a href='http://.twitter'>http://.twitter</a>";

            TestFormatting(markup, html, markup.Replace("www.", "http://www."));
        }

        /// <summary>
        /// Пробел между протоколом и ссылкой
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestSpaceBetweenProtocolAndLink()
        {
            string markup = "http:// www.twitter.com";
            string html = "http:// <a href='http://www.twitter.com'>www.twitter.com</a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Пробел между протоколом https и ссылкой
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestHttpsSpaceBetweenProtocolAndLink()
        {
            string markup = "https:// www.twitter.com";
            string html = "https:// <a href='http://www.twitter.com'>www.twitter.com</a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Слишком много слешей
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestTooManySlashes()
        {
            string markup = "http:////www.twitter.com";
            string html = "<a href='http:////www.twitter.com'>http:////www.twitter.com</a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Протокол https
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestHttps()
        {
            string markup = "https://www.twitter.com";
            string html = "<a href='https://www.twitter.com'>https://www.twitter.com</a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Слишком много слешей, https
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestHttpsTooManySlashes()
        {
            string markup = "https:////www.twitter.com";
            string html = "<a href='https:////www.twitter.com'>https:////www.twitter.com</a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Неверная запись протокола http
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestErrorInProtocol()
        {
            string markup = "http:://www.twitter.com";
            string html = "http:://www.twitter.com";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Неверная запись протокола https
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestHttpsErrorInProtocol()
        {
            string markup = "https:://www.twitter.com";
            string html = "https:://www.twitter.com";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Текст перед ссылкой
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestTextBeforeLink()
        {
            string markup = "hhttp://www.twitter.com";
            string html = "h<a href='http://www.twitter.com'>http://www.twitter.com</a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Текст перед ссылкой, протокол https
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestHttpsTextBeforeLink()
        {
            string markup = "hhttps://www.twitter.com";
            string html = "h<a href='https://www.twitter.com'>https://www.twitter.com</a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Текст после протокола
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestTextAfterProtocol()
        {
            string markup = "http://qwerty";
            string html = "<a href='http://qwerty'>http://qwerty</a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Текст c пробелами после протокола http
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestTextWithSpacesAfterProtocol()
        {
            string markup = "http://qwer ty";
            string html = "<a href='http://qwer'>http://qwer</a> ty";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Текст c пробелами после протокола https
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestHttpsTextWithSpacesAfterProtocol()
        {
            string markup = "https://qwer ty";
            string html = "<a href='https://qwer'>https://qwer</a> ty";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Текст со спец символом после ссылки
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestTextWithSpecialSymbolAfterLink()
        {
            string markup = "http://www.twitter.com#qwerty";
            string html = "<a href='http://www.twitter.com#qwerty'>http://www.twitter.com#qwerty</a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Текст со спец символом после ссылки, протокол https
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestHttpsTextWithSpecialSymbolAfterLink()
        {
            string markup = "https://www.twitter.com#qwerty";
            string html = "<a href='https://www.twitter.com#qwerty'>https://www.twitter.com#qwerty</a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Текст со спец символом в ссылке
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestSpecialSymbolInsideLink()
        {
            string markup = "http://www.twitter.com%qwerty";
            string html = "<a href='http://www.twitter.com%qwerty'>http://www.twitter.com%qwerty</a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Название ссылки в оодиночных кавычках
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestLinkNameInWrongQuote()
        {
            string markup = "'qwerty':http://www.twitter.com";
            string html = "'qwerty':http://www.twitter.com";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Название ссылки содержит пробел
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestLinkNameHaveSpace()
        {
            string markup = "\"qwe rty\":http://www.twitter.com";
            string html = "<a href='http://www.twitter.com'>qwe rty</a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Название ссылки содержит пробел и лишнее двоеточие между названием и ссылкой
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestAfterLinkNameDoubleColon()
        {
            string markup = "\"qwe rty\"::http://www.twitter.com";
            string html = "\"qwe rty\"::http://www.twitter.com";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Название ссылки содержит пробел и лишнее двоеточие между названием и ссылкой, протокол https
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestHttps_AfterLinkNameDoubleColon()
        {
            string markup = "\"qwe rty\"::https://www.twitter.com";
            string html = "\"qwe rty\"::https://www.twitter.com";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Название ссылки содержит пробел и цифру перед ссылкой.
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestHttps_NumberBeforeLink()
        {
            string markup = "\"qwe rty\":9https://www.twitter.com";
            string html = "\"qwe rty\":9<a href='https://www.twitter.com'>https://www.twitter.com</a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Название ссылки содержит пробел и лишнее двоеточие между названием и ссылкой
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestAfterLinkNameDoubleColonWithspace()
        {
            string markup = "\"qwe rty\": :http://www.twitter.com";
            string html = "\"qwe rty\": :http://www.twitter.com";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Название ссылки содержит пробел и лишнее двоеточие между названием и ссылкой, протокол https
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestHtpps_AfterLinkNameDoubleColonWithspace()
        {
            string markup = "\"qwe rty\": :https://www.twitter.com";
            string html = "\"qwe rty\": :https://www.twitter.com";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Название ссылки содержит пробел, протокол https
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestHttpsLinkNameHaveSpace()
        {
            string markup = "\"qwe rty\":https://www.twitter.com";
            string html = "<a href='https://www.twitter.com'>qwe rty</a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Название ссылки - пустая строка
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestLinkNameEmptyString()
        {
            string markup = @""""":http://www.twitter.com";
            string html = "<a href='http://www.twitter.com'></a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Название ссылки - пустая строка, протокол https
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestHttpsLinkNameEmptyString()
        {
            string markup = @""""":https://www.twitter.com";
            string html = "<a href='https://www.twitter.com'></a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Название ссылки - пустая строка, протокол http
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_Test_LinkNameIsDoubleQuote()
        {
            string markup = "\"\"\":http://www.twitter.com";
            string html = "<a href='http://www.twitter.com'>\"</a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Название ссылки - пустая строка, протокол https
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestHttps_LinkNameIsDoubleQuote()
        {
            string markup = "\"\"\":https://www.twitter.com";
            string html = "<a href='https://www.twitter.com'>\"</a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Название ссылки - пробел
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestLinkNameIsSpace()
        {
            string markup = @""" "":http://www.twitter.com";
            string html = "<a href='http://www.twitter.com'> </a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Название ссылки - пробел, протокол https
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_MarkupConvert_TestHttpsLinkNameIsSpace()
        {
            string markup = @""" "":https://www.twitter.com";
            string html = "<a href='https://www.twitter.com'> </a>";

            TestFormatting(markup, html, markup);
        }

        /// <summary>
        /// Перевод текста из textile в html и обратно c проверкой корректности перевода.
        /// </summary>
        /// <param name="markup">Текст в разметке textile.</param>
        /// <param name="html">Текст в разметке html.</param>
        /// <param name="otherMarkup">Текст, с которым нужно сравнивать при переводе обратно (в случае, если отличается от markup).</param>
        private void TestFormatting(string markup, string html, string otherMarkup = null)
        {
            string markupToHtml = StringUtils.ToHtml(markup);
            Assert.AreEqual(html, markupToHtml);

            string htmlToMarkup = StringUtils.FromHtml(markupToHtml);

            if (otherMarkup == null)
            {
                Assert.AreEqual(markup, htmlToMarkup);
            }
            else
            {
                Assert.AreEqual(otherMarkup, htmlToMarkup);
            }
        }

        #endregion

        #region IdList

        /// <summary>
        /// Парсинг строки с int Id
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_GetIntIdList_Must_Return_Id_List()
        {
            string ids = "1_2_34";
            
            IEnumerable<int> listIds = StringUtils.GetIntIdList(ids);
            
            List<int> list = new List<int>();           
            list.Add(1);
            list.Add(2);
            list.Add(34);

            int i = 0;
            foreach (var id in listIds)
            {
                Assert.AreEqual(id, list[i]);
                i++;
            }
        }

        /// <summary>
        /// Парсинг строки с short Id
        /// </summary>
        [TestMethod]
        public void StringUtilsTest_GetShortIdList_Must_Return_Id_List()
        {
            string ids = "1_2_34";
            IEnumerable<short> listIds = StringUtils.GetShortIdList(ids);

            List<short> list = new List<short>();
            list.Add(1);
            list.Add(2);
            list.Add(34);

            int i = 0;
            foreach (var id in listIds)
            {
                Assert.AreEqual(id, list[i]);
                i++;
            }
        }

        #endregion
    }
}