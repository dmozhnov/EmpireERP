<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    WriteoffWaybill_RowGrid.Init();
</script>

 <%= Html.GridHeader("Позиции накладной", "gridWriteoffWaybillRows")%>
    <div class="grid_buttons">
        <%: Html.Button("btnAddWriteoffWaybillRow", "Добавить", Model.ButtonPermissions["AllowToAddRow"], Model.ButtonPermissions["AllowToAddRow"])  %>        
    </div>
<%= Html.GridContent(Model, "/WriteoffWaybill/ShowWriteoffWaybillRowGrid/", false)%>