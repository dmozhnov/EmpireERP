using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.UI;
using ERP.Infrastructure.IoC;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ReceiptWaybill;
using ERP.Wholesale.UI.Web.Infrastructure;
using Excel;
using ERP.Wholesale.UI.ViewModels.AccountingPriceList;
using ERP.Infrastructure.UnitOfWork;
using ERP.Wholesale.Domain.Repositories;


namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult List()
        {
            /*FileStream stream = System.IO.File.Open("D:\\NAL.xls", FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

            excelReader.IsFirstRowAsColumnNames = true;
            DataSet result = excelReader.AsDataSet();

            excelReader.Close();

            var rows = result.Tables[0].Rows;
                        
            var list = new List<DataRow>();
            
            foreach (DataRow item in rows)
	        {
		        list.Add(item);
	        }

            var sum = list.Sum(x => ValidationUtils.TryGetDecimal(x["Z_CENA"]));

            var storages = IoCContainer.Resolve<IStorageService>().GetList();*/

            #region Заливка реестров
            /*
            var accountingPriceListPresenter = IoCContainer.Resolve<IAccountingPriceListPresenter>();

            #region Реестр цен для МХ с закупочными ценами
                          
            var ap_model1 = new AccountingPriceListEditViewModel();
            ap_model1.Number = "1";
            ap_model1.StartDate = DateTime.Today.ToShortDateString();
            ap_model1.StartTime = "23:00:00";
            ap_model1.AccountingPriceListId = Guid.Empty;
            ap_model1.ReasonId = AccountingPriceListReason.Revaluation.ValueToString();

            ap_model1.StorageIDs = "";

            foreach (var storage in storages.Where(x => x.Name.Contains("ТДД") || x.Name.Contains("МАСТЕР") ||
                x.Name.Contains("Штаб") || x.Name.Contains("Инвентарь") || x.Name.Contains("Утилизация") || x.Name.Contains("Склад Хранение 001")))
            {
                if (ap_model1.StorageIDs != "")
                {
                    ap_model1.StorageIDs += "_";
                }

                ap_model1.StorageIDs += storage.Id;
            }

            ap_model1.AccountingPriceCalcRuleType = (short)AccountingPriceCalcRuleType.ByPurchaseCost;
            ap_model1.PurchaseCostDeterminationRuleType = (short)PurchaseCostDeterminationRuleType.ByAveragePurchasePrice;
            ap_model1.LastDigitCalcRuleType = (short)LastDigitCalcRuleType.LeaveAsIs;

            var ap_result1 = accountingPriceListPresenter.Save(ap_model1, UserSession.CurrentUserInfo);

            Guid ap1Id = new Guid(((KeyValuePair<object, string>)ap_result1).Key.GetType().GetProperty("Id").GetValue(((KeyValuePair<object, string>)ap_result1).Key, null).ToString());

            // позиции реестра
            foreach (string articleId in list.Select(x => x["ArticleId"]).Distinct())
	        {
		        var model = new ArticleAccountingPriceEditViewModel();

                model.AccountingPrice = list.Where(x => x["ArticleId"].ToString() == articleId.ToString()).FirstOrDefault()["Z_Price"].ToString();
                model.AccountingPriceListId = ap1Id;
                model.ArticleId = ValidationUtils.TryGetInt(articleId);
                model.Id = Guid.Empty;

                accountingPriceListPresenter.SaveArticle(model, UserSession.CurrentUserInfo);
	        }

            Thread.Sleep(1000);

            // проводка реестра
            accountingPriceListPresenter.Accept(ap1Id, UserSession.CurrentUserInfo);

            Thread.Sleep(1000);

            #endregion

            #region Реестр цен для Новосибирска с розничными ценами

            var ap_model2 = new AccountingPriceListEditViewModel();
            ap_model2.Number = "2";
            ap_model2.StartDate = DateTime.Today.ToShortDateString();
            ap_model2.StartTime = "23:00:00";
            ap_model2.AccountingPriceListId = Guid.Empty;
            ap_model2.ReasonId = AccountingPriceListReason.Revaluation.ValueToString();

            ap_model2.StorageIDs = "";

            foreach (var storage in storages.Where(x => x.Name.Contains("Новосибирск")))
            {
                if (ap_model2.StorageIDs != "")
                {
                    ap_model2.StorageIDs += "_";
                }

                ap_model2.StorageIDs += storage.Id;
            }

            ap_model2.AccountingPriceCalcRuleType = (short)AccountingPriceCalcRuleType.ByPurchaseCost;
            ap_model2.PurchaseCostDeterminationRuleType = (short)PurchaseCostDeterminationRuleType.ByAveragePurchasePrice;
            ap_model2.LastDigitCalcRuleType = (short)LastDigitCalcRuleType.RoundDecimalsAndLeaveLastDigit;

            var ap_result2 = accountingPriceListPresenter.Save(ap_model2, UserSession.CurrentUserInfo);

            Guid ap2Id = new Guid(((KeyValuePair<object, string>)ap_result2).Key.GetType().GetProperty("Id").GetValue(((KeyValuePair<object, string>)ap_result2).Key, null).ToString());

            // позиции реестра
            foreach (string articleId in list.Select(x => x["ArticleId"]).Distinct())
            {
                var model = new ArticleAccountingPriceEditViewModel();

                model.AccountingPrice = list.Where(x => x["ArticleId"].ToString() == articleId.ToString()).FirstOrDefault()["R_Price"].ToString();
                model.AccountingPriceListId = ap2Id;
                model.ArticleId = ValidationUtils.TryGetInt(articleId);
                model.Id = Guid.Empty;

                accountingPriceListPresenter.SaveArticle(model, UserSession.CurrentUserInfo);
            }

            Thread.Sleep(1000);

            // проводка реестра
            accountingPriceListPresenter.Accept(ap2Id, UserSession.CurrentUserInfo);

            Thread.Sleep(1000);

            #endregion

            #region Реестр цен для МХ с Оптовыми ценами

            var ap_model3 = new AccountingPriceListEditViewModel();
            ap_model3.Number = "3";
            ap_model3.StartDate = DateTime.Today.ToShortDateString();
            ap_model3.StartTime = "23:00:00";
            ap_model3.AccountingPriceListId = Guid.Empty;
            ap_model3.ReasonId = AccountingPriceListReason.Revaluation.ValueToString();

            ap_model3.StorageIDs = "";

            foreach (var storage in storages.Where(x => !x.Name.Contains("ТДД") && !x.Name.Contains("МАСТЕР") &&
                !x.Name.Contains("Штаб") && !x.Name.Contains("Инвентарь") && !x.Name.Contains("Утилизация") && !x.Name.Contains("Склад Хранение 001") &&
                !x.Name.Contains("Новосибирск")))                
            {
                if (ap_model3.StorageIDs != "")
                {
                    ap_model3.StorageIDs += "_";
                }

                ap_model3.StorageIDs += storage.Id;
            }

            ap_model3.AccountingPriceCalcRuleType = (short)AccountingPriceCalcRuleType.ByPurchaseCost;
            ap_model3.PurchaseCostDeterminationRuleType = (short)PurchaseCostDeterminationRuleType.ByAveragePurchasePrice;
            ap_model3.LastDigitCalcRuleType = (short)LastDigitCalcRuleType.RoundDecimalsAndLeaveLastDigit;

            var ap_result3 = accountingPriceListPresenter.Save(ap_model3, UserSession.CurrentUserInfo);

            Guid ap3Id = new Guid(((KeyValuePair<object, string>)ap_result3).Key.GetType().GetProperty("Id").GetValue(((KeyValuePair<object, string>)ap_result3).Key, null).ToString());

            // позиции реестра
            foreach (string articleId in list.Select(x => x["ArticleId"]).Distinct())
            {
                var model = new ArticleAccountingPriceEditViewModel();

                model.AccountingPrice = list.Where(x => x["ArticleId"].ToString() == articleId.ToString()).FirstOrDefault()["O_Price"].ToString();
                model.AccountingPriceListId = ap3Id;
                model.ArticleId = ValidationUtils.TryGetInt(articleId);
                model.Id = Guid.Empty;

                accountingPriceListPresenter.SaveArticle(model, UserSession.CurrentUserInfo);
            }

            Thread.Sleep(1000);

            // проводка реестра
            accountingPriceListPresenter.Accept(ap3Id, UserSession.CurrentUserInfo);

            Thread.Sleep(1000);
            
            #endregion
            */
            #endregion

            #region Заливка приходных накладных
            /*
            var receiptWaybillPresenter = IoCContainer.Resolve<IReceiptWaybillPresenter>();

            ReceiptWaybillEditViewModel rwbEditModel = null;
            ReceiptWaybillRowEditViewModel rowModel;

            int receiptWaybillNumber = 1;

            var storagesToProcess = storages.Where(x => list.Select(y => y["StorageId"].ToString()).Distinct().Contains(x.Id.ToString()));

            foreach (var storage in storagesToProcess)
            {                                                
                for (int i = 1; i <= list.Where(x => x["StorageId"].ToString() == storage.Id.ToString()).Max(x => ValidationUtils.TryGetShort(x["AccountingPriceListNumberByStorage"].ToString())); i++)
			    {
                    var accountOrganizations = IoCContainer.Resolve<IAccountOrganizationService>().GetList();                    
                    var countryNames = new List<string>() { "Турция", "Великобритания", "Индия", "США", "Китай", "Корея", "Россия", "Австрия", "Германия", "Тайвань"};                    
                    var countries = IoCContainer.Resolve<ICountryService>().GetList().Where(x => countryNames.Contains(x.Name));

                    AccountOrganization accountOrganization;
                    if (storage.Name.Contains("Краснодар"))
                    {
                        accountOrganization = accountOrganizations.Where(x => x.ShortName == "ООО «Магнит Регион»").First();
                    }
                    else if (storage.Name.Contains("ТДД") || storage.Name.Contains("МАСТЕР") ||
                        storage.Name.Contains("Штаб") || storage.Name.Contains("Инвентарь") || storage.Name.Contains("Утилизация") ||
                        storage.Name.Contains("Склад В ПУТИ") || storage.Name.Contains("Cклад Брак общий") || storage.Name.Contains("Склад Хранение 001"))
                    {
                        accountOrganization = accountOrganizations.Where(x => x.ShortName == "ООО «ТК «ТДД»").First();
                    }
                    else
                    {
                        accountOrganization = accountOrganizations.Where(x => x.ShortName == "ООО «Магнит»").First();
                    }                                       
                    
                    rwbEditModel = new ReceiptWaybillEditViewModel();
                    rwbEditModel.AccountOrganizationId = accountOrganization.Id;
                    rwbEditModel.AllowToChangeProvider = true;
                    rwbEditModel.AllowToChangeStorageAndOrganization = true;
                    rwbEditModel.AllowToEdit = true;
                    rwbEditModel.AllowToEditProviderDocuments = true;
                    rwbEditModel.ContractId = accountOrganization.Contracts.First().Id;
                    rwbEditModel.CuratorId = "1";
                    rwbEditModel.CustomsDeclarationNumber = "";
                    rwbEditModel.Date = DateTime.Now.ToShortDateString();
                    rwbEditModel.DiscountPercent = "0";
                    rwbEditModel.DiscountSum = "0";
                    rwbEditModel.Id = Guid.Empty;                    
                    rwbEditModel.Number = receiptWaybillNumber.ToString();
                    rwbEditModel.NumberIsUnique = 1;
                    rwbEditModel.ProviderInvoiceNumber = "";
                    rwbEditModel.ProviderNumber = "";
                    rwbEditModel.Comment = "";
                    rwbEditModel.PendingSum = list.Where(x => x["StorageId"].ToString() == storage.Id.ToString() && 
                        x["AccountingPriceListNumberByStorage"].ToString() == i.ToString())
                        .Sum(x => Math.Round(ValidationUtils.TryGetDecimal(x["Z_CENA"].ToString()) * ValidationUtils.TryGetInt(x["KOL"].ToString()), 2)).ForEdit();
                    rwbEditModel.PendingValueAddedTaxId = 2;
                    rwbEditModel.ProviderId = 1;
                    rwbEditModel.ReceiptStorageId = storage.Id;

                    receiptWaybillNumber++;

                    bool b;
                    Guid rwbId = new Guid(receiptWaybillPresenter.Save(rwbEditModel, out b, UserSession.CurrentUserInfo));

                    foreach (var articleId in list.Where(x => x["StorageId"].ToString() == storage.Id.ToString() 
                        && x["AccountingPriceListNumberByStorage"].ToString() == i.ToString()).Select(x => ValidationUtils.TryGetInt(x["ArticleId"].ToString())).Distinct())
                    {
                        var filteredList = list.Where(x => x["StorageId"].ToString() == storage.Id.ToString() && x["ArticleId"].ToString() == articleId.ToString()
                            && x["AccountingPriceListNumberByStorage"].ToString() == i.ToString());
                            
                        rowModel = new ReceiptWaybillRowEditViewModel();
                        rowModel.AllowToEdit = true;
                        rowModel.ArticleId = articleId;
                        rowModel.CustomsDeclarationNumber = filteredList.First()["TAMOG_DEKL"].ToString();
                        rowModel.Id = Guid.Empty;
                        rowModel.ManufacturerId = "1";
                        rowModel.PendingCount = filteredList.Sum(x => ValidationUtils.TryGetInt(x["KOL"].ToString())).ToString();
                        rowModel.PendingSum = filteredList.Sum(x => Math.Round(ValidationUtils.TryGetInt(x["KOL"].ToString()) * ValidationUtils.TryGetDecimal(x["Z_CENA"].ToString()), 2)).ForEdit();
                        rowModel.PendingValueAddedTaxId = 2;
                        rowModel.ProductionCountryId = countries.Where(y => y.Name == filteredList.First()["STRANA"].ToString()).First().Id;
                        rowModel.PurchaseCost = ValidationUtils.TryGetDecimal(filteredList.First()["Z_CENA"].ToString()).ForEdit();
                        rowModel.PendingSumIsChangedLast = "1";
                        rowModel.ReceiptWaybillId = rwbId;

                        receiptWaybillPresenter.SaveRow(rowModel, UserSession.CurrentUserInfo);
                    }

                    Thread.Sleep(3000);

                    receiptWaybillPresenter.Accept(rwbId, UserSession.CurrentUserInfo);

                    Thread.Sleep(3000);

                    receiptWaybillPresenter.GetReceiptionViewModel(rwbId, "", UserSession.CurrentUserInfo);

                    Thread.Sleep(1000);

                    receiptWaybillPresenter.PerformReceiption(rwbId,
                        list.Where(x => x["StorageId"].ToString() == storage.Id.ToString() &&
                        x["AccountingPriceListNumberByStorage"].ToString() == i.ToString())
                        .Sum(x => Math.Round(ValidationUtils.TryGetDecimal(x["Z_CENA"].ToString()) * ValidationUtils.TryGetInt(x["KOL"].ToString()), 2)),
                        UserSession.CurrentUserInfo);
			    }
                
                               
            }
            */
            #endregion


            return View();
        }


        public ActionResult Index()
        {
            /*var unitOfWorkFactory = IoCContainer.Resolve<IUnitOfWorkFactory>();
            var storageRepository = IoCContainer.Resolve<IStorageRepository>();

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var s = storageRepository.GetById(1);
                
                s.Name = "123";
                
                var ss = storageRepository. GetAll();

                foreach (var storage in ss)
                {
                    storage.Type = StorageType.ExtraStorage;
                }
            }*/
            
            /*IAccountingPriceListWaybillTakingService d = IoCContainer.Resolve<IAccountingPriceListWaybillTakingService>();

            Guid apl=new System.Guid("127167f1-9cb1-434d-b180-c77dcec79afd");
            Guid wbr1=new System.Guid("28f14843-b306-4dd5-9ad3-56e225b455a1");
            Guid wbr2=new System.Guid("628f8eb8-0fdf-478e-9320-d99a00e3d11e");
            Guid apl2=new System.Guid("628f8eb8-0fdf-478e-9320-d99a00e3d112");*/





            /*d.AddAccountingPriceListWaybill(apl, WaybillType.ExpenditureWaybill, wbr1, 3);
            d.AddAccountingPriceListWaybill(apl, WaybillType.ExpenditureWaybill, wbr2, 3);

            var t = d.GetAccountingPriceListWaybillList(apl);
            var t2 = d.GetAccountingPriceListWaybillList(apl2);*/
            
            //IIncomingWaybillRowService incomingWaybillRowService=IoCContainer.Resolve<IIncomingWaybillRowService>();
            //incomingWaybillRowService.testMethod();
            //ReceiptWaybill r= new ReceiptWaybill("1", DateTime.Now, new
            //var r = new Producer("123", (byte)1, null, false);

            //IEnumerable<Manufacturer> val = r.Manufacturers as IEnumerable<Manufacturer>;

            //var type = val.GetType();

            //var name_space = type.Namespace;
            //var name = type.Name;

            //var rep = new UserRepository();
            //////rep.GetFilteredList(null, false);

            //var query = rep.Query<ReceiptWaybill>()
            //    .Or(
            

            //var list = rep.GetAll();
            //var perm = rep.GetById(1);
                        
            
            
            /*var unitOfWorkFactory = IoCContainer.Resolve<IUnitOfWorkFactory>();
            var repProvType = new ProviderTypeRepository();
            var repProv = new ProviderRepository();
            var repProvOrg = new ProviderOrganizationRepository();
            var repAccountOrg = new AccountOrganizationRepository();
            var repLegalForm = new LegalFormRepository();

            

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {

            //var prov = repProv.GetById(1);
            //var provOrg = repProvOrg.GetById(3);

            var prov = new Provider("Поставщик", repProvType.GetById(1), ProviderReliability.Medium, 0);

            var provOrg = new ProviderOrganization("Организация поставщика", "Организация поставщика", new JuridicalPerson(repLegalForm.GetById(1)));                        
            prov.AddContractorOrganization(provOrg);

            repProv.Save(prov);

            var provContract = new ProviderContract(repAccountOrg.GetById(1), provOrg, "Договор", "123", DateTime.Now, DateTime.Now);
            prov.AddProviderContract(provContract);

            //repProv.Save(prov);

            uow.Commit();


            }*/

            //var contract = new ProviderContract(repAccountOrg.GetById(1), 

            //var rep1 = new ERP.Wholesale.Domain.NHibernate.Repositories.LegalFormRepository();
            //var lf = rep1.GetById(1);

            /*
            var jp = new JuridicalPerson(lf);
            jp.INN = "123";

            var org = new Organization("название", "название", jp);

            //rep.Save(org);

            ((JuridicalPerson)org.EconomicAgent.Get_Concrete()).INN = "111";
            */

            /*var org = rep.GetById(2);
            var jp = (JuridicalPerson)org.EconomicAgent;

            jp.INN = "111";

            rep.Save(org);*/

            //rep.Save

            /*var unitOfWorkFactory = IoCContainer.Resolve<IUnitOfWorkFactory>();
            var rep = new MeasureUnitRepository();

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                
                var mu = rep.GetById(1);
                mu.FullName = "123";

                var mu1 = rep.GetById(1);

                uow.Commit();

                
            }*/

            //var mu2 = rep.GetById(1);

            /*var rep = new MovementWaybillRepository();
            var rowId = Guid.Parse("3B893306-9DC8-4BAD-AB15-E90CD9D9AF8F");
            //var storageId = (short)1;
            DateTime? dt = DateTime.Now;

            List<short> list = new List<short>() { 1,2,3,4,5 };
            var result = rep.Query<ReceiptWaybill>()
                
                .OrderByAsc(x => x.PendingSum)
                .OrderByAsc(x => x.Date)
                .ToList<ReceiptWaybill>();*/

            
            //var s = rep.Query<MovementWaybillRow>()
            //    .Where(x => x.ReceiptWaybillRow.Id == rowId && x.MovementWaybill.Description != "")
            //    .List<MovementWaybillRow>();

            //var subQ = rep.SubQuery<MovementWaybill>()
            //    .Where(x => x.RecipientStorage.Id == 1)
            //    .Select(x=>x.Id);

            //var s = rep.Query<MovementWaybillRow>()
            //    .Where(x => x.ReceiptWaybillRow.Id == rowId)
            //    .PropertyIn(x=>x.MovementWaybill, subQ)
            //    .ToList<MovementWaybillRow>();

            return View();
        }
    }
}
