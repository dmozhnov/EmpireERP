<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ChangeOwnerWaybill_List_NewGrid.Init();
</script>

<%=Html.GridHeader("Новые накладные", "gridChangeOwnerWaybillNewWaybill") %>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateChangeOwnerWaybill", "Новая накладная", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"]) %>
    </div>
<%=Html.GridContent(Model,"/ChangeOwnerWaybill/ShowChangeOwnerWaybillNewGrid") %>