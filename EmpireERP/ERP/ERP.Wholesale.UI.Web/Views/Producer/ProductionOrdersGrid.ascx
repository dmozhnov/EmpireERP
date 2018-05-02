<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    $(document).ready(function () {
        var currentUrl = $("#currentUrl").val();
        $("#gridProductionOrders table.grid_table tr").each(function (i, el) {
            var id = $(this).find(".Id").text();
            $(this).find("a.Name").attr("href", "/ProductionOrder/Details?id=" + id + "&backURL=" + currentUrl);
        });
    });
</script>

<%= Html.GridHeader("Заказы на производство", "gridProductionOrders", "/Help/GetHelp_Producer_Details_ProductionOrdersGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateProductionOrder", "Новый заказ", Model.ButtonPermissions["AllowToAddProductionOrder"], Model.ButtonPermissions["AllowToAddProductionOrder"])%>
    </div>
<%= Html.GridContent(Model, "/Producer/ShowProductionOrdersGrid/")%>