using System;
using System.Linq;
using ERP.Wholesale.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class StorageTest
    {
        [TestMethod]
        public void Storage_Must_Have_Name_And_TypeName()
        {            
            var storage = new Storage("Тестовое хранилище", StorageType.DistributionCenter);

            Assert.AreEqual(storage.Type, StorageType.DistributionCenter);
            Assert.AreEqual(storage.Name, "Тестовое хранилище");
        }
                
        [TestMethod]
        public void Storage_DeletionDate_Test()
        {            
            var storage = new Storage("Тестовое хранилище", StorageType.DistributionCenter);
            var storageSection = new StorageSection("Тестовая секция");                        

            Assert.IsNull(storage.DeletionDate);

            DateTime now = DateTime.Now;

            storage.AddSection(storageSection);
            storage.DeletionDate = now;

            Assert.AreEqual(storage.DeletionDate, now);

            DateTime new_now = new DateTime(2011, 1, 1);

            storage.DeletionDate = new_now;

            // не изменилось
            Assert.AreEqual(storage.DeletionDate, now);
            Assert.AreEqual(storageSection.DeletionDate, now);
        }

        [TestMethod]
        public void AddSection_RemoveSection_Test()
        {
            var storage = new Storage("Тестовое хранилище", StorageType.DistributionCenter);
            var storageSection = new StorageSection("Тестовая секция");
            
            storage.AddSection(storageSection);

            Assert.AreEqual(storage.Sections.Count(), 1);
            Assert.AreEqual(storageSection.Storage.Name, "Тестовое хранилище");
            Assert.AreEqual(storage.Sections.First().Name, "Тестовая секция");

            storage.RemoveSection(storageSection);

            Assert.AreEqual(storage.Sections.Count(), 0);
            Assert.AreNotEqual(storageSection.DeletionDate, null);
        }

        #region Проверка перегруженного равенства, неравенства, Equals и GetHashCode

        #region Проверка перегруженного оператора равенства

        [TestMethod]
        public void Storage_NullsMustBeEqual()
        {
            Storage sNull1 = null;
            Storage sNull2 = null;
            bool areEqual = (sNull1 == sNull2);

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void Storage_NullMustBeNonEqualToNotNull()
        {
            var storage1 = new Storage("Склад 1", StorageType.DistributionCenter);
            Storage storageNull = null;
            bool areEqual = (storageNull == storage1);

            Assert.IsFalse(areEqual);
        }

        [TestMethod]
        public void Storage_NotNullMustBeNonEqualToNull()
        {
            var storage1 = new Storage("Склад 1", StorageType.DistributionCenter);
            Storage storageNull = null;
            bool areEqual = (storage1 == storageNull);

            Assert.IsFalse(areEqual);
        }

        [TestMethod]
        public void Storage_DifferentIdMustBeNonEqual()
        {
            var storage1 = new Storage("Склад 1", StorageType.DistributionCenter) { Id = 1 };
            var storage2 = new Storage("Склад 1", StorageType.DistributionCenter) { Id = 2 };
            bool areEqual = (storage1 == storage2);

            Assert.IsFalse(areEqual);
        }

        [TestMethod]
        public void Storage_SameIdMustBeEqual()
        {
            var storage1 = new Storage("Склад совсем 1", StorageType.ExtraStorage) { Id = 1 };
            var storage2 = new Storage("Склад совсем другой", StorageType.DistributionCenter) { Id = 1 };
            bool areEqual = (storage1 == storage2);

            Assert.IsTrue(areEqual);
        }

        #endregion

        #region Проверка перегруженного оператора неравенства

        [TestMethod]
        public void Storage_NullsMustNotBeNotEqual()
        {
            Storage sNull1 = null;
            Storage sNull2 = null;
            bool areEqual = (sNull1 != sNull2);

            Assert.IsFalse(areEqual);
        }

        [TestMethod]
        public void Storage_NullMustNotBeEqualToNotNull()
        {
            var storage1 = new Storage("Склад 1", StorageType.DistributionCenter);
            Storage storageNull = null;
            bool areEqual = (storageNull != storage1);

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void Storage_NotNullMustNotBeEqualToNull()
        {
            var storage1 = new Storage("Склад 1", StorageType.DistributionCenter);
            Storage storageNull = null;
            bool areEqual = (storage1 != storageNull);

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void Storage_DifferentIdMustNotBeEqual()
        {
            var storage1 = new Storage("Склад 1", StorageType.DistributionCenter) { Id = 1 };
            var storage2 = new Storage("Склад 1", StorageType.DistributionCenter) { Id = 2 };
            bool areEqual = (storage1 != storage2);

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void Storage_SameIdMustNotBeNotEqual()
        {
            var storage1 = new Storage("Склад совсем 1", StorageType.ExtraStorage) { Id = 1 };
            var storage2 = new Storage("Склад совсем другой", StorageType.DistributionCenter) { Id = 1 };
            bool areEqual = (storage1 != storage2);

            Assert.IsFalse(areEqual);
        }

        #endregion

        #region Проверка перегруженного метода Equals

        [TestMethod]
        public void Storage_MustNotBeEqualToNull()
        {
            var storage1 = new Storage("Склад 1", StorageType.DistributionCenter);
            Storage storageNull = null;
            
            Assert.IsFalse(storage1.Equals(storageNull));
            Assert.IsFalse(storage1.Equals(null));
        }

        [TestMethod]
        public void Storage_MustNotBeEqualToNonStorage()
        {
            var storage1 = new Storage("Склад 1", StorageType.DistributionCenter);
            DateTime nonStorage = new DateTime(2007, 12, 5);

            Assert.IsFalse(storage1.Equals(nonStorage));
        }

        [TestMethod]
        public void Storage_DifferentIdMustHaveEqualsFalse()
        {
            var storage1 = new Storage("Склад совсем 1", StorageType.ExtraStorage) { Id = 1 };
            var storage2 = new Storage("Склад совсем 1", StorageType.ExtraStorage) { Id = 2 };

            Assert.IsFalse(storage1.Equals(storage2));
            Assert.IsFalse(storage2.Equals(storage1));
        }

        [TestMethod]
        public void Storage_SameIdMustHaveEqualsTrue()
        {
            var storage1 = new Storage("Склад совсем 1", StorageType.ExtraStorage) { Id = 1 };
            var storage2 = new Storage("Склад совсем другой", StorageType.DistributionCenter) { Id = 1 };

            Assert.IsTrue(storage1.Equals(storage2));
            Assert.IsTrue(storage2.Equals(storage1));
        }

        #endregion

        #region Проверка перегруженного метода GetHashCode

        [TestMethod]
        public void Storage_SameIds_MustHaveEqual_HashCodes()
        {
            var storage1 = new Storage("Склад совсем 1", StorageType.ExtraStorage) { Id = 1 };
            var storage2 = new Storage("Склад совсем другой", StorageType.DistributionCenter) { Id = 1 };

            Assert.IsTrue(storage1.GetHashCode() == storage2.GetHashCode());
        }

        [TestMethod]
        public void Storage_DifferentIds_MustHaveDifferent_HashCodes()
        {
            var storage1 = new Storage("Склад совсем 1", StorageType.ExtraStorage) { Id = 1 };
            var storage2 = new Storage("Склад совсем 1", StorageType.ExtraStorage) { Id = 2 };

            Assert.IsFalse(storage1.GetHashCode() == storage2.GetHashCode());
        }

        [TestMethod]
        public void Storage_ZeroIds_DifferentValues_ShouldHaveDifferent_HashCodes()
        {
            var storage1 = new Storage("Склад совсем 1", StorageType.ExtraStorage) { Id = 0 };
            var storage2 = new Storage("Склад совсем другой", StorageType.DistributionCenter) { Id = 0 };

            Assert.IsFalse(storage1.GetHashCode() == storage2.GetHashCode());
        }

        [TestMethod]
        public void Storage_2CallsOfGetHashCode_MustReturnSameValues()
        {
            var storage1 = new Storage("Склад 1", StorageType.DistributionCenter);

            var hashCode1 = storage1.GetHashCode();
            var hashCode2 = storage1.GetHashCode();

            Assert.IsTrue(hashCode1 == hashCode2);
        }

        #endregion

        #endregion
    }
}
