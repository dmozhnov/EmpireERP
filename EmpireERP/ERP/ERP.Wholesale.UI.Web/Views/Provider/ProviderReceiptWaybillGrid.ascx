<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Provider_ReceiptWaybillGrid.Init();
</script>

<%= Html.GridHeader("Закупки у поставщика", "gridReceiptWaybill", "/Help/GetHelp_Provider_Details_ReceiptWaybillGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateReceiptWaybill", "Создать новую накладную", 
            Model.ButtonPermissions["AllowToCreateReceiptWaybill"], Model.ButtonPermissions["AllowToCreateReceiptWaybill"]) %>
    </div>
<%= Html.GridContent(Model, "/Provider/ShowReceiptWaybillGrid/")%>
