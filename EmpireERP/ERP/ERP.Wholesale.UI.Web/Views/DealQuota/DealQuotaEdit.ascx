<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.DealQuota.DealQuotaEditViewModel>" %>

<script type="text/javascript">
    DealQuota_Edit.Init();

    function OnBeginSaveDealQuota() {
        StartButtonProgress($("#btnSaveQuota"));
    }
</script>

<% using (Ajax.BeginForm("Save", "DealQuota", new AjaxOptions() { OnBegin = "OnBeginSaveDealQuota",
       OnFailure = "DealQuota_Edit.OnFailSaveDealQuota", OnSuccess = "DealQuota_List.OnSuccessSaveDealQuota" }))%>
<%{ %>
    <%:Html.HiddenFor(x => x.Id)%>
    <%:Html.HiddenFor(x => x.AllowToEdit) %>
    <%:Html.HiddenFor(x => x.IsPrepayment) %>

    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_DealQuota_Edit") %></div>
    <div class="h_delim"></div>

    <div style="padding: 10px 30px 5px">
        <div id='messageDealQuotaEdit'></div>

        <table class='editor_table'>
            <tr>
                <td class="row_title" style="width: 160px"><%:Html.LabelFor(x => x.Name)%>:</td>
                <td>
                    <%:Html.TextBoxFor(x => x.Name, new { maxlength = 200, size = 40 }, !Model.IsPossibilityToEdit)%>
                    <%:Html.ValidationMessageFor(x => x.Name)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(x => x.StartDate) %>:</td>
                <td>
                    <%:Model.StartDate%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(x => x.EndDate) %>:</td>
                <td>
                    <%:Model.EndDate%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(x => x.DiscountPercent) %>:</td>
                <td>
                    <%:Html.TextBoxFor(x => x.DiscountPercent, new { maxlength = 5, size = 10 }, !Model.AllowToEdit)%> %
                    <%:Html.ValidationMessageFor(x => x.DiscountPercent)%>
                </td>
            </tr>            
        </table>
            
        <table class="editor_table" style="margin-left:20px">
             <tr>
                <td class="row_title" colspan="2" style="text-align:left">
                    <%:Html.RadioButtonFor(x => x.IsPrepayment, 0, new { id = "rbIsPrepayment_false" })%>
                    <label for="rbIsPrepayment_false"><%:Model.IsPrepayment_false %></label>
                </td>
             </tr>
             <tr>
                <td class="row_title"><%:Html.LabelFor(x => x.PostPaymentDays) %>:</td>
                <td>
                    <span id="PostPaymentGroup">
                      <%:Html.NumericTextBoxFor(x => x.PostPaymentDays, new { size = 10 }, !Model.AllowToEdit)%>&nbsp;дн.
                       <%:Html.ValidationMessageFor(x => x.PostPaymentDays)%>
                    </span>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(x => x.CreditLimitSum)%>:</td>
                <td>
                 <span id="CreditLimitSumGroup">
                       <%:Html.NumericTextBoxFor(x => x.CreditLimitSum, new { maxlength = 20, size = 10 }, !Model.AllowToEdit)%>&nbsp;р.
                     <%:Html.ValidationMessageFor(x => x.CreditLimitSum)%>
                  </span>
                </td>
            </tr>
            <tr>
                <td class="row_title" colspan="2" style="text-align:left">
                    <%:Html.RadioButtonFor(x => x.IsPrepayment, 1, new { id = "rbIsPrepayment_true" })%>
                    <label for="rbIsPrepayment_true"><%:Model.IsPrepayment_true %></label>
                </td>
            </tr>
        </table>

        <br />

        <table class="editor_table" >
            <tr>
                <td class="row_title" style="width: 160px">
                    <%: Html.LabelFor(x => x.IsActive) %>:
                </td>
                <td>
                    <%:Html.YesNoToggleFor(x => x.IsActive, Model.IsPossibilityToEdit) %>
                </td>
             </tr>
        </table>

        <br />

        <div class='button_set'>
            <%: Html.SubmitButton("btnSaveQuota", "Сохранить", Model.IsPossibilityToEdit, Model.IsPossibilityToEdit) %>            
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>
    </div>
<%} %>