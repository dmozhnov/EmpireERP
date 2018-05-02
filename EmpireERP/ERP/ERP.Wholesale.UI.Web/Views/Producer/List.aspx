<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Producer.ProducerListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Производители
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("Producer", "Производители", "", "/Help/GetHelp_Producer_List")%>

    <div id="messageProducerList"></div>

    <%= Html.GridFilterHelper("filterProducer", Model.Filter,
        new List<string>() { "gridProducers"})%>

    <% Html.RenderPartial("ProducersGrid", Model.ProducersGrid); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
