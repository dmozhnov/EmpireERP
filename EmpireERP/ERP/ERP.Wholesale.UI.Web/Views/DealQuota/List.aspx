<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.DealQuota.DealQuotaListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Квоты
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        DealQuota_List.Init();      
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%=Html.PageTitle("DealQuota", "Квоты", "", "/Help/GetHelp_DealQuota_List")%>

    <div id="messageDealQuotaList"></div>

    <%=Html.GridFilterHelper("filterDealQuota", Model.Filter, new List<string>() { "gridActiveDealQuota", "gridInactiveDealQuota" })%>
    
    <div id="messageActiveDealQuotaList"></div>        
    <% Html.RenderPartial("ActiveDealQuotaGrid", Model.ActiveDealQuotaGrid); %>

    <div id="messageInactiveDealQuotaList"></div>
    <% Html.RenderPartial("InactiveDealQuotaGrid", Model.InactiveDealQuotaGrid); %>

    <div id="dealQuotaEdit"></div> 
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
