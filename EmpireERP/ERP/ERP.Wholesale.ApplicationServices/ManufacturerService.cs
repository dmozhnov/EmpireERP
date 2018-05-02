using System;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    /// <summary>
    /// Сервис для сущностей «Фабрика-изготовитель»
    /// </summary>
    public class ManufacturerService : BaseDictionaryService<Manufacturer>, IManufacturerService
    {
        #region Свойства

        protected override string UniquenessErrorString
        {
            get { return "Фабрика-изготовитель с таким наименованием уже существует."; }
        }

        protected override string CheckExistenceErrorString
        {
            get { return "Фабрика-изготовитель не найдена. Возможно, она была удалена."; }
        }

        #region Права на операции

        public override Permission CreationPermission
        {
            get { return Permission.Manufacturer_Create; }
        }

        public override Permission EditingPermission
        {
            get { return Permission.Manufacturer_Edit; }
        }

        public override Permission DeletionPermission
        {
            get { return Permission.Manufacturer_Delete; }
        }

        public override Permission ListViewingPermission
        {
            get { return Permission.Producer_List_Details; }
        }

        #endregion

        #endregion

        #region Конструктор

        public ManufacturerService(IBaseDictionaryRepository<Manufacturer> baseDictionaryRepository)
            : base(baseDictionaryRepository)
        {
        }

        #endregion

        #region Методы

        /// <summary>
        /// Удаление изготовителя. Поскольку изготовители удаляются из разных мест и проверки нужны разные, их в данный метод не включаем
        /// (CheckPossibilityToDeleteManufacturerAndProducer и CheckPossibilityToDeleteManufacturer надо вызывать из вызывающего метода)
        /// </summary>
        /// <param name="manufacturer"></param>
        public void Delete(Manufacturer manufacturer)
        {
            baseDictionaryRepository.Delete(manufacturer);
        }

        /// <summary>
        /// Проверка возможности удалить сущность.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="user"></param>
        /// <param name="checkLogic"></param>
        /// <returns></returns>
        public override bool IsPossibilityToDelete(Manufacturer entity, User user, bool checkLogic = true)
        {
            try
            {
                CheckCommonPossibilityToDelete(entity);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка, является ли указанная фабрика-изготовитель производителем
        /// </summary>
        /// <param name="manufacturer">Фабрика-изготовитель</param>
        /// <returns></returns>
        public bool IsProducer(Manufacturer manufacturer)
        {
            return baseDictionaryRepository.Query<ProducerOrganization>().Where(x => x.Manufacturer.Id == manufacturer.Id).Count() > 0;
        }

        /// <summary>
        /// Проверка на возможность удаления фабрики-изготовителя при удалении производителя, которым она является
        /// </summary>
        /// <param name="manufacturer">Фабрика-изготовитель</param>
        /// <param name="producer">Производитель, которым является данный изготовитель</param>
        public bool IsPossibilityToDeleteManufacturerAndProducer(Manufacturer manufacturer, Producer producer)
        {
            try
            {
                CheckPossibilityToDeleteManufacturerAndProducer(manufacturer, producer);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка на возможность удаления фабрики-изготовителя при удалении производителя, которым она является
        /// </summary>
        /// <param name="manufacturer">Фабрика-изготовитель</param>
        /// <param name="producer">Производитель, которым является данный изготовитель</param>
        public void CheckPossibilityToDeleteManufacturerAndProducer(Manufacturer manufacturer, Producer producer)
        {
            ValidationUtils.Assert(baseDictionaryRepository.Query<Producer>()
                .Where(x => x.Id != producer.Id)
                .Restriction<Manufacturer>(x => x.Manufacturers).Where(x => x.Id == manufacturer.Id)
                .CountDistinct() == 0,
                "Данный производитель используется как фабрика-изготовитель.");

            CheckCommonPossibilityToDelete(manufacturer);
        }

        /// <summary>
        /// Проверка на возможность удаления фабрики-изготовителя
        /// Не удалять! Будет нужно, когда создадим справочник изготовителей с возможностью удаления
        /// </summary>
        /// <param name="manufacturer">Фабрика-изготовитель</param>
        public void CheckPossibilityToDeleteManufacturer(Manufacturer manufacturer)
        {
            ValidationUtils.Assert(baseDictionaryRepository.Query<Producer>()
                .Restriction<Manufacturer>(x => x.Manufacturers).Where(x => x.Id == manufacturer.Id)
                .CountDistinct() == 0,
                "Данный производитель используется как фабрика-изготовитель.");

            CheckCommonPossibilityToDelete(manufacturer);
        }

        /// <summary>
        /// Общая проверка на возможность удаления фабрики-изготовителя
        /// </summary>
        /// <param name="manufacturer">Фабрика-изготовитель</param>
        private void CheckCommonPossibilityToDelete(Manufacturer manufacturer)
        {
            ValidationUtils.Assert(baseDictionaryRepository.Query<Article>().Where(x => x.Manufacturer.Id == manufacturer.Id).Count() == 0,
                "Невозможно удалить фабрику-изготовитель, указанную в качестве изготовителя для какого-либо товара из справочника товаров.");

            ValidationUtils.Assert(baseDictionaryRepository.Query<ReceiptWaybillRow>().Where(x => x.Manufacturer.Id == manufacturer.Id).Count() == 0,
                "Фабрика-изготовитель не может быть удалена, так как она участвует в позициях приходных накладных.");

            var productionOrderCount = baseDictionaryRepository.Query<ProductionOrder>()
                .Restriction<ProductionOrderBatch>(x => x.Batches)
                .Restriction<ProductionOrderBatchRow>(x => x.Rows)
                .Where(x => x.Manufacturer.Id == manufacturer.Id).CountDistinct();

            if (productionOrderCount > 0)
            {
                throw new Exception("Фабрика-изготовитель не может быть удалена, так как она участвует в позициях заказов на производство.");
            }
        }

        protected override void CheckPossibilityToDelete(Manufacturer manufacturer, User user, bool checkLogic = true)
        {
            CheckPermissionToPerformOperation(user, DeletionPermission);

            if (checkLogic)
            {
                CheckCommonPossibilityToDelete(manufacturer);
            }
        }

        #endregion
    }
}