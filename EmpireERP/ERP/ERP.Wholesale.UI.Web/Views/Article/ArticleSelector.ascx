<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Article.ArticleSelectViewModel>" %>

<link href="../../Content/Style/Treeview.css" rel="stylesheet" type="text/css" />
<script type="text/javascript">
    Article_Selector.Init();
</script>

<div style="width: 810px; padding: 10px 10px 0;">
    <%= Html.GridFilterHelper("filterArticle", Model.FilterData,
        new List<string>() { "gridSelectArticle" }, true) %>
    
    <div id="messageArticleSelectList"></div>    

    <% if(Model.AllowToSelectSources) { %><%: Html.CheckBoxFor(x => x.SelectSources) %> <%: Html.LabelFor(x => x.SelectSources) %> <br /><br /><% } %>

    <div id="grid_article_select" style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("ArticleSelectGrid", Model.Data); %>
    </div>

    <div class="button_set">            
         <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>

    <div id="articleGroupFilterSelector"></div>
</div>