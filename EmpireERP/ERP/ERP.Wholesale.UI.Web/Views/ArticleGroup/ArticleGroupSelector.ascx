<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.TreeView.TreeData>" %>

<script type="text/javascript">
    ArticleGroup_Selector.Init();
</script>

<div style="width: 800px; padding: 10px 10px 0;">    
    <% Html.RenderPartial("ArticleGroupSelectTree", Model); %>        
            
    <div class="attention" style="margin-left: 8px;">Товар можно добавить только в группы второго уровня.</div>

    <div class="button_set">            
         <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>