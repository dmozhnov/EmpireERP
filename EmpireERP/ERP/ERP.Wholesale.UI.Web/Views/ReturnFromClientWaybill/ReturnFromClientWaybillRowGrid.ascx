<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ReturnFromClientWaybill_RowGrid.Init();
</script>

<%= Html.GridHeader("Позиции накладной", "gridReturnFromClientWaybillRows", "/Help/GetHelp_ReturnFromClientWaybill_Details_ReturnFromClientWaybillRowGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateRow", "Добавить", Model.ButtonPermissions["AllowToCreateRow"], Model.ButtonPermissions["AllowToCreateRow"])%>
    </div>
<%= Html.GridContent(Model, "/ReturnFromClientWaybill/ShowReturnFromClientWaybillRowGrid/", false)%>