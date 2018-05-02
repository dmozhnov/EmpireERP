<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%=Html.GridHeader("Счета в российских банках", "gridRussianBankAccounts", "/Help/GetHelp_Organization_List_RussianBankAccountGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnAddRussianBankAccount", "Новый счет", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"]) %>
    </div>
<%=Html.GridContent(Model, Model.GridPartialViewAction)%>