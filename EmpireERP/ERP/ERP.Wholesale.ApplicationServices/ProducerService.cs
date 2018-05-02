using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class ProducerService : IProducerService
    {
        #region Поля

        private readonly IProducerRepository producerRepository;
        private readonly IManufacturerService manufacturerService;
        private readonly ITaskRepository taskRepository;

        #endregion

        #region Конструкторы

        public ProducerService(IProducerRepository producerRepository, IManufacturerService manufacturerService, ITaskRepository taskRepository)
        {
            this.producerRepository = producerRepository;
            this.manufacturerService = manufacturerService;
            this.taskRepository = taskRepository;
        }

        #endregion

        #region Методы

        #region Общие методы

        public Producer GetById(int id)
        {
            return producerRepository.GetById(id);
        }

        public int Save(Producer producer)
        {
            CheckProducerNameUniqueness(producer);
            CheckProducerOrganizationNameUniqueness(producer);

            producerRepository.Save(producer);

            return producer.Id;
        }

        public IEnumerable<Producer> GetFilteredList(object state)
        {
            return producerRepository.GetFilteredList(state);
        }

        public IEnumerable<Producer> GetFilteredList(object state, ParameterString parameterString)
        {
            return producerRepository.GetFilteredList(state, parameterString);
        }

        public Producer CheckProducerExistence(int id)
        {
            Producer result = producerRepository.GetById(id);
            ValidationUtils.NotNull(result, "Производитель не найден. Возможно, он был удален.");

            return result;
        }

        private void CheckProducerNameUniqueness(Producer producer)
        {
            ValidationUtils.Assert(producerRepository.Query<Producer>().Where(x => x.Name == producer.Name && x.Id != producer.Id).Count() == 0,
                String.Format("Производитель с названием «{0}» уже существует.", producer.Name));
        }

        private void CheckProducerOrganizationNameUniqueness(Producer producer)
        {
            var producerOrganizationList = producerRepository.Query<ProducerOrganization>().Where(x => x.ShortName == producer.OrganizationName).ToList<ProducerOrganization>();
            ValidationUtils.Assert(!producerOrganizationList.Any(x => x.Producer.Id != producer.Id),
                String.Format("Производитель с организацией «{0}» уже существует.", producer.OrganizationName));
        }

        public ProducerOrganization GetProducerOrganizationByName(string name)
        {
            return producerRepository.Query<ProducerOrganization>().Where(x => x.ShortName == name).FirstOrDefault<ProducerOrganization>();
        }

        public Manufacturer GetManufacturerByName(string name)
        {
            return producerRepository.Query<Manufacturer>().Where(x => x.Name == name).FirstOrDefault<Manufacturer>();
        }

        public IList<ProductionOrderPayment> GetPaymentsFilteredList(object state, ParameterString parameterString)
        {
            return producerRepository.GetPaymentsFilteredList(state, parameterString);
        }

        public IList<ProductionOrder> GetProductionOrders(Producer producer)
        {
            return producerRepository.GetProductionOrders(producer);
        }

        #endregion

        #region Связь "производители - изготовители"

        public void AddManufacturer(Producer producer, Manufacturer manufacturer, User user)
        {
            CheckPossibilityToAddManufacturer(producer, manufacturer, user);

            producer.AddManufacturer(manufacturer);
            producerRepository.Save(producer);
        }

        public void RemoveManufacturer(Producer producer, Manufacturer manufacturer, User user)
        {
            CheckPossibilityToRemoveManufacturer(producer, manufacturer, user);

            producer.RemoveManufacturer(manufacturer);

            producerRepository.Save(producer);
        }

        #endregion

        #region Удаление

        public void Delete(Producer producer, User user)
        {
            Manufacturer manufacturerToDelete;

            CheckPossibilityToDelete(producer, user);

            manufacturerToDelete = producer.Organization.HasManufacturer &&
                manufacturerService.IsPossibilityToDeleteManufacturerAndProducer(producer.Organization.Manufacturer, producer) ?
                producer.Organization.Manufacturer : null;

            producer.Organization.Manufacturer = null;
            producer.Organization.DeletionDate = DateTime.Now;
            producer.As<Producer>().DeletionDate = DateTime.Now;

            producerRepository.Save(producer);

            if (manufacturerToDelete != null)
            {
                manufacturerService.Delete(manufacturerToDelete, user);
            }
        }

        #endregion

        #region Проверка возможности совершения операций

        #region Вспомогательные методы

        private bool IsPermissionToPerformOperation(Producer producer, User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(Producer producer, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(producer, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        #endregion

        #region Удаление

        public bool IsPossibilityToDelete(Producer producer, User user)
        {
            try
            {
                CheckPossibilityToDelete(producer, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDelete(Producer producer, User user)
        {
            // права
            CheckPermissionToPerformOperation(producer, user, Permission.Producer_Delete);

            ValidationUtils.Assert(producerRepository.Query<ProductionOrder>().Where(x => x.Producer.Id == producer.Id).CountDistinct() == 0,
                "Невозможно удалить производителя, имеющего заказы на производство.");

            var countOfLinkedTask = taskRepository.GetTaskCountForContractor(producer.Id);
            ValidationUtils.Assert(countOfLinkedTask == 0, String.Format("Невозможно удалить производителя, так как с ним связаны мероприятия и задачи в количестве {0} шт.", countOfLinkedTask));
        }

        #endregion

        #region Удаление фабрики-изготовителя из списка другого производителя

        public bool IsPossibilityToRemoveManufacturer(Producer producer, Manufacturer manufacturer, User user)
        {
            try
            {
                CheckPossibilityToRemoveManufacturer(producer, manufacturer, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToRemoveManufacturer(Producer producer, Manufacturer manufacturer, User user)
        {
            // права
            CheckPermissionToPerformOperation(producer, user, Permission.Producer_Edit);

            if (producer.Organization.HasManufacturer && producer.Organization.Manufacturer == manufacturer)
            {
                throw new Exception("Связь не может быть разорвана, так как производитель является данной фабрикой-изготовителем.");
            }

            var productionOrders = producerRepository.SubQuery<ProductionOrder>().Where(x => x.Producer.Id == producer.Id).Select(x => x.Id);

            var productionOrderBatchRowsCount = producerRepository.Query<ProductionOrderBatchRow>()
                .Where(x => x.Manufacturer.Id == manufacturer.Id)
                .Restriction<ProductionOrderBatch>(x => x.Batch)
                .Restriction<ProductionOrder>(x => x.ProductionOrder)
                .PropertyIn(x => x.Id, productionOrders).CountDistinct();

            if (productionOrderBatchRowsCount > 0)
            {
                throw new Exception("Связь не может быть разорвана, так как фабрика-изготовитель участвует в заказах этого производителя.");
            }
        }

        #endregion

        #region Добавление фабрики-изготовителя в список другого производителя

        public bool IsPossibilityToAddManufacturer(Producer producer, Manufacturer manufacturer, User user)
        {
            try
            {
                CheckPossibilityToAddManufacturer(producer, manufacturer, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToAddManufacturer(Producer producer, Manufacturer manufacturer, User user)
        {
            // права
            CheckPermissionToPerformOperation(producer, user, Permission.Producer_Edit);

            if (producer.Manufacturers.Contains(manufacturer))
            {
                throw new Exception("Эта фабрика-изготовитель уже связана с производителем.");
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
