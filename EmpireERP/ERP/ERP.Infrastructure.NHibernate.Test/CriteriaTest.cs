using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ERP.Infrastructure.IoC;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Infrastructure.NHibernate.Repositories.Criteria;
using ERP.Infrastructure.NHibernate.SessionManager;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Infrastructure.SessionManager;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.NHibernate.Mappings;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using NHibernate.ByteCode.Castle;
using NHibernate.Cfg;
using NHibernate.Criterion;
using Bizpulse.Infrastructure.FluentNHibernate;

namespace ERP.Infrastructure.NHibernate.Test
{
    [TestClass]
    public class CriteriaTest
    {
        #region Тестовый класс
        
        private class TestEntity
        {
            public string Name { get; set; }
            public decimal Sum { get; set; }

            public decimal GetValue()
            {
                return 42;
            }

            public static decimal Field;
            public static decimal Property { get; set; }
        }

        #endregion

        private Mock<BaseRepository> repository;
        private ISession session;

        public class FluentInitializer : INHibernateInitializer
        {
            public Configuration GetConfiguration(string dbServer, string dbName)
            {
                return Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2008
                        .ConnectionString("Data Source=" + dbServer + ";Initial Catalog=" + dbName + ";Integrated Security=True")                        
                        .DefaultSchema("dbo"))
                    .ProxyFactoryFactory<ProxyFactoryFactory>()
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<OrganizationMap>()
                        .Conventions.AddFromAssemblyOf<AnsiStringConvention>()) // подключаем конвенции                                     
                    //.ExposeConfiguration(BuildSchema)
                    .BuildConfiguration();
            }
        }

        public CriteriaTest()
        {
            repository = new Mock<BaseRepository>();
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));

            var sessionManager = (new NHibernateSessionManager() as ISessionManager);

            IoCContainer.Register<INHibernateInitializer>(new FluentInitializer());

            sessionManager.CreateSession("(local)", "wholesale_000001");
            session = NHibernateSessionManager.CurrentSession;
        }

        [TestMethod]
        public void Criteria_GetAll()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>();
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();
            
            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        } 

        [TestMethod]
        public void Criteria_Operation_Equal1()
        {
            Guid g = Guid.NewGuid();

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.Id == g);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Eq("Id", g));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_Equal_With_Field_Name1()
        {
            Guid g = Guid.NewGuid();

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where("Id", CriteriaCond.Eq, g);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Eq("Id", g));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_Equal2()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.Date == null);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.IsNull("Date"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_Equal_With_Field_Name2()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where("Date", CriteriaCond.Eq, null);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.IsNull("Date"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_Equal_Properties()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.ApprovedSum == x.PendingSum);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.EqProperty("ApprovedSum", "PendingSum"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_NotEqual_Properties()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.ApprovedSum != x.PendingSum);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.NotEqProperty("ApprovedSum", "PendingSum"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_NotEqual1()
        {
            Guid g = Guid.NewGuid();

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.Id != g);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Not(Expression.Eq("Id", g)));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_NotEqual_With_Field_Name1()
        {
            Guid g = Guid.NewGuid();

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where("Id", CriteriaCond.NotEq, g);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Not(Expression.Eq("Id", g)));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_NotEqual2()
        {
            Guid g = Guid.NewGuid();

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.Date != null);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.IsNotNull("Date"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_NotEqual_With_Field_Name2()
        {
            Guid g = Guid.NewGuid();

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where("Date", CriteriaCond.NotEq, null);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.IsNotNull("Date"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_GreateThen1()
        {
            decimal sum = 42;

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum > sum);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Gt("PendingSum", sum));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_GreateThen_With_Field_Name1()
        {
            decimal sum = 42;

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where("PendingSum", CriteriaCond.Gt, sum);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Gt("PendingSum", sum));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_GreateThen2()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.Date > null);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_GreateThen_With_Field_Name2()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where("Date", CriteriaCond.Gt, null);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_GreateThen_Properties()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum > x.ApprovedSum);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Gt("PendingSum", "ApprovedSum"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_GreateThenOrEqual1()
        {
            decimal sum = 42;

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum >= sum);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Ge("PendingSum", sum));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_GreateThenOrEqual_With_Field_Name1()
        {
            decimal sum = 42;

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where("PendingSum", CriteriaCond.Ge, sum);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Ge("PendingSum", sum));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_GreateThenOrEqual2()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.Date >= null);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_GreateThenOrEqual_With_Field_Name2()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where("Date", CriteriaCond.Ge, null);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_GreateThenOrEqual_Properties()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum >= x.ApprovedSum);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Ge("PendingSum", "ApprovedSum"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_LessThen1()
        {
            decimal sum = 42;

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum < sum);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Lt("PendingSum", sum));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_LessThen_With_Field_Name1()
        {
            decimal sum = 42;

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where("PendingSum", CriteriaCond.Lt, sum);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Lt("PendingSum", sum));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_LessThen2()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.Date < null);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_LessThen_With_Field_Name2()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where("Date", CriteriaCond.Lt, null);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_LessThen_Properties()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum < x.ApprovedSum);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Lt("PendingSum", "ApprovedSum"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_LessThenOrEqual1()
        {
            decimal sum = 42;

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum <= sum);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Le("PendingSum", sum));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_LessThenOrEqual_With_Field_Name1()
        {
            decimal sum = 42;

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where("PendingSum", CriteriaCond.Le, sum);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Le("PendingSum", sum));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_LessThenOrEqual2()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.Date <= null);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_LessThenOrEqual_With_Field_Name2()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where("Date", CriteriaCond.Le, null);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_LessThenOrEqual_Properties()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum <= x.ApprovedSum);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Le("PendingSum", "ApprovedSum"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_LogicOr1()
        {
            decimal sumMin = 12;
            decimal sumMax = 42;

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum <= sumMin || x.PendingSum > sumMax);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(
                Expression.Or(
                    Expression.Le("PendingSum", sumMin),
                    Expression.Gt("PendingSum", sumMax)));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_LogicOr2()
        {
            decimal sumMin = 12;
            decimal sumMax = 42;

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum <= sumMin || sumMax*2 > sumMax);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_LogicAnd1()
        {
            decimal sumMin = 12;
            decimal sumMax = 42;

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum <= sumMin && x.PendingSum > sumMax);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(
                Expression.And(
                    Expression.Le("PendingSum", sumMin),
                    Expression.Gt("PendingSum", sumMax)));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_LogicAnd2()
        {
            decimal sumMin = 12;
            decimal sumMax = 42;

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum <= sumMin && sumMin + sumMax > sumMax);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Le("PendingSum", sumMin));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_Call_Method_Of_Static_Class()
        {
            int sum = 42;

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum <= Convert.ToDecimal(sum));
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Le("PendingSum", Convert.ToDecimal(sum)));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_Call_Field_Of_Static_Class()
        {
            TestEntity.Field = 42;

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum <= TestEntity.Field);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Le("PendingSum", TestEntity.Field));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_Call_Property_Of_Static_Class()
        {
            TestEntity.Field = 42;

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum <= TestEntity.Property);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Le("PendingSum", TestEntity.Property));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_Call_Method_Of_Object()
        {
            TestEntity testValue = new TestEntity();

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum <= testValue.GetValue());
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Le("PendingSum", testValue.GetValue()));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_Call_Field_Of_Object()
        {
            TestEntity testValue = new TestEntity() { Sum = 42 };

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum <= testValue.Sum);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Le("PendingSum", testValue.Sum));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Operation_Call_Property_Of_Object()
        {
            TestEntity testValue = new TestEntity() { Name = "42" };

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.Number == testValue.Name);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Eq("Number", testValue.Name));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Using_Constant1()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.Number == "42");
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Eq("Number", "42"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Using_Constant2()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.Date == null);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.IsNull("Date"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Using_AutoConverTypes1()
        {
            short value = 42;

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingSum == value);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Eq("PendingSum", value));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Using_AutoConverTypes2()
        {
            decimal value = 42;

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.PendingValueAddedTax.Id == value);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Eq("PendingValueAddedTax.Id", value));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Using_AutoConverTypes3()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Where(x => x.State == ReceiptWaybillState.New);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Eq("State", ReceiptWaybillState.New));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_OneOf()
        {
            List<decimal> testArr = new List<decimal>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<object> testArrObj = new List<object>();
            foreach (var val in testArr)
                testArrObj.Add(val);

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().OneOf(x => x.PendingSum, testArr);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.In("PendingSum", testArrObj.ToArray<object>() ));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_OneOf_With_Field_Name()
        {
            List<decimal> testArr = new List<decimal>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<object> testArrObj = new List<object>();
            foreach (var val in testArr)
                testArrObj.Add(val);

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().OneOf("PendingSum", testArr);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.In("PendingSum", testArrObj.ToArray<object>()));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_NotOneOf()
        {
            List<decimal> testArr = new List<decimal>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<object> testArrObj = new List<object>();
            foreach (var val in testArr)
                testArrObj.Add(val);

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().NotOneOf(x => x.PendingSum, testArr);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Not(Expression.In("PendingSum", testArrObj.ToArray<object>())));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_NotOneOf_In_Criterion()
        {
            List<decimal> testArr = new List<decimal>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<object> testArrObj = new List<object>();
            foreach (var val in testArr)
                testArrObj.Add(val);

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>();
            crit2.Restriction<ReceiptWaybillRow>(x=>x.Rows).NotOneOf(x => x.PendingSum, testArr);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            var r = expected.CreateCriteria("Rows");
            r.Add(Expression.Not(Expression.In("PendingSum", testArrObj.ToArray<object>())));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_NotOneOf_With_Field_Name_In_Criterion()
        {
            List<decimal> testArr = new List<decimal>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<object> testArrObj = new List<object>();
            foreach (var val in testArr)
                testArrObj.Add(val);

            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>();
            crit2.Restriction<ReceiptWaybillRow>(x => x.Rows).NotOneOf("PendingSum", testArr);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            var r = expected.CreateCriteria("Rows");
            r.Add(Expression.Not(Expression.In("PendingSum", testArrObj.ToArray<object>())));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_OrderByAsc()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().OrderByAsc(x => x.PendingSum);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.AddOrder(Order.Asc("PendingSum"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_OrderByAsc_With_Field_Name()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().OrderByAsc("PendingSum");
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.AddOrder(Order.Asc("PendingSum"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_OrderByDesc()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().OrderByDesc(x => x.PendingSum);
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.AddOrder(Order.Desc("PendingSum"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_OrderByDesc_With_Field_Name()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().OrderByDesc("PendingSum");
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.AddOrder(Order.Desc("PendingSum"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Like()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Like(x => x.Number, "123");
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Like("Number", "123", MatchMode.Anywhere));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Like_With_Field_Name()
        {
            //Генерируем актуальный критерий
            var crit2 = repository.Object.Query<ReceiptWaybill>().Like("Number", "123");
            var actual = (crit2 as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Expression.Like("Number", "123", MatchMode.Anywhere));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Restriction()
        {
            //Генерируем актуальный критерий
            var crit = repository.Object.Query<ReceiptWaybill>();
            crit.Restriction<ReceiptWaybillRow>(x=>x.Rows).Where(x => x.Article.Id == 42);
            var actual = (crit as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.CreateCriteria("Rows").Add(Expression.Eq("Article.Id", 42));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Refer_To_Parent()
        {
            //Генерируем актуальный критерий
            var crit = repository.Object.Query<ReceiptWaybillRow>();
            crit.Restriction<ReceiptWaybill>(x => x.ReceiptWaybill).Where(x => x.PendingSum > 42);
            var actual = (crit as Criteria<ReceiptWaybillRow>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybillRow>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.CreateCriteria("ReceiptWaybill").Add(Expression.Gt("PendingSum", (decimal)42));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_Select()
        {
            //Генерируем актуальный критерий
            var crit = repository.Object.Query<ReceiptWaybill>();
            crit.Select(x => x.Id, x => x.Number);
            var actual = (crit as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.SetProjection(Property.ForName("Id"), Property.ForName("Number"));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_PropertyIn()
        {
            //Генерируем актуальный критерий
            var subQuery = repository.Object.SubQuery<Article>()
                .Like(x => x.Number, "123")
                .Select(x => x.Id);
            var crit2 = repository.Object.Query<ReceiptWaybillRow>().PropertyIn(x => x.Article.Id, subQuery);
            var actual = (crit2 as Criteria<ReceiptWaybillRow>).GetCriteria();

            //Генерируем ожидаемый критерий
            var subExpected = DetachedCriteria.For<Article>()
                .Add(Expression.Like("Number", "123", MatchMode.Anywhere))
                .SetProjection(Property.ForName("Id"));

            var expected = session.CreateCriteria<ReceiptWaybillRow>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Subqueries.PropertyIn("Article.Id", subExpected));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_PropertyNotIn()
        {
            //Генерируем актуальный критерий
            var subQuery = repository.Object.SubQuery<Article>()
                .Like(x => x.Number, "123")
                .Select(x => x.Id);
            var crit2 = repository.Object.Query<ReceiptWaybillRow>().PropertyNotIn(x => x.Article.Id, subQuery);
            var actual = (crit2 as Criteria<ReceiptWaybillRow>).GetCriteria();

            //Генерируем ожидаемый критерий
            var subExpected = DetachedCriteria.For<Article>()
                .Add(Expression.Like("Number", "123", MatchMode.Anywhere))
                .SetProjection(Property.ForName("Id"));

            var expected = session.CreateCriteria<ReceiptWaybillRow>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.Add(Subqueries.PropertyNotIn("Article.Id", subExpected));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_SetMaxResult()
        {
            int maxR = 42;

            //Генерируем актуальный критерий
            var crit = repository.Object.Query<ReceiptWaybill>();
            crit.SetMaxResults(maxR);
            var actual = (crit as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.SetMaxResults(maxR);

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_SetFirstResult()
        {
            int firstR = 42;

            //Генерируем актуальный критерий
            var crit = repository.Object.Query<ReceiptWaybill>();
            crit.SetFirstResult(firstR);
            var actual = (crit as Criteria<ReceiptWaybill>).GetCriteria();

            //Генерируем ожидаемый критерий
            var expected = session.CreateCriteria<ReceiptWaybill>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.SetFirstResult(firstR);

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void Criteria_ComplexTest()
        {
            //Генерируем актуальный критерий
            var subQuery = repository.Object.SubQuery<Article>()
                .Like(x => x.Number, "123")
                .Select(x => x.Id);
            var crit = repository.Object.Query<ReceiptWaybillRow>();
            crit.Restriction<ReceiptWaybill>(x => x.ReceiptWaybill).PropertyIn(x => x.ReceiptStorage.Id, subQuery);
            var actual = (crit as Criteria<ReceiptWaybillRow>).GetCriteria();

            //Генерируем ожидаемый критерий
            var dc = DetachedCriteria.For<Article>();
            dc.Add(Expression.Like("Number", "123", MatchMode.Anywhere)).SetProjection(Property.ForName("Id"));

            var expected = session.CreateCriteria<ReceiptWaybillRow>();
            expected.Add(Expression.IsNull("DeletionDate"));
            expected.CreateCriteria("ReceiptWaybill").Add(Subqueries.PropertyIn("ReceiptStorage.Id", dc));

            var sq = repository.Object.SubQuery<Article>();

            crit.Or(
                y => y.PropertyIn(x => x.ReceiptWaybill, sq),
                y => y.PropertyIn(x => x.ReceiptWaybill, sq));

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }
    }
}
