<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Article.ArticleListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Товары
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/Content/Style/Treeview.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript">
        Article_List.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("Article", "Товары", "", "/Help/GetHelp_Article_List")%>
    
    <div id="messageArticleList"></div>

    <%= Html.GridFilterHelper("filterArticle", Model.FilterData,
                new List<string>() { "gridActualArticles", "gridObsoleteArticles" })%>
    
	<div id="messageActualArticleList"></div>    	
    <% Html.RenderPartial("ActualArticleGrid", Model.ActualArticleGrid); %>

	<div id="messageObsoleteArticleList"></div>    	
    <% Html.RenderPartial("ObsoleteArticleGrid", Model.ObsoleteArticleGrid); %>

    <div id="articleEdit"></div> 
    <div id="articleGroupFilterSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>
