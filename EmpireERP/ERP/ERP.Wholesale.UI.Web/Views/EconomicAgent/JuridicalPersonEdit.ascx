<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.EconomicAgent.JuridicalPersonEditViewModel>" %>

<script type="text/javascript">
    EconomicAgent_JuridicalPersonEdit.Init();

    function OnBeginJuridicalPersonEdit() {
        StartButtonProgress($("#btnSaveJuridicalPerson"));
    }
</script>

<% using (Ajax.BeginForm(Model.ActionName, Model.ControllerName, new AjaxOptions() { OnFailure = "EconomicAgent_JuridicalPersonEdit.OnFailJuridicalPersonEdit", 
       OnSuccess = Model.SuccessFunctionName, OnBegin = "OnBeginJuridicalPersonEdit" }))%>
<%{ %>
    <%:Html.HiddenFor(x => x.OrganizationId) %>
    <%:Html.HiddenFor(x => x.ContractorId) %>
    
    <div style='width:590px'>
        <div class='modal_title'><%:Model.Title %><%: Html.Help("/Help/GetHelp_EconomicAgentType_Edit_JuridicalPerson") %></div>
        <div class='h_delim'></div>
        
        <div style='padding: 10px 10px 5px;'>
            <div style='padding: 0 20px'>
                <table>
                    <tr>
                        <td>                    
                            <div id='editModePanel' class='inner_menu'>
                                <span id='linkMainInfo' class="selected"><%=Model.MainInfo %></span>
                                <span id='linkContacts'><%=Model.Contacts %></span>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
                    
            <br />
            <div id="messageJuridicalPersonEdit"></div>
        
            <div id='juridicalPersonEditMainInfo'>
                <table class='editor_table'>
                    <tr>
                        <td class='row_title' style="width: 140px"><%:Html.LabelFor(x => x.ShortName)%>:</td>
                        <td colspan="5">
                            <%:Html.TextBoxFor(x => x.ShortName, new { maxlength = 100, style="width: 96%" })%>
                            <%:Html.ValidationMessageFor(model => model.ShortName)%>
                        </td>                    
                    </tr>
                    <tr>
                        <td class='row_title'><%:Html.LabelFor(x => x.FullName)%>:</td>
                        <td colspan="5">
                            <%:Html.TextBoxFor(x => x.FullName, new { maxlength = 250, style = "width: 96%" })%>
                            <%: Html.ValidationMessageFor(model => model.FullName)%>
                        </td>                    
                    </tr>
                    <tr>
                        <td class='row_title'><%:Html.LabelFor(x => x.LegalFormId)%>:</td>
                        <td>
                            <%:Html.DropDownListFor(x => x.LegalFormId, Model.LegalFormList, new { style = "width:105px" })%>
                            <%: Html.ValidationMessageFor(model => model.LegalFormId)%>
                        </td>
                        <td class='row_title'><%:Html.LabelFor(x => x.INN)%>:</td>
                        <td>
                            <%:Html.TextBoxFor(x => x.INN, new { maxlength = 10, style = "width:74px" })%>
                            <%:Html.ValidationMessageFor(model => model.INN)%>
                        </td>
                        <td class='row_title' style="width: 35px"><%:Html.LabelFor(x => x.KPP)%>:</td>
                        <td>
                            <%:Html.TextBoxFor(x => x.KPP, new { maxlength = 9, style = "width:70px" })%>
                            <%: Html.ValidationMessageFor(model => model.KPP)%>
                        </td>
                    </tr>
                    <tr>
                        <td class='row_title'><%:Html.LabelFor(x => x.OGRN)%>:</td>
                        <td>
                            <%:Html.TextBoxFor(x => x.OGRN, new { maxlength = 13, style = "width:97px" })%>
                            <%: Html.ValidationMessageFor(model => model.OGRN)%>
                        </td>
                        <td class='row_title'><%:Html.LabelFor(x => x.OKPO)%>:</td>
                        <td colspan="4">
                            <%:Html.TextBoxFor(x => x.OKPO, new { maxlength = 10, style = "width:74px" })%>
                            <%: Html.ValidationMessageFor(model => model.OKPO)%>
                        </td>
                    </tr>
                    <tr>
                        <td class='row_title'><%:Html.LabelFor(x => x.Phone)%>:</td>
                        <td>
                            <%:Html.TextBoxFor(x => x.Phone, new { maxlength = 20, style = "width:97px" })%>
                            <%: Html.ValidationMessageFor(model => model.Phone)%>
                        </td>
                        <td class='row_title'><%:Html.LabelFor(x => x.Fax)%>:</td>
                        <td colspan="4">
                            <%:Html.TextBoxFor(x => x.Fax, new { maxlength = 20, style = "width:74px" })%>
                            <%: Html.ValidationMessageFor(model => model.Fax)%>
                        </td>
                    </tr>
                </table>
            </div>

            <div id='juridicalPersonEditContactInfo'>
                <table class='editor_table'>
                    <tr>
                        <td class='row_title' style="width: 140px"><%:Html.LabelFor(x => x.DirectorName)%>:</td>
                        <td>
                            <%:Html.TextBoxFor(x => x.DirectorName, new { maxlength = 100, style="width: 96%" })%>
                            <%: Html.ValidationMessageFor(model => model.DirectorName)%>
                        </td>
                    </tr>
                    <tr>
                        <td class='row_title'><%:Html.LabelFor(x => x.DirectorPost)%>:</td>
                        <td>
                            <%:Html.TextBoxFor(x => x.DirectorPost, new { maxlength = 100, style = "width: 96%" })%>
                            <%: Html.ValidationMessageFor(model => model.DirectorPost)%>
                        </td>
                    </tr>
                    <tr>
                        <td class='row_title'><%:Html.LabelFor(x => x.Bookkeeper)%>:</td>
                        <td>
                            <%:Html.TextBoxFor(x => x.Bookkeeper, new { maxlength = 100, style = "width: 96%" })%>
                            <%: Html.ValidationMessageFor(model => model.Bookkeeper)%>
                        </td>
                    </tr>
                    <tr>
                        <td class='row_title'><%:Html.LabelFor(x => x.Cashier)%>:</td>
                        <td>
                            <%:Html.TextBoxFor(x => x.Cashier, new { maxlength = 100, style = "width: 96%" })%>
                            <%: Html.ValidationMessageFor(model => model.Cashier)%>
                        </td>
                    </tr>
                </table>
            </div>        

            <table class='editor_table'>
                <tr>
                    <td class='row_title' style="width: 140px"><%:Html.LabelFor(x => x.Address)%>:</td>
                    <td>
                        <%:Html.TextBoxFor(x => x.Address, new { style="width: 96%", maxlength = 250 })%>
                        <%: Html.ValidationMessageFor(model => model.Address)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title' >
                        <%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:
                    </td>
                    <td>
                        <%:Html.CommentFor(x => x.Comment, new { style = "width: 98%" }, rowsCount: 3)%>
                        <%:Html.ValidationMessageFor(x => x.Comment) %>
                    </td>
                </tr>
            </table>
        </div>
    
        <div class='button_set'>
            <input id="btnSaveJuridicalPerson" type="submit" value="Сохранить" />
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>
    </div>    
<%} %>