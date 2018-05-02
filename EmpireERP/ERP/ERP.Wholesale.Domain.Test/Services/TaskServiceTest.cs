using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ERP.Wholesale.Domain.Repositories;
using ERP.Test.Infrastructure;
using Moq;
using ERP.Infrastructure.IoC;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.ApplicationServices;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class TaskServiceTest
    {
        #region Поля

        private ITaskService taskService;

        private string topic;
        private TaskExecutionState state;
        private DateTime currentDate;

        private Mock<Deal> deal;
        private Mock<Contractor> contractor;
        private Mock<ProductionOrder> productionOrder;
        private User user;

        private TaskType taskType;
        private TaskPriority taskPriority;

        #endregion

        #region Инифиализация
        
        [TestInitialize]
        public void Init()
        {
            IoCInitializer.Init();

            var taskRepository = IoCContainer.Resolve<ITaskRepository>();
            var taskExecutionItemRepository = IoCContainer.Resolve<ITaskExecutionItemRepository>();
            taskService = new TaskService(taskRepository, taskExecutionItemRepository);

            topic = "123";
            deal = new Mock<Deal>();
            contractor = new Mock<Contractor>();
            productionOrder = new Mock<ProductionOrder>();
            taskType = new TaskType("Type_1");
            state = new TaskExecutionState("State_1", TaskExecutionStateType.New, 1);
            taskType.AddState(state);
            taskPriority = new TaskPriority("Priority_1", 1);
            currentDate = DateTime.Now;

            user = new User(new Employee("Иван", "Иванов", "Иванович", new EmployeePost("Менеджер"), null), "Иванов Иван", "ivanov", "pa$$w0rd", new Team("Тестовая команда", null), null);
            var role = new Role("Администратор");
            role.AddPermissionDistribution(new PermissionDistribution(Permission.Task_Create, PermissionDistributionType.All));
            user.AddRole(role);
        }

        #endregion

        #region Тесты

        /// <summary>
        /// История задачи должна сохраняться при сохранении задачи
        /// </summary>
        [TestMethod]
        public void TaskServiceTest_History_Must_Be_Save()
        {
            var task = new Task(topic, taskType, taskPriority, state, currentDate, user);

            taskService.Save(task, currentDate, user);

            Assert.AreEqual(1, task.History.Count());
        }

        #endregion
    }
}
