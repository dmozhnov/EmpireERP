<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Deal_List_ClosedDealGrid.Init();
</script>

<%=Html.GridHeader(Model.Title, "gridClosedDeal", "/Help/GetHelp_Deal_List_ClosedDealGrid")%>
<%=Html.GridContent(Model, "/Deal/ShowClosedDealGrid/")%>

