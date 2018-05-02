<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Bank_List_RussianBankGrid.Init();

    function OnSuccessRussianBankEdit(ajaxContext) {
        Bank_List_RussianBankGrid.OnSuccessRussianBankEdit(ajaxContext);
    }
</script>

<%= Html.GridHeader("Российские банки", "gridRussianBank", "/Help/GetHelp_Bank_List_RussianBankGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateRussianBank", "Новый банк", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>
    </div>
<%= Html.GridContent(Model, "/Bank/ShowRussianBankGrid") %>