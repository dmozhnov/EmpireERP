<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ArticleCertificate.ArticleCertificateListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Сертификаты товаров
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function OnSuccessArticleCertificateSave() {
            ArticleCertificate_List.OnSuccessArticleCertificateSave();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">   
    <%= Html.PageTitle("ArticleCertificate", "Сертификаты товаров", "", "/Help/GetHelp_ArticleCertificate_List")%>

    <%= Html.GridFilterHelper("filterArticleCertificate", Model.Filter, new List<string> { "gridArticleCertificates" })%>

    <div id="messageArticleCertificateList"></div>
    <% Html.RenderPartial("ArticleCertificateGrid", Model.Data); %>

    <div id="articleCertificateEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>

