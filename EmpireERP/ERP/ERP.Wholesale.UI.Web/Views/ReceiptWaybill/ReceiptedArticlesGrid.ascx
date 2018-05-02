<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ReceiptWaybill_ReceiptedArticlesGrid.Init();
</script>

<%= Html.GridHeader("Пришедшие товары", "gridReceipt") %>
    <div class="grid_buttons">
        <input id="btnAddArticle" type="button" value="Добавить другой товар" />    
    </div>
<%= Html.GridContent(Model, "/Receiptwaybill/ShowReceiptArticlesGrid/", false)%>