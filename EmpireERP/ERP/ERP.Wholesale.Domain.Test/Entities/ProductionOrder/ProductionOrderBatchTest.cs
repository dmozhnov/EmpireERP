using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ERP.Wholesale.Domain.Entities;
using Moq;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Test.Entities.ProductionOrder
{
    [TestClass]
    public class ProductionOrderBatchTest
    {
        private ProductionOrderBatch_Accessor batch;
        private User user;

        [TestInitialize]
        public void Init()
        {
            user = new User(new Employee("Иван", "Иванов", "Иванович", new EmployeePost("Менеджер"), null), "Иванов Иван", "ivanov", "pa$$w0rd", new Team("Тестовая команда", null), null);
            var currentDateTime = DateTimeUtils.GetCurrentDateTime();

            batch = new ProductionOrderBatch_Accessor(
                new ProductionOrderBatchStage("ddd", ProductionOrderBatchStageType.Design, 11, true),
                new ProductionOrderBatchStage("1", ProductionOrderBatchStageType.Design, 11, true),
                new ProductionOrderBatchStage("2", ProductionOrderBatchStageType.Design, 11, true),
                user,
                currentDateTime);

             batch.ProductionOrder =  new Mock<ERP.Wholesale.Domain.Entities.ProductionOrder>().Object;
            
        }

        /// <summary>
        /// Проверка отмена утверждения партии при созданнии накладной
        /// </summary>
        [TestMethod]
        public void ProductionOrderBatch_CheckPossibilityToCancelApprovement_Throw_Exception()
        {
            var waybill = new Mock<ReceiptWaybill>();
            batch = new ProductionOrderBatch_Accessor();
            batch.ReceiptWaybill = waybill.Object;
            batch.State = ProductionOrderBatchState.Approved;
            try
            {
                batch.CheckPossibilityToCancelApprovement();
                Assert.Fail("Исключение не выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно отменить утверждение партии заказа, по которой создана приходная накладная.",
                    ex.Message);
            }

        }

        /// <summary>
        /// Проверка возможности изменения стадии при отсутсвии статуса "Готово"
        /// </summary>
        [TestMethod]
        public void ProductionOrderBatch_CheckPossibilityToMoveToNextStage_Must_Not_Throw_Exception()
        {
            batch.State = ProductionOrderBatchState.Approvement;
            batch.CheckPossibilityToMoveToNextStage();
        }

        /// <summary>
        /// Проверка возможности создания накладной при отсутсвии статуса "Готово"
        /// </summary>
        [TestMethod]
        public void ProductionOrderBatch_CheckPossibilityToHaveReceiptWaybill_Must_Throw_Exception()
        {
            batch.State = ProductionOrderBatchState.Approvement;
            try
            {
                batch.CheckPossibilityToHaveReceiptWaybill();
                Assert.Fail("Исключение не выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно создать приходную накладную по партии заказа, так как партия не переведена в статус «Готово».",
                    ex.Message);
            }
        }

        /// <summary>
        /// Проверка возможности добавление позиции товара в проведенную партию 
        /// </summary>
        [TestMethod]
        public void ProductionOrderBatch_AddRow_Must_Throw_Exception()
        {
            batch.State = ProductionOrderBatchState.Approvement;
            try
            {
                var row = new Mock<ProductionOrderBatchRow>();
                batch.AddRow(row.Object);

                Assert.Fail("Исключение не выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно добавить позицию в партию со статусом «Утверждение».",
                    ex.Message);
            }
        }

        /// <summary>
        /// Проверка проверки возможности добавление позиции товара в проведенной партии 
        /// </summary>
        [TestMethod]
        public void ProductionOrderBatch_CheckPossibilityToCreateRow_Must_Throw_Exception()
        {
            batch.State = ProductionOrderBatchState.Approvement;
            try
            {
                batch.CheckPossibilityToCreateRow();

                Assert.Fail("Исключение не выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно добавить позицию в партию со статусом «Утверждение».",
                    ex.Message);
            }
        }

        /// <summary>
        /// Проверка проверки возможности редактирования позиции товара в проведенной партии 
        /// </summary>
        [TestMethod]
        public void ProductionOrderBatch_CheckPossibilityToEditRow_Must_Throw_Exception()
        {
            batch.State = ProductionOrderBatchState.Approvement;
            try
            {
                batch.CheckPossibilityToEditRow();

                Assert.Fail("Исключение не выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно редактировать партию со статусом «Утверждение».",
                    ex.Message);
            }
        }

        /// <summary>
        /// Проверка проверки возможности удаления позиции товара в проведенной партии 
        /// </summary>
        [TestMethod]
        public void ProductionOrderBatch_CheckPossibilityToDeleteRow_Must_Throw_Exception()
        {
            batch.State = ProductionOrderBatchState.Approvement;
            try
            {
                batch.CheckPossibilityToDeleteRow();

                Assert.Fail("Исключение не выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно удалить партию со статусом «Утверждение».",
                    ex.Message);
            }
        }

        /// <summary>
        /// Проверка возможности удаления позиции товара в проведенной партии 
        /// </summary>
        [TestMethod]
        public void ProductionOrderBatch_DeleteRow_Must_Throw_Exception()
        {
            try
            {
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();
                var row = new Mock<ProductionOrderBatchRow>();

                batch.State = ProductionOrderBatchState.Approvement;
                batch.DeleteRow(row.Object, currentDateTime);

                Assert.Fail("Исключение не выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно удалить позицию в партии со статусом «Утверждение».",
                    ex.Message);
            }
        }
    }
}
