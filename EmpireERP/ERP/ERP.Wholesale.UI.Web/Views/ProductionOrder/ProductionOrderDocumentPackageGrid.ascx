<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    //ProductionOrder_Details_ProductionOrderDocumentPackageGrid.Init();
    $(document).ready(function () {
        var currentUrl = $("#currentUrl").val();
        $("#gridProductionOrderDocumentPackage table.grid_table tr").each(function (i, el) {
            var id = $(this).find(".Id").text();
            $(this).find("a.Name").attr("href", "/ProductionOrderMaterialsPackage/Details?id=" + id + "&backURL=" + currentUrl);
        });

        $("#btnCreateMaterialsPackage").click(function () {
            window.location = "/ProductionOrderMaterialsPackage/Create?productionOrderId=" + $("#Id").val() + "&backURL=" + $("#currentUrl").val();
        });
    });
</script>

<%= Html.GridHeader("Пакеты материалов по заказу", "gridProductionOrderDocumentPackage", "/Help/GetHelp_ProductionOrder_Details_ProductionOrderDocumentPackageGrid")%>
    <div class="grid_buttons">
        <%=Html.Button("btnCreateMaterialsPackage", "Новый пакет материалов", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%= Html.GridContent(Model, "/ProductionOrder/ShowProductionOrderDocumentPackageGrid/")%>