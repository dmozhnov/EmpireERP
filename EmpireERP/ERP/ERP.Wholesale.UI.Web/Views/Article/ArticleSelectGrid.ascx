<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Выбор товара", "gridSelectArticle", "/Help/GetHelp_Article_Select_ArticleSelectGrid")%>
<%= Html.GridContent(Model, "/Article/ShowArticleSelectGrid/")%>   
 
