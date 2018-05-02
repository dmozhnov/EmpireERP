<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12.TORG12PrintingFormSettingsViewModel>" %>

<script type="text/javascript">
    $(document).ready(function () {
        $("#btnPrint").click(function () {
            if (ValidateParameters()) {
                window.open(CreateActionURLParameters($("#ActionUrl").val()));
                HideModal();
            }
        });

        $('#btnExportToExcel').click(function () {
            if (ValidateParameters()) {
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
            }
        });

        function CreateActionURLParameters(actionUrl) {
            var url = actionUrl + '?PriceTypeId=' + $("#PriceTypeId option:selected").val() + '&WaybillId=' + $("#Id").val();

            if ($('#ConsiderReturns:checked').val() != undefined) {
                url = url + "&ConsiderReturns=" + $('#ConsiderReturns:checked').val();
            }

            if ($('#ClientOrganizationId').val() != "" && $('#ClientOrganizationId').val() != undefined) {
                url = url + "&ClientOrganizationId=" + $('#ClientOrganizationId').val();
            }
            return url;
        }

        function ValidateParameters() {
            if ($('#ClientOrganizationId').val() == "") {
                $('#ClientOrganizationId').ValidationError("Укажите организацию контрагента");
                return false;
            }
            return true;
        }

        $('#clientOrganizationName').click(function () {
            StartLinkProgress($(this));

            $.ajax({
                type: "GET",
                url: "/Client/SelectAllClientOrganization/",
                success: function (result) {
                    $("#contractorOrganizationSelector").hide().html(result);
                    $.validator.unobtrusive.parse($("#contractorOrganizationSelector"));
                    ShowModal("contractorOrganizationSelector");
                    StopLinkProgress($(this));
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    alert(XMLHttpRequest.responseText);
                    StopLinkProgress($('#clientOrganizationName'));
                }
            });
        });
    });

    function OnContractorOrganizationSelectLinkClick(organizationId, organizationShortName) {        
        HideModal(function () {
            $('#ClientOrganizationId').val(organizationId);
            $('#clientOrganizationName').text(organizationShortName);
        });
    }
</script>

<%:Html.HiddenFor(model => model.ActionUrl) %>
<%:Html.HiddenFor(model => model.ExportToExcelUrl) %>


<div class="modal_title">Настройки печатной формы</div>
<div class="h_delim"></div>

<div style="padding: 10px 10px 5px; min-width: 330px;">    
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

        <%if(Model.UseClientOrganization) { %>
        <tr>
            <td class="row_title">
                <%: Html.LabelFor(x => x.ClientOrganizationId)%>:
            </td>
            <td>
                <span class="select_link no_auto_progress" id="clientOrganizationName">Выберите организацию контрагента</span>
                <%: Html.HiddenFor(model => model.ClientOrganizationId) %>
                <span class="field-validation-valid" data-valmsg-for="ClientOrganizationId" data-valmsg-replace="true"></span>
            </td>
        </tr>
        <%} %>
    </table>
</div>

<div class="button_set" style="padding: 0px 10px 5px 0px">
    <input type="button" id="btnPrint" value="Печатать" />
    <input type="button" id="btnExportToExcel" value="Выгрузить в Excel" />
    <input type="button" value="Закрыть" onclick="HideModal();" />
</div>

<div id="contractorOrganizationSelector"></div>