<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Producer_ProducerPaymentsGrid.Init();
</script>

<%= Html.GridHeader("Оплаты", "gridProducerPayments", "/Help/GetHelp_Producer_Details_ProducerPaymentsGrid")%>    
<%= Html.GridContent(Model, "/Producer/ShowPaymentsGrid/")%>