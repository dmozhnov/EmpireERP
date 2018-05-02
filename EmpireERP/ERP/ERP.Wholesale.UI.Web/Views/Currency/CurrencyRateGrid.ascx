<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    CurrencyRate_Grid.Init();
</script>

<%= Html.GridHeader("Курсы валют", "gridCurrencyRate", "/Help/GetHelp_Currency_Edit_CurrencyRateGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateCurrencyRate", "Новый курс", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
        <input id="btnImportRate" type="button" value="Импортировать" class="hidden"/>    
    </div>
<%= Html.GridContent(Model, "/Currency/ShowCurrencyRateGrid") %>