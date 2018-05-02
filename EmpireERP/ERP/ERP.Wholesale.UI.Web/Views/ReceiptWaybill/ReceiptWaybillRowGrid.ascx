<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Позиции накладной", "gridReceiptWaybillRows")%>
    <div class="grid_buttons">           
        <%: Html.Button("btnAddReceiptWaybillRow", "Добавить", Model.ButtonPermissions["AllowToAddRow"], Model.ButtonPermissions["AllowToAddRow"])%>     
    </div>
<%= Html.GridContent(Model, "/ReceiptWaybill/ShowReceiptWaybillRowGrid/", false)%>