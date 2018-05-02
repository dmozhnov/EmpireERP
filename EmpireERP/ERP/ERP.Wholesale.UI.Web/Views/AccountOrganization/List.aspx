<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.AccountOrganization.AccountOrganizationListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Собственные организации
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">    
    <script type="text/javascript">
        AccountOrganization_List.Init();

        function OnSuccessAccountOrganizationEdit(ajaxContext) {
            AccountOrganization_List.OnSuccessAccountOrganizationEdit(ajaxContext);
        }

        function OnSuccessEconomicAgentTypeSelect(ajaxContext) {
            AccountOrganization_List.OnSuccessEconomicAgentTypeSelect(ajaxContext);
        }
        </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%=Html.PageTitle("AccountOrganization", "Собственные организации", "", "/Help/GetHelp_AccountOrganization_List")%>

    <div id='messageAccountOrganizationList'></div>
    <% Html.RenderPartial("AccountOrganizationGrid", Model.AccountOrganizationGrid); %>

    <div id="economicAgentEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>
