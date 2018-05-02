using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderContractEditViewModel
    {
        #region Свойства
        
        /// <summary>
        /// Идентификатор (код) контракта
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Идентификатор (код) заказа, к которому относится контракт
        /// </summary>
        public string ProductionOrderId { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Номер контракта
        /// </summary>
        [DisplayName("Номер")]
        [StringLength(50, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Number { get; set; }

        /// <summary>
        /// Наименование контракта
        /// </summary>
        [DisplayName("Название")]
        [Required(ErrorMessage = "Укажите название контракта")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        public string Name { get; set; }

        /// <summary>
        /// Дата контракта
        /// </summary>
        [DisplayName("Дата")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату")]
        public string ContractDate { get; set; }

        /// <summary>
        /// Код выбранной собственной организации
        /// </summary>
        [Required(ErrorMessage = "Укажите собственную организацию")]
        [Range(1, int.MaxValue, ErrorMessage = "Укажите собственную организацию")]
        public string AccountOrganizationId { get; set; }

        /// <summary>
        /// Название выбранной собственной организации
        /// </summary>
        [DisplayName("Собственная организация")]
        public string AccountOrganizationName { get; set; }

        /// <summary>
        /// Название организации производителя
        /// </summary>
        [DisplayName("Организация производителя")]
        public string ProducerOrganizationName { get; set; }

        #endregion

        #region Разрешенные операции

        /// <summary>
        /// Разрешено ли изменять собственную организацию
        /// </summary>
        public bool AllowToChangeAccountOrganization { get; set; }

        /// <summary>
        /// Разрешено ли изменять дату создания контракта
        /// </summary>
        public bool AllowToChangeDate { get; set; }

        #endregion

        #region Конструкторы
        
        /// <summary>
        /// Конструктор для MVC
        /// </summary>
        public ProductionOrderContractEditViewModel()
        {
        }

        /// <summary>
        /// Конструктор для создания контракта
        /// </summary>
        public ProductionOrderContractEditViewModel(ERP.Wholesale.Domain.Entities.ProductionOrder productionOrder)
        {
            Id = "0";
            Name = "Основной контракт";
            Number = String.Empty;
            ContractDate = DateTime.Now.ToShortDateString();

            AccountOrganizationId = "0";
            AccountOrganizationName = "Выбрать собственную организацию";
            ProductionOrderId = productionOrder.Id.ToString();
            ProducerOrganizationName = productionOrder.Producer.OrganizationName;

            AllowToChangeDate = true;
        }

        /// <summary>
        /// Конструктор для редактирования контракта
        /// </summary>
        public ProductionOrderContractEditViewModel(ERP.Wholesale.Domain.Entities.Contract contract)
        {
            Id = contract.Id.ToString();
            Name = contract.Name;
            Number = contract.Number;
            ContractDate = contract.Date.ToShortDateString();

            AccountOrganizationId = contract.AccountOrganization.Id.ToString();
            AccountOrganizationName = contract.AccountOrganization.ShortName;
            ProductionOrderId = "0";
            ProducerOrganizationName = contract.ContractorOrganization.ShortName;            
        }

        #endregion
    }
}