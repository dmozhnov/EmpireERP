<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    $(document).ready(function () {
        $("#gridSelectArticleSale table.grid_table tr").each(function (i, el) {
            var id = $(this).find(".SaleWaybillId").text();
            $(this).find("a.SaleWaybillName").attr("href", "/ExpenditureWaybill/Details?id=" + id + GetBackUrl());
        });
    });
</script>

<%= Html.GridHeader("Список реализаций", "gridSelectArticleSale")%>
<%= Html.GridContent(Model, "/Article/ShowArticleSaleSelectGrid/")%>   