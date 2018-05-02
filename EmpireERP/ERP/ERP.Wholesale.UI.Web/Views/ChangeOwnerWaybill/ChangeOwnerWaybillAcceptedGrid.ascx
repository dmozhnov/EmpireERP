<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ChangeOwnerWaybill_List_AcceptedGrid.Init();
</script>

<%=Html.GridHeader("Принятые накладные", "gridChangeOwnerWaybillAcceptedWaybill") %>
<%=Html.GridContent(Model,"/ChangeOwnerWaybill/ShowChangeOwnerWaybillAcceptedGrid") %>

