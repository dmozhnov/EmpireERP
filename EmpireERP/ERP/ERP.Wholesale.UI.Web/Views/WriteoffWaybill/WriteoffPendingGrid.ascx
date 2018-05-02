<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    WriteoffWaybill_List_WriteoffPendingGrid.Init();
</script>

<%= Html.GridHeader("Ожидается списание", "gridWriteoffPending")%>
    <div class="grid_buttons">          
        <%: Html.Button("btnCreateWriteoffWaybill", "Новая накладная", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%= Html.GridContent(Model, "/WriteoffWaybill/ShowWriteoffPendingGrid/")%>