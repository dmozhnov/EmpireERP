using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Wholesale.ApplicationServices.Test
{
    [TestClass]
    public class AccountingPriceListServiceTest
    {
        #region Поля
        
        private readonly ArticleGroup articleGroup;
        private readonly Storage storage1;
        private readonly Storage storage2;
        private readonly MeasureUnit measureUnit;
        private readonly Article article1;
        private readonly Article article2;
        private readonly Article article3;
        private readonly ArticleAccountingPrice articleAccountingPrice1_1;
        private readonly ArticleAccountingPrice articleAccountingPrice1_2;
        private readonly ArticleAccountingPrice articleAccountingPrice1_3;
        private readonly ArticleAccountingPrice articleAccountingPrice2_1;
        private readonly ArticleAccountingPrice articleAccountingPrice2_2;        
        private readonly AccountingPriceList accountingPriceList1;
        private readonly AccountingPriceList accountingPriceList2;
        private User user;

        #endregion

        public AccountingPriceListServiceTest()
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

            storage1 = new Storage("Склад 1", StorageType.DistributionCenter);
            storage2 = new Storage("Склад 2", StorageType.DistributionCenter);

            articleAccountingPrice1_1 = new ArticleAccountingPrice(article1, 1M);
            articleAccountingPrice1_2 = new ArticleAccountingPrice(article2, 1001M);
            articleAccountingPrice1_3 = new ArticleAccountingPrice(article3, 1192.45M);

            user = new User(new Employee("Иван", "Иванов", "Иванович", new EmployeePost("Менеджер"), null), "Иванов Иван", "ivanov", "pa$$w0rd", new Team("Тестовая команда", null), null);

            accountingPriceList1 = new AccountingPriceList("27", DateTime.Now, null, new List<Storage>{storage1}, user);
            accountingPriceList1.AddArticleAccountingPrice(articleAccountingPrice1_1);
            accountingPriceList1.AddArticleAccountingPrice(articleAccountingPrice1_2);
            accountingPriceList1.AddArticleAccountingPrice(articleAccountingPrice1_3);

            articleAccountingPrice2_1 = new ArticleAccountingPrice(article1, 34M);
            articleAccountingPrice2_2 = new ArticleAccountingPrice(article3, 56M);

            accountingPriceList2 = new AccountingPriceList("39", DateTime.Now, null, new List<Storage>{storage1, storage2}, user);
            accountingPriceList2.AddArticleAccountingPrice(articleAccountingPrice2_1);
            accountingPriceList2.AddArticleAccountingPrice(articleAccountingPrice2_2);
        }
    }
}
