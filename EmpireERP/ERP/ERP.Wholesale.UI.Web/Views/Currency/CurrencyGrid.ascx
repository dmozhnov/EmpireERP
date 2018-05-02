<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Currency_ListGrid.Init();
</script>

<%= Html.GridHeader("Валюты", "gridCurrency", "/Help/GetHelp_Currency_List_CurrencyGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateCurrency", "Новая валюта", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%= Html.GridContent(Model, "/Currency/ShowCurrencyGrid") %>