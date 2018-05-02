<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.PrintingForm.Common.InvoicePrintingFormSettingsViewModel>" %>

<script type="text/javascript">
    $(document).ready(function () {
        $("#btnPrint").click(function () {
            window.open(CreateActionURLParameters($("#ActionUrl").val()));
            HideModal();
        });

        $('#btnExportToExcel').click(function () {
            var url = CreateActionURLParameters($("#ExportToExcelUrl").val());
            StartButtonProgress($(this));
            $.fileDownload(url, {
                successCallback: function (response) {
                    StopButtonProgress();
                    HideModal();
                },
                failCallback: function (response) {
                    StopButtonProgress();
                    alert("Произошла ошибка при выгрузке отчета: " + response);
                }
            });
        });

        function CreateActionURLParameters(actionUrl) {
            var url = actionUrl + '?PriceTypeId=' + $("#PriceTypeId option:selected").val() + '&WaybillId=' + $("#Id").val();

            if ($('#ConsiderReturns:checked').val() != undefined) {
                url = url + "&ConsiderReturns=" + $('#ConsiderReturns:checked').val();
            }
            return url;
        }
    });
</script>

<%:Html.HiddenFor(model => model.ActionUrl) %>
<%:Html.HiddenFor(model => model.ExportToExcelUrl) %>

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

<div class="button_set" style="padding: 0px 10px 0px 0px">
    <input type="button" id="btnPrint" value="Печать" />
    <input type="button" id="btnExportToExcel" value="Выгрузить в Excel" />
    <input type="button" value="Закрыть" onclick="HideModal();" />
</div>