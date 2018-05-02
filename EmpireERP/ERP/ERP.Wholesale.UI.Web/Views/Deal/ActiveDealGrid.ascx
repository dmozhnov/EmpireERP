<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Deal_List_ActiveDealGrid.Init();
</script>

<%=Html.GridHeader(Model.Title, "gridActiveDeal", "/Help/GetHelp_Deal_List_ActiveDealGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateDeal", "Новая сделка")%>
    </div>
<%=Html.GridContent(Model, "/Deal/ShowActiveDealGrid/")%>

