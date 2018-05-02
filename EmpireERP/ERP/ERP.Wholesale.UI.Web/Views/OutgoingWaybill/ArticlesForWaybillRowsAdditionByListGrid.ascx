<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

    <script type="text/javascript">
        Waybill_ForWaybillRowsAdditionByListGrid.BindAddingRowsByList();
    </script>

<%= Html.GridHeader("Доступные товары", "gridArticlesForWaybillRowsAdditionByList", "/Help/GetHelp_OutgoingWaybill_List_ArticlesForWaybillRowsAdditionByListGrid")%>
<%= Html.GridContent(Model, Model.GridPartialViewAction)%>