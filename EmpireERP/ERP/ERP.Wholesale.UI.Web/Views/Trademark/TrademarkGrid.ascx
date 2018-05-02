<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Trademark_TrademarkGrid.Init();   
</script>

<%= Html.GridHeader("Торговые марки", "gridTrademark", "/Help/GetHelp_Trademark_List_TrademarkGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateTrademark", "Новая торговая марка", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%= Html.GridContent(Model, "/Trademark/ShowTrademarkGrid/")%>