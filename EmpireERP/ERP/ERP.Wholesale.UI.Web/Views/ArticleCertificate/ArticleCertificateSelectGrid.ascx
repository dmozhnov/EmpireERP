<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%=Html.GridHeader("Выбор сертификата товара", "gridArticleCertificate", "/Help/GetHelp_ArticleCertificate_Select_ArticleCertificateGrid")%>
<%=Html.GridContent(Model, "/ArticleCertificate/ShowArticleCertificateSelectGrid")%>
