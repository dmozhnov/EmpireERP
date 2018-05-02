<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    AccountingPriceList_StoragesGrid.Init();
</script>

<%= Html.GridHeader("Распространения - места хранения", "gridAccountingPriceStorages")%>   
    <div class="grid_buttons" style="margin-left: 5px">
        <%: Html.Button("btnAddOne", "Добавить один", Model.ButtonPermissions["AllowToAdd"], Model.ButtonPermissions["AllowToAdd"])%>
        <%: Html.Button("btnAddAll", "Добавить все", Model.ButtonPermissions["AllowToAdd"], Model.ButtonPermissions["AllowToAdd"])%>
        <%: Html.Button("btnAddTradePoint", "Добавить магазины", Model.ButtonPermissions["AllowToAdd"], Model.ButtonPermissions["AllowToAdd"])%>           
    </div>   
<%= Html.GridContent(Model, "ShowAccountingPriceStoragesGrid") %>