<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    MeasureUnit_MeasureUnitGrid.Init();   
</script>

<%= Html.GridHeader("Единицы измерения", "gridMeasureUnits", "/Help/GetHelp_MeasureUnit_List_MeasureUnitGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateMeasureUnit", "Новая ЕИ", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%= Html.GridContent(Model, "/MeasureUnit/ShowMeasureUnitGrid/")%>