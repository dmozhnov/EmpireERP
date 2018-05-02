<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader(Model.Title, "gridDealDebitInitialBalanceCorrectionDistribution", "/Help/GetHelp_DealPaymentFromClient_Details_DealDebitInitialBalanceCorrectionGrid")%>
<%= Html.GridContent(Model, "/DealPaymentDocument/ShowDealDebitInitialBalanceCorrectionGrid/")%>
