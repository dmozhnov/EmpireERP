<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ClientType_ClientTypeGrid.Init();   
</script>

<%= Html.GridHeader("Типы клиентов", "gridClientType", "/Help/GetHelp_ClientType_List_ClientTypeGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateClientType", "Новый тип клиента", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%= Html.GridContent(Model, "/ClientType/ShowClientTypeGrid/")%>