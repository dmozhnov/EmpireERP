using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class TaskExecutionItemTest
    {
        #region Поля

        private Task task;
        private Mock<User> user;
        private DateTime currentDate;
        private TaskExecutionItem execution;
        private Mock<TaskType> taskType = new Mock<TaskType>();
        private Mock<TaskPriority> taskPriority = new Mock<TaskPriority>();
        private Mock<TaskExecutionState> taskExecutionState, taskExecutionState2, taskExecutionState3, taskExecutionState4;
        private IList<TaskExecutionState> executionStates = new List<TaskExecutionState>();

        #endregion

        #region Инициализация
        
        [TestInitialize]
        public void Init()
        {
            taskExecutionState =  new Mock<TaskExecutionState>() ;
            taskExecutionState.Setup(x => x.Id).Returns(1);
            taskExecutionState.Setup(x => x.ExecutionStateType).Returns(TaskExecutionStateType.New);
            taskExecutionState.Setup(x => x.Equals(It.Is<TaskExecutionState>(y => y.Id == 1))).Returns(true);

            taskExecutionState2 = new Mock<TaskExecutionState>();
            taskExecutionState2.Setup(x => x.Id).Returns(2);
            taskExecutionState2.Setup(x => x.ExecutionStateType).Returns(TaskExecutionStateType.New);
            taskExecutionState2.Setup(x => x.Equals(It.Is<TaskExecutionState>(y=>y.Id == 2))).Returns(true);

            taskExecutionState3 = new Mock<TaskExecutionState>();
            taskExecutionState3.Setup(x => x.Id).Returns(3);
            taskExecutionState3.Setup(x => x.ExecutionStateType).Returns(TaskExecutionStateType.Executing);
            taskExecutionState3.Setup(x => x.Equals(It.Is<TaskExecutionState>(y => y.Id == 3))).Returns(true);

            taskExecutionState4 = new Mock<TaskExecutionState>();
            taskExecutionState4.Setup(x => x.Id).Returns(4);
            taskExecutionState4.Setup(x => x.ExecutionStateType).Returns(TaskExecutionStateType.Completed);
            taskExecutionState4.Setup(x => x.Equals(It.Is<TaskExecutionState>(y => y.Id == 4))).Returns(true);

            taskPriority = new Mock<TaskPriority>();
            executionStates.Add(taskExecutionState.Object);
            executionStates.Add(taskExecutionState2.Object);
            executionStates.Add(taskExecutionState3.Object);
            executionStates.Add(taskExecutionState4.Object);

            taskType = new Mock<TaskType>();
            taskType.Setup(x => x.States).Returns(executionStates);

            currentDate = DateTime.Now;
            user = new Mock<User>();
            user.Setup(x => x.Id).Returns(1);

            task = new Task("_", taskType.Object, taskPriority.Object, executionStates[0], currentDate, user.Object);            
        }

        #endregion

        #region Тесты

        #region
        
        /// <summary>
        /// Параметры, передаваемые через конструктор, должны быть сохранены
        /// </summary>
        [TestMethod]
        public void TaskExecutionItemTest_Constructor_Param_Must_Be_Set()
        {
            execution = new TaskExecutionItem(task, taskType.Object, executionStates[0], (byte)0, user.Object, true, currentDate, currentDate.AddDays(1));

            Assert.AreEqual(task, execution.Task);
            Assert.AreEqual(user.Object, execution.CreatedBy);
            Assert.AreEqual(currentDate, execution.CreationDate);
            Assert.AreEqual(currentDate.AddDays(1), execution.Date);
        }

        /// <summary>
        /// Конструктор должен бросить исключение из-за равенства задачи NULL
        /// </summary>
        [TestMethod]
        public void TaskExecutionItemTest_Constructor_Must_Throw_Exception_On_NULL_Task()
        {
            try
            {
                execution = new TaskExecutionItem(null, taskType.Object, taskExecutionState.Object, (byte)0,user.Object, true,  currentDate, currentDate.AddDays(1));
                Assert.Fail("Исключение не сгенерированно.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Необходимо указать задачу, к которой относится исполнение.", ex.Message);
            }
        }

        /// <summary>
        /// Конструктор должен бросить исключение из-за равенства автора исполнения NULL
        /// </summary>
        [TestMethod]
        public void TaskExecutionItemTest_Constructor_Must_Throw_Exception_On_NULL_CreatedBy()
        {
            try
            {
                execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState.Object, (byte)0, null, true, currentDate, currentDate.AddDays(1));
                Assert.Fail("Исключение не сгенерированно.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Необходимо указать пользователя, создавшего исполнение.", ex.Message);
            }
        }

        /// <summary>
        /// При создании исполнения, его первоначальные значения дожны попасть в историю изменений
        /// </summary>
        [TestMethod]
        public void TaskExecutionItemTest_Init_Params_Must_Be_In_History()
        {
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState.Object, (byte)0, user.Object, true, currentDate, currentDate.AddDays(1));
            execution.SaveHistory(currentDate, user.Object);

            Assert.AreEqual(1, execution.History.Count());
            var item = execution.History.ElementAt(0);
            Assert.AreEqual(1, item.CreatedBy.Id);
            Assert.AreEqual(currentDate, item.CreationDate);
            Assert.AreEqual(TaskHistoryItemType.TaskExecutionHistory, item.HistoryItemType);
            Assert.AreEqual(execution.Id, item.TaskExecutionItem.Id);
            Assert.AreEqual(task.Id, item.Task.Id);

            Assert.AreEqual(true, item.IsCompletionPercentageChanged);
            Assert.AreEqual((byte)0, item.CompletionPercentage);
            Assert.AreEqual(currentDate.AddDays(1), item.Date);
            Assert.AreEqual(true, item.IsDateChanged);
            Assert.AreEqual(true, item.IsDeletionDateChanged);
            Assert.AreEqual(null, item.DeletionDate);
            Assert.AreEqual(true, item.IsTaskExecutionStateChanged);
            Assert.AreEqual(executionStates[0].Id, item.TaskExecutionState.Id);
            Assert.AreEqual(true, item.IsResultDescriptionChanged);
            Assert.AreEqual(true, String.IsNullOrEmpty(item.ResultDescription));
            Assert.AreEqual(true, item.IsSpentTimeChanged);
            Assert.AreEqual(0, item.SpentTime);
        }

        /// <summary>
        /// В историю исполнения должно попасть только измененное значение (% выполнения)
        /// </summary>
        [TestMethod]
        public void TaskExecutionItemTest_CompletionPercentage_Must_Be_In_History()
        {
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState.Object, (byte)0,user.Object, true,currentDate, currentDate.AddDays(1));
            execution.SaveHistory(currentDate, user.Object);

            execution.CompletionPercentage = 55;
            execution.SaveHistory(currentDate, user.Object);

            Assert.AreEqual(2, execution.History.Count());
            var item = execution.History.ElementAt(1);
            
            Assert.AreEqual(true, item.IsCompletionPercentageChanged);
            Assert.AreEqual((byte)55, item.CompletionPercentage);

            Assert.AreEqual(false, item.IsDateChanged);
            Assert.AreEqual(false, item.IsDeletionDateChanged);
            Assert.AreEqual(false, item.IsTaskExecutionStateChanged);
            Assert.AreEqual(false, item.IsResultDescriptionChanged);
            Assert.AreEqual(false, item.IsSpentTimeChanged);
        }

        /// <summary>
        /// В историю исполнения должно попасть только измененное значение (дата исполнения)
        /// </summary>
        [TestMethod]
        public void TaskExecutionItemTest_Date_Must_Be_In_History()
        {
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState.Object, (byte)0, user.Object, true, currentDate, currentDate);
            execution.SaveHistory(currentDate, user.Object);

            execution.Date = currentDate.AddDays(-1);
            execution.SaveHistory(currentDate, user.Object);

            Assert.AreEqual(2, execution.History.Count());
            var item = execution.History.ElementAt(1);

            Assert.AreEqual(true, item.IsDateChanged);
            Assert.AreEqual(currentDate.AddDays(-1), item.Date);

            Assert.AreEqual(false, item.IsCompletionPercentageChanged);
            Assert.AreEqual(false, item.IsDeletionDateChanged);
            Assert.AreEqual(false, item.IsTaskExecutionStateChanged);
            Assert.AreEqual(false, item.IsResultDescriptionChanged);
            Assert.AreEqual(false, item.IsSpentTimeChanged);
        }

        /// <summary>
        /// В историю исполнения должно попасть только измененное значение (дата удаления)
        /// </summary>
        [TestMethod]
        public void TaskExecutionItemTest_DeletionDate_Must_Be_In_History()
        {
            // Исполнение, которое создается при создании задачи
            var ex0 = new TaskExecutionItem(task, taskType.Object, taskExecutionState.Object, (byte)0, user.Object, true, currentDate, currentDate);

            // тестируемое исполнение
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState.Object, (byte)0, user.Object, true, currentDate, currentDate);
            execution.SaveHistory(currentDate, user.Object);
            

            execution.DeletionDate = currentDate;
            execution.SaveHistory(currentDate, user.Object);

            Assert.AreEqual(2, execution.History.Count());
            var item = execution.History.ElementAt(1);

            Assert.AreEqual(true, item.IsDeletionDateChanged);
            Assert.AreEqual(currentDate, item.DeletionDate);

            Assert.AreEqual(false, item.IsCompletionPercentageChanged);
            Assert.AreEqual(false, item.IsDateChanged);
            Assert.AreEqual(false, item.IsTaskExecutionStateChanged);
            Assert.AreEqual(false, item.IsResultDescriptionChanged);
            Assert.AreEqual(false, item.IsSpentTimeChanged);
        }

        /// <summary>
        /// В историю исполнения должно попасть только измененное значение (статус исполнения)
        /// </summary>
        [TestMethod]
        public void TaskExecutionItemTest_ExecutionState_Must_Be_In_History()
        {
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState.Object, (byte)0, user.Object, true, currentDate, currentDate);
            execution.SaveHistory(currentDate, user.Object);

            execution.ExecutionState = taskExecutionState2.Object;
            execution.SaveHistory(currentDate, user.Object);

            Assert.AreEqual(2, execution.History.Count());
            var item = execution.History.ElementAt(1);

            Assert.AreEqual(true, item.IsTaskExecutionStateChanged);
            Assert.AreEqual(taskExecutionState2.Object.Id, item.TaskExecutionState.Id);

            Assert.AreEqual(false, item.IsCompletionPercentageChanged);
            Assert.AreEqual(false, item.IsDateChanged);
            Assert.AreEqual(false, item.IsDeletionDateChanged);
            Assert.AreEqual(false, item.IsResultDescriptionChanged);
            Assert.AreEqual(false, item.IsSpentTimeChanged);
        }

        /// <summary>
        /// В историю исполнения должно попасть только измененное значение (описание результата)
        /// </summary>
        [TestMethod]
        public void TaskExecutionItemTest_ResultDescription_Must_Be_In_History()
        {
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState.Object, (byte)0, user.Object, true, currentDate, currentDate);
            execution.SaveHistory(currentDate, user.Object);

            execution.ResultDescription = "1234567890";
            execution.SaveHistory(currentDate, user.Object);

            Assert.AreEqual(2, execution.History.Count());
            var item = execution.History.ElementAt(1);

            Assert.AreEqual(true, item.IsResultDescriptionChanged);
            Assert.AreEqual("1234567890", item.ResultDescription);

            Assert.AreEqual(false, item.IsCompletionPercentageChanged);
            Assert.AreEqual(false, item.IsDateChanged);
            Assert.AreEqual(false, item.IsDeletionDateChanged);
            Assert.AreEqual(false, item.IsTaskExecutionStateChanged);
            Assert.AreEqual(false, item.IsSpentTimeChanged);
        }

        #endregion

        
        /// <summary>
        /// Статус и дата фактического начала исполнения должны измениться
        /// </summary>
        [TestMethod]
        public void TaskTest_ExecutionState_And_StartDate_Must_Change()
        {
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState.Object, (byte)0, user.Object, true, currentDate, currentDate);
            Assert.AreEqual(taskExecutionState.Object, task.ExecutionState);
            Assert.AreEqual((DateTime?)null, task.StartDate);

            var exDate = currentDate.AddDays(1);
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState3.Object, (byte)0, user.Object, true, exDate, exDate);

            Assert.AreEqual(taskExecutionState3.Object, task.ExecutionState);
            Assert.AreEqual(exDate, task.StartDate);
        }

        /// <summary>
        /// Статус и дата фактического завершения должны измениться
        /// </summary>
        [TestMethod]
        public void TaskTest_ExecutionState_StartDate_And_FactualCompletionDate_Must_Change()
        {
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState.Object, (byte)0, user.Object, true, currentDate, currentDate);
            Assert.AreEqual(taskExecutionState.Object, task.ExecutionState);
            Assert.AreEqual((DateTime?)null, task.StartDate);

            var exDate = currentDate.AddDays(1);
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState4.Object, (byte)0, user.Object, true, exDate, exDate);

            Assert.AreEqual(taskExecutionState4.Object, task.ExecutionState);
            Assert.AreEqual(exDate, task.StartDate);
            Assert.AreEqual(exDate, task.FactualCompletionDate);
        }

        /// <summary>
        /// Дата фактического завершения должна быть сброшена
        /// </summary>
        [TestMethod]
        public void TaskTest_Task_Must_Drop_CompletionDate()
        {
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState.Object, (byte)0, user.Object, true, currentDate, currentDate);
            Assert.AreEqual(taskExecutionState.Object, task.ExecutionState);
            Assert.AreEqual((DateTime?)null, task.StartDate);

            var completionDate = currentDate.AddDays(1);
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState4.Object, (byte)0, user.Object, true, completionDate, completionDate);
            
            var exDate = currentDate.AddDays(1);
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState3.Object, (byte)0, user.Object, true, exDate, exDate);

            Assert.AreEqual(taskExecutionState3.Object, task.ExecutionState);
            Assert.AreEqual(completionDate, task.StartDate);
            Assert.AreEqual((DateTime?)null, task.FactualCompletionDate);
        }

        /// <summary>
        /// Должно быть установлено затраченное время
        /// </summary>
        [TestMethod]
        public void TaskTest_SpentTime_Must_Increase()
        {
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState.Object, (byte)0, user.Object, true, currentDate, currentDate);
            Assert.AreEqual(taskExecutionState.Object, task.ExecutionState);
            Assert.AreEqual((DateTime?)null, task.StartDate);

            var completionDate = currentDate.AddDays(1);
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState3.Object, (byte)0, user.Object, true, completionDate, completionDate);
            execution.SpentTime = 100;

            Assert.AreEqual(taskExecutionState3.Object, task.ExecutionState);
            Assert.AreEqual(completionDate, task.StartDate);
            Assert.AreEqual((DateTime?)null, task.FactualCompletionDate);
            Assert.AreEqual(100, task.FactualSpentTime);
        }

        /// <summary>
        /// Затраченное время по всем исполнениям должно суммироваться
        /// </summary>
        [TestMethod]
        public void TaskTest_SpentTime_Must_Add()
        {
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState.Object, (byte)0, user.Object, true, currentDate, currentDate);
            Assert.AreEqual(taskExecutionState.Object, task.ExecutionState);
            Assert.AreEqual((DateTime?)null, task.StartDate);

            var completionDate = currentDate.AddDays(1);
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState3.Object, (byte)0, user.Object, true, completionDate, completionDate);
            execution.SpentTime = 100;

            var completionDate2 = currentDate.AddDays(1);
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState3.Object, (byte)0, user.Object, true, completionDate2, completionDate2);
            execution.SpentTime = 79;

            Assert.AreEqual(taskExecutionState3.Object, task.ExecutionState);
            Assert.AreEqual(completionDate, task.StartDate);
            Assert.AreEqual((DateTime?)null, task.FactualCompletionDate);
            Assert.AreEqual(179, task.FactualSpentTime);
        }

        /// <summary>
        /// Затраченное время по всем исполнениям должно суммироваться
        /// </summary>
        [TestMethod]
        public void TaskTest_CompletionPrecentage_Must_Be_Set()
        {
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState.Object, (byte)0, user.Object, true, currentDate, currentDate);
            Assert.AreEqual(taskExecutionState.Object, task.ExecutionState);
            Assert.AreEqual((DateTime?)null, task.StartDate);

            var completionDate = currentDate.AddDays(1);
            execution = new TaskExecutionItem(task, taskType.Object, taskExecutionState3.Object, (byte)33, user.Object, true, completionDate, completionDate);

            Assert.AreEqual(completionDate, task.StartDate);
            Assert.AreEqual((DateTime?)null, task.FactualCompletionDate);
            Assert.AreEqual(33, task.CompletionPercentage);
        }

        #endregion
    }
}
