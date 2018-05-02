<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ProviderOrganization.ProviderOrganizationDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали организации поставщика
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        ProviderOrganization_Details.Init();

        function OnSuccessRussianBankAccountAdd(ajaxContext) {
            ProviderOrganization_Details.OnSuccessRussianBankAccountAdd(ajaxContext);
        }

        function OnSuccessForeignBankAccountAdd(ajaxContext) {
            ProviderOrganization_Details.OnSuccessForeignBankAccountAdd(ajaxContext);
        }

        function OnSuccessRussianBankAccountEdit(ajaxContext) {
            ProviderOrganization_Details.OnSuccessRussianBankAccountEdit(ajaxContext);
        }
        
        function OnSuccessForeignBankAccountEdit(ajaxContext) {
            ProviderOrganization_Details.OnSuccessForeignBankAccountEdit(ajaxContext);
        }

        function OnSuccessProviderOrganizationEdit(ajaxContext) {
            ProviderOrganization_Details.OnSuccessProviderOrganizationEdit(ajaxContext);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%:Html.HiddenFor(x=>x.Id) %>
    <%:Html.HiddenFor(x => x.BackURL) %>

    <%= Html.PageTitle("ProviderOrganization", "Детали организации поставщика", Model.ProviderOrganizationName, "/Help/GetHelp_ProviderOrganization_Details")%>
    
    <div class="button_set">
        <%: Html.Button("btnEditProviderOrganization", "Редактировать", Model.AllowToEdit, Model.AllowToEdit)%>
        <%: Html.Button("btnDeleteProviderOrganization", "Удалить", Model.AllowToDelete, Model.AllowToDelete)%>
        <%: Html.Button("btnBack", "Назад")%>
    </div>

    <div id="messageProviderOrganizationEdit"></div>

    <div id="providerOrganizationMainDetails">
        <% Html.RenderPartial("ProviderOrganizationMainDetails", Model.MainDetails); %>
    </div>
    <br />

    <% Html.RenderPartial("ProviderOrganizationReceiptWaybillGrid", Model.ReceiptWaybillGrid); %>
    <% Html.RenderPartial("ProviderOrganizationProviderContractGrid", Model.ProviderContractGrid); %>
                
    <div id='messageRussianBankAccountList'></div>
    <% Html.RenderPartial("~/Views/Organization/RussianBankAccountGrid.ascx", Model.BankAccountGrid); %>

    <div id='messageForeignBankAccountList'></div>
    <% Html.RenderPartial("~/Views/Organization/ForeignBankAccountGrid.ascx", Model.ForeignBankAccountGrid); %>

    <div id="RussianBankAccountEdit"></div>
    <div id="ForeignBankAccountEdit"></div>
    <div id="organizationEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>
