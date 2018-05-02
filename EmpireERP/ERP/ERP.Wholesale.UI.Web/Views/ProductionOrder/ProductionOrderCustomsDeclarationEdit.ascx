<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderCustomsDeclarationEditViewModel>" %>

<script src="/Scripts/DatePicker.min.js" type="text/javascript"></script>
<script type="text/javascript" src="/Scripts/DatePicker.js"></script>

<script type="text/javascript">

    function OnBeginProductionOrderCustomsDeclarationEdit() {
        StartButtonProgress($("#btnProductionOrderCustomsDeclarationSave"));
    }
</script>

<div style="width:670px;">

<% using (Ajax.BeginForm("SaveProductionOrderCustomsDeclaration", "ProductionOrder",
       new AjaxOptions()
       {
           OnBegin = "OnBeginProductionOrderCustomsDeclarationEdit",
           OnFailure = "OnFailProductionOrderCustomsDeclarationEdit",
           OnSuccess = "OnSuccessProductionOrderCustomsDeclarationEdit"
       }))%>
<%{ %>
    <%:Html.HiddenFor(model => model.CustomsDeclarationId)%>
    <%:Html.HiddenFor(model => model.ProductionOrderId)%>

    <div class="modal_title"><%:Model.Title%><%: Html.Help("/Help/GetHelp_ProductionOrder_Edit_ProductionOrderCustomsDeclaration") %></div>
    <div class="h_delim"></div>

    <div style='padding: 10px 10px 5px;'>
        <div id="messageProductionOrderCustomsDeclarationEdit"></div>

        <table class="editor_table">
            <tr>
                <td class="row_title" style="width: 80px"><%:Html.LabelFor(model => model.ProductionOrderName)%>:</td>
                <td style="width: 380px">
                    <%: Model.ProductionOrderName%>
                </td>
                <td class="row_title" style="width: 60px"><%:Html.LabelFor(model => model.CustomsDeclarationDate)%>:</td>
                <td style="width: 90px">
                    <%= Html.DatePickerFor(model => model.CustomsDeclarationDate, isDisabled: !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.CustomsDeclarationDate)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.Name)%>:</td>
                <td colspan="3">
                    <%: Html.TextBoxFor(model => model.Name, new { style = "width:540px", maxlength = "200" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.Name)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.CustomsDeclarationNumber)%>:</td>
                <td colspan="3">
                    <%: Html.TextBoxFor(model => model.CustomsDeclarationNumber, new { style = "width:230px", maxlength = "33" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.CustomsDeclarationNumber)%>
                </td>
            </tr>
        </table>

        <div class="h_delim"></div>
        <br />

        <table class="editor_table">
            <tr>
                <td class="row_title" style="width: 170px"><%:Html.LabelFor(model => model.ImportCustomsDutiesSum)%>:</td>
                <td style="width: 125px">
                    <%: Html.NumericTextBoxFor(model => model.ImportCustomsDutiesSum, new { maxlength = 19, size = 14 }, !Model.AllowToEdit)%>&nbsp;р.
                    <%: Html.ValidationMessageFor(model => model.ImportCustomsDutiesSum)%>
                </td>
                <td class="row_title" style="width: 190px"><%:Html.LabelFor(model => model.ExciseSum)%>:</td>
                <td style="width: 125px">
                    <%: Html.NumericTextBoxFor(model => model.ExciseSum, new { maxlength = 19, size = 14 }, !Model.AllowToEdit)%>&nbsp;р.
                    <%: Html.ValidationMessageFor(model => model.ExciseSum)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.ExportCustomsDutiesSum)%>:</td>
                <td>
                    <%: Html.NumericTextBoxFor(model => model.ExportCustomsDutiesSum, new { maxlength = 19, size = 14 }, !Model.AllowToEdit)%>&nbsp;р.
                    <%: Html.ValidationMessageFor(model => model.ExportCustomsDutiesSum)%>
                </td>
                <td class="row_title"><%:Html.LabelFor(model => model.CustomsFeesSum)%>:</td>
                <td>
                    <%: Html.NumericTextBoxFor(model => model.CustomsFeesSum, new { maxlength = 19, size = 14 }, !Model.AllowToEdit)%>&nbsp;р.
                    <%: Html.ValidationMessageFor(model => model.CustomsFeesSum)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.ValueAddedTaxSum)%>:</td>
                <td>
                    <%: Html.NumericTextBoxFor(model => model.ValueAddedTaxSum, new { maxlength = 19, size = 14 }, !Model.AllowToEdit)%>&nbsp;р.
                    <%: Html.ValidationMessageFor(model => model.ValueAddedTaxSum)%>
                </td>
                <td class="row_title"><%:Html.LabelFor(model => model.CustomsValueCorrection)%>:</td>
                <td>
                    <%: Html.NumericTextBoxFor(model => model.CustomsValueCorrection, new { maxlength = 19, size = 14 }, !Model.AllowToEdit)%>&nbsp;р.
                    <%: Html.ValidationMessageFor(model => model.CustomsValueCorrection)%>
                </td>
            </tr>
        </table>

        <div class="h_delim"></div>
        <br />

        <table class="editor_table">
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.PaymentSum)%>:</td>
                <td>
                    <%: Model.PaymentSum%>&nbsp;р.
                </td>
                <td class="row_title"><%:Html.LabelFor(model => model.PaymentPercent)%>:</td>
                <td>
                    <%: Model.PaymentPercent%>&nbsp;%
                </td>
            </tr>
            <tr>
                <td class="greytext" colspan="4"<%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:</td>
            </tr>
            <tr>
                <td colspan="4">                        
                    <%:Html.CommentFor(model => model.Comment, new { style = "width: 98%" }, !Model.AllowToEdit, rowsCount: 3)%>  
                    <%:Html.ValidationMessageFor(model => model.Comment)%>                  
                </td>
            </tr>
        </table>

        <div class="button_set">
            <%= Html.SubmitButton("btnProductionOrderCustomsDeclarationSave", "Сохранить", Model.AllowToEdit, Model.AllowToEdit)%>
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>
    </div>
<%} %>

</div>
