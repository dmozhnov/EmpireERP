using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.ClientContract
{
    public class ClientContractEditViewModel
    {
        /// <summary>
        /// Идентификатор (код) договора
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Идентификатор (код) сделки, к которой относится договор
        /// </summary>
        public string DealId { get; set; }

        /// <summary>
        /// Идентификатор (код) клиента, к которому относится договор
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

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
        /// Код выбранной организации клиента
        /// </summary>
        [Required(ErrorMessage = "Укажите организацию данного клиента")]
        [Range(1, int.MaxValue, ErrorMessage = "Укажите организацию данного клиента")]
        public string ClientOrganizationId { get; set; }

        /// <summary>
        /// Название выбранной организации клиента
        /// </summary>
        [DisplayName("Организация клиента")]
        public string ClientOrganizationName { get; set; }

        /// <summary>
        /// Разрешение редактировать организации, между которыми заключен договор
        /// </summary>
        public bool AllowToEditOrganization { get; set; }

        /// <summary>
        /// Разрешение редактировать организацию клиента
        /// </summary>
        public bool AllowToEditClientOrganization { get; set; }

        public string PostControllerName { get; set; }
        public string PostActionName { get; set; }

        /// <summary>
        /// Конструктор для создания договора
        /// </summary>
        public ClientContractEditViewModel()
        {
            Id = "0";
            Name = "Основной договор";
            Number = String.Empty;
            Date = "";
            AccountOrganizationId = "0";
            AccountOrganizationName = "Выбрать собственную организацию";
            ClientOrganizationId = "0";
            ClientOrganizationName = "Выбрать организацию данного клиента";
        }
    }
}