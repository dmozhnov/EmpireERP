<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Bank.ForeignBankEditViewModel>" %>

<script type="text/javascript">
    $(document).ready(function () {
        $("#EditForeignBank #Name").focus();
        SetValidationRegExp($("#ClearingCodeType").val());

        $("#ClearingCodeType").change(function () {
            var val = $(this).val();
            $("#ClearingCode").val("");

            SetValidationRegExp(val);
        });
    });

    function SetValidationRegExp(val) {
        $("#ClearingCode").removeAttr("disabled").ValidationValid();

        switch (val) {
            case "1":
            case "2":
                $("#ClearingCode").attr("data-val-regex-pattern", "((.){9})?").attr("maxlength", "9").attr("data-val-regex", "Укажите счет длиной 9 символов");
                break;
            case "3":
                $("#ClearingCode").attr("data-val-regex-pattern", "((.){6})?([^^]{4})?").attr("maxlength", "6").attr("data-val-regex", "Укажите счет длиной 4 или 6 символов");
                break;
            case "4":
                $("#ClearingCode").attr("data-val-regex-pattern", "((.){8})?").attr("maxlength", "8").attr("data-val-regex", "Укажите счет длиной 8 символов");
                break;
            case "5":
            case "6":
                $("#ClearingCode").attr("data-val-regex-pattern", "((.){6})?").attr("maxlength", "6").attr("data-val-regex", "Укажите счет длиной 6 символов");
                break;
            case "7":
                $("#ClearingCode").attr("data-val-regex-pattern", "((.){9})?([^^]{3})?").attr("maxlength", "9").attr("data-val-regex", "Укажите счет длиной 3 или 9 символов");
                break;
            default:
                $("#ClearingCode").attr("disabled", "disabled");
                break;
        }

        $.validator.unobtrusive.parse($("#EditForeignBank"));
    }

    function OnFailForeignBankEdit(ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageForeignBankEdit");
    }

    function OnBeginForeignBankEdit() {
        StartButtonProgress($("#btnSaveForeignBank"));
    }
</script>

<% using (Ajax.BeginForm("EditForeignBank", "Bank", new AjaxOptions() { OnBegin = "OnBeginForeignBankEdit",
       OnSuccess = "OnSuccessForeignBankEdit", OnFailure = "OnFailForeignBankEdit" }))%>
<%{ %>
    <%: Html.HiddenFor(model => model.Id) %>

    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Bank_Edit_ForeignBank") %></div>
    <div class="h_delim"></div>
        
    <div id="EditForeignBank" style="padding: 10px 10px 5px">
        <div id="messageForeignBankEdit"></div>  

        <table class='editor_table'>
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.Name) %>:
                </td>
                <td>
                   <%: Html.TextBoxFor(model => model.Name, new { style = "width: 320px;", maxlength = "250" }, !Model.AllowToEdit)%>
                   <%: Html.ValidationMessageFor(model => model.Name) %>
                </td>                   
            </tr>
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.Address) %>:
                </td>
                <td>
                   <%: Html.TextBoxFor(model => model.Address, new { style = "width: 320px;", maxlength = "250" }, !Model.AllowToEdit)%>
                   <%: Html.ValidationMessageFor(model => model.Address) %>
                </td>                   
            </tr>
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.SWIFT) %>:
                </td>
                <td>
                   <%: Html.TextBoxFor(model => model.SWIFT, new { style = "width: 320px;", maxlength = "11" }, !Model.AllowToEdit)%>
                   <%: Html.ValidationMessageFor(model => model.SWIFT)%>
                </td>                   
            </tr>
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.ClearingCodeType) %>:
                </td>
                <td>
                   <%: Html.DropDownListFor(x => x.ClearingCodeType, Model.ClearingCodeTypeList, !Model.AllowToEdit)%>
                   <%: Html.ValidationMessageFor(model => model.ClearingCodeType)%>
                </td>                   
            </tr>
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.ClearingCode) %>:
                </td>
                <td>
                   <%: Html.TextBoxFor(x => x.ClearingCode, new { style = "width: 320px;", maxlength = "9" }, !Model.AllowToEdit)%>
                   <%: Html.ValidationMessageFor(model => model.ClearingCode)%>
                </td>                   
            </tr>
        </table>

        <div class="button_set">
            <%: Html.SubmitButton("btnSaveForeignBank", "Сохранить", Model.AllowToEdit, Model.AllowToEdit)%>
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>

    </div>
<%} %>






