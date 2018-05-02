<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Team_DealsGrid.Init();
</script>

<%= Html.GridHeader("Область видимости - сделки", "gridDeals", "/Help/GetHelp_Team_Details_DealsGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnAddDeal", "Добавить сделку в область видимости", Model.ButtonPermissions["AllowToAddDeal"], Model.ButtonPermissions["AllowToAddDeal"])%>
    </div>
<%= Html.GridContent(Model, "/Team/ShowDealsGrid/")%>