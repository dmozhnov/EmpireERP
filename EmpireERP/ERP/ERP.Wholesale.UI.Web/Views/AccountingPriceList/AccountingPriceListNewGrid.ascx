<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    AccountingPriceList_List_NewGrid.Init();
</script>

<%= Html.GridHeader("Новые документы", "gridNewAccountingPriceList") %>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateAccountingPriceListRevaluation", "Новый реестр цен", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"]) %>        
    </div>
<%= Html.GridContent(Model, "/AccountingPriceList/ShowNewAccountingPriceListGrid/") %>
