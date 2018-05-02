<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.PrintingForm.Common.ExpenditureWaybillPrintingFormSettingsViewModel>" %>

<script type="text/javascript">
    $(document).ready(function () {
        $("#btnPrint").click(function () {
            var actionUrl = $("#ActionUrl").val();
            actionUrl = actionUrl + '?WaybillId=' + $("#Id").val();
           
            if ($('#ConsiderReturns:checked').val() != undefined) {
                actionUrl = actionUrl + "&ConsiderReturns=" + $('#ConsiderReturns:checked').val();
            }

            window.open(actionUrl);
            
            HideModal();
        });
    });
</script>

<%:Html.HiddenFor(model => model.ActionUrl) %>

<div class="modal_title">Настройки печатной формы</div>
<div class="h_delim"></div>

<div style="padding: 10px 10px 5px">    
    <table class="editor_table">
        <%if (Model.ConsiderReturns.HasValue) { %>
        <tr>
            <td class="row_title">
                <%: Html.LabelFor(x => x.ConsiderReturns)%>:
            </td>
            <td>
                <label><%: Html.RadioButtonFor(x => x.ConsiderReturns, true)%>Да</label>
                <label><%: Html.RadioButtonFor(x => x.ConsiderReturns, false)%>Нет</label>
            </td>
        </tr>
        <%} %>
    </table>
</div>

<div class="button_set">
    <input type="button" id="btnPrint" value="Печать" />
    <input type="button" value="Закрыть" onclick="HideModal();" />
</div>