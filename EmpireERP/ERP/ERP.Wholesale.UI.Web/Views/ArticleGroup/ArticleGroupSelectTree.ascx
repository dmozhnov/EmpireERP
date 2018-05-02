<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.TreeView.TreeData>" %>

<%= Html.TreeHeader("Группы товаров", "treeArticleGroups") %>
<%= Html.TreeContent(Model)%>
