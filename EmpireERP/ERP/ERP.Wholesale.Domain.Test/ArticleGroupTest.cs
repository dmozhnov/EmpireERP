using System;
using System.Linq;
using ERP.Wholesale.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class ArticleGroupTest
    {
        /// <summary>
        /// Все параметры конструктора должны быть выставлены
        /// </summary>
        [TestMethod]
        public void ArticleGroup_Initial_Parameters_By_Constractor_Must_Be_Set()
        {
            var articleGroup = new ArticleGroup("Тестовая группа", "Тестовая бухгалтерская группа");

            Assert.IsNotNull(articleGroup.Childs);
            Assert.AreEqual(0, articleGroup.Childs.Count());
            Assert.AreEqual(String.Empty, articleGroup.Comment);
            Assert.AreEqual(0.0M, articleGroup.MarkupPercent);
            Assert.AreEqual(0.0M, articleGroup.SalaryPercent);
            Assert.AreEqual("Тестовая группа", articleGroup.Name);
            Assert.AreEqual("Тестовая бухгалтерская группа", articleGroup.NameFor1C);
            Assert.AreEqual(0, articleGroup.Id);
            Assert.IsNull(articleGroup.Parent);
        }

        /// <summary>
        /// Здание пустого названия группы в конструкторе должно привести к ошибке
        /// </summary>
        [TestMethod]
        public void ArticleGroup_Empty_Name_In_Constractor_Must_Be_Fail()
        {
            try
            {
                var articleGroup = new ArticleGroup("", "Тестовая бухгалтерская группа");

                Assert.Fail("Исключение не сгенерировано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Укажите название группы товаров.", ex.Message);
            }
        }

        /// <summary>
        /// Здание названия группы в конструкторе как null должно привести к ошибке
        /// </summary>
        [TestMethod]
        public void ArticleGroup_Null_Name_In_Constractor_Must_Be_Fail()
        {
            try
            {
                var articleGroup = new ArticleGroup(null, "Тестовая бухгалтерская группа");

                Assert.Fail("Исключение не сгенерировано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Укажите название группы товаров.", ex.Message);
            }
        }

        /// <summary>
        /// Здание пустого бухгалтерского названия группы в конструкторе должно привести к ошибке
        /// </summary>
        [TestMethod]
        public void ArticleGroup_Empty_NameFor1C_In_Constractor_Must_Be_Fail()
        {
            try
            {
                var articleGroup = new ArticleGroup("Тестовая группа", "");

                Assert.Fail("Исключение не сгенерировано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Укажите бухгалтерское название группы товаров.", ex.Message);
            }
        }

        /// <summary>
        /// Здание бухгалтерского названия группы в конструкторе как null должно привести к ошибке
        /// </summary>
        [TestMethod]
        public void ArticleGroup_Null_NameFor1C_In_Constractor_Must_Be_Fail()
        {
            try
            {
                var articleGroup = new ArticleGroup("Тестовая группа", null);

                Assert.Fail("Исключение не сгенерировано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Укажите бухгалтерское название группы товаров.", ex.Message);
            }
        }

        [TestMethod]
        public void ArticleGroup_Initial_Parameters_Must_Be_Set()
        {
            var articleGroup = new ArticleGroup("Тестовая группа", "Тестовая группа") { Id = 1, Comment = "Описание", MarkupPercent = 12.2M, SalaryPercent = 15.5M };

            Assert.IsNotNull(articleGroup.Childs);
            Assert.AreEqual("Описание", articleGroup.Comment);
            Assert.AreEqual(12.2M, articleGroup.MarkupPercent);
            Assert.AreEqual(15.5M, articleGroup.SalaryPercent);
            Assert.AreEqual("Тестовая группа", articleGroup.Name);
            Assert.AreEqual(1, articleGroup.Id);
            Assert.IsNull(articleGroup.Parent);
        }

        [TestMethod]
        public void ArticleGroup_Add_ChildGroup_Must_Be_Ok()
        {
            var parentGroup = new ArticleGroup("Родительская группа", "Родительская группа") { Id = 1 };
            var articleGroup = new ArticleGroup("Тестовая группа", "Тестовая группа") { Id = 2 };

            parentGroup.AddChildGroup(articleGroup);

            Assert.IsNotNull(articleGroup.Parent);
            Assert.AreEqual(1, articleGroup.Parent.Id);
            Assert.AreEqual(1, parentGroup.Childs.Count());
        }

        [TestMethod]
        public void ArticleGroup_Delete_ChildGroup_Must_Be_Ok()
        {
            var parentGroup = new ArticleGroup("Родительская группа", "Родительская группа") { Id = 1 };
            var articleGroup = new ArticleGroup("Тестовая группа", "Тестовая группа") { Id = 2 };

            parentGroup.AddChildGroup(articleGroup);
            parentGroup.RemoveChildGroup(articleGroup);

            Assert.IsNull(articleGroup.Parent);
            Assert.AreEqual(0, parentGroup.Childs.Count());
        }
    }
}
