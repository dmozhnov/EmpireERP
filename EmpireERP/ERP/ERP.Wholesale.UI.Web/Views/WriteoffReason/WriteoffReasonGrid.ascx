<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    WriteoffReason_WriteoffReasonGrid.Init();   
</script>

<%= Html.GridHeader("Основания для списания", "gridWriteoffReason", "/Help/GetHelp_WriteoffReason_List_WriteoffReasonGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateWriteoffReason", "Новое основание для списания", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%= Html.GridContent(Model, "/WriteoffReason/ShowWriteoffReasonGrid/")%>