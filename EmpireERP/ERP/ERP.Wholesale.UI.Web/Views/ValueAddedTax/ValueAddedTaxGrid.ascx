<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ValueAddedTax_ValueAddedTaxGrid.Init();   
</script>

<%= Html.GridHeader("Ставки НДС", "gridValueAddedTax", "/Help/GetHelp_ValueAddedTax_List_ValueAddedTaxGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateValueAddedTax", "Новая ставка НДС", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%= Html.GridContent(Model, "/ValueAddedTax/ShowValueAddedTaxGrid/")%>