<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>


<%= Html.GridHeader("Выбор фабрики-изготовителя", "gridManufacturerSelectorList", "/Help/GetHelp_Manufacturer_Select_ManufacturerGrid")%>
<%= Html.GridContent(Model, "/Manufacturer/ShowManufacturerSelectorGrid/") %>