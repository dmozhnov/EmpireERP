using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class ArticleAccountingPriceTest
    {
        private readonly ArticleGroup articleGroup = null;
        private readonly MeasureUnit measureUnit = null;
        private readonly Article article1 = null;
        private readonly Article article2 = null;
        private readonly Article article3 = null;
        private readonly ArticleAccountingPrice articleAccountingPrice1 = null;
        private readonly ArticleAccountingPrice articleAccountingPrice2 = null;
        private readonly ArticleAccountingPrice articleAccountingPrice3 = null;
        private readonly List<ArticleAccountingPrice> articleAccountingPriceList1 = null;

        public ArticleAccountingPriceTest()
        {
            articleGroup = new ArticleGroup("Бытовая техника", "Бытовая техника");
            articleGroup.SalaryPercent = 15;
            articleGroup.Id = 8;

            measureUnit = new MeasureUnit("шт.", "Штука", "123", 0);            
            measureUnit.Id = 17;

            article1 = new Article("Пылесос", articleGroup, measureUnit, true);
            article1.Id = 9;
            article2 = new Article("Фен", articleGroup, measureUnit, true);
            article2.Id = 29;
            article3 = new Article("Утюг", articleGroup, measureUnit, true);
            article3.Id = 421;

            articleAccountingPrice1 = new ArticleAccountingPrice(article1, 1M);
            articleAccountingPrice2 = new ArticleAccountingPrice(article2, 1001M);
            articleAccountingPrice3 = new ArticleAccountingPrice(article3, 1192.45M);

            articleAccountingPriceList1 = new List<ArticleAccountingPrice>();
            articleAccountingPriceList1.Add(articleAccountingPrice1);
            articleAccountingPriceList1.Add(articleAccountingPrice2);
            articleAccountingPriceList1.Add(articleAccountingPrice3);

        }

        [TestMethod]
        public void ArticleAccountingPrice_Initial_Parameters_Must_Be_Set()
        {
            var articleAccountingPrice = new ArticleAccountingPrice(article1, 1M);

            Assert.AreEqual(1M, articleAccountingPrice.AccountingPrice);
            Assert.AreEqual(article1.Id, articleAccountingPrice.Article.Id);
            Assert.AreEqual(article1.ArticleGroup.Id, articleAccountingPrice.Article.ArticleGroup.Id);
            Assert.AreEqual(article1.FullName, articleAccountingPrice.Article.FullName);
            Assert.AreEqual(article1.ShortName, articleAccountingPrice.Article.ShortName);
            Assert.AreEqual(article1.MeasureUnit.Id, articleAccountingPrice.Article.MeasureUnit.Id);
        }

        [TestMethod]
        public void ArticleAccountingPrice_ReDeletion_Must_Not_Work()
        {
            var articleAccountingPrice = new ArticleAccountingPrice(article1, 1M);
            var curDate = DateTime.Now;
            var nextDate = curDate + new TimeSpan(1, 0, 0, 0);

            Assert.IsNull(articleAccountingPrice.DeletionDate);

            articleAccountingPrice.DeletionDate = curDate;
            
            Assert.AreEqual(curDate, articleAccountingPrice.DeletionDate);

            articleAccountingPrice.DeletionDate = nextDate;

            Assert.AreEqual(curDate, articleAccountingPrice.DeletionDate);
            Assert.AreNotEqual(nextDate, articleAccountingPrice.DeletionDate);
        }
    }
}
