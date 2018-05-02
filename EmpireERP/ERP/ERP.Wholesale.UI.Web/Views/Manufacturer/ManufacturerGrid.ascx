<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Manufacturer_ManufacturerGrid.Init();   
</script>

<%= Html.GridHeader("Фабрики-изготовители", "gridManufacturer", "/Help/GetHelp_Manufacturer_List_ManufacturerGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateManufacturer", "Новая фабрика-изготовитель", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%= Html.GridContent(Model, "/Manufacturer/ShowManufacturerGrid/")%>