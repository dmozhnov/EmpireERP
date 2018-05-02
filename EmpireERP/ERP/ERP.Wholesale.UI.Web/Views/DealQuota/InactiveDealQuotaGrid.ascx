<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Недействующие квоты", "gridInactiveDealQuota", "/Help/GetHelp_DealQuota_List_InactiveDealQuotaGrid")%>    
<%= Html.GridContent(Model, "/DealQuota/ShowInactiveDealQuotaGrid/") %>

