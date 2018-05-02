<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Country_CountryGrid.Init();   
</script>

<%= Html.GridHeader("Страны", "gridCountry", "/Help/GetHelp_Country_List_CountryGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateCountry", "Новая cтрана", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%= Html.GridContent(Model, "/Country/ShowCountryGrid/")%>