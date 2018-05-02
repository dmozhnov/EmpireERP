using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class ProductionOrderBatchLifeCycleTemplateService : IProductionOrderBatchLifeCycleTemplateService
    {
        #region Поля

        private readonly IProductionOrderBatchLifeCycleTemplateRepository productionOrderBatchLifeCycleTemplateRepository;

        #endregion

        #region Конструкторы

        public ProductionOrderBatchLifeCycleTemplateService(IProductionOrderBatchLifeCycleTemplateRepository productionOrderBatchLifeCycleTemplateRepository)
        {
            this.productionOrderBatchLifeCycleTemplateRepository = productionOrderBatchLifeCycleTemplateRepository;
        }

        #endregion

        #region Методы



        public ProductionOrderBatchLifeCycleTemplate GetById(short id)
        {
            return productionOrderBatchLifeCycleTemplateRepository.GetById(id);
        }

        public short Save(ProductionOrderBatchLifeCycleTemplate productionOrderBatchLifeCycleTemplate)
        {
            // Проверяем, нет ли этапов с повторяющимися названиями
            foreach (var stage in productionOrderBatchLifeCycleTemplate.Stages)
            {
                foreach (var anotherStage in productionOrderBatchLifeCycleTemplate.Stages)
                {
                    ValidationUtils.Assert(stage.Name != anotherStage.Name || stage == anotherStage, "Названия этапов не должны повторяться.");
                }
            }

            productionOrderBatchLifeCycleTemplateRepository.Save(productionOrderBatchLifeCycleTemplate);

            return productionOrderBatchLifeCycleTemplate.Id;
        }

        public void Delete(ProductionOrderBatchLifeCycleTemplate productionOrderBatchLifeCycleTemplate)
        {
            productionOrderBatchLifeCycleTemplateRepository.Delete(productionOrderBatchLifeCycleTemplate);
        }

        public IEnumerable<ProductionOrderBatchLifeCycleTemplate> GetFilteredList(object state)
        {
            return productionOrderBatchLifeCycleTemplateRepository.GetFilteredList(state);
        }

        public IEnumerable<ProductionOrderBatchLifeCycleTemplate> GetFilteredList(object state, ParameterString parameterString)
        {
            return productionOrderBatchLifeCycleTemplateRepository.GetFilteredList(state, parameterString);
        }

        /// <summary>
        /// Получение шаблона жизненного цикла заказа по id с проверкой его существования
        /// </summary>
        /// <param name="id">Код</param>
        public ProductionOrderBatchLifeCycleTemplate CheckProductionOrderBatchLifeCycleTemplateExistence(short id, User user)
        {
            var productionOrderBatchLifeCycleTemplate = GetById(id);
            ValidationUtils.NotNull(productionOrderBatchLifeCycleTemplate, "Шаблон жизненного цикла заказа не найден. Возможно, он был удален.");

            return productionOrderBatchLifeCycleTemplate;
        }

        /// <summary>
        /// Проверка названия шаблона жизненного цикла заказа на уникальность
        /// </summary>
        /// <param name="name">Название шаблона</param>
        /// <param name="id">Код текущего шаблона</param>
        /// <returns>Результат проверки</returns>
        public bool IsNameUnique(string name, short id)
        {
            return productionOrderBatchLifeCycleTemplateRepository.Query<ProductionOrderBatchLifeCycleTemplate>().Where(x => x.Name == name && x.Id != id).Count() == 0;
        }

        #region Права на совершение операций

        #region Вспомогательные методы

        private bool IsPermissionToPerformOperation(ProductionOrderBatchLifeCycleTemplate template, User user, Permission permission)
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

        private void CheckPermissionToPerformOperation(ProductionOrderBatchLifeCycleTemplate template, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(template, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        #endregion

        #region Создание / редактирование

        public bool IsPossibilityToEdit(ProductionOrderBatchLifeCycleTemplate template, User user)
        {
            try
            {
                CheckPossibilityToEdit(template, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEdit(ProductionOrderBatchLifeCycleTemplate template, User user)
        {
            // права
            CheckPermissionToPerformOperation(template, user, Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit);
        }

        #endregion

        #region Удаление

        public bool IsPossibilityToDelete(ProductionOrderBatchLifeCycleTemplate template, User user)
        {
            try
            {
                CheckPossibilityToDelete(template, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDelete(ProductionOrderBatchLifeCycleTemplate template, User user)
        {
            // права
            CheckPermissionToPerformOperation(template, user, Permission.ProductionOrderBatchLifeCycleTemplate_Delete);
        }

        #endregion

        #region Создание / редактирование этапа в шаблоне

        public bool IsPossibilityToCreateStageAfter(ProductionOrderBatchLifeCycleTemplateStage stage, User user)
        {
            try
            {
                CheckPossibilityToCreateStageAfter(stage, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreateStageAfter(ProductionOrderBatchLifeCycleTemplateStage stage, User user)
        {
            // права
            CheckPermissionToPerformOperation(stage.Template, user, Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit);

            stage.CheckPossibilityToCreateStageAfter();
        }

        public bool IsPossibilityToEditStage(ProductionOrderBatchLifeCycleTemplateStage stage, User user)
        {
            try
            {
                CheckPossibilityToEditStage(stage, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditStage(ProductionOrderBatchLifeCycleTemplateStage stage, User user)
        {
            // права
            CheckPermissionToPerformOperation(stage.Template, user, Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit);

            stage.CheckPossibilityToEdit();
        }

        #endregion

        #region Удаление этапа в шаблоне

        public bool IsPossibilityToDeleteStage(ProductionOrderBatchLifeCycleTemplateStage stage, User user)
        {
            try
            {
                CheckPossibilityToDeleteStage(stage, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDeleteStage(ProductionOrderBatchLifeCycleTemplateStage stage, User user)
        {
            // права
            CheckPermissionToPerformOperation(stage.Template, user, Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit);

            stage.CheckPossibilityToDelete();
        }

        #endregion

        #region Перемещение этапов в шаблоне

        public bool IsPossibilityToMoveStageUp(ProductionOrderBatchLifeCycleTemplateStage stage, User user)
        {
            try
            {
                CheckPossibilityToMoveStageUp(stage, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToMoveStageUp(ProductionOrderBatchLifeCycleTemplateStage stage, User user)
        {
            // права
            CheckPermissionToPerformOperation(stage.Template, user, Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit);

            stage.CheckPossibilityToMoveUp();
        }

        public bool IsPossibilityToMoveStageDown(ProductionOrderBatchLifeCycleTemplateStage stage, User user)
        {
            try
            {
                CheckPossibilityToMoveStageDown(stage, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToMoveStageDown(ProductionOrderBatchLifeCycleTemplateStage stage, User user)
        {
            // права
            CheckPermissionToPerformOperation(stage.Template, user, Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit);

            stage.CheckPossibilityToMoveDown();
        }

        #endregion

        #endregion

        #endregion
    }
}