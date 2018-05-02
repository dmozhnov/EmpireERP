<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Список партий товара", "gridSelectArticleBatch")%>
<%= Html.GridContent(Model, "/Article/ShowArticleBatchSelectGrid/")%>   