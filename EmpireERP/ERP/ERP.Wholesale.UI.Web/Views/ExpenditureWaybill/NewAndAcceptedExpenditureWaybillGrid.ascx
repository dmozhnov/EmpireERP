<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ExpenditureWaybill_NewAndAcceptedGrid.Init();
</script>

<%= Html.GridHeader("Новые и проведенные накладные", "gridNewAndAcceptedExpenditureWaybill", "/Help/GetHelp_ExpenditureWaybill_List_NewAndAcceptedExpenditureWaybillGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateExpenditureWaybill", "Новая накладная", Model.ButtonPermissions["AllowToCreateExpenditureWaybill"], Model.ButtonPermissions["AllowToCreateExpenditureWaybill"])%>
    </div>
<%= Html.GridContent(Model, "/ExpenditureWaybill/ShowNewAndAcceptedExpenditureWaybillGrid/")%>