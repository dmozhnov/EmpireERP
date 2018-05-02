using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bizpulse.Admin.UI.ViewModels.Client;
using ERP.Infrastructure.UnitOfWork;
using System.Data;
using ERP.Utils;
using ERP.Utils.Mvc;
using System.Threading;
using Bizpulse.Admin.Domain.Entities;
using Bizpulse.Admin.Domain.ValueObjects;
using Bizpulse.Admin.Domain.Repositories;
using System.IO;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Bizpulse.Admin.Domain.AbstractServices;
using Bizpulse.Admin.UI.Web.Infrastructure;
using ERP.UI.ViewModels.Grid;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

namespace Bizpulse.Admin.UI.Web.Controllers
{
    public class ClientController : BaseAdminController
    {
        #region Свойства

        private readonly ICityRepository cityRepository;
        private readonly IRegionRepository regionRepository;
        private readonly IClientRepository clientRepository;
        private readonly IRateRepository rateRepository;

        private readonly IClientService сlientService;

        #endregion

        #region Конструкторы
        
        public ClientController(ICityRepository cityRepository, IRegionRepository regionRepository, IClientRepository clientRepository,
            IRateRepository rateRepository, IClientService сlientService)
        {
            this.cityRepository = cityRepository;
            this.regionRepository = regionRepository;
            this.clientRepository = clientRepository;
            this.rateRepository = rateRepository;
            
            this.сlientService = сlientService;
        }

        #endregion

        #region Методы

        #region Список

        [NeedAdministratorAuthorization]
        public ActionResult List()
        {
            try
            {
                using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
                {
                    var model = new ClientListViewModel()
                    {
                        ClientGrid = GetClientGridLocal(new GridState() { PageSize = 25, Sort = "CreationDate=Desc" })
                    };

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        [NeedAdministratorAuthorization]
        public ActionResult ShowClientGrid(GridState state)
        {
            try
            {
                using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
                {
                    return PartialView("ClientGrid", GetClientGridLocal(state));
                }
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        private GridData GetClientGridLocal(GridState state)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            GridData model = new GridData();
            //model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("Number", "№ акк.", Unit.Pixel(35), align: GridColumnAlign.Right);
            model.AddColumn("CreationDate", "Дата создания", Unit.Pixel(20));
            model.AddColumn("LastActivityDate", "Последняя активность", Unit.Pixel(60));
            model.AddColumn("Type", "Тип", Unit.Pixel(100));
            model.AddColumn("DisplayName", "Название", Unit.Percentage(40));
            model.AddColumn("AdminDisplayName", "ФИО администратора", Unit.Percentage(30));
            model.AddColumn("AdminEmail", "E-mail администратора", Unit.Pixel(100));            
            model.AddColumn("Phone", "Телефон", Unit.Pixel(100));
            model.AddColumn("PromoCode", "Промокод", Unit.Pixel(60));            
            model.AddColumn("PrepaymentSum", "Неизрасх. аванс (руб.)", Unit.Pixel(67), align: GridColumnAlign.Right);            
            model.AddColumn("CurrentConfiguration", "Текущая конфигурация", Unit.Percentage(30));            
            model.AddColumn("DBServerName", "Сервер БД", Unit.Pixel(65));
            model.AddColumn("DBName", "БД", Unit.Pixel(90));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.State = state;

            var clientList = clientRepository.GetFilteredList(state);

            foreach (var client in clientList)
            {
                var actions = new GridActionCell("Action");
                actions.AddAction("Заблокировать", "block_link");

                var currentConfigurationName = "";
                var currentServiceSet = client.ServiceSets.Where(x => x.IsCurrent).FirstOrDefault();

                if (currentServiceSet != null)
                {
                    currentConfigurationName = currentServiceSet.Configuration.Name;
                }

                GridRowStyle style = GridRowStyle.Normal;
                // не заходили больше недели
                if ((DateTime.Now - client.LastActivityDate).Days > 7)
                {
                    style = GridRowStyle.Error;
                }                
                // зарегистрированы сегодня
                if(client.CreationDate.Date == DateTime.Today)
                {
                    style = GridRowStyle.Success;
                }

                model.AddRow(new GridRow(
                    //actions,
                    new GridLabelCell("Number") { Value = client.Number.ForDisplay() },
                    new GridLabelCell("CreationDate") { Value = client.CreationDate.ToShortDateString() + " " + client.CreationDate.ToShortTimeString() },
                    new GridLabelCell("LastActivityDate") { Value = client.LastActivityDate.ToShortDateString() + " " + client.LastActivityDate.ToShortTimeString() },
                    new GridLabelCell("Type") { Value = client.Type.GetDisplayName() },
                    new GridLabelCell("DisplayName") { Value = client.DisplayName },
                    new GridLabelCell("AdminDisplayName") { Value = client.Users.First(x => x.IsClientAdmin).DisplayName },
                    new GridLabelCell("AdminEmail") { Value = client.AdminEmail },
                    new GridLabelCell("Phone") { Value = client.Phone },
                    new GridLabelCell("PromoCode") { Value = client.PromoCode },                    
                    new GridLabelCell("PrepaymentSum") { Value = client.PrepaymentSum.ForDisplay() },
                    new GridLabelCell("CurrentConfiguration") { Value = currentConfigurationName },                    
                    new GridLabelCell("DBServerName") { Value = client.DBServerName },
                    new GridLabelCell("DBName") { Value = client.DBName },
                    new GridHiddenCell("Id") { Value = client.Id.ToString() }
                ) { Style = style });
            }

            return model;
        }
        #endregion

        #region Регистрация и создание аккаунта

        #region Полная регистрация
        
        public ActionResult Register()
        {
            ViewBag.RegionList = regionRepository.GetList().OrderBy(x => x.SortOrder).ThenBy(x => x.Name)
                .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false);

            var rateList = rateRepository.GetList().Where(x => x.Id != 1 && x.Id != 5); // исключаем тарифы «Бесплатный» и «Тестовый период»

            var serializer = new JavaScriptSerializer();
            ViewBag.RateList = rateList.GetParamComboBoxItemList(s => s.Name, s => s.Id.ToString(), s => serializer.Serialize(s), addEmptyItem: false, sort: false);

            return View();
        }

        [HttpPost]
        public ActionResult RegisterJuridicalPerson(JuridicalPersonRegistrationViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var firstError = ModelState.Values.Where(x => x.Errors.Any()).FirstOrDefault();

                    ValidationUtils.IsNull(firstError, firstError.Errors.FirstOrDefault().ErrorMessage);
                }

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var juridicalAddressCityId = ValidationUtils.TryGetShort(model.JuridicalAddressCityId, "Неверное значение входного параметра.");
                var postalAddressCityId = ValidationUtils.TryGetShort(model.PostalAddressCityId, "Неверное значение входного параметра.");
                var rateId = ValidationUtils.TryGetShort(model.RateId, "Неверное значение входного параметра.");
                
                var extraUserCount = ValidationUtils.TryGetShort(model.ExtraUserCount);
                var extraTeamCount = ValidationUtils.TryGetShort(model.ExtraTeamCount);
                var extraStorageCount = ValidationUtils.TryGetShort(model.ExtraStorageCount);
                var extraAccountOrganizationCount = ValidationUtils.TryGetShort(model.ExtraAccountOrganizationCount);
                var extraGigabyteCount = ValidationUtils.TryGetShort(model.ExtraGigabyteCount);

                using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
                {
                    var juridicalAddressCity = cityRepository.GetById(juridicalAddressCityId);
                    ValidationUtils.NotNull(juridicalAddressCity, "Город не найден.");

                    var juridicalAddress = new Address(juridicalAddressCity, model.JuridicalAddressPostalIndex, model.JuridicalAddressLocalAddress);

                    var rate = rateRepository.GetById(rateId);
                    ValidationUtils.NotNull(rate, "Тариф не найден.");

                    Address postalAddress = null;

                    if (model.PostalAddressEqualsJuridical)
                    {
                        postalAddress = new Address(juridicalAddress.City, juridicalAddress.PostalIndex, juridicalAddress.LocalAddress);
                    }
                    else
                    {
                        var postalAddressCity = cityRepository.GetById(postalAddressCityId);
                        ValidationUtils.NotNull(postalAddressCityId, "Город не найден.");

                        postalAddress = new Address(postalAddressCity, model.PostalAddressPostalIndex, model.PostalAddressLocalAddress);
                    }

                    var client = new JuridicalPerson(model.ShortName, juridicalAddress, postalAddress, model.INN,
                        model.DirectorPost, model.DirectorName, currentDateTime);

                    client.AdminEmail = model.AdminEmail;
                    client.DBServerName = AppSettings.ClientDBServerName;
                    client.DirectorEmail = model.DirectorEmail;
                    client.KPP = model.KPP;
                    client.OGRN = model.OGRN;
                    client.OKPO = model.OKPO;
                    client.Phone = model.Phone;
                    client.PromoCode = model.PromoCode;
                    
                    clientRepository.Save(client);

                    client.DBName = "bizpulse_" + client.Number.ToString();

                    var admin = new ClientUser(model.AdminLastName, model.AdminFirstName, model.AdminLogin, CryptographyUtils.ComputeHash(model.AdminPassword),
                        true, currentDateTime);
                    admin.Patronymic = model.AdminPatronymic;

                    client.AddUser(admin);

                    // добавление первого набора услуг и первой услуги в набор
                    client.CreateInitialServiceSet(client, rate, extraUserCount, extraTeamCount, extraStorageCount, 
                        extraAccountOrganizationCount, extraGigabyteCount, currentDateTime);

                    uow.Commit();

                    // создание БД клиента
                    CreateClientDatabase(client, currentDateTime);

                    // отправка письма о регистрации
                    SendRegistrationLetter(client, admin, model.AdminPassword);

                    return Content(client.Id.ToString());
                }
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult RegisterPhysicalPerson(PhysicalPersonRegistrationViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var firstError = ModelState.Values.Where(x => x.Errors.Any()).FirstOrDefault();

                    ValidationUtils.IsNull(firstError, firstError.Errors.FirstOrDefault().ErrorMessage);
                }

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var registrationAddressCityId = ValidationUtils.TryGetShort(model.RegistrationAddressCityId, "Неверное значение входного параметра.");
                var postalAddressCityId = ValidationUtils.TryGetShort(model.PostalAddressCityId, "Неверное значение входного параметра.");
                var rateId = ValidationUtils.TryGetShort(model.RateId, "Неверное значение входного параметра.");

                var extraUserCount = ValidationUtils.TryGetShort(model.ExtraUserCount);
                var extraTeamCount = ValidationUtils.TryGetShort(model.ExtraTeamCount);
                var extraStorageCount = ValidationUtils.TryGetShort(model.ExtraStorageCount);
                var extraAccountOrganizationCount = ValidationUtils.TryGetShort(model.ExtraAccountOrganizationCount);
                var extraGigabyteCount = ValidationUtils.TryGetShort(model.ExtraGigabyteCount);

                using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
                {
                    var registrationAddressCity = cityRepository.GetById(registrationAddressCityId);
                    ValidationUtils.NotNull(registrationAddressCity, "Город не найден.");

                    var registrationAddress = new Address(registrationAddressCity, model.RegistrationAddressPostalIndex, model.RegistrationAddressLocalAddress);

                    var rate = rateRepository.GetById(rateId);
                    ValidationUtils.NotNull(rate, "Тариф не найден.");

                    Address postalAddress = null;

                    if (model.PostalAddressEqualsRegistration)
                    {
                        postalAddress = new Address(registrationAddress.City, registrationAddress.PostalIndex, registrationAddress.LocalAddress);
                    }
                    else
                    {
                        var postalAddressCity = cityRepository.GetById(postalAddressCityId);
                        ValidationUtils.NotNull(postalAddressCityId, "Город не найден.");

                        postalAddress = new Address(postalAddressCity, model.PostalAddressPostalIndex, model.PostalAddressLocalAddress);
                    }

                    var client = new PhysicalPerson(model.LastName, model.FirstName, model.Patronymic, model.INNIP, registrationAddress, postalAddress, currentDateTime);

                    client.AdminEmail = model.AdminEmail;
                    client.DBServerName = AppSettings.ClientDBServerName;
                    client.OGRNIP = model.OGRNIP;
                    client.Phone = model.Phone;
                    client.PromoCode = model.PromoCode;

                    clientRepository.Save(client);

                    client.DBName = "bizpulse_" + client.Number.ToString();

                    var admin = new ClientUser(model.AdminLastName, model.AdminFirstName, model.AdminLogin, CryptographyUtils.ComputeHash(model.AdminPassword),
                        true, currentDateTime);
                    admin.Patronymic = model.AdminPatronymic;

                    client.AddUser(admin);

                    // добавление первого набора услуг и первой услуги в набор
                    client.CreateInitialServiceSet(client, rate, extraUserCount, extraTeamCount, extraStorageCount,
                        extraAccountOrganizationCount, extraGigabyteCount, currentDateTime);

                    uow.Commit();

                    // создание БД клиента
                    CreateClientDatabase(client, currentDateTime);

                    // отправка письма о регистрации
                    SendRegistrationLetter(client, admin, model.AdminPassword);

                    return Content(client.Id.ToString());
                }
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        
        #endregion

        #region Быстрая регистрация

        public ActionResult RegisterEasy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterEasy(FreeClientRegistrationViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var firstError = ModelState.Values.Where(x => x.Errors.Any()).FirstOrDefault();

                    ValidationUtils.IsNull(firstError, firstError.Errors.FirstOrDefault().ErrorMessage);
                }

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
                {
                    var rate = rateRepository.GetById(5 /* тариф «Тестовый период»*/);
                    ValidationUtils.NotNull(rate, "Тариф не найден.");
                    
                    var client = new Client(currentDateTime);
                    client.AdminEmail = model.AdminEmail;
                    client.DBServerName = AppSettings.ClientDBServerName;
                    client.Phone = model.Phone;
                    client.PromoCode = model.PromoCode;

                    clientRepository.Save(client);

                    client.DBName = "bizpulse_" + client.Number.ToString();

                    var admin = new ClientUser(model.AdminLastName, model.AdminFirstName, model.AdminLogin, CryptographyUtils.ComputeHash(model.AdminPassword),
                        true, currentDateTime);

                    client.AddUser(admin);

                    // добавление первого набора услуг и первой услуги в набор
                    client.CreateInitialStandardServiceSet(client, rate, currentDateTime);

                    uow.Commit();

                    // создание БД клиента
                    CreateClientDatabase(client, currentDateTime);

                    // отправка письма о регистрации
                    SendRegistrationLetter(client, admin, model.AdminPassword);

                    return Content(client.Id.ToString());
                }                
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Общие методы

        /// <summary>
        /// Создание БД для аккаунта клиента
        /// </summary>
        /// <param name="client">Клиент системы</param>
        private void CreateClientDatabase(Client client, DateTime currentDateTime)
        {
            // создание сессии для БД master
            ISessionFactory sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString("Data Source=" + client.DBServerName + ";Initial Catalog=master;Integrated Security=True"))
                .BuildSessionFactory();

            // создание пустой БД (находясь в контексте БД master)
            using (ISession session = sessionFactory.OpenSession())
            {
                // создание клиентской БД
                session.CreateSQLQuery("CREATE DATABASE " + client.DBName).ExecuteUpdate();

                var dbTemplateName = "bizpulse_template";

                // восстановление шаблона БД на клиентскую БД
                var restoreScriptTemplate = string.Format("RESTORE DATABASE {0} FROM  DISK = '{1}' WITH  FILE = 1, " +
                    "MOVE '{2}' TO '{3}.mdf', MOVE '{2}_log' TO '{3}_log.ldf',  NOUNLOAD,  REPLACE,  STATS = 10",
                    client.DBName,
                    Path.Combine(AppSettings.ClientDBTemplatePath, dbTemplateName + ".bak"),
                    dbTemplateName,
                    Path.Combine(AppSettings.ClientDBPath, client.DBName));

                session.CreateSQLQuery(restoreScriptTemplate).ExecuteUpdate();
            }

            // переключаем контекст на новую БД
            sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString("Data Source=" + client.DBServerName + ";Initial Catalog=" + client.DBName + ";Integrated Security=True"))
                .BuildSessionFactory();

            // обновляем информацию в новой БД для конкретного клиента
            using (ISession session = sessionFactory.OpenSession())
            {
                var admin = client.Users.First();

                // обновление для сотрудника
                var updateEmployeeScriptTemplate = string.Format(
                    "UPDATE Employee SET LastName = '{0}', FirstName = '{1}', Patronymic = '{2}', CreationDate = {3} WHERE Id = 1",
                    admin.LastName, admin.FirstName, admin.Patronymic, DateTimeUtils.ConvertToSqlDate(currentDateTime));

                // обновление для пользователя
                var updateUserScriptTemplate = string.Format(
                    "UPDATE [User] SET DisplayName = '{0}', DisplayNameTemplate = '{1}', Login = '{2}', PasswordHash = '{3}', CreationDate = {4} WHERE Id = 1",
                    admin.FirstName + " " + admin.LastName, "FL", admin.Login, admin.PasswordHash, DateTimeUtils.ConvertToSqlDate(currentDateTime));

                // пока единственный набор
                var currentServiceSet = client.ServiceSets.First();

                // обновление максимального кол-ва сущностей
                var updateEntityCountLimitScriptTemplate = string.Format(
                    "UPDATE [Setting] SET ActiveUserCountLimit = '{0}', TeamCountLimit = '{1}', StorageCountLimit = '{2}', AccountOrganizationCountLimit = '{3}', GigabyteCountLimit = '{4}'",
                    currentServiceSet.Configuration.TotalActiveUserCountLimit, currentServiceSet.Configuration.TotalTeamCountLimit,
                    currentServiceSet.Configuration.TotalStorageCountLimit, currentServiceSet.Configuration.TotalAccountOrganizationCountLimit,
                    currentServiceSet.Configuration.TotalGigabyteCountLimit);

                session.CreateSQLQuery(updateEmployeeScriptTemplate + " " + updateUserScriptTemplate + " " + updateEntityCountLimitScriptTemplate).ExecuteUpdate();
            }
        } 

        /// <summary>
        /// Отправка письма о регистрации
        /// </summary>
        /// <param name="client">Аккаунт клиента</param>
        /// <param name="admin">Администратор аккаунта клиента</param>
        /// <param name="adminPassword">Пароль администратора аккаунта клиента</param>
        private void SendRegistrationLetter(Client client, ClientUser admin, string adminPassword)
        {
            // получение шаблона письма для восстановления пароля
            StreamReader reader = new StreamReader(Request.MapPath("~/Templates/Email/RegistrationLetter.html"));
            string body = reader.ReadToEnd();
            reader.Close();

            // формирование тела письма
            body = string.Format(body, admin.FirstName, client.Number, admin.Login, adminPassword);

            var mailer = new Mailer(AppSettings.InfoEMail, AppSettings.SenderName, AppSettings.SmtpServer, AppSettings.SmtpLogin, AppSettings.SmtpPassword);
            mailer.SendMail(client.AdminEmail, "Регистрация в Электронном менеджере Bizpulse", body);
        } 
        
        #endregion 
        
        #endregion

        #region Детали аккаунта клиента

        [NeedClientAdministratorAuthorization]
        public ActionResult Details()
        {
            try
            {
                using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
                {
                    var client = сlientService.CheckClientExistence(UserSession.CurrentClientAdministratorInfo.ClientAccountId);

                    var model = new ClientDetailsViewModel()
                    {
                        MainDetails = GetMainDetails(client),
                        DisplayName = client.DisplayName,
                        InvoiceForPaymentGrid = GetInvoiceForPaymentGridLocal(new GridState() { PageSize = 5 }),
                        PaymentGrid = GetPaymentGridLocal(new GridState() { PageSize = 5 }),
                        CertificateOfCompletionGrid = GetCertificateOfCompletionGridLocal(new GridState() { PageSize = 5 })
                    };

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        private ClientMainDetailsViewModel GetMainDetails(Client client)
        {
            var currentServiceSet = client.ServiceSets.FirstOrDefault(x => x.IsActivated);
            
            var model = new ClientMainDetailsViewModel()
            {
                ConfigurationName = (currentServiceSet != null ? currentServiceSet.Configuration.Name : ""),
                CreationDate = client.CreationDate.ToShortDateString(),
                PrepaymentSum = client.PrepaymentSum.ForDisplay(),
                PaidPeriodEnd = (currentServiceSet == null || currentServiceSet.EndDate == null ? "" : currentServiceSet.EndDate.Value.ToShortDateString())
            };

            return model;
        }

        #region Счета на оплату
        
        [HttpPost]
        [NeedClientAdministratorAuthorization]
        public ActionResult ShowInvoiceForPaymentGrid(GridState state)
        {
            try
            {
                using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
                {
                    return PartialView("InvoiceForPaymentGrid", GetInvoiceForPaymentGridLocal(state));
                }
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        private GridData GetInvoiceForPaymentGridLocal(GridState state)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            var client = сlientService.CheckClientExistence(UserSession.CurrentClientAdministratorInfo.ClientAccountId);

            GridData model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("Number", "Номер", Unit.Percentage(40));
            model.AddColumn("Date", "Дата", Unit.Pixel(60));
            model.AddColumn("Summa", "Сумма (руб.)", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("State", "Статус", Unit.Percentage(60));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.State = state;

            return model;
        }
 
        #endregion

        #region Оплаты

        [HttpPost]
        [NeedClientAdministratorAuthorization]
        public ActionResult ShowPaymentGrid(GridState state)
        {
            try
            {
                using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
                {
                    return PartialView("PaymentGrid", GetPaymentGridLocal(state));
                }
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        private GridData GetPaymentGridLocal(GridState state)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            var client = сlientService.CheckClientExistence(UserSession.CurrentClientAdministratorInfo.ClientAccountId);

            GridData model = new GridData();
            model.AddColumn("Number", "Номер платежного документа", Unit.Percentage(40));
            model.AddColumn("Date", "Дата", Unit.Pixel(60));
            model.AddColumn("Summa", "Сумма оплаты (руб.)", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Purpose", "Назначение платежа", Unit.Percentage(60));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.State = state;

            return model;
        } 

        #endregion

        #region Акты выполненных работ

        [HttpPost]
        [NeedClientAdministratorAuthorization]
        public ActionResult ShowCertificateOfCompletionGrid(GridState state)
        {
            try
            {
                using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
                {
                    return PartialView("CertificateOfCompletionGrid", GetCertificateOfCompletionGridLocal(state));
                }
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        private GridData GetCertificateOfCompletionGridLocal(GridState state)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            var client = сlientService.CheckClientExistence(UserSession.CurrentClientAdministratorInfo.ClientAccountId);

            GridData model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("Number", "Номер документа", Unit.Percentage(40));
            model.AddColumn("Date", "Дата", Unit.Pixel(60));
            model.AddColumn("Summa", "Стоимость работ (руб.)", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Period", "Период выполнения работ", Unit.Percentage(60));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.State = state;

            return model;
        } 

        #endregion

        #endregion

        #endregion

    }
}
