using System.ComponentModel;

namespace Bizpulse.Admin.UI.ViewModels.Client
{
    public class ClientMainDetailsViewModel
    {        
        [DisplayName("Текущая конфигурация")]
        public string ConfigurationName { get; set; }

        [DisplayName("Остаток средств")]
        public string PrepaymentSum { get; set; }

        [DisplayName("Дата создания аккаунта")]
        public string CreationDate { get; set; }

        [DisplayName("Оплачено до")]
        public string PaidPeriodEnd { get; set; }
    }
}
