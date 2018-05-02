<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ExpenditureWaybill_RowGrid.Init();
</script>

<%= Html.GridHeader("Позиции накладной", "gridExpenditureWaybillRows", "/Help/GetHelp_ExpenditureWaybill_Details_ExpenditureWaybillRowGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateExpenditureWaybillRow", "Добавить", Model.ButtonPermissions["AllowToCreateRow"], Model.ButtonPermissions["AllowToCreateRow"])%>
    </div>
<%= Html.GridContent(Model, "/ExpenditureWaybill/ShowExpenditureWaybillRowGrid/", false)%>