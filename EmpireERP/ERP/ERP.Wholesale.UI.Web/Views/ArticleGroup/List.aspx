<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ArticleGroup.ArticleGroupListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Группы товаров
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../../Content/Style/Treeview.css" rel="stylesheet" type="text/css" />   
    
    <script type="text/javascript">
        ArticleGroup_List.Init();

        function OnSuccessArticleGroupEdit(ajaxContext) {
            ArticleGroup_List.OnSuccessArticleGroupEdit(ajaxContext);
        }
    </script>    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">    
    <%= Html.PageTitle("ArticleGroup", "Группы товаров", "", "/Help/GetHelp_ArticleGroup_List")%>

    <div id="messageArticleGroupList"></div>

    <%= Html.TreeHeader("Группы товаров", "treeArticleGroups", "/Help/GetHelp_ArticleGroup_List_ArticleGroupTree")%>

    <div class="grid_buttons">      
        <%: Html.Button("btnAddToFirstLevel", "Новая группа", Model.AllowToCreate, Model.AllowToCreate) %>           
    </div>

    <%= Html.TreeContent(Model.ArticleGroupTree)%>

    <div id="articleGroupDetails" class="popup"></div>
    <div id="articleGroupDetailsForEdit"></div>    
</asp:Content>  

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">   
</asp:Content>
