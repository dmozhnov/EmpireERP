<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    AccountingPriceList_ArticlesGrid.Init();
</script>

<%= Html.GridHeader("Состав - товары", "gridAccountingPriceArticles") %>
    <div class="grid_buttons">        
        <%= Html.Button("btnAddArticle", "Добавить", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>
        <%= Html.Button("btnAddArticleAccountingPriceSet", "Добавить набор товаров", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>
   </div>
<%= Html.GridContent(Model, "ShowAccountingPriceArticlesGrid")%>

