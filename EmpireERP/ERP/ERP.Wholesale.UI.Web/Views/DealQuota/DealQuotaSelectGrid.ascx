<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Выбор квоты", "gridDealQuotaSelect", "/Help/GetHelp_DealQuota_Select_DealQuotaSelectGrid")%>
<%= Html.GridContent(Model, "/DealQuota/ShowDealQuotaSelectGrid/")%>   
 
