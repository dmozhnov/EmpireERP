<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Пришедшие товары", "gridApproveArticles") %>
<%= Html.GridContent(Model, "/ReceiptWaybill/ShowReceiptWaybillApprovementArticlesGrid/", false) %>
