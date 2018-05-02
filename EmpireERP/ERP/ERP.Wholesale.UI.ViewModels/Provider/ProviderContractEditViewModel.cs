using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.Provider
{
    public class ProviderContractEditViewModel
    {
        /// <summary>
        /// Идентификатор (код) договора
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Идентификатор (код) поставщика, к которому относится договор
        /// </summary>
        public string ProviderId { get; set; }

        /// <summary>
        /// Номер договора
        /// </summary>
        [DisplayName("Номер")]
        [StringLength(50, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Number { get; set; }

        /// <summary>
        /// Наименование договора
        /// </summary>
        [DisplayName("Название")]
        [Required(ErrorMessage = "Укажите название договора")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        public string Name { get; set; }

        //TODO: валидация дат? Первая обязательна, 2 3 нет

        /// <summary>
        /// Дата договора
        /// </summary>
        [DisplayName("Дата")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату")]
        public string Date { get; set; }

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
        /// Код выбранной организации поставщика
        /// </summary>
        [Required(ErrorMessage = "Укажите организацию данного поставщика")]
        [Range(1, int.MaxValue, ErrorMessage = "Укажите организацию данного поставщика")]
        public string ProviderOrganizationId { get; set; }

        /// <summary>
        /// Название выбранной организации поставщика
        /// </summary>
        [DisplayName("Организация поставщика")]
        public string ProviderOrganizationName { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Comment { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToChangeOrganizations { get; set; }

        /// <summary>
        /// Конструктор для создания договора
        /// </summary>
        public ProviderContractEditViewModel()
        {
            Id = "0";
            Name = "Основной договор";
            Number = String.Empty;
            Date = "";
            AccountOrganizationId = "0";
            AccountOrganizationName = "Выбрать собственную организацию";
            ProviderOrganizationId = "0";
            ProviderOrganizationName = "Выбрать организацию данного поставщика";
            Comment = String.Empty;
        }

        /// <summary>
        /// Конструктор для редактирования договора
        /// </summary>
        public ProviderContractEditViewModel(ERP.Wholesale.Domain.Entities.Contract contract)
        {
            Id = contract.Id.ToString();
            Name = contract.Name;
            Number = contract.Number;
            Date = contract.Date.ToShortDateString();
            AccountOrganizationId = contract.AccountOrganization.Id.ToString();
            AccountOrganizationName = contract.AccountOrganization.ShortName;
            ProviderOrganizationId = contract.ContractorOrganization.Id.ToString();
            ProviderOrganizationName = contract.ContractorOrganization.ShortName;
            Comment = contract.Comment;            
        }
    }
}