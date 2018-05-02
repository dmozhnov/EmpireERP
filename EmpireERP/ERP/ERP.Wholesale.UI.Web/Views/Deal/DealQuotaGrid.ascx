<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Deal_Details_QuotaGrid.Init();
</script>

<%=Html.GridHeader("Квоты по сделке", "gridDealQuota", "/Help/GetHelp_Deal_Details_QuotaGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnAddQuota", "Добавить квоту", Model.ButtonPermissions["AllowToAddQuota"], Model.ButtonPermissions["AllowToAddQuota"])%>
        <%: Html.Button("btnAddAllQuotas", "Добавить все квоты", Model.ButtonPermissions["AllowToAddQuota"], Model.ButtonPermissions["AllowToAddQuota"])%>
    </div>
<%= Html.GridContent(Model, "/Deal/ShowQuotaGrid/") %>

