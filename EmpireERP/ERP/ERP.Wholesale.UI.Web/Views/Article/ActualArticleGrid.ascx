<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Актуальные товары", "gridActualArticles", "/Help/GetHelp_Article_List_ActualArticleGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateArticle", "Новый товар", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>
    </div>
<%= Html.GridContent(Model, "/Article/ShowActualArticleGrid/")%>   
 