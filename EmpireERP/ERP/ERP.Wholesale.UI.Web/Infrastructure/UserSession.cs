using System;
using System.Web;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Infrastructure.Security;

namespace ERP.Wholesale.UI.Web.Infrastructure
{
    /// <summary>
    /// Класс-обертка вокруг объекта Session
    /// </summary>
    public static class UserSession
    {
        /// <summary>
        /// Раскрыто ли меню дополнительных функций
        /// </summary>
        public static bool IsFeatureMenuExpanded
        {
            get { return (HttpContext.Current.Session["IsFeatureMenuExpanded"] != null ? Convert.ToBoolean(HttpContext.Current.Session["IsFeatureMenuExpanded"]) : false); }
            set { HttpContext.Current.Session["IsFeatureMenuExpanded"] = value; }
        }

        /// <summary>
        /// Название сервера БД, на которой находится БД пользователя 
        /// </summary>
        public static string DBServerName
        {
            get { return (HttpContext.Current.Session["DBServerName"] != null ? HttpContext.Current.Session["DBServerName"].ToString() : ""); }
            set { HttpContext.Current.Session["DBServerName"] = value; }
        }

        /// <summary>
        /// Название БД пользователя 
        /// </summary>
        public static string DBName
        {
            get { return (HttpContext.Current.Session["DBName"] != null ? HttpContext.Current.Session["DBName"].ToString() : ""); }
            set { HttpContext.Current.Session["DBName"] = value; }
        }

        /// <summary>
        /// Информация о текущем пользователе
        /// </summary>
        public static UserInfo CurrentUserInfo
        {
            get { return (HttpContext.Current.Session != null && HttpContext.Current.Session["CurrentUserInfo"] != null ? (UserInfo)HttpContext.Current.Session["CurrentUserInfo"] : null); }
            set { HttpContext.Current.Session["CurrentUserInfo"] = value; }
        }


        /// <summary>
        /// Используется для предотвращения повторного входа при выходе из сайта
        /// </summary>
        public static bool AlreadyEntered
        {
            get { return HttpContext.Current.Session["AlreadyEntered"] == null ? false : bool.Parse(HttpContext.Current.Session["AlreadyEntered"].ToString()); }
            set { HttpContext.Current.Session["AlreadyEntered"] = value; }
        }

        #region Отображение пунктов меню

        #region Товары
        
        public static bool ShowProviderMenu
        {
            get { return HttpContext.Current.Session["ShowProviderMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowProviderMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowProviderMenu"] = value; }
        }

        public static bool ShowReceiptWaybillMenu
        {
            get { return HttpContext.Current.Session["ShowReceiptWaybillMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowReceiptWaybillMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowReceiptWaybillMenu"] = value; }
        }

        public static bool ShowMovementWaybillMenu
        {
            get { return HttpContext.Current.Session["ShowMovementWaybillMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowMovementWaybillMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowMovementWaybillMenu"] = value; }
        }

        public static bool ShowWriteoffWaybillMenu
        {
            get { return HttpContext.Current.Session["ShowWriteoffWaybillMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowWriteoffWaybillMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowWriteoffWaybillMenu"] = value; }
        }

        public static bool ShowAccountingPriceListMenu
        {
            get { return HttpContext.Current.Session["ShowAccountingPriceListMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowAccountingPriceListMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowAccountingPriceListMenu"] = value; }
        }

        public static bool ShowChangeOwnerWaybillMenu
        {
            get { return HttpContext.Current.Session["ShowChangeOwnerWaybillMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowChangeOwnerWaybillMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowChangeOwnerWaybillMenu"] = value; }
        } 

        #endregion

        #region Справочники
        
        public static bool ShowArticleMenu
        {
            get { return HttpContext.Current.Session["ShowArticleMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowArticleMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowArticleMenu"] = value; }
        }

        public static bool ShowStorageMenu
        {
            get { return HttpContext.Current.Session["ShowStorageMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowStorageMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowStorageMenu"] = value; }
        } 
        #endregion

        #region Производство

        public static bool ShowProducerMenu
        {
            get { return HttpContext.Current.Session["ShowProducerMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowProducerMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowProducerMenu"] = value; }
        }

        public static bool ShowProductionOrderMenu
        {
            get { return HttpContext.Current.Session["ShowProductionOrderMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowProductionOrderMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowProductionOrderMenu"] = value; }
        }        

        public static bool ShowProductionOrderPaymentMenu
        {
            get { return HttpContext.Current.Session["ShowProductionOrderPaymentMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowProductionOrderPaymentMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowProductionOrderPaymentMenu"] = value; }
        }

        public static bool ShowProductionOrderTransportSheetMenu
        {
            get { return HttpContext.Current.Session["ShowProductionOrderTransportSheetMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowProductionOrderTransportSheetMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowProductionOrderTransportSheetMenu"] = value; }
        }

        public static bool ShowProductionOrderExtraExpensesSheetMenu
        {
            get { return HttpContext.Current.Session["ShowProductionOrderExtraExpensesSheetMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowProductionOrderExtraExpensesSheetMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowProductionOrderExtraExpensesSheetMenu"] = value; }
        }

        public static bool ShowProductionOrderCustomsDeclarationMenu
        {
            get { return HttpContext.Current.Session["ShowProductionOrderCustomsDeclarationMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowProductionOrderCustomsDeclarationMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowProductionOrderCustomsDeclarationMenu"] = value; }
        }

        public static bool ShowProductionOrderMaterialsPackageMenu
        {
            get { return HttpContext.Current.Session["ShowProductionOrderMaterialsPackageMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowProductionOrderMaterialsPackageMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowProductionOrderMaterialsPackageMenu"] = value; }
        }

        public static bool ShowProductionOrderBatchLifeCycleTemplateMenu
        {
            get { return HttpContext.Current.Session["ShowProductionOrderBatchLifeCycleTemplateMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowProductionOrderBatchLifeCycleTemplateMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowProductionOrderBatchLifeCycleTemplateMenu"] = value; }
        }

        #endregion

        #region Реализации

        public static bool ShowClientMenu
        {
            get { return HttpContext.Current.Session["ShowClientMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowClientMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowClientMenu"] = value; }
        }

        public static bool ShowDealMenu
        {
            get { return HttpContext.Current.Session["ShowDealMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowDealMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowDealMenu"] = value; }
        }

        public static bool ShowExpenditureWaybillMenu
        {
            get { return HttpContext.Current.Session["ShowExpenditureWaybillMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowExpenditureWaybillMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowExpenditureWaybillMenu"] = value; }
        }

        public static bool ShowDealPaymentMenu
        {
            get { return HttpContext.Current.Session["ShowDealPaymentMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowDealPaymentMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowDealPaymentMenu"] = value; }
        }

        public static bool ShowDealInitialBalanceCorrectionMenu
        {
            get { return HttpContext.Current.Session["ShowDealInitialBalanceCorrectionMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowDealInitialBalanceCorrectionMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowDealInitialBalanceCorrectionMenu"] = value; }
        }

        public static bool ShowReturnFromClientWaybillMenu
        {
            get { return HttpContext.Current.Session["ShowReturnFromClientWaybillMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowReturnFromClientWaybillMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowReturnFromClientWaybillMenu"] = value; }
        }

        public static bool ShowDealQuotaMenu
        {
            get { return HttpContext.Current.Session["ShowDealQuotaMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowDealQuotaMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowDealQuotaMenu"] = value; }
        }

        #endregion

        #region Пользователи

        public static bool ShowUserMenu
        {
            get { return HttpContext.Current.Session["ShowUserMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowUserMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowUserMenu"] = value; }
        }

        public static bool ShowTeamMenu
        {
            get { return HttpContext.Current.Session["ShowTeamMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowTeamMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowTeamMenu"] = value; }
        }

        public static bool ShowRoleMenu
        {
            get { return HttpContext.Current.Session["ShowRoleMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowRoleMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowRoleMenu"] = value; }
        }       

        #endregion

        #region Выгрузка в 1С

        public static bool ShowExportTo1CMenu
        {
            get { return HttpContext.Current.Session["ShowExportTo1CMenu"] == null ? false : bool.Parse(HttpContext.Current.Session["ShowExportTo1CMenu"].ToString()); }
            set { HttpContext.Current.Session["ShowExportTo1CMenu"] = value; }
        }    

        #endregion
        
        #endregion
    }
}