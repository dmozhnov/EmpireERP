<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ContractorOrganization.ContractorOrganizationListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Организации
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%=Html.PageTitle("Organization", "Организации", "", "/Help/GetHelp_ContractorOrganization_List")%>
    
    <%=Html.GridFilterHelper("filterGridContractorOrganization", Model.Filter,
                new List<string>() { "gridContractorOrganization" })%>

    <div id='messageContractorOrganizationList'></div>
    <% Html.RenderPartial("ContractorOrganizationGrid", Model.ContractorOrganizationGrid); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
