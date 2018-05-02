<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.EconomicAgent.PhysicalPersonEditViewModel>" %>

<script src="/Scripts/DatePicker.min.js" type="text/javascript"></script>
<script type="text/javascript" src="/Scripts/DatePicker.js"></script>

<script type="text/javascript">
    EconomicAgent_PhysicalPersonEdit.Init();

    function OnBeginPhysicalPersonEdit() {
        StartButtonProgress($("#btnSavePhysicalPerson"));
    }
</script>

<% using (Ajax.BeginForm(Model.ActionName, Model.ControllerName, new AjaxOptions() { OnFailure = "EconomicAgent_PhysicalPersonEdit.OnFailPhysicalPersonEdit", 
       OnSuccess = Model.SuccessFunctionName, OnBegin = "OnBeginPhysicalPersonEdit" }))%>
<%{ %>
    <%:Html.HiddenFor(x => x.OrganizationId) %>
    <%:Html.HiddenFor(x => x.ContractorId) %>

    <div style="width: 710px;">
        <div class='modal_title'><%:Model.Title %><%: Html.Help("/Help/GetHelp_EconomicAgentType_Edit_PhysicalPerson") %></div>
        <div class='h_delim'></div>
        
        <div style='padding: 10px 10px 5px'>
            <div id="messageOrganizationEdit"></div>
        
            <table class='editor_table'>
                <tr>
                    <td class='row_title'><%:Html.LabelFor(x => x.ShortName)%>:</td>
                    <td colspan="5">
                        <%: Html.TextBoxFor(x => x.ShortName, new { maxlength = 100, style = "width: 520px" })%>
                        <%: Html.ValidationMessageFor(model => model.ShortName)%>
                    </td>                
                </tr>
                <tr>
                    <td class='row_title'><%:Html.LabelFor(x => x.FullName)%>:</td>
                    <td colspan="5">
                        <%: Html.TextBoxFor(x => x.FullName, new { maxlength = 250, style = "width: 520px" })%>
                        <%: Html.ValidationMessageFor(model => model.FullName)%>
                    </td>                
                </tr>
                <tr>
                    <td class='row_title'><%:Html.LabelFor(x => x.LegalFormId)%>:</td>
                    <td>
                        <%: Html.DropDownListFor(x => x.LegalFormId, Model.LegalFormList, new { style = "width:100px" })%>
                        <%: Html.ValidationMessageFor(model => model.LegalFormId)%>
                    </td>
                    <td class='row_title'><%:Html.LabelFor(x => x.FIO)%>:</td>
                    <td colspan="3">
                        <%: Html.TextBoxFor(x => x.FIO, new { maxlength = 100, style = "width:335px" })%>
                        <%: Html.ValidationMessageFor(model => model.FIO)%>
                    </td>                
                </tr>
                <tr>
                    <td class='row_title'><%:Html.LabelFor(x => x.INN)%>:</td>
                    <td>
                        <%: Html.TextBoxFor(x => x.INN, new { maxlength = 12, style = "width:90px" })%>
                        <%: Html.ValidationMessageFor(model => model.INN)%>
                    </td>
                    <td class='row_title'><%:Html.LabelFor(x => x.OGRNIP)%>:</td>
                    <td colspan="3">
                        <%: Html.TextBoxFor(x => x.OGRNIP, new { maxlength = 15, style = "width:335px" })%>
                        <%: Html.ValidationMessageFor(model => model.OGRNIP)%>
                    </td>                
                </tr>
                <tr>
                    <td class='row_title'><%:Html.LabelFor(x => x.Series)%>:</td>
                    <td>
                        <%: Html.TextBoxFor(x => x.Series, new { maxlength = 10, style = "width:90px" })%>
                        <%: Html.ValidationMessageFor(model => model.Series)%>
                    </td>
                    <td class='row_title'><%:Html.LabelFor(x => x.Number)%>:</td>
                    <td>
                        <%: Html.TextBoxFor(x => x.Number, new { maxlength = 10, style = "width:110px" })%>
                        <%: Html.ValidationMessageFor(model => model.Number)%>
                    </td>                
                    <td class='row_title'><%:Html.LabelFor(x => x.IssueDate)%>:</td>
                    <td>
                        <%= Html.DatePickerFor(model => model.IssueDate)%>
                        <%: Html.ValidationMessageFor(model => model.IssueDate)%>
                    </td>
                </tr>            
                <tr>
                    <td class='row_title'><%:Html.LabelFor(x => x.IssuedBy)%>:</td>
                    <td colspan="3">
                        <%: Html.TextBoxFor(x => x.IssuedBy, new { maxlength = 200, style = "width:290px" })%>
                        <%: Html.ValidationMessageFor(model => model.IssuedBy)%>
                    </td>
                    <td class='row_title'><%:Html.LabelFor(x => x.DepartmentCode)%>:</td>
                    <td>
                        <%: Html.TextBoxFor(x => x.DepartmentCode, new { maxlength = 10, style = "width:80px" })%>
                        <%: Html.ValidationMessageFor(model => model.DepartmentCode)%>
                    </td>                
                </tr>
                <tr>
                    <td class='row_title'><%:Html.LabelFor(x => x.Phone)%>:</td>
                    <td>
                        <%:Html.TextBoxFor(x => x.Phone, new { maxlength = 20, style = "width:90px" })%>
                        <%: Html.ValidationMessageFor(model => model.Phone)%>
                    </td>
                    <td class='row_title'><%:Html.LabelFor(x => x.Fax)%>:</td>
                    <td>
                        <%:Html.TextBoxFor(x => x.Fax, new { maxlength = 20, style = "width:110px" })%>
                        <%: Html.ValidationMessageFor(model => model.Fax)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'><%:Html.LabelFor(x => x.Address)%>:</td>
                    <td colspan="5">
                        <%: Html.TextBoxFor(x => x.Address, new { maxlength = 250, style = "width: 520px" })%>
                        <%: Html.ValidationMessageFor(model => model.Address)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:
                    </td>
                    <td colspan="5">
                        <%: Html.CommentFor(x => x.Comment, new { style = "width: 520px" }, rowsCount: 3)%>
                        <%: Html.ValidationMessageFor(x => x.Comment) %>
                    </td>
                </tr>
            </table>
        
            <div class='button_set'>
                <input id="btnSavePhysicalPerson" type="submit" value="Сохранить" />
                <input type="button" value="Закрыть" onclick="HideModal()" />
            </div>
        </div>
    </div>
<%} %>