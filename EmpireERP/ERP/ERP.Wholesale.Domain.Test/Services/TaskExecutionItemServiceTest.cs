using System;
using System.Linq;
using System.Collections.Generic;
using ERP.Infrastructure.IoC;
using ERP.Test.Infrastructure;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class TaskExecutionItemServiceTest
    {
        #region Поля

        private ITaskExecutionItemRepository taskExecutionItemRepository;
        private ITaskExecutionItemService taskExecutionItemService;

        private Mock<Task> task;
        private Mock<TaskType> taskType;
        private Mock<User> user;
        private DateTime currentDate;
        private TaskExecutionState taskExecutionState = new TaskExecutionState("qwerty", TaskExecutionStateType.New, (short)1);

        #endregion

        #region Инициализация
        
        [TestInitialize]
        public void Init()
        {
            IoCInitializer.Init();

            taskExecutionItemRepository = IoCContainer.Resolve<ITaskExecutionItemRepository>();
            taskExecutionItemService = new TaskExecutionItemService(taskExecutionItemRepository);

            currentDate = DateTime.Now;
            user = new Mock<User>();
            user.Setup(x => x.GetPermissionDistributionType(It.IsAny<Permission>())).Returns(PermissionDistributionType.All);

            taskType = new Mock<TaskType>();
            taskType.Setup(x => x.States).Returns(new List<TaskExecutionState>() { taskExecutionState });

            task = new Mock<Task>();
            task.Setup(x => x.Type).Returns(taskType.Object);
        }

        #endregion

        #region Тесты

        /// <summary>
        /// При сохранении исполнения ее история тоже должна сохраняться
        /// </summary>
        [TestMethod]
        public void TaskExecutionItemServiceTest_Histroy_Must_Be_Save()
        {
            var execution = new TaskExecutionItem(task.Object, taskType.Object, taskExecutionState, 42, user.Object, true, currentDate, currentDate);
            taskExecutionItemService.Save(execution, currentDate, user.Object);

            Assert.AreEqual(1, execution.History.Count());
        }

        #endregion
    }
}
