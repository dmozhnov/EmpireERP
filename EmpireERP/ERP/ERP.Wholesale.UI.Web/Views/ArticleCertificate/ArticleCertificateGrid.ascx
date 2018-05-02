<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ArticleCertificate_ArticleCertificateGrid.Init();   
</script>

<%= Html.GridHeader("Сертификаты товаров", "gridArticleCertificates", "/Help/GetHelp_ArticleCertificate_List_ArticleCertificateGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateArticleCertificate", "Новый сертификат", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%= Html.GridContent(Model, "/ArticleCertificate/ShowArticleCertificateGrid/")%>