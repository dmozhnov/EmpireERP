<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%=Html.GridHeader("Действующие квоты", "gridActiveDealQuota", "/Help/GetHelp_DealQuota_List_ActiveDealQuotaGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreate", "Новая квота", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>
    </div>
<%= Html.GridContent(Model, "/DealQuota/ShowActiveDealQuotaGrid/") %>

