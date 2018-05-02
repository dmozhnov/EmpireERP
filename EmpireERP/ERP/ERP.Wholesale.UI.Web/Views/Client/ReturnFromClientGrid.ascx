<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Client_ReturnFromClientGrid.Init();
</script>

<%= Html.GridHeader("Возвраты от клиента", "gridReturnFromClient", "/Help/GetHelp_Client_Details_ReturnFromClientGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateReturnFromClientWaybill", "Новый возврат", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"]) %>        
    </div>
<%= Html.GridContent(Model, "/Client/ShowReturnFromClientGrid") %>
