using System;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class TaskTest
    {
        #region Поля

        private string topic, topic2;
        private TaskExecutionState state1, state2, state3, state4;
        private DateTime currentDate;
        private byte completionPercentage, completionPercentage2;
        private string description;

        private Mock<Deal> deal;
        private Mock<Contractor> contractor;
        private Mock<ProductionOrder> productionOrder;
        private Guid productionOrderId = Guid.NewGuid();
        private Mock<User> user;

        private TaskType taskType, taskType2;
        private TaskPriority taskPriority, taskPriority2;
        
        #endregion

        #region Инициализация

        [TestInitialize]
        public void Init()
        {

            topic = "123";
            topic2 = "456";

            description = "qwerty";

            completionPercentage = 0;
            completionPercentage2 = 42;

            deal = new Mock<Deal>();
            deal.Setup(x => x.Id).Returns(711);

            contractor = new Mock<Contractor>();
            contractor.Setup(x => x.Id).Returns(71);
            
            productionOrder = new Mock<ProductionOrder>();
            productionOrder.Setup(x => x.Id).Returns(productionOrderId);

            taskType = new TaskType("Type_1");
            taskType2 = new TaskType("Type_2");
            var taskExecutionState = new Mock<TaskExecutionState>();
            taskExecutionState.Setup(x => x.Id).Returns(1);
            taskExecutionState.Setup(x => x.ExecutionStateType).Returns(TaskExecutionStateType.New);
            taskExecutionState.Setup(x => x.Equals(It.Is<TaskExecutionState>(y => y.Id == 1))).Returns(true);

            var taskExecutionState2 = new Mock<TaskExecutionState>();
            taskExecutionState2.Setup(x => x.Id).Returns(2);
            taskExecutionState2.Setup(x => x.ExecutionStateType).Returns(TaskExecutionStateType.Executing);
            taskExecutionState2.Setup(x => x.Equals(It.Is<TaskExecutionState>(y => y.Id == 2))).Returns(true);
            var taskExecutionState3 = new Mock<TaskExecutionState>();
            taskExecutionState3.Setup(x => x.Id).Returns(3);
            taskExecutionState3.Setup(x => x.ExecutionStateType).Returns(TaskExecutionStateType.Completed);
            taskExecutionState3.Setup(x => x.Equals(It.Is<TaskExecutionState>(y => y.Id == 3))).Returns(true);

            var taskExecutionState4 = new Mock<TaskExecutionState>();
            taskExecutionState4.Setup(x => x.Id).Returns(4);
            taskExecutionState4.Setup(x => x.ExecutionStateType).Returns(TaskExecutionStateType.New);
            taskExecutionState4.Setup(x => x.Equals(It.Is<TaskExecutionState>(y => y.Id == 4))).Returns(true);
            taskExecutionState4.Setup(x => x.Name).Returns("State_4");

            state1 = taskExecutionState.Object;
            state2 = taskExecutionState2.Object;
            state3 = taskExecutionState3.Object;
            state4 = taskExecutionState4.Object;

            taskType.AddState(state1);
            taskType.AddState(state2);
            taskType.AddState(state3);
            taskType2.AddState(state4);

            taskPriority = new TaskPriority("Priority_1", 1);
            taskPriority2 = new TaskPriority("Priority_2", 2);

            currentDate = DateTime.Now;

            user = new Mock<User>();
            user.Setup(x => x.Id).Returns(1);
        }

        #endregion
        
        #region Тесты
        
        /// <summary>
        /// Парметры, передаваемые через конструктор должны быть сохранены
        /// </summary>
        [TestMethod]
        public void TaskTest_Contructor_Params_Must_Be_Set()
        {
            var task = new Task(topic, taskType, taskPriority, state1, currentDate, user.Object);

            Assert.AreEqual(task.Topic, topic);
            Assert.AreEqual(task.Type, taskType);
            Assert.AreEqual(task.Priority, taskPriority);
            Assert.AreEqual(task.ExecutionState, state1);
            Assert.AreEqual(task.CompletionPercentage, completionPercentage);
            Assert.AreEqual(task.CreationDate, currentDate);
            Assert.AreEqual(task.CreatedBy, user.Object);
        }

        /// <summary>
        /// Конструктор должен сгенерировать исключение, т.к. не указана тема
        /// </summary>
        [TestMethod]
        public void TaskTest_Contructor_Must_Throw_Exception_On_Topic()
        {
            try
            {
                var task = new Task("", taskType, taskPriority, state1, currentDate, user.Object);
                Assert.Fail("Исключение не сгенерировано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Необходимо указать тему задачи.", ex.Message);
            }
        }

        /// <summary>
        /// Конструктор должен сгенерировать исключение, т.к. вместо темы передается null
        /// </summary>
        [TestMethod]
        public void TaskTest_Contructor_Must_Throw_Exception_On_Topic2()
        {
            try
            {
                var task = new Task(null, taskType, taskPriority, state1, currentDate, user.Object);
                Assert.Fail("Исключение не сгенерировано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Необходимо указать тему задачи.", ex.Message);
            }
        }

        /// <summary>
        /// Конструктор должен сгенерировать исключение, т.к. вместо типа задачи передается null
        /// </summary>
        [TestMethod]
        public void TaskTest_Contructor_Must_Throw_Exception_On_TaskType()
        {
            try
            {
                var task = new Task(topic, null, taskPriority, state1, currentDate, user.Object);
                Assert.Fail("Исключение не сгенерировано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Необходимо указать тип задачи.", ex.Message);
            }
        }

        /// <summary>
        /// Конструктор должен сгенерировать исключение, т.к. вместо приоритета задачи передается null
        /// </summary>
        [TestMethod]
        public void TaskTest_Contructor_Must_Throw_Exception_On_TaskPriority()
        {
            try
            {
                var task = new Task(topic, taskType, null, state1, currentDate, user.Object);
                Assert.Fail("Исключение не сгенерировано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Необходимо указать приоритет задачи.", ex.Message);
            }
        }

        /// <summary>
        /// Конструктор должен сгенерировать исключение, т.к. вместо статуса задачи передается null
        /// </summary>
        [TestMethod]
        public void TaskTest_Contructor_Must_Throw_Exception_On_TaskExecutuonState()
        {
            try
            {
                var task = new Task(topic, taskType, taskPriority, null, currentDate, user.Object);
                Assert.Fail("Исключение не сгенерировано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Необходимо указать статус задачи.", ex.Message);
            }
        }

        /// <summary>
        /// Конструктор должен сгенерировать исключение, т.к. статус не соответствует типу задачи
        /// </summary>
        [TestMethod]
        public void TaskTest_Contructor_Must_Throw_Exception_On_TaskExecutuonState_Not_Agree_With_Type()
        {
            try
            {
                var task = new Task(topic, taskType, taskPriority, state4, currentDate, user.Object);
                Assert.Fail("Исключение не сгенерировано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Статус исполнения задачи «State_4» не допустим для типа задачи «Type_1».", ex.Message);
            }
        }

        /// <summary>
        /// Конструктор должен сгенерировать исключение, т.к. пользователь не указан
        /// </summary>
        [TestMethod]
        public void TaskTest_Contructor_Must_Throw_Exception_On_User()
        {
            try
            {
                var task = new Task(topic, taskType, taskPriority, state1, currentDate, null);
                Assert.Fail("Исключение не сгенерировано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Необходимо указать пользователя, создавшего задачу.", ex.Message);
            }
        }

        /// <summary>
        /// Начальные значения задачи должны сохраниться в истории
        /// </summary>
        [TestMethod]
        public void TaskTest_Init_Values_Must_Be_Save_In_History()
        {
            var task = new Task(topic, taskType, taskPriority, state1, currentDate, user.Object);
            task.SaveHistory(currentDate, user.Object);

            Assert.AreEqual(1, task.History.Count());
            var item = task.History.ElementAt(0);
            Assert.AreEqual(true, item.IsContractorChanged);
            Assert.AreEqual(true, item.IsDeadLineChanged);
            Assert.AreEqual(true, item.IsDealChanged);
            Assert.AreEqual(true, item.IsDeletionDateChanged);
            Assert.AreEqual(true, item.IsDescriptionChanged);
            Assert.AreEqual(true, item.IsExecutedByChanged);
            Assert.AreEqual(true, item.IsTaskExecutionStateChanged);
            Assert.AreEqual(true, item.IsFactualCompletionDateChanged);
            Assert.AreEqual(true, item.IsFactualSpentTimeChanged);
            Assert.AreEqual(true, item.IsTaskPriorityChanged);
            Assert.AreEqual(true, item.IsProductionOrderChanged);
            Assert.AreEqual(true, item.IsStartDateChanged);
            Assert.AreEqual(true, item.IsTopicChanged);
            Assert.AreEqual(true, item.IsTaskTypeChanged);

            Assert.AreEqual(null, item.Contractor);
            Assert.AreEqual(null, item.Deadline);
            Assert.AreEqual(null, item.Deal);
            Assert.AreEqual(null, item.DeletionDate);
            Assert.AreEqual("", item.Description);
            Assert.AreEqual(null, item.ExecutedBy);
            Assert.AreEqual(state1.Id, item.TaskExecutionState.Id);
            Assert.AreEqual(null, item.FactualCompletionDate);
            Assert.AreEqual(0, item.FactualSpentTime);
            Assert.AreEqual(taskPriority.Id, item.TaskPriority.Id);
            Assert.AreEqual(null, item.ProductionOrder);
            Assert.AreEqual(null, item.StartDate);
            Assert.AreEqual(topic, item.Topic);
            Assert.AreEqual(taskType.Id, item.TaskType.Id);
        }

        /// <summary>
        /// Контрагент должен быть сохранен в истории
        /// </summary>
        [TestMethod]
        public void TaskTest_Contractor_Must_Be_In_History()
        {
            var task = new Task(topic, taskType, taskPriority, state1, currentDate, user.Object);
            task.SaveHistory(currentDate, user.Object);

            task.Contractor = contractor.Object;
            task.SaveHistory(currentDate, user.Object);

            Assert.AreEqual(2, task.History.Count());
            var item = task.History.ElementAt(1);
            Assert.AreEqual(true, item.IsContractorChanged);
            Assert.AreEqual(false, item.IsDeadLineChanged);
            Assert.AreEqual(false, item.IsDealChanged);
            Assert.AreEqual(false, item.IsDeletionDateChanged);
            Assert.AreEqual(false, item.IsDescriptionChanged);
            Assert.AreEqual(false, item.IsExecutedByChanged);
            Assert.AreEqual(false, item.IsTaskExecutionStateChanged);
            Assert.AreEqual(false, item.IsFactualCompletionDateChanged);
            Assert.AreEqual(false, item.IsFactualSpentTimeChanged);
            Assert.AreEqual(false, item.IsTaskPriorityChanged);
            Assert.AreEqual(false, item.IsProductionOrderChanged);
            Assert.AreEqual(false, item.IsStartDateChanged);
            Assert.AreEqual(false, item.IsTopicChanged);
            Assert.AreEqual(false, item.IsTaskTypeChanged);

            Assert.AreEqual(contractor.Object.Id, item.Contractor.Id);
        }

        /// <summary>
        /// Требуемая дата завершения должена быть сохранена в истории
        /// </summary>
        [TestMethod]
        public void TaskTest_DeadLine_Must_Be_In_History()
        {
            var task = new Task(topic, taskType, taskPriority, state1, currentDate, user.Object);
            task.SaveHistory(currentDate, user.Object);

            task.DeadLine = currentDate.AddDays(10);
            task.SaveHistory(currentDate, user.Object);

            Assert.AreEqual(2, task.History.Count());
            var item = task.History.ElementAt(1);
            Assert.AreEqual(false, item.IsContractorChanged);
            Assert.AreEqual(true, item .IsDeadLineChanged);
            Assert.AreEqual(false, item.IsDealChanged);
            Assert.AreEqual(false, item.IsDeletionDateChanged);
            Assert.AreEqual(false, item.IsDescriptionChanged);
            Assert.AreEqual(false, item.IsExecutedByChanged);
            Assert.AreEqual(false, item.IsTaskExecutionStateChanged);
            Assert.AreEqual(false, item.IsFactualCompletionDateChanged);
            Assert.AreEqual(false, item.IsFactualSpentTimeChanged);
            Assert.AreEqual(false, item.IsTaskPriorityChanged);
            Assert.AreEqual(false, item.IsProductionOrderChanged);
            Assert.AreEqual(false, item.IsStartDateChanged);
            Assert.AreEqual(false, item.IsTopicChanged);
            Assert.AreEqual(false, item.IsTaskTypeChanged);

            Assert.AreEqual(currentDate.AddDays(10), item.Deadline);
        }

        /// <summary>
        /// Сделка должена быть сохранена в истории
        /// </summary>
        [TestMethod]
        public void TaskTest_Deal_Must_Be_In_History()
        {
            var task = new Task(topic, taskType, taskPriority, state1, currentDate, user.Object);
            task.SaveHistory(currentDate, user.Object);

            task.Deal = deal.Object;
            task.SaveHistory(currentDate, user.Object);

            Assert.AreEqual(2, task.History.Count());
            var item = task.History.ElementAt(1);
            Assert.AreEqual(false, item.IsContractorChanged);
            Assert.AreEqual(false, item.IsDeadLineChanged);
            Assert.AreEqual(true, item. IsDealChanged);
            Assert.AreEqual(false, item.IsDeletionDateChanged);
            Assert.AreEqual(false, item.IsDescriptionChanged);
            Assert.AreEqual(false, item.IsExecutedByChanged);
            Assert.AreEqual(false, item.IsTaskExecutionStateChanged);
            Assert.AreEqual(false, item.IsFactualCompletionDateChanged);
            Assert.AreEqual(false, item.IsFactualSpentTimeChanged);
            Assert.AreEqual(false, item.IsTaskPriorityChanged);
            Assert.AreEqual(false, item.IsProductionOrderChanged);
            Assert.AreEqual(false, item.IsStartDateChanged);
            Assert.AreEqual(false, item.IsTopicChanged);
            Assert.AreEqual(false, item.IsTaskTypeChanged);

            Assert.AreEqual(deal.Object.Id, item.Deal.Id);
        }

        /// <summary>
        /// Дата удаления должена быть сохранена в истории
        /// </summary>
        [TestMethod]
        public void TaskTest_DelationDate_Must_Be_In_History()
        {
            var task = new Task(topic, taskType, taskPriority, state1, currentDate, user.Object);
            task.SaveHistory(currentDate, user.Object);

            task.DeletionDate = currentDate;
            task.SaveHistory(currentDate, user.Object);

            Assert.AreEqual(2, task.History.Count());
            var item = task.History.ElementAt(1);
            Assert.AreEqual(false, item.IsContractorChanged);
            Assert.AreEqual(false, item.IsDeadLineChanged);
            Assert.AreEqual(false, item.IsDealChanged);
            Assert.AreEqual(true, item. IsDeletionDateChanged);
            Assert.AreEqual(false, item.IsDescriptionChanged);
            Assert.AreEqual(false, item.IsExecutedByChanged);
            Assert.AreEqual(false, item.IsTaskExecutionStateChanged);
            Assert.AreEqual(false, item.IsFactualCompletionDateChanged);
            Assert.AreEqual(false, item.IsFactualSpentTimeChanged);
            Assert.AreEqual(false, item.IsTaskPriorityChanged);
            Assert.AreEqual(false, item.IsProductionOrderChanged);
            Assert.AreEqual(false, item.IsStartDateChanged);
            Assert.AreEqual(false, item.IsTopicChanged);
            Assert.AreEqual(false, item.IsTaskTypeChanged);

            Assert.AreEqual(currentDate, item.DeletionDate);
        }

        /// <summary>
        /// Описание достигнутых результатов должено быть сохранено в истории
        /// </summary>
        [TestMethod]
        public void TaskTest_DescriptionChange_Must_Be_In_History()
        {
            var task = new Task(topic, taskType, taskPriority, state1, currentDate, user.Object);
            task.SaveHistory(currentDate, user.Object);

            task.Description = description;
            task.SaveHistory(currentDate, user.Object);

            Assert.AreEqual(2, task.History.Count());
            var item = task.History.ElementAt(1);
            Assert.AreEqual(false, item.IsContractorChanged);
            Assert.AreEqual(false, item.IsDeadLineChanged);
            Assert.AreEqual(false, item.IsDealChanged);
            Assert.AreEqual(false, item.IsDeletionDateChanged);
            Assert.AreEqual(true, item. IsDescriptionChanged);
            Assert.AreEqual(false, item.IsExecutedByChanged);
            Assert.AreEqual(false, item.IsTaskExecutionStateChanged);
            Assert.AreEqual(false, item.IsFactualCompletionDateChanged);
            Assert.AreEqual(false, item.IsFactualSpentTimeChanged);
            Assert.AreEqual(false, item.IsTaskPriorityChanged);
            Assert.AreEqual(false, item.IsProductionOrderChanged);
            Assert.AreEqual(false, item.IsStartDateChanged);
            Assert.AreEqual(false, item.IsTopicChanged);
            Assert.AreEqual(false, item.IsTaskTypeChanged);

            Assert.AreEqual(description, item.Description);
        }

        /// <summary>
        /// Ответственное лицо должено быть сохранено в истории
        /// </summary>
        [TestMethod]
        public void TaskTest_ExecutedBy_Must_Be_In_History()
        {
            var task = new Task(topic, taskType, taskPriority, state1, currentDate, user.Object);
            task.SaveHistory(currentDate, user.Object);

            task.ExecutedBy = user.Object;
            task.SaveHistory(currentDate, user.Object);

            Assert.AreEqual(2, task.History.Count());
            var item = task.History.ElementAt(1);
            Assert.AreEqual(false, item.IsContractorChanged);
            Assert.AreEqual(false, item.IsDeadLineChanged);
            Assert.AreEqual(false, item.IsDealChanged);
            Assert.AreEqual(false, item.IsDeletionDateChanged);
            Assert.AreEqual(false, item.IsDescriptionChanged);
            Assert.AreEqual(true, item. IsExecutedByChanged);
            Assert.AreEqual(false, item.IsTaskExecutionStateChanged);
            Assert.AreEqual(false, item.IsFactualCompletionDateChanged);
            Assert.AreEqual(false, item.IsFactualSpentTimeChanged);
            Assert.AreEqual(false, item.IsTaskPriorityChanged);
            Assert.AreEqual(false, item.IsProductionOrderChanged);
            Assert.AreEqual(false, item.IsStartDateChanged);
            Assert.AreEqual(false, item.IsTopicChanged);
            Assert.AreEqual(false, item.IsTaskTypeChanged);

            Assert.AreEqual(user.Object.Id, item.ExecutedBy.Id);
        }

        /// <summary>
        /// Приоритет задачи должен быть сохранен в истории
        /// </summary>
        [TestMethod]
        public void TaskTest_Priority_Must_Be_In_History()
        {
            var task = new Task(topic, taskType, taskPriority, state1, currentDate, user.Object);
            task.SaveHistory(currentDate, user.Object);

            task.Priority = taskPriority2;
            task.SaveHistory(currentDate, user.Object);

            Assert.AreEqual(2, task.History.Count());
            var item = task.History.ElementAt(1);
            Assert.AreEqual(false, item.IsContractorChanged);
            Assert.AreEqual(false, item.IsDeadLineChanged);
            Assert.AreEqual(false, item.IsDealChanged);
            Assert.AreEqual(false, item.IsDeletionDateChanged);
            Assert.AreEqual(false, item.IsDescriptionChanged);
            Assert.AreEqual(false, item.IsExecutedByChanged);
            Assert.AreEqual(false, item.IsTaskExecutionStateChanged);
            Assert.AreEqual(false, item.IsFactualCompletionDateChanged);
            Assert.AreEqual(false, item.IsFactualSpentTimeChanged);
            Assert.AreEqual(true, item. IsTaskPriorityChanged);
            Assert.AreEqual(false, item.IsProductionOrderChanged);
            Assert.AreEqual(false, item.IsStartDateChanged);
            Assert.AreEqual(false, item.IsTopicChanged);
            Assert.AreEqual(false, item.IsTaskTypeChanged);

            Assert.AreEqual(taskPriority2.Id, item.TaskPriority.Id);
        }

        /// <summary>
        /// Заказ на производство должен быть сохранен в истории
        /// </summary>
        [TestMethod]
        public void TaskTest_ProductionOrder_Must_Be_In_History()
        {
            var task = new Task(topic, taskType, taskPriority, state1, currentDate, user.Object);
            task.SaveHistory(currentDate, user.Object);

            task.ProductionOrder = productionOrder.Object;
            task.SaveHistory(currentDate, user.Object);

            Assert.AreEqual(2, task.History.Count());
            var item = task.History.ElementAt(1);
            Assert.AreEqual(false, item.IsContractorChanged);
            Assert.AreEqual(false, item.IsDeadLineChanged);
            Assert.AreEqual(false, item.IsDealChanged);
            Assert.AreEqual(false, item.IsDeletionDateChanged);
            Assert.AreEqual(false, item.IsDescriptionChanged);
            Assert.AreEqual(false, item.IsExecutedByChanged);
            Assert.AreEqual(false, item.IsTaskExecutionStateChanged);
            Assert.AreEqual(false, item.IsFactualCompletionDateChanged);
            Assert.AreEqual(false, item.IsFactualSpentTimeChanged);
            Assert.AreEqual(false, item.IsTaskPriorityChanged);
            Assert.AreEqual(true, item .IsProductionOrderChanged);
            Assert.AreEqual(false, item.IsStartDateChanged);
            Assert.AreEqual(false, item.IsTopicChanged);
            Assert.AreEqual(false, item.IsTaskTypeChanged);

            Assert.AreEqual(productionOrderId, item.ProductionOrder.Id);
        }

        /// <summary>
        /// Тема задачи должна быть сохранена в истории
        /// </summary>
        [TestMethod]
        public void TaskTest_Topic_Must_Be_In_History()
        {
            var task = new Task(topic, taskType, taskPriority, state1, currentDate, user.Object);
            task.SaveHistory(currentDate, user.Object);

            task.Topic = topic2;
            task.SaveHistory(currentDate, user.Object);

            Assert.AreEqual(2, task.History.Count());
            var item = task.History.ElementAt(1);
            Assert.AreEqual(false, item.IsContractorChanged);
            Assert.AreEqual(false, item.IsDeadLineChanged);
            Assert.AreEqual(false, item.IsDealChanged);
            Assert.AreEqual(false, item.IsDeletionDateChanged);
            Assert.AreEqual(false, item.IsDescriptionChanged);
            Assert.AreEqual(false, item.IsExecutedByChanged);
            Assert.AreEqual(false, item.IsTaskExecutionStateChanged);
            Assert.AreEqual(false, item.IsFactualCompletionDateChanged);
            Assert.AreEqual(false, item.IsFactualSpentTimeChanged);
            Assert.AreEqual(false, item.IsTaskPriorityChanged);
            Assert.AreEqual(false, item.IsProductionOrderChanged);
            Assert.AreEqual(false, item.IsStartDateChanged);
            Assert.AreEqual(true, item .IsTopicChanged);
            Assert.AreEqual(false, item.IsTaskTypeChanged);

            Assert.AreEqual(topic2, item.Topic);
        }

        /// <summary>
        /// Исполнение должно быть сохранено и параметры задачи обновлены
        /// </summary>
        [TestMethod]
        public void TaskTest_Execution_Must_Be_Save()
        {
            var task = new Task(topic, taskType, taskPriority, state1, currentDate, user.Object);
            var ex0 = new TaskExecutionItem(task, task.Type, state1, task.CompletionPercentage, user.Object, false, currentDate, currentDate);
            task.SaveHistory(currentDate, user.Object);
            
            var newCurrentDate = currentDate.AddDays(1);
            var execution = new TaskExecutionItem(task, task.Type, state3, completionPercentage2, user.Object, true, newCurrentDate, newCurrentDate);
            execution.SpentTime = 100;
            execution.SaveHistory(newCurrentDate, user.Object);
            task.SaveHistory(newCurrentDate, user.Object);

            Assert.AreEqual(2, task.History.Count());
            var item = task.History.ElementAt(1);
            Assert.AreEqual(false, item.IsContractorChanged);
            Assert.AreEqual(false, item.IsDeadLineChanged);
            Assert.AreEqual(false, item.IsDealChanged);
            Assert.AreEqual(false, item.IsDeletionDateChanged);
            Assert.AreEqual(false, item.IsDescriptionChanged);
            Assert.AreEqual(false, item.IsExecutedByChanged);
            Assert.AreEqual(true, item .IsTaskExecutionStateChanged);
            Assert.AreEqual(true, item .IsFactualCompletionDateChanged);
            Assert.AreEqual(true, item .IsFactualSpentTimeChanged);
            Assert.AreEqual(false, item.IsTaskPriorityChanged);
            Assert.AreEqual(false, item.IsProductionOrderChanged);
            Assert.AreEqual(true, item .IsStartDateChanged);
            Assert.AreEqual(false, item.IsTopicChanged);
            Assert.AreEqual(false, item.IsTaskTypeChanged);

            Assert.AreEqual(state3, task.ExecutionState);
            Assert.AreEqual(newCurrentDate, task.StartDate);
            Assert.AreEqual(100, task.FactualSpentTime);
            Assert.AreEqual(newCurrentDate, task.FactualCompletionDate);
        }

        #endregion
    }
}
