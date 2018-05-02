<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Provider.ProviderListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Поставщики
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">       
        function OnSuccessProviderSave(ajaxContext) {
            Provider_List.OnSuccessProviderSave(ajaxContext);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%=Html.PageTitle("Provider", "Поставщики", "", "/Help/GetHelp_Provider_List")%>
    
    <%=Html.GridFilterHelper("filterGridProvider", Model.Filter, new List<string>() { "gridProvider" })%>

    <div id='messageProviderList'></div>

    <% Html.RenderPartial("ProviderGrid", Model.ProviderGrid); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
