<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    OutgoingWaybillRow_IncomingWaybillRowGrid.Init();
</script>

<%= Html.GridHeader(Model.Title, "gridIncomingWaybillRow")%>
<%= Html.GridContent(Model, Model.GridPartialViewAction)%>