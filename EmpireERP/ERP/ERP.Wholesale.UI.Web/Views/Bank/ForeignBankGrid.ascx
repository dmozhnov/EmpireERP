<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>


<script type="text/javascript">
    Bank_List_ForeignBankGrid.Init();

    function OnSuccessForeignBankEdit(ajaxContext) {
        Bank_List_ForeignBankGrid.OnSuccessForeignBankEdit(ajaxContext);
    }
</script>

<%= Html.GridHeader("Иностранные банки", "gridForeignBank", "/Help/GetHelp_Bank_List_ForeignBankGrid")%>
    <div class="grid_buttons">
    <%: Html.Button("btnCreateForeignBank", "Новый банк", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"]) %>
    </div>
<%= Html.GridContent(Model, "/Bank/ShowForeignBankGrid") %>