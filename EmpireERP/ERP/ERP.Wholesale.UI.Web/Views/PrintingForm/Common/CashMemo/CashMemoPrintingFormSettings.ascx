<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.PrintingForm.Common.CashMemoPrintingFormSettingsViewModel>" %>

<script type="text/javascript">
    $(document).ready(function () {
        $("#btnPrint").click(function () {
            window.open('/MovementWaybill/ShowCashMemoPrintingForm?PriceTypeId=' + $("#PriceTypeId option:selected").val() +
             '&WaybillId=' + $("#Id").val());
            HideModal();
        });
    });
</script>

<div class="modal_title">Настройки печатной формы</div>
<div class="h_delim"></div>

<div style="padding: 10px 10px 5px">    
    <table class="editor_table">
        <tr>
            <td class="row_title">
                <%: Html.LabelFor(x => x.PriceTypeId)%>:
            </td>
            <td>
                <%: Html.DropDownListFor(model => model.PriceTypeId, Model.PriceTypes)%>
            </td>
        </tr>
    </table>
</div>

<div class="button_set">
    <input type="button" id="btnPrint" value="Печать" />
    <input type="button" value="Закрыть" onclick="HideModal();" />
</div>