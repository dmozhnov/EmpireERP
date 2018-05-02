<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    WriteoffWaybill_List_WrittenoffWaybillGrid.Init();
</script>

<%= Html.GridHeader("Выполненные списания", "gridWrittenoff")%>
<%= Html.GridContent(Model, "/WriteoffWaybill/ShowWrittenOffGrid/")%>