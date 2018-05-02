<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Report.Report0006.Report0006PrintingFormSettingsViewModel>" %>

<script src="/Scripts/DatePicker.min.js" type="text/javascript"></script>
<script type="text/javascript" src="/Scripts/DatePicker.js"></script>

<script type="text/javascript">
    Report0006PrintingForm_Settings.Init();
</script>

<div class="modal_title">Настройки печатной формы<%: Html.Help("/Help/GetHelp_Report_Report0006") %></div>
<div class="h_delim"></div>

<div style="padding: 10px 10px 5px">
    <div id="messageReport0006PrintingFormSettings"></div>

    <%: Html.HiddenFor(model => model.PrintingFormClientId)%>
    <%: Html.HiddenFor(model => model.PrintingFormClientOrganizationId)%>

    <table class="editor_table">
        <tr>
            <td class="row_title">
                Даты формирования акта
                <%: Html.LabelFor(model => model.StartDate)%>:
            </td>
            <td style="text-align: left;">
                <%= Html.DatePickerFor(model => model.StartDate)%>
                <%: Html.ValidationMessageFor(model => model.StartDate)%>
            </td>
            <td class="row_title">
                <%: Html.LabelFor(model => model.EndDate)%>:
            </td>
            <td style="text-align: left;">
                <%= Html.DatePickerFor(model => model.EndDate)%>
                <%: Html.ValidationMessageFor(model => model.EndDate)%>
            </td>
        </tr>
    </table>
</div>

<div class="button_set">
    <input type="button" id="btnPrint" value="Печать" />
    <input type="button" value="Закрыть" onclick="HideModal();" />
</div>
