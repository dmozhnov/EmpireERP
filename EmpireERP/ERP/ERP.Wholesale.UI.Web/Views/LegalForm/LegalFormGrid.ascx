<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    LegalForm_LegalFormGrid.Init();   
</script>

<%= Html.GridHeader("Организационно-правовые формы", "gridLegalForm", "/Help/GetHelp_LegalForm_List_LegalFormGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateLegalForm", "Новая организационно-правовая форма", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%= Html.GridContent(Model, "/LegalForm/ShowLegalFormGrid/")%>