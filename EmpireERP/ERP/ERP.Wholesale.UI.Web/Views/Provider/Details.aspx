<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Provider.ProviderDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали поставщика
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Provider_Details.Init();

        function OnFailContractEdit(ajaxContext) {
            Provider_Details.OnFailContractEdit(ajaxContext);
        }

        function OnSuccessContractEdit(result) {
            Provider_Details.OnSuccessContractEdit(result);
        }

        function OnSuccessEconomicAgentTypeSelect(ajaxContext) {
            Provider_Details.OnSuccessEconomicAgentTypeSelect(ajaxContext);
        }

        function OnSuccessOrganizationEdit(result) {
            Provider_Details.OnSuccessOrganizationEdit(result);
        }

        function OnAccountOrganizationSelectLinkClick(accountOrganizationId, accountOrganizationShortName) {
            Provider_Details.OnAccountOrganizationSelectLinkClick(accountOrganizationId, accountOrganizationShortName);
        }

        function OnContractorOrganizationSelectLinkClick(organizationId, organizationShortName) {
            Provider_Details.OnContractorOrganizationSelectLinkClick(organizationId, organizationShortName);
        }

        function RefreshMainDetails(details) {
            Provider_Details.RefreshMainDetails(details);
        }

        $("#btnCreateNewTask").live("click", function () {
            Task_NewTaskGrid.CreateNewTaskByContractor($("#Id").val());
        });   
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model => model.MainDetails.Id) %>
    <%: Html.HiddenFor(model => model.BackURL) %>

    <%= Html.PageTitle("Provider", "Детали поставщика", Model.MainDetails.Name, "/Help/GetHelp_Provider_Details")%>

    <div class="button_set">
        <%: Html.Button("btnEditProvider", "Редактировать", Model.AllowToEdit, Model.AllowToEdit) %>
        <%: Html.Button("btnDeleteProvider", "Удалить", Model.AllowToDelete, Model.AllowToDelete)%>
        <input type="button" id="btnBack" value="Назад" />
    </div>

    <div id="messageProviderDetails"></div>
    <% Html.RenderPartial("ProviderMainDetails", Model.MainDetails); %>

    <br />

    <% Html.RenderPartial("~/Views/Task/NewTaskGrid.ascx", Model.TaskGrid);%>

    <% if(Model.AllowToViewReceiptWaybillList) { %>
        <% Html.RenderPartial("ProviderReceiptWaybillGrid", Model.ReceiptWaybillGrid); %>
    <% } %>

    <div id="messageProviderOrganizationList"></div>
    <% if(Model.AllowToViewProviderOrganizationList) { %>
        <% Html.RenderPartial("ProviderOrganizationGrid", Model.ProviderOrganizationGrid); %>
    <% } %>
    
    <div id="messageContractList"></div>
    <% Html.RenderPartial("ProviderContractGrid", Model.ProviderContractGrid); %>

    <div id="accountOrganizationSelector"></div>
    <div id="contractorOrganizationSelector"></div>
    <div id="providerContractEdit"></div>
    <div id="economicAgentEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>
