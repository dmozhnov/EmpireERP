<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Deal_ReturnFromClientGrid.Init();
</script>

<%= Html.GridHeader("Возвраты по сделке", "gridReturnFromClient", "/Help/GetHelp_Deal_Details_ReturnFromClientGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateReturnFromClientWaybill", "Новый возврат", Model.ButtonPermissions["AllowToCreateReturnFromClientWaybill"], Model.ButtonPermissions["IsPossibilityToCreateReturnFromClientWaybill"])%>        
    </div>
<%= Html.GridContent(Model, "/Deal/ShowReturnFromClientGrid") %>
