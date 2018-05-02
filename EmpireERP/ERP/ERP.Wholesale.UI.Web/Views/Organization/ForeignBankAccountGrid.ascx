<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%=Html.GridHeader("Счета в иностранных банках", "gridForeignBankAccounts", "/Help/GetHelp_Organization_List_ForeignBankAccountGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnAddForeignBankAccount", "Новый счет", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%=Html.GridContent(Model, Model.GridPartialViewAction)%>