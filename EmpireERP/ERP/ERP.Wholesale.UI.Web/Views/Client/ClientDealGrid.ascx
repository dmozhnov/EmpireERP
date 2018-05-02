<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Client_DealGrid.Init();
</script>

<%=Html.GridHeader("Сделки", "gridClientDeal", "/Help/GetHelp_Client_Details_DealGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateDeal", "Новая сделка", Model.ButtonPermissions["AllowToCreateDeal"], Model.ButtonPermissions["AllowToCreateDeal"]) %>        
    </div>
<%= Html.GridContent(Model, "/Client/ShowDealGrid/") %>

