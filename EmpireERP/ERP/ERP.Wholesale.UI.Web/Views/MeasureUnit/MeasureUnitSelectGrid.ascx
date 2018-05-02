<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%=Html.GridHeader("Выбор единицы измерения", "gridMeasureUnit", "/Help/GetHelp_MeasureUnit_Select_MeasureUnitGrid")%>
<%=Html.GridContent(Model, "/MeasureUnit/ShowSelectMeasureUnitGrid") %>
