<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Устаревшие товары", "gridObsoleteArticles", "/Help/GetHelp_Article_List_ObsoleteArticleGrid")%>
<%= Html.GridContent(Model, "/Article/ShowObsoleteArticleGrid/")%>   
