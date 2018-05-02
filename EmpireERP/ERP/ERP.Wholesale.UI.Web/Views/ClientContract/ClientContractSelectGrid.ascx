<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ClientContract_SelectGrid.Init();
</script>

<%= Html.GridHeader(Model.Title, "gridSelectClientContract")%>
<%= Html.GridContent(Model, "/ClientContract/ShowSelectGrid")%>