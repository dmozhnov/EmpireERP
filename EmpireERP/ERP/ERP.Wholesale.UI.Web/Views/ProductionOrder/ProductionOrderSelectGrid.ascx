<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    $(document).ready(function () {
        var currentUrl = $("#currentUrl").val();
        $("#gridProductionOrder table.grid_table tr").each(function (i, el) {
            var id = $(this).find(".Id").text();
            $(this).find("a.Name").attr("href", "/ProductionOrder/Details?id=" + id + "&backURL=" + currentUrl);

            var producerId = $(this).find(".ProducerId").text();
            $(this).find("a.ProducerName").attr("href", "/Producer/Details?id=" + producerId + "&backURL=" + currentUrl);
        });
    });
</script>

<%=Html.GridHeader("Активные заказы", "gridProductionOrder", "/Help/GetHelp_ProductionOrder_Select_ProductionOrderSelectGrid")%>
<%=Html.GridContent(Model, "/ProductionOrder/ShowProductionOrderSelectGrid")%>