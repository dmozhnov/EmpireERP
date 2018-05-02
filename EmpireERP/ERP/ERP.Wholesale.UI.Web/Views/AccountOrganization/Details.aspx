<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.AccountOrganization.AccountOrganizationDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали собственной организации
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        AccountOrganization_Details.Init();

        function OnSuccessAccountOrganizationEdit(ajaxContext) {
            AccountOrganization_Details.OnSuccessAccountOrganizationEdit(ajaxContext);
        }

        function OnSuccessRussianBankAccountEdit(ajaxContext) {
            AccountOrganization_Details.OnSuccessRussianBankAccountEdit(ajaxContext);
        }

        function OnSuccessForeignBankAccountEdit(ajaxContext) {
            AccountOrganization_Details.OnSuccessForeignBankAccountEdit(ajaxContext);
        }

        function OnSuccessStorageAdd() {
            AccountOrganization_Details.OnSuccessStorageAdd();
        }            
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">   
    <%:Html.HiddenFor(x => x.AccountOrganizationId) %>
    <%:Html.HiddenFor(x => x.BackURL) %> 

    <%= Html.PageTitle("AccountOrganization", "Детали собственной организации", Model.OrganizationName, "/Help/GetHelp_AccountOrganization_Details")%>
    
    <div class="button_set">
        <%: Html.Button("btnEditAccountOrganization", "Редактировать", Model.AllowToEdit, Model.AllowToEdit) %>
        <%: Html.Button("btnDeleteAccountOrganization", "Удалить", Model.AllowToDelete, Model.AllowToDelete)%>        
        <input id="btnBack" type="button" value="Назад" />
    </div>

    <div id="messageAccountOrganizationEdit"></div>

    <div id="accountOrganizationMainDetails">
        <% Html.RenderPartial("AccountOrganizationMainDetails", Model.MainDetails); %>
    </div>
    
    <br />
                
    <div id='messageRussianBankAccountList'></div>
    <% Html.RenderPartial("~/Views/Organization/RussianBankAccountGrid.ascx", Model.BankAccountGrid); %>
    <div id="accountOrganizationBankAccountDetailsForEdit"></div>
    
    <div id='messageForeignBankAccountList'></div>
    <% Html.RenderPartial("~/Views/Organization/ForeignBankAccountGrid.ascx", Model.ForeignBankAccountGrid); %>
    <div id="accountOrganizationForeignBankAccountDetailsForEdit"></div>

    <div id="messageStorageList"></div>
    <% Html.RenderPartial("StorageGrid", Model.StorageGrid); %>

    <div id="storageSelectList"></div>
    <div id="organizationEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>
