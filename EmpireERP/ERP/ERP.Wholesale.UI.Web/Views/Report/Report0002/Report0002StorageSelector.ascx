<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Report.Report0002.Report0002StorageSelectorViewModel>" %>

<%= Html.MultipleSelector("multipleSelectorStorages", Model.Storages, "Список доступных мест хранения", "Выбранные места хранения для отчета")  %>

