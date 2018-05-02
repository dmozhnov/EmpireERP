<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ReturnFromClientReason_ReturnFromClientReasonGrid.Init();   
</script>

<%= Html.GridHeader("Основания для возврата товара от клиента", "gridReturnFromClientReason", "/Help/GetHelp_ReturnFromClientReason_List_ReturnFromClientReasonGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateReturnFromClientReason", "Новое основание для возврата товара от клиента", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%= Html.GridContent(Model, "/ReturnFromClientReason/ShowReturnFromClientReasonGrid/")%>