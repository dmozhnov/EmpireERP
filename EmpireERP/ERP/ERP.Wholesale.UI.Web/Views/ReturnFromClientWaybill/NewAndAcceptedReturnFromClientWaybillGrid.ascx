<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ReturnFromClientWaybill_NewAndAcceptedGrid.Init();
</script>

<%= Html.GridHeader("Новые и проведенные накладные", "gridNewAndAcceptedReturnFromClientWaybill", "/Help/GetHelp_ReturnFromClientWaybill_List_NewAndAcceptedExpenditureWaybillGrid")%>
    <div class="grid_buttons">          
        <%: Html.Button("btnCreateReturnFromClientWaybill", "Новая накладная", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"]) %>        
    </div>
<%= Html.GridContent(Model, "/ReturnFromClientWaybill/ShowNewAndAcceptedReturnFromClientWaybillGrid/")%>